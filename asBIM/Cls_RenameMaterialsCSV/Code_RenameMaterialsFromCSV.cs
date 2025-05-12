using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using Notifications.Wpf;
using Nice3point.Revit.Extensions;
using System.Text;
using asBIM;

namespace asBIM
{
    [Transaction(TransactionMode.Manual)]

    public class Code_RenameMaterialsFromCSV : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            StringBuilder sb = new StringBuilder();

            // Задание пути файлу с наименованиями материалов
            string mappingFilePath = OpenFile.OpenSingleFile(
                "Выберите файл Mapping с новыми именами материалов", "csv");
            // Обработка ошибки. Если будет нажат [ESC]
            if (mappingFilePath.IsNullOrEmpty())
                return Result.Cancelled;

            // Передача в словарь str-str значений из файла Мэппинга
            Dictionary<string, string> materialMapping = ReadMaterialMapping(mappingFilePath);

            // Проверяем, есть ли данные для обновления
            if (materialMapping.Count == 0)
            {
                NotificationManagerWPF.MessageSmileInfoTm("Ошибка чтения файла Mapping!",
                    "\n\u00af\u00af\u00af\u00af\u00af\u00af\\_(ツ)_/\u00af\u00af\u00af" +
                    "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af" +
                    "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af",
                    $"\n\nФайл Mapping пуст или отсутствуют новые имена материалов!",
                    NotificationType.Warning, 20);

                return Result.Failed;
            }

            // Начинаем транзакцию для переименования материалов
            using (Transaction trans = new Transaction(doc, "Переименование материалов"))
            {
                trans.Start();

                // Создание коллекции из материалов из документа
                FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Material));

                // Проходим по всем материалам и переименовываем их, если есть соответствие в мэпинге
                foreach (Material material in collector)
                {
                    // if (materialMapping.TryGetValue(material.Name, out string newName) && !string.IsNullOrWhiteSpace(newName))
                    if (materialMapping.ContainsKey(material.Name) && !string.IsNullOrWhiteSpace(materialMapping[material.Name]))
                    {
                        try
                        {
                            // Применяем новое имя
                            // material.Name = newName;
                            material.Name = materialMapping[material.Name];
                        }
                        catch (Exception ex)
                        {
                            sb.AppendLine($"{material.Name}");
                        }
                    }
                }
                
                if (sb.Length != 0)
                {
                    // Задание пути файлу со списком пропущенных материалов
                    string logFilePath = CreateFile.CreateSingleFile(
                        "Сохранение файла со списком пропущенных материалов", "csv");
                    // Обработка исключений при щелчке правой кнопкой или нажатии [ESC]
                    if (logFilePath.IsNullOrEmpty())
                        return Result.Cancelled;

                    using (StreamWriter writer = new StreamWriter(logFilePath))
                    {
                        writer.WriteLine("ЭТОТ ФАЙЛ СОДЕРЖИТ СПИСОК МАТЕРИАЛОВ, КОТОРЫЕ НЕ БЫЛИ ПЕРЕИМЕНОВАНЫ");
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
                
                trans.Commit();
                
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
        private Dictionary<string, string> ReadMaterialMapping(string mappingFilePath)
        {
            // Создаем словарь для хранения мэпинга материалов
            Dictionary<string, string> mapping = new Dictionary<string, string>();

            // Проверяем, существует ли файл мэпинга
            if (!File.Exists(mappingFilePath))
            {
                NotificationManagerWPF.MessageSmileInfoTm("Файл Mapping не найден!",
                    "\n\u00af\u00af\u00af\u00af\u00af\u00af\\_(ツ)_/\u00af\u00af\u00af" +
                    "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af" +
                    "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af",
                    $"\n\nФайл Mapping не найден!" +
                    $"\n\nПопробуйте снова!",
                    NotificationType.Warning, 20);
                
                return mapping;
                
            }

            // Читаем файл CSV построчно
            using (StreamReader reader = new StreamReader(mappingFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Разделяем строку по запятой
                    string[] columns = line.Split('\t');
                    // Проверяем, что есть минимум два столбца и второй столбец не пуст
                    if (columns.Length >= 2 && !string.IsNullOrWhiteSpace(columns[1]))
                    {
                        mapping[columns[0].Trim()] = columns[1].Trim();
                    }
                }
            }

            // Возвращаем заполненный словарь с мэпингом имен материалов
            return mapping;
        }
    }
}