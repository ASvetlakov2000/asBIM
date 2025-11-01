using Autodesk.Revit.UI;
using asBIM.Commands;
using asBIM.External;
using asBIM.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Autodesk.Revit.DB;
using System.Linq;

namespace RevitRoomUnfoldPlugin.ViewModels
{
    public class CreateFacadesViewModel : INotifyPropertyChanged
    {
        public FacadeCreationSettings Settings { get; set; } = new FacadeCreationSettings();

        private FacadeCreationHandler handler;
        private ExternalEvent externalEvent;

        public ICommand CreateFacadesCommand { get; }

        public List<string> ViewTemplates { get; set; } = new List<string>();

        public CreateFacadesViewModel(UIApplication uiApp)
        {
            // Загружаем шаблоны вида Elevation
            var doc = uiApp.ActiveUIDocument.Document;
            var collector = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .OfType<View>()
                .Where(v => v.IsTemplate && v.ViewType == ViewType.Elevation);

            foreach (var v in collector)
            {
                ViewTemplates.Add(v.Name);
            }

            handler = new FacadeCreationHandler();
            externalEvent = ExternalEvent.Create(handler);

            CreateFacadesCommand = new RelayCommand(execute =>
            {
                handler.Settings = Settings;
                externalEvent.Raise();
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void Raise(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}