using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using Autodesk.Revit.UI.Selection;
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

namespace asBIM.Cls_TX_PlaceGroupsInSpaces
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    
    public class Code_TX_PlaceGroupsInSpaces_DelGrp : IExternalCommand
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
                DeleteGroupFromSpace(doc, uiapp);
            }
            // Обработка исключений при щелчке правой кнопкой или нажатии ESC
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ

            return Result.Succeeded;
        }

        /// <summary>
        /// Метод для удаления групп после расстановки с помощью Code_TX_PlaceGroupsInSpaces.
        /// Удаляет группы и значение в параметре "PRO_ID группы в пространстве"
        /// <param name = "doc" > Документ </param>
        /// <param name = "uiapp" > Документ, интерфейс </param>
        /// </summary>
        public void DeleteGroupFromSpace(Document doc, UIApplication uiapp)
        {
            Reference pickedRef;
            Selection sel = uiapp.ActiveUIDocument.Selection;
            pickedRef = sel.PickObject(ObjectType.Element, "Выберите группу для удаления");
            Element pickedElement = doc.GetElement(pickedRef);
            Group group = pickedElement as Group;

            if (group != null)
            {
                ElementId pickedGroup = group.Id;
                using (Transaction transaction = new Transaction(doc, "Удалить группу"))
                {
                    transaction.Start();
                    try
                    {
                        ClearSpaceParamValue(doc, group);
                        doc.Delete(pickedGroup);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("Ошибка!", ex.Message);
                        transaction.RollBack();
                    }
                }
            }
            else
                NotificationManagerWPF.MessageInfoSmile(
                    "Удаление группы", 
                    "\nУдалять можно только группы!",
                    "\n\u00af\u00af\u00af\u00af\u00af\u00af\\_(ツ)_/\u00af\u00af\u00af\u00af" +
                    "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af", 
                    NotificationType.Warning);
            
        }
        
        /// <summary>
        /// Удаляет значение в параметре "PRO_ID группы в пространстве"
        /// <param name = "doc" > Документ </param>
        /// <param name = "group" > Группа, в которой удаляется значение "PRO_ID группы в пространстве"</param>
        /// </summary>
        public void ClearSpaceParamValue(Document doc, Group group)
        {
            // Сбор Пространств в коллекцию с сортировкой по категории и выбор только элементов
            FilteredElementCollector collectorSpaces = new FilteredElementCollector(doc);
            collectorSpaces.OfCategory(BuiltInCategory.OST_MEPSpaces).ToElements();
            // Сбор Пространств в список.
            List<SpatialElement> spacesList = collectorSpaces.Cast<SpatialElement>().ToList();

            foreach (SpatialElement space in spacesList)
            {
                Parameter spaceShParamId = space.get_Parameter(shParamGuid);
                string spaceShParamIdString = spaceShParamId.AsString();
                
                if (spaceShParamIdString == group.Id.ToString())
                {
                    spaceShParamId.Set(String.Empty);
                }
            }
        }
    }
    
}
