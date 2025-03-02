using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using asBIM;
using Notifications.Wpf;
using Nice3point.Revit.Extensions;

namespace asBIM
{
    [Transaction(TransactionMode.Manual)]
    public class Code_ExportMaterialsToCSV : IExternalCommand
    {
        
        // Задание пути файлу со стандартными наименованиями материалов
        private string stMtlFilePath = OpenFile.OpenSingleFile(
            "Выберите файл со стандартными материалами", "csv"); 
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            //Основной код плагина НАЧАЛО
            
            try
            {
                // Обработка ошибки. Если будет нажат [ESC]
                if (stMtlFilePath.IsNullOrEmpty())
                    return Result.Cancelled;
                
                // Задание пути файлу с наименованиями материалов
                string mappingFilePath = CreateFile.CreateSingleFile(
                    "Сохранение файла Mapping с именами материалов", "csv");
                // Обработка исключений при щелчке правой кнопкой или нажатии [ESC]
                if (mappingFilePath.IsNullOrEmpty())
                    return Result.Cancelled;
                
                using (StreamWriter writer = new StreamWriter(mappingFilePath))
                {
                    foreach (string mtl in StandartMtlCleanerCSV(doc, stMtlFilePath))
                    {
                        writer.WriteLine(mtl);
                    }
                }
                
                NotificationManagerWPF.MessageSmileInfoTm("Инструкция для переименования",
                    "", 
                    $"\n\n 1. Откройте файл [ .csv ] в Libre Office" +
                        $"\n\n 2. Столбец 1 - старые имена. \n     Столбец 2 - новые имена" +
                        $"\n\n 3. Сохраните файл. \n     Это и есть файл Mapping" +
                        $"\n\n 4. Загрузите файл Mapping через кнопку\n     [ Импорт имен ]",
                    NotificationType.Information, 
                    20);
                
                NotificationManagerWPF.MessageSmileInfoTm("Создание файла Mapping",
                    "\n(￢‿￢)",
                    $"\n\nФайл с именами материалов создан!\n\nПуть к файлу: \n{mappingFilePath}",
                    NotificationType.Success, 
                    20);
            }
            // Обработка исключений при щелчке правой кнопкой или нажатии [ESC]
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }

            //Основной код плагина КОНЕЦ
            
            return Result.Succeeded;
        }
        
        /// <summary>
        /// Метод StandartMtlCleanerCSV - Выгружает имена материалов в таблицу CSV. Вычитает имена стандартных материалов.
        /// Алгоритм:
        /// 1. Загружает имена стандартных материалов для вычитания. Формат - таблица CSV.
        /// 2. Собирает имена материалов из документа. Формат - таблица CSV.
        /// 3. Вычитает совпавшие имена и записывает результат. Формат - таблица CSV.
        /// <param name = "filePath" > Путь к файлу Mapping </param>
        /// </summary>
        private List<string> StandartMtlCleanerCSV(Document doc, string stMtlFilePath)
        {
            //  Список №1
            // Загрузка в список стандартных имен материалов для их вычитания
            List<string> stMtlList = new List<string>();
            foreach (string line in File.ReadAllLines(stMtlFilePath))
            {
                stMtlList.Add(line);
            }
            //  Список №2
            // Сбор всех материалов в список
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Material));
            List<string> allMtlNamesInDoc = new List<string>();
            foreach (Material mat in collector.ToList())
            {
                allMtlNamesInDoc.Add(mat.Name);
            }
            //  Список №3
            // Вычитание стандартных материалов от всех материалов из документа
            List<string> result = new List<string>();
            result = allMtlNamesInDoc.Except(stMtlList).ToList();

            return result;
        }
    }
    
}