using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
using Autodesk.Revit.ApplicationServices;

namespace asBIM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class SpAdderEom : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            var categories = new List<BuiltInCategory>
            {
                BuiltInCategory.OST_LightingFixtures,           // Осветительные приборы
                BuiltInCategory.OST_ElectricalEquipment,        // Электрооборудование
                BuiltInCategory.OST_DataDevices,                // Сетевые устройства
                BuiltInCategory.OST_CommunicationDevices,       // Телефон/коммуникации
                BuiltInCategory.OST_CableTray,                  // Кабельные лотки
                BuiltInCategory.OST_CableTrayFitting,           // Соединительные детали кабельных лотков
                BuiltInCategory.OST_SpecialityEquipment,        // Специальное оборудование
                BuiltInCategory.OST_GenericModel,               // Обобщенные модели
                BuiltInCategory.OST_ElectricalFixtures          // Электрические приборы
            };

            ScheduleHelper.CreateSchedulesForCategories(doc, categories);
            return Result.Succeeded;
        }
    }
}
