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

                // Создание коллекции из материалов из документа
                FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Material));
                foreach (Material mat in collector)
                {
                    // Если в файле Мэппинга есть строка, совпадающая с именем материала в документе && строка не пустая,
                    // то перезапись имени материала
                    if (materialMapping.ContainsKey(mat.Name) && !string.IsNullOrWhiteSpace(materialMapping[mat.Name]))
                    {
                        try
                        {
                            mat.Name = materialMapping[mat.Name];
                        }
                        catch (Exception ex)
                        {
                            sb.AppendLine($"{mat.Name}");
                        }
                    }
                }

                if (sb.Length != 0)
                {
                    // Задание пути файлу со списком пропущенных материалов
                    string logFilePath = CreateFile.CreateSingleFile(
                        "Сохранение файла со списком пропущенных материалов", "txt");
                    // Обработка исключений при щелчке правой кнопкой или нажатии [ESC]
                    if (logFilePath.IsNullOrEmpty())
                        return Result.Cancelled;

                    using (StreamWriter writer = new StreamWriter(logFilePath))
                    {
                        writer.WriteLine("ЭТОТ ФАЙЛ СОДЕРЖИТ СПИСОК МАТЕРИАЛОВ, КОТОРЫЕ НЕ БЫЛИ ПЕРЕИМЕНОВАНЫ");
                        writer.WriteLine("\n\n");
                        writer.WriteLine(sb);
                    }
                    
                    NotificationManagerWPF.MessageSmileInfoTm("Некоторые материалы пропущены!",
                        "\n\u00af\u00af\u00af\u00af\u00af\u00af\\_(ツ)_/\u00af\u00af\u00af" +
                        "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af" +
                        "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af",
                        $"\n\nНекоторые материалы не удалось переименовать!" +
                        $"\n\nСозданный файл содержит список материалов, которые не удалось переименовать" +
                        $"\n\nПроверь файл с пропущенными материалами и повтори процедуру",
                        NotificationType.Warning, 20);
                }
                
                tx.Commit();

                NotificationManagerWPF.MessageSmileInfoTm("Переименование материалов",
                    "\n(￢‿￢)",
                    $"\n\nМатериалы успешно переименованы!",
                    NotificationType.Success, 20);
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// Метод LoadMaterialMapping - загружает в словарь [старое имя] - [новое имя] с Mapping материалов.
        /// <param name = "filePath" > Путь к файлу Mapping </param>
        /// <returns> Mapping - возвращает словарь с [старое имя] - [новое имя]</returns>>
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