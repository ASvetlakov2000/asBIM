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
public class Code_RenameMaterialsfromMapping : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIApplication uiapp = commandData.Application;
        Document doc = uiapp.ActiveUIDocument.Document;
        
        StringBuilder sb = new StringBuilder();
        
        string mappingFilePath = OpenFile.OpenSingleFile("Выберите файл мэппинга с новыми именами материалов", "txt"); // Укажи путь к файлу
        Dictionary<string, string> materialMapping = LoadMaterialMapping(mappingFilePath);
        
        using (Transaction tx = new Transaction(doc, "Переименование материалов"))
        {
            tx.Start();
            
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Material));
            foreach (Material mat in collector)
            {
                if (materialMapping.ContainsKey(mat.Name) && !string.IsNullOrWhiteSpace(materialMapping[mat.Name]))
                {
                    try
                    {
                        mat.Name = materialMapping[mat.Name];
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Не удалось переименовать {mat.Name}");
                        TaskDialog.Show("Ошибка", sb.ToString());
                    }
                }
            }
            
            TaskDialog.Show("Успешно", "Материалы переименованы");

            tx.Commit();
        }
        
        return Result.Succeeded;
    }
    
    private Dictionary<string, string> LoadMaterialMapping(string filePath)
    {
        Dictionary<string, string> mapping = new Dictionary<string, string>();
        foreach (string line in File.ReadAllLines(filePath))
        {
            string[] parts = line.Split('\t'); // Разделитель - табуляция
            if (parts.Length == 2)
            {
                mapping[parts[0].Trim()] = parts[1].Trim();
            }
        }
        return mapping;
    }
}
}