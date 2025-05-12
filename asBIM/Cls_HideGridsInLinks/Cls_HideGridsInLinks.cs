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

    public class Cls_HideGridsInLinks : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            // ОСНОВНОЙ КОД ПЛАГИНА // НАЧАЛО  
            
            // Получаем текущий активный вид
            View activeView = doc.ActiveView;

            // Проверяем, что скрипт запускается на плане или разрезе
            if (!(activeView is ViewPlan || activeView is ViewSection))
            {
                TaskDialog.Show("Ошибка", "Скрипт работает только на планах и разрезах.");
                return Result.Failed;
            }

            // Фильтр для всех связанных файлов (RevitLinkInstance)
            FilteredElementCollector linkCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkInstance));

            List<ElementId> gridsToHide = new List<ElementId>();

            // Обрабатываем каждый связанный файл
            foreach (RevitLinkInstance link in linkCollector.Cast<RevitLinkInstance>())
            {
                Document linkedDoc = link.GetLinkDocument();
                if (linkedDoc == null) continue; // Пропускаем, если связанный файл не загружен

                // Фильтруем оси (Grid) в связанном файле
                FilteredElementCollector gridCollector = new FilteredElementCollector(linkedDoc)
                    .OfClass(typeof(Grid));

                // Добавляем найденные оси в список для скрытия
                gridsToHide.AddRange(gridCollector.Select(g => g.Id));
            }

            if (gridsToHide.Count == 0)
            {
                TaskDialog.Show("Результат", "Не найдено осей в связанных файлах.");
                return Result.Succeeded;
            }

            // Создаём транзакцию для скрытия осей
            using (Transaction trans = new Transaction(doc, "Скрытие осей из связанных файлов"))
            {
                trans.Start();
                activeView.HideElements(gridsToHide);
                trans.Commit();
            }

            TaskDialog.Show("Готово", $"Скрыто осей: {gridsToHide.Count}");
            return Result.Succeeded;
        }
    }
}