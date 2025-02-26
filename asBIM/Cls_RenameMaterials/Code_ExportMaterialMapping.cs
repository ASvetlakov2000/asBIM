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

namespace asBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Code_ExportMaterialMapping : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            string mappingFilePath = "D:\\Test\\Test01.txt"; // Укажи путь к файлу

            using (StreamWriter writer = new StreamWriter(mappingFilePath))
            {
                FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Material));
                foreach (Material mat in collector)
                {
                    writer.WriteLine($"{mat.Name}\t"); // Старое имя, новое пустое
                }
            }

            TaskDialog.Show("Export Completed", "Файл мэпинга создан.");

            return Result.Succeeded;
        }
    }
}
