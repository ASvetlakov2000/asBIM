using System;
using System.Collections.Generic;
using asBIM.Helpers;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Notifications.Wpf;

namespace asBIM.Cls_TX_PlaceGroupsInSpaces
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Code_TX_PlaceGroupsInSpaces_ParamAdd : IExternalCommand
    {
        // Guid общего параметра "PRO_ID группы в пространстве"
        Guid shParamGuid = new Guid("bae21547-43fa-423f-91f9-ff8b42d50560");

        // Имя общего параметра "PRO_ID группы в пространстве
        string shParamName = "PRO_ID группы в пространстве";

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            // ОСНОВНОЙ КОД ПЛАГИНА // НАЧАЛО  
            
                try
                {
                    // Добавление общего параметра [PRO_ТХ_Группа в пространстве]
                    List<Category> categories = new List<Category>
                    {
                        doc.Settings.Categories.get_Item(BuiltInCategory.OST_MEPSpaces), // Пространства
                        doc.Settings.Categories.get_Item(BuiltInCategory.OST_IOSModelGroups), // Группы
                    };

                    SharedParameterHelper.AddSharedParameterFromFOP(
                        doc,
                        // путь к ФОП
                        OpenFile.OpenSingleFile("Выберите ФОП [PRO_SharedParametr] для добавления параметра", "txt"),
                        // Имя создаваемого параметра
                        shParamName,
                        // Куда относится параметр
                        BuiltInParameterGroup.PG_IDENTITY_DATA,
                        // Параметр для экземпляра
                        true,
                        // ссылка на список с категориями
                        categories,
                        false);
                    
                    // Уведомление. "Общий параметр добавлен!"
                    // NotificationManagerWPF.MessageInfo(
                    //     "Общий параметр добавлен!",
                    //     "\n(￢‿￢ )" +
                    //     "\n\nПараметр: " +
                    //     "\n[PRO_ID группы в пространстве] добавлен для Пространств!" +
                    //     "\n\nВ параметр записывается ID группы, которая была добавлена в пространство" +
                    //     "\n\nПример заполнения: 010101", NotificationType.Success);
                    // Добавление общего параметра [PRO_ТХ_Группа в пространстве]
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Ошибка", ex.Message);
                    throw;
                }
                
                // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ

            return Result.Succeeded;
        }
    }
}