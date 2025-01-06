using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.Input;
using asBIM;

namespace asBIM.ViewModel
{
    public class SetMaxMinPtToElements_ViewModel
    {
        private UIApplication uiApp { get; }
        private UIDocument uidoc => uiApp.ActiveUIDocument;
        private Document doc => uidoc.Document;
        private ViewModelEvent InternalEvent { get; }
        
        public SetMaxMinPtToElements_ViewModel(UIApplication uiApp)
        {
            this.uiApp = uiApp;
            InternalEvent = new ViewModelEvent();
        }
        

        public ICommand CodeSetMaxMinPtToElements => new RelayCommand(DoSmth, CanDoSmth);
        private void DoSmth(object sender)
        {
            InternalEvent.Run(() =>
            {
            if (sender is int s)
            {
                switch (s)
                {
                    case 1:
                        var tr1 = new Transaction(doc, "Запись для Элементов");
                        tr1.Start();
                
                        ///paste code here
                        Code_SetMaxMinPtToElements forElements = new Code_SetMaxMinPtToElements();
                        forElements.SetElementsTBPoints(doc);
                        
                        tr1.Commit();
                        break;
                    
                    case 2:
                        var tr2 = new Transaction(doc, "Запись для Линейных объектов");
                        tr2.Start();
                
                        ///paste code here
                        Code_SetMaxMinPtToElements forLinear = new Code_SetMaxMinPtToElements();
                        forLinear.SetLinearElemTBPoints(doc);
                        
                        tr2.Commit();
                        break;
                    
                    case 3:
                        var tr3 = new Transaction(doc, "Вывод информации о плагине");
                        tr3.Start();
                    
                        ///paste code here
                        Form_SetMaxMinPtToElements_Info info = new Form_SetMaxMinPtToElements_Info();
                        info.Show();
                    
                        tr3.Commit();
                        break;
                    
                    // case "val4":
                    //     var tr4 = new Transaction(doc, "Закрытие окна");
                    //     tr4.Start();
                    //
                    //     tr4.Commit();
                    //     break;
                }
            }
            });
        }

        private bool CanDoSmth(object sender)
        {
            return true;
        }
    }
}