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
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class Code_ExportMaterialMapping : IExternalCommand
    {
        // Задание пути файлу со стандартными наименованиями материалов
        // string stMtlFilePath = OpenFile.OpenSingleFile(
        //     "Выберите файл со стандартными материалами", "txt");

        private string stMtlFilePath = "S:\\standarts\\00_BIM\\BIM_Автоматизация\\Материалы_Переименование\\Standart_Mtl_List.txt";
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            try
            {
                // Задание пути файлу с наименованиями материалов
                string mappingFilePath = CreateFile.CreateSingleFile(
                    "Сохранение файла Mapping с именами материалов", "txt");
                // Обработка исключений при щелчке правой кнопкой или нажатии [ESC]
                if (mappingFilePath.IsNullOrEmpty())
                    return Result.Cancelled;
                
                using (StreamWriter writer = new StreamWriter(mappingFilePath))
                {
                    foreach (string mtl in StandartMtlCleaner(stMtlFilePath, doc))
                    {
                        writer.WriteLine(mtl);
                    }
                }

                NotificationManagerWPF.MessageSmileInfoTm("Создание файла Mapping",
                    "\n(￢‿￢)",
                    $"\n\nФайл с именами материалов создан!\n\nПуть к файлу: \n{mappingFilePath}",
                    NotificationType.Success, 
                    20);
                
                NotificationManagerWPF.MessageSmileInfoTm("Инструкция для переименования",
                    "",
                    $"\n 1. Сохраните созданный файл по ссылке ниже в формат [ .csv ] путем переименования расширения" +
                    $"\n\n 2. Откройте файл формата [ .csv ] в Libre Office" +
                    $"\n\n 3. Слева - материалы из модели. В столбец справа от имен материалов впишите новые имена" +
                    $"\n\n 4. Сохраните файл и переименуйте обратно в [ .txt ] - Это и есть файл Mapping" +
                    $"\n\n 5. Загрузите файл Mapping через кнопку [ Импорт имен ]",
                    NotificationType.Information, 
                    20);
            }
            // Обработка исключений при щелчке правой кнопкой или нажатии [ESC]
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// Метод StandartMtlCleaner - 
        /// <param name = "filePath" > Путь к файлу Mapping </param>
        /// </summary>
        public List<string> StandartMtlCleaner(string stMtlFilePath ,Document doc)
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
