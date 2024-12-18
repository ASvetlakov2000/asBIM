using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asBIM;

namespace asBIM
{
    [Transaction(TransactionMode.Manual)]
    // ЗАПИСЬ ПАРАМЕТРА В ЭЛЕМЕНТ, ВИДИМЫЙ НА ВИДЕ 
    public class Code_SetParamertToVisibleElementInActiveView : IExternalCommand
    {
        static Document _doc;
        static Helpers _help;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            _doc = doc;
            _help = new Helpers();

           //Вызов окна
            Form_SetParamertToVisibleElementInActiveView form = new Form_SetParamertToVisibleElementInActiveView();
            form.ShowDialog();

            TaskDialog.Show("Запись параметра", "Запись параметра завершена!");

            return Result.Succeeded;
        }

        //Запись параметра
        public void setParameter(string parametrName, string value)
        {
            List<Element> elements = _help.AllElementsInActiveView(_doc).ToList();
            foreach (var el in elements)
            {
                var parName = el.GetParameters(parametrName).FirstOrDefault();
                using (Transaction tr = new Transaction(_doc, "Запись параметра"))
                {
                    tr.Start();
                    try
                    {
                        parName.Set(value);
                    }
                    catch { }
                    
                    tr.Commit();
                }
            }
        }
    }
}