// Пространства имён, подключаемые для работы с Revit API и дополнительным функционалом
using Autodesk.Revit.UI; 
using Autodesk.Revit.DB; 
using Autodesk.Revit.Attributes; 
using Autodesk.Revit.DB.Architecture; 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.IO; 
using System.Windows.Forms; 
using System.Threading.Tasks; 
using asBIM.Properties;
using System.Windows.Data; 
using System.Collections.ObjectModel; 
using System.Diagnostics; 
using System.Dynamic;
using System.Globalization;
using System.Xaml;
using System.Windows.Documents; 
using System.Xml.Linq; 
using System.Windows; 
using asBIM.ViewModel; 
using Notifications.Wpf; 
using CommunityToolkit.Mvvm.Input; 
using Nice3point.Revit.Extensions; 
using System.Text.RegularExpressions; 
using Group = Autodesk.Revit.DB.Group; 
using Autodesk.Revit.UI.Selection; 
using CsvHelper; 

namespace asBIM
{
    // Атрибут транзакции — задаёт режим выполнения транзакции в Revit
    [TransactionAttribute(TransactionMode.Manual)]
    // Атрибут регенерации — управляет обновлением модели при выполнении команды
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Code_CreateFormFromRoom : IExternalCommand // Класс реализует внешний плагин для Revit
    {
        
        // Имя общего параметра "PRO_ID помещения"
        string shParamName = "PRO_ID помещения";
        // Guid общего параметра "PRO_ID помещения"
        Guid shParamGuid = new Guid("f0737b4b-8343-47e5-9018-b9e05378fa33");
        
        // Guid общего параметра "PRO_ID помещения"
        Guid shParamGuid02 = new Guid("ab4bf165-1419-4fef-b80a-3d1b38b5ec23");
        // Имя общего параметра "PRO_ID помещения"
        string shParamName02 = "PRO_Назначение помещения";
        
        // Основной метод, вызываемый Revit при запуске команды
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Получаем доступ к интерфейсу и документу Revit
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            // Сбор всех помещений, у которых задано расположение (Location)
            IList<Room> rooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .Cast<Room>()
                .Where(r => r.Location != null)
                .ToList();

            // Поиск уже существующих форм (DirectShape), у которых задан параметр "RoomId"
            var existingForms = new FilteredElementCollector(doc)
                .OfClass(typeof(DirectShape))
                .Cast<DirectShape>()
                .Where(ds => ds.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Mass) // Только формы в категории "Массы"
                .Where(ds => ds.get_Parameter(shParamGuid) != null) // У которых есть параметр "RoomId"
                .ToDictionary(
                    ds => ds.get_Parameter(shParamGuid).AsString(), // Ключ — ID помещения
                    ds => ds // Значение — сама форма
                );

            // Формы, которые могут быть удалены (если не будут обновлены)
            var formsToRemove = new HashSet<string>(existingForms.Keys);
            // Лог пропущенных помещений
            var skippedRooms = new List<string>();

            using (Transaction trans = new Transaction(doc, "Обновление форм по помещениям"))
            {
                trans.Start(); // Начало транзакции

                foreach (Room room in rooms)
                {
                    string roomIdStr = room.Id.IntegerValue.ToString(); // ID помещения в виде строки
                    string roomName = room.Name; // Имя помещения
                    string roomNumber = room.Number; // Номер помещения
                    double height = room.get_Parameter(BuiltInParameter.ROOM_HEIGHT)?.AsDouble() ?? 0; // Высота помещения
                    string roomPurpose = room.LookupParameter("Назначение")?.AsString() ?? ""; // Считываем значение параметра "Назначение" из помещения

                    // Получение контуров помещения
                    IList<IList<BoundarySegment>> boundaries = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
                    if (boundaries == null || boundaries.Count == 0)
                    {
                        skippedRooms.Add($"{roomNumber}\t{roomName}\t{room.Id.IntegerValue}\tнет границ");
                        continue; // Пропуск помещения без границ
                    }

                    List<CurveLoop> loops = new List<CurveLoop>(); // Контуры помещения
                    double tolerance = 0.001; // Допуск для сравнения точек

                    // Перебор границ помещения
                    foreach (IList<BoundarySegment> boundaryList in boundaries)
                    {
                        List<Curve> curveList = boundaryList.Select(seg => seg.GetCurve()).ToList();
                        if (curveList.Count < 2) continue;

                        // Проверка замкнутости контура
                        XYZ first = curveList.First().GetEndPoint(0);
                        XYZ last = curveList.Last().GetEndPoint(1);
                        if (first.DistanceTo(last) > tolerance) continue;

                        CurveLoop loop = new CurveLoop();
                        try
                        {
                            foreach (Curve c in curveList)
                                loop.Append(c); // Добавляем кривые в контур
                            loops.Add(loop);
                        }
                        catch
                        {
                            continue; // Пропускаем некорректный контур
                        }
                    }

                    // Пропуск помещений с некорректными контурами или нулевой высотой
                    if (loops.Count == 0 || height <= 0)
                    {
                        skippedRooms.Add($"{roomNumber}\t{roomName}\t{room.Id.IntegerValue}\tнекорректный контур или нулевая высота");
                        continue;
                    }

                    bool needCreate = true; // Нужно ли создать форму

                    // Проверка: существует ли уже форма для этого помещения
                    if (existingForms.TryGetValue(roomIdStr, out DirectShape oldShape))
                    {
                        GeometryElement geom = oldShape.get_Geometry(new Options());
                        Solid oldSolid = geom?.OfType<Solid>().FirstOrDefault(s => s.Volume > 0);

                        // Проверка изменений: контур и высота
                        bool contourChanged = !AreContoursEqual(loops, oldSolid, 0.01);
                        double oldHeight = oldShape.get_BoundingBox(null).Max.Z - oldShape.get_BoundingBox(null).Min.Z;
                        bool heightChanged = Math.Abs(oldHeight - height) > 0.01;

                        if (!contourChanged && !heightChanged)
                        {
                            formsToRemove.Remove(roomIdStr); // Форма актуальна — не удаляем
                            needCreate = false;
                        }
                        else
                        {
                            doc.Delete(oldShape.Id); // Удаляем старую форму
                            formsToRemove.Remove(roomIdStr); // Не нужно удалять дважды
                        }
                    }

                    if (needCreate)
                    {
                        // Создание новой формы на основе контуров и высоты
                        Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(loops, XYZ.BasisZ, height);
                        DirectShape newShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_Mass));
                        newShape.SetShape(new List<GeometryObject> { solid });

                        // Назначение RoomId в общий параметр
                        Parameter paramRoomId = newShape.get_Parameter(shParamGuid);
                        if (paramRoomId != null && paramRoomId.StorageType == StorageType.String)
                            paramRoomId.Set(roomIdStr);
                        
                        // Устанавливаем Назначение в общий параметр ""
                        Parameter roomFunk = newShape.get_Parameter(shParamGuid02);
                        if (roomFunk != null && roomFunk.StorageType == StorageType.String)
                            roomFunk.Set(roomPurpose);
                        
                    }
                }

