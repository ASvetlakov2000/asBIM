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

namespace asBIM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Code_TX_WriteSpaceParamFrSCV_ParamAdd : IExternalCommand
    {
        // Путь до ФОП выбором файла
        string filePathFprFOP =  OpenFile.OpenSingleFile("Выберите ФОП [PRO_SharedParametr] для добавления параметра", "txt");
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            // ОСНОВНОЙ КОД ПЛАГИНА // НАЧАЛО  
            
            // Добавление общих параметров 
            AddSharedParams(doc);
            
            // Задание функции "Параметр изменяется по экземплярам групп"
            SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(
                doc, SpatElemShParamInfo.spgAirTempCalc, true);
            SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(
                doc, SpatElemShParamInfo.spgAirTempMin, true);
            SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(
                doc, SpatElemShParamInfo.spgAirTempMax, true);
            SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(
                doc, SpatElemShParamInfo.spgFireCat, true);
            SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(
                doc, SpatElemShParamInfo.spgClnCls, true);
            SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(
                doc, SpatElemShParamInfo.spgHumanCountConst, true);
            SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(
                doc, SpatElemShParamInfo.spgHumanCountVar, true);
            SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(
                doc, SpatElemShParamInfo.spgSuppVnRt, true);
            SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(
                doc, SpatElemShParamInfo.spgExVnRt, true);
            
            
            // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ

            return Result.Succeeded;
        }
        
        
        private void AddSharedParams(Document doc)
        {
            try
            {
                // Категории для общего параметра
                List<Category> categories = new List<Category>
                {
                    doc.Settings.Categories.get_Item(BuiltInCategory.OST_MEPSpaces) // Пространства
                };

                // Добавление общего параметра
                SharedParameterHelper.AddSharedParameterFromFOP(
                    doc,
                    // путь к ФОП
                    filePathFprFOP,
                    // Имя создаваемого параметра
                    SpatElemShParamInfo.spnAirTempCalc,
                    // Куда относится параметр
                    BuiltInParameterGroup.PG_IDENTITY_DATA,
                    // Параметр для экземпляра
                    true,
                    // ссылка на список с категориями
                    categories,
                    false);
                
                // Добавление общего параметра
                SharedParameterHelper.AddSharedParameterFromFOP(
                    doc,
                    // путь к ФОП
                    filePathFprFOP,
                    // Имя создаваемого параметра
                    SpatElemShParamInfo.spnAirTempMin,
                    // Куда относится параметр
                    BuiltInParameterGroup.PG_IDENTITY_DATA,
                    // Параметр для экземпляра
                    true,
                    // ссылка на список с категориями
                    categories,
                    false);
                
                // Добавление общего параметра
                SharedParameterHelper.AddSharedParameterFromFOP(
                    doc,
                    // путь к ФОП
                    filePathFprFOP,
                    // Имя создаваемого параметра
                    SpatElemShParamInfo.spnAirTempMax,
                    // Куда относится параметр
                    BuiltInParameterGroup.PG_IDENTITY_DATA,
                    // Параметр для экземпляра
                    true,
                    // ссылка на список с категориями
                    categories,
                    false);
                
                // Добавление общего параметра
                SharedParameterHelper.AddSharedParameterFromFOP(
                    doc,
                    // путь к ФОП
                    filePathFprFOP,
                    // Имя создаваемого параметра
                    SpatElemShParamInfo.spnFireCat,
                    // Куда относится параметр
                    BuiltInParameterGroup.PG_IDENTITY_DATA,
                    // Параметр для экземпляра
                    true,
                    // ссылка на список с категориями
                    categories,
                    false);
                
                // Добавление общего параметра
                SharedParameterHelper.AddSharedParameterFromFOP(
                    doc,
                    // путь к ФОП
                    filePathFprFOP,
                    // Имя создаваемого параметра
                    SpatElemShParamInfo.spnClnCls,
                    // Куда относится параметр
                    BuiltInParameterGroup.PG_IDENTITY_DATA,
                    // Параметр для экземпляра
                    true,
                    // ссылка на список с категориями
                    categories,
                    false);
                
                // Добавление общего параметра
                SharedParameterHelper.AddSharedParameterFromFOP(
                    doc,
                    // путь к ФОП
                    filePathFprFOP,
                    // Имя создаваемого параметра
                    SpatElemShParamInfo.spnHumanCountConst,
                    // Куда относится параметр
                    BuiltInParameterGroup.PG_IDENTITY_DATA,
                    // Параметр для экземпляра
                    true,
                    // ссылка на список с категориями
                    categories,
                    false);
                
                // Добавление общего параметра
                SharedParameterHelper.AddSharedParameterFromFOP(
                    doc,
                    // путь к ФОП
                    filePathFprFOP,
                    // Имя создаваемого параметра
                    SpatElemShParamInfo.spnHumanCountVar,
                    // Куда относится параметр
                    BuiltInParameterGroup.PG_IDENTITY_DATA,
                    // Параметр для экземпляра
                    true,
                    // ссылка на список с категориями
                    categories,
                    false);
                
                // Добавление общего параметра
                SharedParameterHelper.AddSharedParameterFromFOP(
                    doc,
                    // путь к ФОП
                    filePathFprFOP,
                    // Имя создаваемого параметра
                    SpatElemShParamInfo.spnSuppVnRt,
                    // Куда относится параметр
                    BuiltInParameterGroup.PG_IDENTITY_DATA,
                    // Параметр для экземпляра
                    true,
                    // ссылка на список с категориями
                    categories,
                    false);
                
                // Добавление общего параметра
                SharedParameterHelper.AddSharedParameterFromFOP(
                    doc,
                    // путь к ФОП
                    filePathFprFOP,
                    // Имя создаваемого параметра
                    SpatElemShParamInfo.spnExVnRt,
                    // Куда относится параметр
                    BuiltInParameterGroup.PG_IDENTITY_DATA,
                    // Параметр для экземпляра
                    true,
                    // ссылка на список с категориями
                    categories,
                    false);
                
                
                // Уведомление. "Общий параметр добавлен!"
                NotificationManagerWPF.MessageInfo(
                    "Общие параметры добавлены!",
                    "\n(￢‿￢ )" +
                    "\n\nПараметры: " +
                    $"\n[{SpatElemShParamInfo.spnAirTempCalc}]" + 
                    $"\n[{SpatElemShParamInfo.spnAirTempMin}]" + 
                    $"\n[{SpatElemShParamInfo.spnAirTempMax}]" +
                    $"\n[{SpatElemShParamInfo.spnFireCat}]" +
                    $"\n[{SpatElemShParamInfo.spnClnCls}]" +
                    $"\n[{SpatElemShParamInfo.spnHumanCountConst}]" +
                    $"\n[{SpatElemShParamInfo.spnHumanCountVar}]" +
                    $"\n[{SpatElemShParamInfo.spnSuppVnRt}]" +
                    $"\n[{SpatElemShParamInfo.spnExVnRt}]"
                    , NotificationType.Success);
                    
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", ex.Message);
                throw;
            }
        }
    }
    
    /// <summary>
    /// Класс для хранения имен и Guid общих параметров Пространств.
    /// </summary>
    public class SpatElemShParamInfo
    {
        // Температура воздуха (расчет)
        public static string spnAirTempCalc = "ТХ_Пр-Температура воздуха (расчет)";
        public static  Guid spgAirTempCalc = new Guid("e88c1899-6706-4f45-91be-1711ca0e530c");
        // Температура воздуха (min)
        public static  string spnAirTempMin = "ТХ_Пр-Температура воздуха (min)";
        public static  Guid spgAirTempMin = new Guid("9ecc3fa4-f95d-4bde-aea7-b3d3349a11e1");
        // Пр-Температура воздуха (max)
        public static  string spnAirTempMax = "ТХ_Пр-Температура воздуха (max)";
        public static  Guid spgAirTempMax = new Guid("307da082-17c1-4e11-be06-3524cca92338");
        // Кат. ПБ
        public static  string spnFireCat = "ТХ_Пр-Кат. ПБ";
        public static  Guid spgFireCat = new Guid("42bf2b48-da99-4e15-9ab7-20ad06ca4e03");
        // Класс чистоты
        public static  string spnClnCls = "ТХ_Пр-Класс чистоты";
        public static  Guid spgClnCls = new Guid("07cb9950-6e4a-4940-b0a7-261821b1dcbc");
        // Пост. кол. чел.
        public static  string spnHumanCountConst = "ТХ_Пр-Пост. кол. чел.";
        public static  Guid spgHumanCountConst = new Guid("a4aea630-fde2-4f92-a760-f6b9e5404663");
        // Врем. кол. чел
        public static  string spnHumanCountVar = "ТХ_Пр-Врем. кол. чел.";
        public static  Guid spgHumanCountVar = new Guid("afada1f1-e170-4f6d-ad4a-8da3c2727593");
        // Кратность приточная
        public static  string spnSuppVnRt = "ТХ_Пр-Кратность приточная";
        public static  Guid spgSuppVnRt = new Guid("f94cc98b-3358-4aa6-aa4c-81d04a6b3311");
        // Кратность вытяжная
        public static  string spnExVnRt = "ТХ_Пр-Кратность вытяжная";
        public static  Guid spgExVnRt = new Guid("2d5827df-b957-4ca4-9be8-cb9ce30914b2");
    }
}