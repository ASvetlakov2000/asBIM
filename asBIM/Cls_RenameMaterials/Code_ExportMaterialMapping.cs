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
        private string standartMltFilePath = "\\\\serverproxima\\users$\\Profiles\\svetlakov.a\\Desktop\\C#\\00_Revit Plugins\\04_Модели_Тест\\2.1_Материалы\\Мэппинги\\txt\\Mlt_St.txt";
        
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
                // Запись в файл .txt всех имен материалов из документа
                using (StreamWriter writer = new StreamWriter(mappingFilePath))
                {
                    // TODO: 1. Проверка на наличие стандартных материалов и их пропуск
                    FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(Material));
                    
                    foreach (Material mat in collector)
                    {
                        // Запись по строчно "Имя материала + табуляция"
                        writer.WriteLine($"{mat.Name}");
                    }
                }

                NotificationManagerWPF.MessageSmileInfo("Создание файла Mapping",
                    "\n(￢‿￢)",
                    $"\n\nФайл с именами материалов создан!\n\nПуть к файлу: \n{mappingFilePath}",
                    NotificationType.Information);
            }
            // Обработка исключений при щелчке правой кнопкой или нажатии [ESC]
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }

        private List<string> StandartMltCkecker (string standartMltFilePath)
        {
            List<string> lst = new List<string>();

            foreach (string line in File.ReadAllLines(standartMltFilePath))
            {
                lst.Add(line);
            }
            return lst;
        }
    }
}
