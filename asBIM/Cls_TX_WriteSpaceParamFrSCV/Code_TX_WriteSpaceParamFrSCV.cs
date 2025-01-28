using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using asBIM.Helpers;
using System;
using System.Collections.Generic;
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
using System.IO;

namespace asBIM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Code_TX_WriteSpaceParamFrSCV : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            // ОСНОВНОЙ КОД ПЛАГИНА // НАЧАЛО  
            
            
            
            // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ

            return Result.Succeeded;
        }
        
        
        

    }

    public class SpaceUpdater
    {
        // Путь к файлу CSV
        private string csvFilePath = OpenFile.OpenSingleFile("Выберите шаблон для записи параметров в Пространства","csv");
        
        /// <summary>
        /// Метод для загрузки данных из CSV-файла и преобразования их в список объектов SpaceData.
        /// <param name="filePath">Путь к файлу CSV</param>>
        /// <returns>CsvReader</returns>>
        /// </summary>
        public List<SpaceData> LoadCsv(string filePath)
        {
            // Метод для загрузки данных из CSV-файла и преобразования их в список объектов SpaceData.
            using (var reader = new StreamReader(csvFilePath)) // Создаём поток чтения файла по указанному пути.
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)) 
            {
                // Создаём объект CsvReader для считывания содержимого файла. Указываем культуру, чтобы правильно обрабатывать разделители.
                return csv.GetRecords<SpaceData>().ToList(); 
                // Читаем записи из CSV-файла и преобразуем их в список объектов SpaceData.
            }
        }

        // TODO: 1. Возможны изменения параметров поиска Пространств
        /// <summary>
        /// Метод для сбора Пространств.  Сортировка +- по номеру
        /// <param name = "doc" > Документ </param>
        /// <returns>centerOfSpace - список с Пространствами, отсортированный по возрастанию значению номера (целое)</returns>
        /// </summary>
        public List<SpatialElement> GetSpacesFromDoc(Document doc)
        {
            string spaceFullName;
            string spaceClearName;
            
            // Замена "," на "."
            NumberFormatInfo format = new NumberFormatInfo {NumberDecimalSeparator = "."};
            // Сбор Пространств в коллекцию с сортировкой по категории и выбор только элементов
            FilteredElementCollector collectorSpaces = new FilteredElementCollector(doc);
            collectorSpaces.OfCategory(BuiltInCategory.OST_MEPSpaces).ToElements();
            // Сбор Пространств в список.
            // Сортировка Пространств в списке по Номеру Пространств
            List<SpatialElement> spacesList = collectorSpaces.Cast<SpatialElement>()
                // Конвертация номера Пространств из строки в целое
                .OrderBy(space => Convert.ToString(space.Number, format))
                .ToList();

            foreach (SpatialElement space in spacesList)
            {
                spaceFullName = space.Name;
                spaceClearName = Regex.Replace(spaceFullName, @"\s[^\s]+$", "");
                if (spaceFullName == spaceClearName)
                {
                    NotificationManagerWPF.MessageInfoSmile(
                        "Ошибка!\nВ проекте пространства без имени!",
                        $"\nПространство: [№] - [{spaceClearName}],  [ID] - [{space.Id}]",
                        "\n\u00af\u00af\u00af\u00af\u00af\u00af\\_(ツ)_/\u00af\u00af\u00af" +
                        "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af" +
                        "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af",
                        NotificationType.Error);
                }
            }
            
            return spacesList;
        }
    }

    /// <summary>
    /// Класс для хранения данных из CSV-файла.
    /// </summary>
    public class SpaceData
    {
        public string SpaceName { get; set; } // Имя пространства
        public double SpaceAirTempCalc { get; set; } // Температура воздуха (расчет)
        public double SpaceAirTempMin { get; set; } // Температура воздуха (min)
        public double SpaceAirTempMax { get; set; } // Пр-Температура воздуха (max)
        public string SpaceFireCat { get; set; } // Кат. ПБ
        public string SpaceClnCls { get; set; } // Класс чистоты
        public int SpaceHumanCountConst { get; set; } // Пост. кол. чел.
        public int SpaceHumanCountVar { get; set; } // Врем. кол. чел
        public string SpaceSuppVnRt { get; set; } // Кратность приточная
        public string SpaceExVnRt { get; set; } // Кратность вытяжная
    }

}