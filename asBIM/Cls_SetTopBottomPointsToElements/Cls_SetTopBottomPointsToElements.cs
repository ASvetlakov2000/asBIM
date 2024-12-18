using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asBIM
{ 
    [Transaction(TransactionMode.Manual)]
    // Заменить имя класса "Cls_SetTopBottomPointsToElements"  
    public class Cls_SetTopBottomPointsToElements : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            //Основной код плагина НАЧАЛО  

            public XYZ GetElementCenter(Element elem) 
            { 

            }

            //Основной код плагина КОНЕЦ  
            return Result.Succeeded;
        }
    }
}