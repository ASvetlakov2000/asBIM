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
    public class Code_ExportMaterialsFrLinksToCSV : IExternalCommand
    {
        
        // Задание пути файлу со стандартными наименованиями материалов
        // private string stMtlFilePath = OpenFile.OpenSingleFile(
        //     "Выберите файл со стандартными материалами", "csv"); 
        
        // Задание пути файлу со стандартными наименованиями материалов. Хардкодом
        private string stMtlFilePath = 
            "\\\\omv\\share\\projects\\2024_04_Рублево-Архангельское_Гравион_Квартал Г03\\05_ДД\\00_Координация" +
            "\\Проверки_RA\\Отработка замечаний\\Настройки Revit\\01_Переименование материалов\\Standart_Mtl_List.csv";
        
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
        /// Метод StandartMtlCleanerCSV - Выгружает имена материалов из связанных моделей, исключая стандартные.
        /// 1. Загружает имена стандартных материалов из CSV.
        /// 2. Извлекает материалы из связанных моделей (RevitLinkInstance).
        /// 3. Исключает стандартные.
        /// </summary>
        private List<string> StandartMtlCleanerCSV(Document doc, string stMtlFilePath)
        {
            // Список №1 — загрузка стандартных имен материалов
            List<string> stMtlList = new List<string>();
            foreach (string line in File.ReadAllLines(stMtlFilePath))
            {
                stMtlList.Add(line);
            }

            // Список №2 — сбор всех материалов из связанных документов
            List<string> allMtlNamesInLinkedDocs = new List<string>();

            // Получаем все связанные экземпляры моделей
            FilteredElementCollector linkInstances = new FilteredElementCollector(doc)
                .OfClass(typeof(RevitLinkInstance));

            foreach (RevitLinkInstance linkInstance in linkInstances)
            {
                // Получаем связанный документ
                Document linkedDoc = linkInstance.GetLinkDocument();

                // Проверяем, что документ доступен
                if (linkedDoc == null) continue;

                // Собираем все материалы из связанного документа
                FilteredElementCollector mtlCollector = new FilteredElementCollector(linkedDoc)
                    .OfClass(typeof(Material));

                foreach (Material mat in mtlCollector)
                {
                    allMtlNamesInLinkedDocs.Add(mat.Name);
                }
            }

            // Список №3 — исключение стандартных материалов
            List<string> result = allMtlNamesInLinkedDocs
                .Except(stMtlList)
                .Distinct() // Убираем дубликаты, если одно и то же имя встречается в нескольких моделях
                .ToList();

            return result;
        }
    }
    
}