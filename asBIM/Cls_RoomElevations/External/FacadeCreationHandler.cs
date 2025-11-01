using Autodesk.Revit.UI;
using asBIM.Models;
using asBIM.Services;

namespace asBIM.External
{
    public class FacadeCreationHandler : IExternalEventHandler
    {
        public FacadeCreationSettings Settings { get; set; }

        public void Execute(UIApplication app)
        {
            FacadeCreationService.Instance.StartFacadeCreation(app, Settings);
        }

        public string GetName() => "Создание фасадов по линии";
    }
}