using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using RevitRoomUnfoldPlugin.Views;
using System;

namespace asBIM.Commands
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class ShowCreateFacadesWindow : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            try
            {
                UIApplication uiApp = commandData.Application;

                var window = new CreateFacadesWindow(uiApp);
                window.Owner = System.Windows.Application.Current.MainWindow;
                window.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}