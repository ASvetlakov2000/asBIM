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



namespace asBIM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Code_DeleteGroupFromSpaceTX : IExternalCommand
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
            
            DeleteGroupFromSpace(doc, uiapp);
            
            // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ

            return Result.Succeeded;
        }

        public void DeleteGroupFromSpace(Document doc, UIApplication uiapp)
        {
            Reference pickedRef = null;
            Selection sel = uiapp.ActiveUIDocument.Selection;
            pickedRef = sel.PickObject(ObjectType.Element, "Выберите группу для удаления");
            Element pickedElement = doc.GetElement(pickedRef);
            Group group = pickedElement as Group;

            if (group != null)
            {
                ElementId pickedGroup = group.Id;
                using (Transaction transaction = new Transaction(doc, "Delete Element"))
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
        
        public void ClearSpaceParamValue(Document doc, Group group)
        {
            // Сбор Пространств в коллекцию с сортировкой по категории и выбор только элементов
            FilteredElementCollector collectorSpaces = new FilteredElementCollector(doc);
            collectorSpaces.OfCategory(BuiltInCategory.OST_MEPSpaces).ToElements();
            // Сбор Пространств в список.
            // Сортировка Пространств в списке по Номеру Пространств
            List<SpatialElement> spacesList = collectorSpaces.Cast<SpatialElement>()
                // Конвертация номера Пространств из строки в целое
                .OrderBy(space => Convert.ToDouble(space.Number))
                .ToList();

            foreach (SpatialElement space in spacesList)
            {
                Parameter spaceShParamID = space.get_Parameter(shParamGuid);
                string spaceShParamIDString = spaceShParamID.AsString();
                if (spaceShParamIDString == group.Id.ToString())
                {
                    spaceShParamID.Set("Test");
                }
            }
        }
    }
    
}