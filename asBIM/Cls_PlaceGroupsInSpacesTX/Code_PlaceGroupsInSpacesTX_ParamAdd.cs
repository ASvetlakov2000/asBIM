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
using System.Xaml;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Windows;
using asBIM.ViewModel;
using Notifications.Wpf;
using CommunityToolkit.Mvvm.Input;
using Nice3point.Revit.Extensions;
using asBIM;

namespace asBIM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Code_PlaceGroupsInSpacesTX_ParamAdd : IExternalCommand
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
            
            // TODO: Добавить проверку на наличие параметра
                try
                {
                    // Добавление общего параметра [PRO_ТХ_Группа в пространстве]
                    List<Category> categories = new List<Category>
                    {
                        doc.Settings.Categories.get_Item(BuiltInCategory.OST_MEPSpaces), // Пространства
                        doc.Settings.Categories.get_Item(BuiltInCategory.OST_IOSModelGroups), // Группы
                    };
                    
                    // TODO: Вызывается каждый раз при выполнении. Исправить
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
                        categories);
                    
                    // Уведомление. "Общий параметр добавлен!"
                    NotificationManagerWPF.MessageInfo(
                        "Общий параметр добавлен!",
                        "\n(￢‿￢ )" +
                        "\n\nПараметр: " +
                        "\n[PRO_ID группы в пространстве] добавлен для Пространств!" +
                        "\n\nВ параметр записывается ID группы, которая была добавлена в пространство" +
                        "\n\nПример заполнения: 010101", NotificationType.Success);
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