                // Удаляем неиспользуемые формы
                foreach (string unusedRoomId in formsToRemove)
                {
                    doc.Delete(existingForms[unusedRoomId].Id);
                }

                trans.Commit(); // Завершаем транзакцию
            }

            // Если есть пропущенные помещения — сохраняем лог
            if (skippedRooms.Count > 0)
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Title = "Сохранить список пропущенных помещений",
                    Filter = "Text files (*.txt)|*.txt",
                    FileName = "Помещения без площадей_.txt",
                    DefaultExt = "txt"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var output = new List<string> { "Номер помещения\tИмя помещения\tID\tПричина пропуска" };
                    output.AddRange(skippedRooms);
                    File.WriteAllLines(saveDialog.FileName, output, Encoding.UTF8);

                    // TaskDialog.Show("Результат", $"Пропущено помещений: {skippedRooms.Count}\nФайл: {saveDialog.FileName}");
                    
                    // Уведомление. "Пропущенные помещения сохранены!"
                    NotificationManagerWPF.MessageSmileInfoTm(
                        "Пропущенные помещения сохранены",
                        "\n(￢‿￢ )", 
                        $"\n\nПропущено помещений: {skippedRooms.Count}" +
                        $"\n\nФайл сохранен по пути: \n {saveDialog.FileName}"
                        , NotificationType.Information, 15);
                    
                }
            }
                // Уведомление. "Пропущенные помещения сохранены!"
                NotificationManagerWPF.MessageSmileInfoTm(
                    "Формы созданы!",
                    "\n(￢‿￢ )", 
                    $"\n\nФормы на основе помещений созданы!" + 
                    "\n\n Если надо обновить формы по измененным помещениям - запусти плагин заново." + 
                    "\n\n При повторном запуске формы не будут дублироваться"
                    , NotificationType.Success, 15);
            return Result.Succeeded; // Завершение команды
        }

        // Метод для сравнения контуров помещения и формы
        private bool AreContoursEqual(List<CurveLoop> roomLoops, Solid formSolid, double tolerance = 0.01)
        {
            if (formSolid == null || formSolid.Edges.Size == 0)
                return false;

            // Сбор рёбер по идентификатору (группировка по петлям)
            Dictionary<int, List<Curve>> groupedCurves = new Dictionary<int, List<Curve>>();
            foreach (Edge edge in formSolid.Edges)
            {
                int loopId = edge.Id;
                if (!groupedCurves.ContainsKey(loopId))
                {
                    groupedCurves.Add(loopId, new List<Curve>());
                }
                groupedCurves[loopId].Add(edge.AsCurve());
            }

            // Преобразование в CurveLoop
            List<CurveLoop> formLoops = new List<CurveLoop>();
            foreach (KeyValuePair<int, List<Curve>> kvp in groupedCurves)
            {
                CurveLoop loop = new CurveLoop();
                foreach (Curve curve in kvp.Value)
                {
                    try
                    {
                        loop.Append(curve);
                    }
                    catch
                    {
                        return false; // Пропуск некорректного контура
                    }
                }
                formLoops.Add(loop);
            }

            // Сравнение количества контуров
            if (roomLoops.Count != formLoops.Count)
                return false;

            // Сравнение каждого сегмента
            for (int i = 0; i < roomLoops.Count; i++)
            {
                List<Curve> roomCurves = new List<Curve>(roomLoops[i]);
                List<Curve> formCurves = new List<Curve>(formLoops[i]);

                if (roomCurves.Count != formCurves.Count)
                    return false;

                for (int j = 0; j < roomCurves.Count; j++)
                {
                    Curve rc = roomCurves[j];
                    Curve fc = formCurves[j];

                    XYZ rStart = rc.GetEndPoint(0);
                    XYZ rEnd = rc.GetEndPoint(1);
                    XYZ fStart = fc.GetEndPoint(0);
                    XYZ fEnd = fc.GetEndPoint(1);

                    // Проверка на совпадение концов кривых (вперёд и назад)
                    bool directMatch = rStart.IsAlmostEqualTo(fStart, tolerance) && rEnd.IsAlmostEqualTo(fEnd, tolerance);
                    bool reverseMatch = rStart.IsAlmostEqualTo(fEnd, tolerance) && rEnd.IsAlmostEqualTo(fStart, tolerance);

                    if (!(directMatch || reverseMatch))
                        return false;
                }
            }

            return true; // Контуры идентичны
        }
    }
}
