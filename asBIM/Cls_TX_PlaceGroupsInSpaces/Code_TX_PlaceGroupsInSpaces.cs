using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using Notifications.Wpf;
using Nice3point.Revit.Extensions;
using System.Text.RegularExpressions;
using Group = Autodesk.Revit.DB.Group;
using asBIM.Helpers;



namespace asBIM.Cls_TX_PlaceGroupsInSpaces
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Code_TX_PlaceGroupsInSpaces : IExternalCommand
    {
        // Guid общего параметра "PRO_ID группы в пространстве"
        Guid shParamGuid = new Guid("bae21547-43fa-423f-91f9-ff8b42d50560");
        // Имя общего параметра "PRO_ID группы в пространстве
        string shParamName = "PRO_ID группы в пространстве";
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            // ОСНОВНОЙ КОД ПЛАГИНА // НАЧАЛО

            // Время выполнения
            Stopwatch swPlaceGroups = new Stopwatch();
            swPlaceGroups.Start();
            // Размещение групп по одноименным пространствам
            PlacementOfGroupsInSpaces(doc, GetSpacesFromDoc(doc), GetTxGroupsFromDoc(doc));
            swPlaceGroups.Stop();
            // Время выполнения
            var timeInSecForCommand = swPlaceGroups.Elapsed.TotalSeconds;
            double timeInMin = TimeOfWorkConverter.ConvertTime(timeInSecForCommand).timeInMinOutput;
            double timeInSec = TimeOfWorkConverter.ConvertTime(timeInSecForCommand).timeInSecOutput;
        
            // Уведомление. "Время работы"
            NotificationManagerWPF.TimeOfWork("Время работы", 
                timeInSec:"\nВремя выполнения: " +
                          Convert.ToString(Math.Round(Convert.ToDouble(timeInMin), 0,
                              MidpointRounding.AwayFromZero) + " мин") + " " +
                          Convert.ToString(Math.Round(Convert.ToDouble(timeInSec), 0,
                              MidpointRounding.AwayFromZero) + " сек"),
                NotificationType.Information);
            
            // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ
            return Result.Succeeded;
        }
        
        /// <summary>
        /// Метод для проверки наличия общего параметра в проекте. По имени параметра
        /// <param name = "doc" > Документ </param>
        /// <param name = "shParamName" > Guid общего параметра</param>
        /// <returns>True - если параметр найден, False - если параметр не найден</returns>
        /// </summary>
        public bool SharedParametrValidCheckerByName(Document doc, string shParamName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IEnumerable<SharedParameterElement> sharedParameters = collector
                .OfClass(typeof(SharedParameterElement))
                .Cast<SharedParameterElement>();
            string paramName = sharedParameters.First().Name;
            return sharedParameters.Any(param => paramName == shParamName);
        }

        /// <summary>
        /// Метод для сбора Пространств.  Сортировка +- по номеру
        /// <param name = "doc" > Документ </param>
        /// <returns>centerOfSpace - список с Пространствами, отсортированный по возрастанию значению номера (целое)</returns>
        /// </summary>
        public List<SpatialElement> GetSpacesFromDoc(Document doc)
        {
            string spaceFullName;
            string spaceClearName;
            
            // Замена "," на "."
            NumberFormatInfo format = new NumberFormatInfo {NumberDecimalSeparator = "."};
            // Сбор Пространств в коллекцию с сортировкой по категории и выбор только элементов
            FilteredElementCollector collectorSpaces = new FilteredElementCollector(doc);
            collectorSpaces.OfCategory(BuiltInCategory.OST_MEPSpaces).ToElements();
            // Сбор Пространств в список.
            // Сортировка Пространств в списке по Номеру Пространств
            List<SpatialElement> spacesList = collectorSpaces.Cast<SpatialElement>()
                // Конвертация номера Пространств из строки в целое
                .OrderBy(space => Convert.ToString(space.Number, format))
                .ToList();

            // TODO: 2. Добавить проверку на наличие параметра
            // NotificationManagerWPF.MessageInfoSmile(
            //     "Ошибка!\nВ проекте нет параметров!",
            //     $"\nДобавьте общий параметр [{shParamName}]",
            //     "\n\u00af\u00af\u00af\u00af\u00af\u00af\\_(ツ)_/\u00af\u00af\u00af" +
            //     "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af" +
            //     "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af",
            //     NotificationType.Error);
            
            
            foreach (SpatialElement space in spacesList)
            {
                spaceFullName = space.Name;
                spaceClearName = Regex.Replace(spaceFullName, @"\s[^\s]+$", "");
                if (spaceFullName == spaceClearName)
                {
                    NotificationManagerWPF.MessageInfoSmile(
                        "Ошибка!\nВ проекте пространства без имени!",
                        $"\nПространство: [№] - [{spaceClearName}],  [ID] - [{space.Id}]",
                        "\n\u00af\u00af\u00af\u00af\u00af\u00af\\_(ツ)_/\u00af\u00af\u00af" +
                        "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af" +
                        "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af",
                        NotificationType.Error);
                }
                
            }
            

            return spacesList;
        }
        
        /// <summary>
        /// Метод GetTxGroupsFromDoc для сбора Групп. Сортировка +- по имени
        /// <param name = "doc" > Документ </param>
        /// <returns>centerOfSpace - список с Группами, отсортированный по возрастанию значения имени (строки)</returns>
        /// </summary>
        public List<Group> GetTxGroupsFromDoc(Document doc)
        {
            // Сбор Групп в коллекцию с сортировкой по категории и выбор только элементов
            FilteredElementCollector collectorGroups = new FilteredElementCollector(doc);
            collectorGroups.OfClass(typeof(Group)).ToElements();
            // Сбор Групп в список.
            // Сортировка Групп в списке по Номеру Групп
            List<Group> groupsList = collectorGroups.Cast<Group>()
                // Конвертация номера Групп из строки в целое
                .OrderBy(group => group.Name)
                .ToList();
            
            return groupsList;
        }
        
        /// <summary>
        /// Возвращает координаты центра элемента
        /// </summary>
        /// <param name="elem">Элемент</param>
        /// <returns>Координаты центра</returns>
        public XYZ GetSpaceBottomCenter(Element elem)
        {
            BoundingBoxXYZ bounding = elem.get_BoundingBox(null);
            if (bounding != null)
            {
                XYZ center = (bounding.Max + bounding.Min) * 0.5;
                LocationPoint loc = elem.Location as LocationPoint;
                XYZ spaceCenter  = new XYZ(center.X, center.Y, loc.Point.Z);
                return spaceCenter;
            }
            return null;
        }
        
        /// <summary>
        /// Сопоставляет Имена Пространств и Групп
        /// <param name = "doc" > Документ </param>
        /// <param name = "spacesList" > Список с Пространствами </param>
        /// <param name = "groupsList" > Список с Группами </param>
        /// </summary>
        public void PlacementOfGroupsInSpaces(Document doc, List<SpatialElement> spacesList, List<Group> groupsList)
        {
            
            // TODO: 2. Добавить проверку на наличие параметра
            
            // Счетчик для количества элементов. Удачно
            string placedGroupCountStr;
            IList<Group> placedGroupCount = new List<Group>();
            
            // Счетчик для количества элементов. Не дачно
            string placedErrGroupCountStr;
            IList<Group> placedErrGroupCount = new List<Group>();
            
            // Проверка на отсутствие Пространств
            if (!spacesList.Any())
            {
                NotificationManagerWPF.MessageInfoSmile(
                    "Ошибка!",
                    "\nВ проекте отсутствуют пространства", 
                    "\n\u00af\u00af\u00af\u00af\u00af\u00af\\_(ツ)_/\u00af\u00af\u00af" +
                    "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af" +
                    "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af",
                    NotificationType.Warning );
                
                return;
            }
            
            // Проверка на отсутствие Групп
            if (!groupsList.Any())
            {
                NotificationManagerWPF.MessageInfoSmile(
                    "Ошибка!",
                    "\nВ проекте отсутствуют группы", 
                    "\n\u00af\u00af\u00af\u00af\u00af\u00af\\_(ツ)_/\u00af\u00af\u00af\u00af" +
                    "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af",
                    NotificationType.Warning );
                
                return;
            }
            
            StringBuilder sb = new StringBuilder();

            using (Transaction tr = new Transaction(doc, "Расстановка групп в пространства"))
            {
                tr.Start();
                foreach (SpatialElement space in spacesList)
                {
                    // Получаем имя пространства
                    string spaceName = space.Name.Split(' ').FirstOrDefault();
                    string spaceID = space.Id.ToString();

                    // GUID общего параметра из ФОП [PRO_ТХ_Группа в пространстве]
                    Guid shParamGuid = new Guid("bae21547-43fa-423f-91f9-ff8b42d50560");
                    
                    // TODO: 2. Добавить проверку на наличие параметра
                    Parameter placedGropupsChecker = space.get_Parameter(shParamGuid);
                    string paramValue = placedGropupsChecker.AsString(); 
                    
                    
                    if (placedGropupsChecker != null && placedGropupsChecker.AsString().IsNullOrEmpty())
                    {
                                                List<Group> matchingGroups = groupsList
                            .Where(group => !string.IsNullOrEmpty(group.Name) && // Проверка имени группы
                                            group.Name.Equals(spaceName, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                                                
                        if (matchingGroups.Any())
                        {
                            foreach (Group group in matchingGroups)
                            {
                                XYZ placementPoint = GetSpaceBottomCenter(space);
                                // string groupId = group.GroupId.ToString();
                                if (placementPoint != null && placedGropupsChecker.AsString().IsNullOrEmpty())
                                {
                                    try
                                    {
                                        // Размещение групп в одноименных пространствах 
                                        Group placedGroup = doc.Create.PlaceGroup(placementPoint, group.GroupType);
                                        // Получение ID размещенной группы
                                        ElementId placedGroupId = placedGroup.Id;
                                        // Подсчет количества элементов. Удачных
                                        // Добавление элемента в счетчик отработанных пространств
                                        placedGroupCount.Add(placedGroup);
                                        // Запись ID Группы в пространство
                                        Parameter spaceParameterID = space.get_Parameter(shParamGuid);
                                        if (spaceParameterID != null)
                                            spaceParameterID.Set(placedGroupId.ToString());
                                        // Запись ID Группы в группу
                                        Parameter groupParameterID = placedGroup.get_Parameter(shParamGuid);
                                        if (groupParameterID != null)
                                            groupParameterID.Set(placedGroupId.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        sb.AppendLine(
                                            $"Ошибка при размещении исходной группы с ID [{group.Id}] и Именем [{group.Name}]: {ex.Message}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Подсчет количества элементов. Не удачных
                            // Добавление элемента в счетчик не отработанных пространств
                            placedErrGroupCount.Add(matchingGroups.FirstOrDefault());
                            sb.AppendLine($"[Имя] - {spaceName},  [ID] - {spaceID}.");
                        }
                    }
                }
                
                placedGroupCountStr = $"\nКол-во размещенных групп: {placedGroupCount.Count.ToString()}";
                placedErrGroupCountStr = $"\nКол-во пропущенных пространств: {placedErrGroupCount.Count.ToString()}";
                
                // TODO: Обьединить списки с пространствами. Вывод только числа
                // Уведомление. Подсчет количества элементов. Не удачно
                NotificationManagerWPF.ElemCount(
                    "Не обработанные элементы",
                    elementCount: $"{placedErrGroupCountStr} \n\nПеречень неопределенных пространств:\n{sb}", NotificationType.Warning);

                // Уведомление. Подсчет количества элементов. Успех
                NotificationManagerWPF.ElemCount(
                    "Обработанные элементы",
                    elementCount: placedGroupCountStr, NotificationType.Success);
                
                tr.Commit();
                
            }
        }
    }
    
} 