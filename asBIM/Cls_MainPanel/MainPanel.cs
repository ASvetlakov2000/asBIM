using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCRevitRibbonUtil;
using Autodesk.Revit.UI;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using asBIM.Properties;
using asBIM;
using Nice3point.Revit.Toolkit.External;
using Autodesk.Revit.DB;



namespace asBIM
{
    internal class MainPanel : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            //Создание вкладки "asBIM"
            var tab = Ribbon.GetApplicationRibbon(a).Tab("asBIM");

            //Кнопка №1
            //Создание панели "Запись"
            var panel1 = tab.Panel("RA | Запись");
            panel1.CreateButton<Code_SetMaxMinPtToElements>("Отметки \nВерха/Низа", "Отметки \nВерха/Низа", b =>
            {
                b.SetLargeImage(Resources.TB_Points_AddParam_32);
                b.SetSmallImage(Resources.TB_Points_AddParam_16);
                b.SetLongDescription("Параметризация для Гравиона\n\nЗапись значений в параметры: \nPRO_Отметка верха \nPRO_Отметка низа\n\nОтметки записываются от уровня на котором находится элемент");
            });

            //Кнопка №2
            panel1.CreateButton<Code_SetMaxMinPtToElements>("Отметки \nВерха/Низа", "Отметки \nВерха/Низа", b =>
            {
                b.SetLargeImage(Resources.TB_Points_32);
                b.SetSmallImage(Resources.TB_Points_16);
                b.SetLongDescription("Параметризация для Гравиона\n\nЗапись значений в параметры: \nPRO_Отметка верха \nPRO_Отметка низа\n\nОтметки записываются от уровня на котором находится элемент");
            });

            //Кнопка №3
            //Создание панели "Разместить"
            var panel2 = tab.Panel("ТХ");
            //Создание кнопки "Создать"
            panel2.CreateButton<Code_TX_PlaceGroupsInSpaces_ParamAdd>("Добавить\nпараметры",
                "Добавить\nпараметры", b =>
                {
                    b.SetLargeImage(Resources.PlaceGroupsInSpacesTX_Params_32);
                    b.SetSmallImage(Resources.PlaceGroupsInSpacesTX_Params_16);
                    b.SetLongDescription("Добавить параметры для записи ID групп в пространствах ТХ");
                });

            panel2.CreateSeparator();

            //Кнопка №4
            panel2.CreateButton<Code_TX_PlaceGroupsInSpaces>("Разместить группы",
                "Разместить\nГруппы ТХ", b =>
                {
                    b.SetLargeImage(Resources.PlaceGroupsInSpacesTX_32);
                    b.SetSmallImage(Resources.PlaceGroupsInSpacesTX_16);
                    b.SetLongDescription("Размещение групп в пространствах ТХ");
                });

            panel2.CreateSeparator();

            //Кнопка №5
            panel2.CreateButton<Code_TX_PlaceGroupsInSpaces_DelGrp>("Удалить группы",
                "Удалить\nГруппы ТХ", b =>
                {
                    b.SetLargeImage(Resources.Delete_32);
                    b.SetSmallImage(Resources.Delete_16);
                    b.SetLongDescription("Удаление групп в пространствах ТХ с очисткой параметра пространств");
                });



            // //Создание панели "Разместить"
            // var panel3 = tab.Panel("Разместить");
            // //Создание кнопки "Создать"
            // panel3.CreateButton<Code_PlaceGroupsInSpacesTX>("Разместить группы",
            // "Группы ТХ", b =>
            // {
            //     b.SetLargeImage(Resources.PlaceGroupsInSpacesTX_32);
            //     b.SetSmallImage(Resources.PlaceGroupsInSpacesTX_16);
            //     b.SetLongDescription("Размещение групп в пространствах ТХ");
            // });

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
        
        
        
        
    
