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

    public class SpAdderAr : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            var categories = new List<BuiltInCategory>
            {
                BuiltInCategory.OST_Walls,                // Стены
                BuiltInCategory.OST_Floors,               // Полы
                BuiltInCategory.OST_Roofs,                // Крыши
                BuiltInCategory.OST_Windows,              // Окна
                BuiltInCategory.OST_Doors,                // Двери
                BuiltInCategory.OST_Stairs,               // Лестницы
                BuiltInCategory.OST_Railings,             // Ограждения
                BuiltInCategory.OST_GenericModel,         // Обобщенные модели
                BuiltInCategory.OST_SpecialityEquipment,  // Специальное оборудование
                BuiltInCategory.OST_Columns,              // Колонны
                BuiltInCategory.OST_Furniture,            // Мебель
                BuiltInCategory.OST_PlumbingFixtures,     // Сантехника
                BuiltInCategory.OST_ElectricalEquipment   // Электрооборудование
            };

            ScheduleHelper.CreateSchedulesForCategories(doc, categories);
            return Result.Succeeded;
        }
    }
}
