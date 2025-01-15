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
using Autodesk.Revit.UI.Selection;


namespace asBIM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Cls_DeleteGroupFromSpaceTX : IExternalCommand
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
            DeleteSelectionGroup deleteSelGroup = new DeleteSelectionGroup();
            
            // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ

            return Result.Succeeded;
        }
        
        
    }
    
    
    
    internal class DeleteSelectionGroup : ISelectionFilter
    {
        // Guid groupGuid, Guid spaceGuid,
        
        public void DeleteGroupFromSpace(Document doc, ref string message)
        {
            /*Reference pickedRef = null;
            Selection selection;
            Group deletingGroup  = selection.PickObject(ObjectType.Element);
            ElementId elementIdToDelete = deletingGroup.Id;
            using (Transaction transaction = new Transaction(doc, "Delete Element"))
            {
                transaction.Start();
                try
                {
                    // Удаление элемента
                    doc.Delete(elementIdToDelete);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Обработка ошибок
                    message = ex.Message;
                    transaction.RollBack();
                }
            }*/
        }
        public bool AllowElement(Element element)
        {
            return true;
        }

        public bool AllowReference(Reference reference, XYZ point)
        {
            return false;
        }
    }
}