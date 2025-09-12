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

namespace asBIM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

       public static class ScheduleHelper
    {
        public const string SharedParameterName = "ADSK_Этаж";
        private const string SheetNume = "111";
        private const string SheetName = "Атрибуты";

        public static void CreateSchedulesForCategories(Document doc, List<BuiltInCategory> categories)
        {
            var columnWidths = new Dictionary<string, double>
            {
                { "Семейство и типоразмер", 100 },
                { "Уровень", 40 },
                { SharedParameterName, 20 }
            };

            using (Transaction trans = new Transaction(doc, "Создание спецификаций"))
            {
                trans.Start();

                // ⚡ Получаем или создаём лист
                ViewSheet sheet = CreateSheet(doc);
                double yOffset = 0; // координата Y для размещения спецификаций на листе

                foreach (BuiltInCategory bic in categories)
                {
                    Category category = Category.GetCategory(doc, bic);
                    if (category == null) continue;

                    ViewSchedule schedule = ViewSchedule.CreateSchedule(doc, new ElementId(bic));
                    if (schedule == null) continue;

                    schedule.Name = "Атрибуты_" + category.Name;

                    ScheduleDefinition definition = schedule.Definition;

                    // ⚡ Добавляем стандартные поля
                    definition.AddField(ScheduleFieldType.Instance, new ElementId(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM));
                    // definition.AddField(ScheduleFieldType.Instance, new ElementId(BuiltInParameter.LEVEL_PARAM));
                    AddFieldByName(schedule, SharedParameterName);

                    // ⚡ Отключаем группировку экземпляров
                    definition.IsItemized = false;

                    // ⚡ Настройка сортировки
                    ScheduleField familyField = null;
                    ScheduleField adskeField = null;

                    foreach (ScheduleFieldId fieldId in definition.GetFieldOrder())
                    {
                        ScheduleField field = definition.GetField(fieldId);
                        string name = field.GetName();

                        if (field.FieldType == ScheduleFieldType.Instance &&
                            field.ParameterId == new ElementId(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM))
                        {
                            familyField = field;
                        }

                        if (name == SharedParameterName)
                        {
                            adskeField = field;
                        }
                    }

                    if (familyField != null)
                        definition.AddSortGroupField(new ScheduleSortGroupField(familyField.FieldId));

                    if (adskeField != null)
                        definition.AddSortGroupField(new ScheduleSortGroupField(adskeField.FieldId));

                    // ⚡ Устанавливаем ширину столбцов
                    SetColumnWidths(schedule, columnWidths);

                    // ⚡ Записываем значение в параметр "ADSK_Назначение вида"
                    Parameter sharedParam = schedule.LookupParameter("ADSK_Назначение вида");
                    if (sharedParam != null && !sharedParam.IsReadOnly)
                        sharedParam.Set("Атрибуты");
                    

                    // ⚡ Размещаем спецификацию на листе
                    PlaceScheduleOnSheet(doc, schedule, sheet, yOffset);

                    // ⚡ Смещаем Y для следующей спецификации
                    yOffset += 0.45;
                }

                trans.Commit();
            }
        }

        public static void AddFieldByName(ViewSchedule schedule, string paramName)
        {
            ScheduleDefinition definition = schedule.Definition;

            foreach (SchedulableField sf in definition.GetSchedulableFields())
            {
                if (sf.GetName(schedule.Document) == paramName)
                {
                    definition.AddField(sf);
                    break;
                }
            }
        }

        public static void SetColumnWidths(ViewSchedule schedule, Dictionary<string, double> columnWidthsMm)
        {
            ScheduleDefinition definition = schedule.Definition;

            foreach (ScheduleFieldId fieldId in definition.GetFieldOrder())
            {
                ScheduleField scheduleField = definition.GetField(fieldId);
                string fieldName = scheduleField.GetName();

                if (columnWidthsMm.ContainsKey(fieldName))
                {
                    double widthMm = columnWidthsMm[fieldName];
                    double widthFeet = UnitUtils.ConvertToInternalUnits(widthMm, UnitTypeId.Millimeters);
                    scheduleField.GridColumnWidth = widthFeet;
                }
            }
        }

        private static ViewSheet CreateSheet (Document doc)
        {
            var titleBlock = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .WhereElementIsElementType().FirstOrDefault();
            
            var sheet = ViewSheet.Create(titleBlock.Document, titleBlock.Id);
            sheet.SheetNumber = SheetNume;
            sheet.Name=SheetName;
            
            return sheet;
        }

        private static void PlaceScheduleOnSheet(Document doc, ViewSchedule schedule, ViewSheet sheet, double offsetY)
        {
            if (schedule == null || sheet == null) return;

            XYZ location = new XYZ(offsetY, 0, 0);
            ScheduleSheetInstance.Create(doc, sheet.Id, schedule.Id, location);
        }
    }
}
