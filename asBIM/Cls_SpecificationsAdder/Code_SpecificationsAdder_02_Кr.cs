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

    public class SpAdderKr : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            var categories = new List<BuiltInCategory>
            {
                BuiltInCategory.OST_StructuralColumns,    // Колонны
                BuiltInCategory.OST_StructuralFraming,    // Балки, фермы
                BuiltInCategory.OST_StructuralFoundation, // Фундаменты
                BuiltInCategory.OST_Walls,                // Стены
                BuiltInCategory.OST_Floors,               // Перекрытия
                BuiltInCategory.OST_Windows,              // Окна
                BuiltInCategory.OST_Doors,                // Двери
                BuiltInCategory.OST_StructuralStiffener,  // Рёбра плит
                BuiltInCategory.OST_GenericModel          // Обобщенные модели
            };

            ScheduleHelper.CreateSchedulesForCategories(doc, categories);
            return Result.Succeeded;
        }
    }
}
