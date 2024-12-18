using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace asBIM
{
    [Transaction(TransactionMode.Manual)]
    // ВЫВОД ИНФОРМАЦИИ О ПЛАГИНЕ  
    public class Info : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            //Основной код плагина НАЧАЛО  
            TaskDialog.Show("Информация о плагине asBIM", "Плагин поможет: \n1. Записать параметры для элементов\n ...\n\nВнимание! Плагин разработан дилетантом\n\nПеред использованием сохраните свои изменения!");
            //Основной код плагина КОНЕЦ  
            return Result.Succeeded;
        }
    }
}