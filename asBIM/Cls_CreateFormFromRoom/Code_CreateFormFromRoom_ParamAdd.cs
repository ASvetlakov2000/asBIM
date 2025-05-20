using System;
using System.Collections.Generic;
using asBIM.Helpers;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Notifications.Wpf;

namespace asBIM
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Code_CreateFormFromRoom_ParamAdd : IExternalCommand
    {
        // Guid общего параметра "PRO_ID помещения"
        Guid shParamGuid01 = new Guid("f0737b4b-8343-47e5-9018-b9e05378fa33");
        // Имя общего параметра "PRO_ID помещения"
        string shParamName01 = "PRO_ID помещения";
        
        // Guid общего параметра "PRO_ID помещения"
        Guid shParamGuid02 = new Guid("ab4bf165-1419-4fef-b80a-3d1b38b5ec23");
        // Имя общего параметра "PRO_ID помещения"
        string shParamName02 = "PRO_Назначение помещения";

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            // ОСНОВНОЙ КОД ПЛАГИНА // НАЧАЛО  

            string fopPath =
                OpenFile.OpenSingleFile("Выберите ФОП [PRO_SharedParametr] для добавления параметра", "txt");
            
                try
                {
                    // Добавление общего параметра [PRO_ТХ_Группа в пространстве]
                    List<Category> categories = new List<Category>
                    {
                        doc.Settings.Categories.get_Item(BuiltInCategory.OST_Mass), // Формы
                    };

                    SharedParameterHelper.AddSharedParameterFromFOP(
                        doc,
                        // путь к ФОП
                        fopPath,
                        // Имя создаваемого параметра
                        shParamName01,
                        // Куда относится параметр
                        BuiltInParameterGroup.PG_IDENTITY_DATA,
                        // Параметр для экземпляра
                        true,
                        // ссылка на список с категориями
                        categories,
                        false);
                    
                    // Задание функции "Параметр изменяется по экземплярам групп"
                    SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(doc, shParamGuid01, true);
                    
                    // Уведомление. "Общий параметр добавлен!"
                    NotificationManagerWPF.MessageSmileInfoTm(
                        "Общий параметр добавлен!",
                        "\n(￢‿￢ )", 
                        "\n\nПараметры: " +
                        "\n 1. [PRO_ID помещения] добавлен для Форм!" +
                        "\n\nВ параметр Формы записывается ID помещения с которого была создана форма" +
                        "\n\nПример заполнения: 010101"+ 
                        "\n\n\n 2. [PRO_Назначение помещения]"+
                        "\n\n В параметр Формы записывается значение из параметра Назначение из Помещения", NotificationType.Success, 15);
                    
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Ошибка", ex.Message);
                    throw;
                }
                
                // 2
                
                try
                {
                    // Добавление общего параметра [PRO_ТХ_Группа в пространстве]
                    List<Category> categories = new List<Category>
                    {
                        doc.Settings.Categories.get_Item(BuiltInCategory.OST_Mass), // Формы
                    };

                    SharedParameterHelper.AddSharedParameterFromFOP(
                        doc,
                        // путь к ФОП
                        fopPath,
                        // Имя создаваемого параметра
                        shParamName02,
                        // Куда относится параметр
                        BuiltInParameterGroup.PG_IDENTITY_DATA,
                        // Параметр для экземпляра
                        true,
                        // ссылка на список с категориями
                        categories,
                        false);
                    
                    // Задание функции "Параметр изменяется по экземплярам групп"
                    SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(doc, shParamGuid02, true);
                    
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Ошибка", ex.Message);
                    throw;
                }
                
                //2
                
                // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ

            return Result.Succeeded;
        }
    }
}