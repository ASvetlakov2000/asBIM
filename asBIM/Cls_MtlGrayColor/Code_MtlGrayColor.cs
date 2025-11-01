using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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

    public class Code_MtlGrayColor : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            
            try
            {
                // Ищем "Сплошная заливка" (Solid Fill)
                FillPatternElement solidFill = new FilteredElementCollector(doc)
                    .OfClass(typeof(FillPatternElement))
                    .Cast<FillPatternElement>()
                    .FirstOrDefault(f => f.GetFillPattern().IsSolidFill);

                if (solidFill == null)
                {
                    TaskDialog.Show("Ошибка", "В проекте не найден паттерн 'Сплошная заливка'.");
                    return Result.Failed;
                }

                // Собираем все материалы
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                ICollection<Element> materials = collector.OfClass(typeof(Material)).ToElements();

                int counter = 0; // счётчик изменённых материалов

                using (Transaction t = new Transaction(doc, "Update Material Graphics"))
                {
                    t.Start();

                    foreach (Material mat in materials)
                    {
                        // --- Штриховка поверхности (Projection) ---
                        mat.SurfaceForegroundPatternId = solidFill.Id; // всегда "Сплошная заливка"
                        mat.SurfaceForegroundPatternColor = new Color(245, 245, 245); // светло-серый
                        mat.SurfaceBackgroundPatternId = ElementId.InvalidElementId; // фон отключаем

                        // --- Штриховка сечения (Cut) ---
                        mat.CutForegroundPatternId = ElementId.InvalidElementId; // передний план "Нет"
                        mat.CutBackgroundPatternId = solidFill.Id; // фон "Сплошная заливка"
                        mat.CutBackgroundPatternColor = new Color(210, 210, 210); // серый фон

                        counter++;
                    }

                    t.Commit();
                }

                TaskDialog.Show("Готово", $"Материалы обновлены: {counter} шт.\n\n" +
                                          "• Поверхность: Сплошная заливка (245,245,245)\n" +
                                          "• Сечение: передний план = Нет, фон = Сплошная заливка (210,210,210)");

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}