        /*static AddInId addInId = new AddInId(new Guid("fb72e042-343b-4caf-beab-505d04d36477"));
        private readonly string assemblyPath = Assembly.GetExecutingAssembly().Location;
        
        
        public Result OnStartup(UIControlledApplication application)
        {            
            string tabName = "asBIM";
            application.CreateRibbonTab(tabName);
            
            RibbonPanel ribbonPanel01 = application.CreateRibbonPanel(tabName, "RA | Запись");
            AddSplitButton(ribbonPanel01, "Cписок команд \nSplit");
            
            RibbonPanel ribbonPanel02 = application.CreateRibbonPanel(tabName, "ТХ");
            AddSplitButton(ribbonPanel02, "Cписок команд \nSplit");

            
            //application.DialogBoxShowing += Application_DialogBoxShowing;


            return Result.Succeeded;
        }

        private void Application_DialogBoxShowing(object sender, Autodesk.Revit.UI.Events.DialogBoxShowingEventArgs e)
        {
            //if (e.DialogId == "Dialog_API_MacroManager")
            //{
                //e.OverrideResult(2); // it should close macro manager right after its opening, be careful
                TaskDialog.Show("Yes", "You opened a macro manager, cool!");
            //}
        }

        private PushButton AddPushButton(RibbonPanel ribbonPanel, string buttonName, string path, string linkToCommand, string toolTip)
        {
            var buttonData = new PushButtonData(buttonName, buttonName, path, linkToCommand);            
            var button = ribbonPanel.AddItem(buttonData) as PushButton;
            button.ToolTip = toolTip;
            button.LargeImage = (ImageSource) new BitmapImage(new Uri(@"/Resources/TB_Points_32.png", UriKind.Relative));
            return button;
        }

        /*private void AddStackedButtons(RibbonPanel ribbonPanel, string buttonName, string path, string linkToCommand, string toolTip)
        {
            var list = new List<PushButtonData>();
            for (var i = 0; i<3; i++)
            {
                var buttonData = new PushButtonData(buttonName + $" {i.ToString()}", buttonName + $" {i.ToString()}", path, linkToCommand);
                list.Add(buttonData);
            }
            var result = ribbonPanel.AddStackedItems(list[0], list[1], list[2]);
            foreach (PushButton button in result)
            {
                button.ToolTip = toolTip;
                button.LargeImage = (ImageSource)new BitmapImage(new Uri(@"/FirstRevitPlugin;component/Resources/wheel32.png", UriKind.RelativeOrAbsolute));
                button.Image = (ImageSource)new BitmapImage(new Uri(@"/FirstRevitPlugin;component/Resources/wheel16.png", UriKind.RelativeOrAbsolute));
            }
        }

        private void AddRadioButtonGroup(RibbonPanel ribbonPanel, string name)
        {
            var data = new RadioButtonGroupData(name);
            var item = ribbonPanel.AddItem(data) as RadioButtonGroup;
            AddToggleButton(item, "ToggleButton 1 ", assemblyPath, "FirstRevitPlugin.FirstRevitCommand", "Кнопка-переключатель 1", 1);
            AddToggleButton(item, "ToggleButton 2", assemblyPath, "FirstRevitPlugin.FirstRevitCommand", "Кнопка-переключатель 2", 3);
        }

        private void AddToggleButton(RadioButtonGroup group, string buttonName, string path, string linkToCommand, string toolTip, int i)
        {
            ToggleButtonData data = new ToggleButtonData(buttonName, buttonName, path, linkToCommand);
            var item = group.AddItem(data) as ToggleButton;
            item.ToolTip = toolTip;
            item.LargeImage = (ImageSource) new BitmapImage(
                new Uri(@"/FirstRevitPlugin;component/Resources/engineer"+i.ToString()+".png", UriKind.RelativeOrAbsolute));
        }#1#

        private void AddPullDownButton(RibbonPanel ribbonPanel, string name)
        {
            var data = new PulldownButtonData(name, name);
            FillPullDown(ribbonPanel, data);
        }

        private void AddSplitButton(RibbonPanel ribbonPanel, string name)
        {
            var data = new SplitButtonData(name, name);
            FillPullDown(ribbonPanel, data);
        }

        private void FillPullDown(RibbonPanel ribbonPanel, PulldownButtonData data)
        {
            var item = ribbonPanel.AddItem(data) as PulldownButton;
            item.LargeImage = (ImageSource)new BitmapImage(new Uri(@"/Resources/TB_Points_32.png", UriKind.RelativeOrAbsolute));
            
            AddButtonToPullDownButton(item, "Отметки \nВерха/Низа", assemblyPath, "asBIM.Code_SetMaxMinPtToElements", "Команда 1 — подсказка");
            AddButtonToPullDownButton(item, "Инфо", assemblyPath, "asBIM.Form_SetMaxMinPtToElements", "Команда 2 — подсказка");
            // AddButtonToPullDownButton(item, "Команда 2", assemblyPath, "asBIM.FirstRevitCommand", "Команда 2 — подсказка");
            // item.AddSeparator();
            
        }

        private void AddButtonToPullDownButton (PulldownButton button, string name, string path, string linkToCommand, string toolTip)
        {
            var data = new PushButtonData(name, name, path, linkToCommand);
            var pushButton = button.AddPushButton(data) as PushButton;
            pushButton.ToolTip = toolTip;
            // pushButton.Image = (ImageSource)new BitmapImage(new Uri(@"/Resources/TB_Points_16.png", UriKind.RelativeOrAbsolute));
            pushButton.LargeImage = (ImageSource)new BitmapImage(new Uri(@"/Resources/TB_Points_32.png", UriKind.RelativeOrAbsolute));
        }

        
        
        /*private void AddComboBox(RibbonPanel ribbonPanel)
        {
            var data = new ComboBoxData("Combo Box");
            var item = ribbonPanel.AddItem(data) as ComboBox;
            AddComboBoxMember(item, "Value 1");
            AddComboBoxMember(item, "Value 2");
            AddComboBoxMember(item, "Value 3");
            item.CurrentChanged += ComboBoxChanged;
            HelloWorldCommand.ComboBoxValue = item.Current.ItemText;
        }        

        private void AddComboBoxMember(ComboBox box, string name)
        {
            var data = new ComboBoxMemberData(name, name);
            var member = box.AddItem(data) as ComboBoxMember;
            member.Image = (ImageSource)new BitmapImage(new Uri(@"/FirstRevitPlugin;component/Resources/wheel16.png", UriKind.RelativeOrAbsolute));
        }

        private void ComboBoxChanged(object sender, Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs e)
        {
            var box = sender as ComboBox;
            HelloWorldCommand.ComboBoxValue = box.Current.ItemText;
        }

        private void AddTextBox(RibbonPanel ribbonPanel)
        {
            var data = new TextBoxData("Text");
            var item = ribbonPanel.AddItem(data) as TextBox;
            item.EnterPressed += TextBoxChanged;
        }

        private void TextBoxChanged(object sender, Autodesk.Revit.UI.Events.TextBoxEnterPressedEventArgs e)
        {
            var textBox = (TextBox)sender;
            HelloWorldCommand.TextBoxValue = textBox.Value.ToString();            
        }#1#

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }*/
    }
    
}


