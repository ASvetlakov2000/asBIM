using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
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
using asBIM.Helpers;

namespace asBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Code_ExportMaterialMapping : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            
            try
            {
                string mappingFilePath =
                    CreateFile.CreateSingleFile("Сохранение файла мэппинга с именами материалов",
                        "txt"); // Укажи путь к файлу
                
                if (mappingFilePath.IsNullOrEmpty())
                    return Result.Cancelled;
            
                using (StreamWriter writer = new StreamWriter(mappingFilePath))
                {
                    FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Material));
                    foreach (Material mat in collector)
                    {
                        writer.WriteLine($"{mat.Name}\t"); // Старое имя, новое пустое
                    }
                }
                TaskDialog.Show("Имена материалов", "Файл c именами материалов создан.");
            }
            // Обработка исключений при щелчке правой кнопкой или нажатии ESC
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            
            return Result.Succeeded;
        }
    }
}
