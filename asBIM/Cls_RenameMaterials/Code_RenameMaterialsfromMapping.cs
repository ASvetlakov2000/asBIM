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
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Code_RenameMaterialsfromMapping : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            StringBuilder sb = new StringBuilder();

            // Задание пути файлу с наименованиями материалов
            string mappingFilePath = OpenFile.OpenSingleFile(
                "Выберите файл Mapping с новыми именами материалов", "txt"); 
            // Обработка ошибки. Если будет нажат [ESC]
            if (mappingFilePath.IsNullOrEmpty())
                return Result.Cancelled;
            
            // Передача в словарь str-str значений из файла Мэппинга
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

                tx.Commit();

                TaskDialog.Show("Успешно", "Материалы переименованы");
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// Метод LoadMaterialMapping - 
        /// <param name = "filePath" > Путь к файлу Mapping </param>
        /// </summary>
        private Dictionary<string, string> LoadMaterialMapping(string filePath)
        {
            // Создаем словарь для хранения соответствия старых и новых имен материалов
            // Словарь состоит из строчек
            // Строчки состоят из массивов с вдумя значениями [0] и [1]
            Dictionary<string, string> mapping = new Dictionary<string, string>(); 
    
            // Читаем все строки из файла мэпинга
            foreach (string line in File.ReadAllLines(filePath))
            {
                // Разделяем строку по символу табуляции на два элемента: старое имя и новое имя
                string[] parts = line.Split('\t'); // Разделитель - табуляция
        
                // Проверяем, что строка содержит ровно два элемента (старое и новое имя)
                if (parts.Length == 2)
                {
                    // Добавляем в словарь: ключ - старое имя (обрезаем пробелы), значение - новое имя (обрезаем пробелы)
                    mapping[parts[0].Trim()] = parts[1].Trim();
                }
            }
    
            // Возвращаем заполненный словарь с мэпингом имен материалов
            return mapping;
        }
    }
}