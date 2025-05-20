using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Code_Name : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            // ОСНОВНОЙ КОД ПЛАГИНА // НАЧАЛО  
            
            // Получаем все размещённые помещения
            IList<Room> rooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .Cast<Room>()
                .Where(r => r.Location != null)
                .ToList();

            using (Transaction trans = new Transaction(doc, "Создание форм из помещений"))
            {
                trans.Start();

                foreach (Room room in rooms)
                {
                    // Получаем границы помещения
                    IList<IList<BoundarySegment>> boundaries = room.GetBoundarySegments(new SpatialElementBoundaryOptions());
                    if (boundaries == null || boundaries.Count == 0)
                        continue;

                    // Получаем основные параметры помещения
                    // string roomName = room.Name;
                    // string paramAValue = room.LookupParameter("А")?.AsString() ?? "";
                    double height = room.get_Parameter(BuiltInParameter.ROOM_HEIGHT)?.AsDouble() ?? 0;

                    List<CurveLoop> loops = new List<CurveLoop>();
                    double tolerance = 0.001; // допуск в футах

                    // Обработка каждого замкнутого контура
                    foreach (IList<BoundarySegment> boundaryList in boundaries)
                    {
                        List<Curve> curveList = boundaryList
                            .Select(seg => seg.GetCurve())
                            .Where(c => c != null)
                            .ToList();

                        if (curveList.Count < 2)
                            continue;

                        // Проверка на замкнутость: первая точка == последней
                        XYZ firstPoint = curveList.First().GetEndPoint(0);
                        XYZ lastPoint = curveList.Last().GetEndPoint(1);

                        if (firstPoint.DistanceTo(lastPoint) > tolerance)
                        {
                            // Контур не замкнут — пропускаем
                            continue;
                        }

                        // Создаём CurveLoop из замкнутого контура
                        CurveLoop loop = new CurveLoop();
                        try
                        {
                            foreach (Curve c in curveList)
                            {
                                loop.Append(c);
                            }

                            loops.Add(loop);
                        }
                        catch
                        {
                            // Пропускаем контур при ошибке
                            continue;
                        }
                    }

                    // Если нет валидных контуров — пропускаем помещение
                    if (loops.Count == 0 || height <= 0)
                        continue;

                    // Создаём Solid из контуров
                    Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(loops, XYZ.BasisZ, height);

                    // Создаём форму (DirectShape)
                    DirectShape ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_Mass));
                    ds.SetShape(new List<GeometryObject> { solid });
                    // ds.Name = $"Форма: {roomName}";

                    // // Присваиваем значение параметра "А", если он существует у формы
                    // Parameter formParamA = ds.LookupParameter("А");
                    // if (formParamA != null && formParamA.StorageType == StorageType.String)
                    // {
                    //     formParamA.Set(paramAValue);
                    // }
                }
                
                TaskDialog.Show("Готово", "Формы созданы на основе помещений.");

                trans.Commit();
            }

            return Result.Succeeded;
        }
        }
        // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ
    }



