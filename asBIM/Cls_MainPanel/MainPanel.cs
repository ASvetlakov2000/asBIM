using asBIM.Helpers;
using Autodesk.Revit.UI;
using System.Windows.Media;

namespace asBIM
{
    internal class MainPanel : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            //Создание вкладки "asBIM"
            string tabName = "asBIM";
            a.CreateRibbonTab(tabName);
            
            // Создаём панель #1. "RA | Параметры"
            string panelName01 = "RA | Параметры";
            RibbonPanel panel01 = a.CreateRibbonPanel(tabName, panelName01);

            // Создаём SplitButton #1
            string splitButtonName01 = "Отметки ВН";
            SplitButtonData splitButtonData01 = new SplitButtonData(splitButtonName01, "Запись Отметок Верха и Низа, Отметок Начала и Конца");

            // Добавляем SplitButton #1 на панель
            SplitButton splitButton01 = panel01.AddItem(splitButtonData01) as SplitButton;

            if (splitButton01 != null)
            {
                // Добавляем кнопки в SplitButton
                Ribbon.AddPushButtonToSplit(splitButton01, 
                    "Отметки\nВерха/Низа", 
                    "Отметки\nВерха/Низа", 
                    "asBIM.Code_SetMaxMinPtToElements", 
                    "asBIM.Resources.TB_Points_16.png",
                    "asBIM.Resources.TB_Points_32.png");
                
                // Добавляем кнопки в SplitButton
                Ribbon.AddPushButtonToSplit(splitButton01, 
                    "Добавить\nпараметры RA",
                    "Добавить\nпараметры RA", 
                    "asBIM.Code_SetMaxMinPtToElements_ParamAdd", 
                    "asBIM.Resources.TB_Points_AddParam_16.png",
                    "asBIM.Resources.TB_Points_AddParam_32.png");
            }
            // Создаём панель #1. "RA | Параметры"
            
            
            // Создаём панель #2. "ТХ"
            string panelName02 = "ТХ";
            RibbonPanel panel02 = a.CreateRibbonPanel(tabName, panelName02);
            
            // Создаём SplitButton #2
            string splitButtonName02 = "Группы ТХ";
            SplitButtonData splitButtonData02 = new SplitButtonData(splitButtonName02, "Размещение групп в пространствах ТХ");

            // Добавляем SplitButton #2 на панель
            SplitButton splitButton02 = panel02.AddItem(splitButtonData02) as SplitButton;

            if (splitButton01 != null)
            {
                // Добавляем кнопки в SplitButton
                Ribbon.AddPushButtonToSplit(splitButton02, 
                    "Разместить\nГруппы ТХ", 
                    "Разместить\nГруппы ТХ", 
                    "asBIM.Cls_TX_PlaceGroupsInSpaces.Code_TX_PlaceGroupsInSpaces", 
                    "asBIM.Resources.PlaceGroupsInSpacesTX_16.png",
                    "asBIM.Resources.PlaceGroupsInSpacesTX_32.png");
                
                // Добавляем кнопки в SplitButton
                Ribbon.AddPushButtonToSplit(splitButton02, 
                    "Удалить\nГруппы ТХ", 
                    "Удалить\nГруппы ТХ", 
                    "asBIM.Cls_TX_PlaceGroupsInSpaces.Code_TX_PlaceGroupsInSpaces_DelGrp", 
                    "asBIM.Resources.Delete_16.png",
                    "asBIM.Resources.Delete_32.png");
                
                // Добавляем кнопки в SplitButton
                Ribbon.AddPushButtonToSplit(splitButton02, 
                    "Добавить\nпараметры",
                    "Добавить\nпараметры", 
                    "asBIM.Cls_TX_PlaceGroupsInSpaces.Code_TX_PlaceGroupsInSpaces_ParamAdd",
                    "asBIM.Resources.PlaceGroupsInSpacesTX_Params_16.png",
                    "asBIM.Resources.PlaceGroupsInSpacesTX_Params_32.png");
            }
            // Создаём панель #2. "RA | Параметры"
            
            
            // Создаём панель #3. "ТХ"
            // string panelName03 = "Тест";
            // RibbonPanel panel03 = a.CreateRibbonPanel(tabName, panelName03);
            //
            // Ribbon.AddPushButtonSingle(
            //     panel03,
            //     "Тест", 
            //     "Тест", 
            //     "asBIM.Code_TX_PlaceGroupsInSpaces_DelGrp", 
            //     "asBIM.Resources.Delete_32.png",
            //     "asBIM.Resources.Delete_16.png");
            // Создаём панель #3. "ТХ"
            
            
            // Создаём панель #1. "RA | Параметры"
            string panelName03 = "Материалы";
            RibbonPanel panel03 = a.CreateRibbonPanel(tabName, panelName03);

            // Создаём SplitButton #1
            string splitButtonName03 = "Экспорт имен";
            SplitButtonData splitButtonData03 = new SplitButtonData(splitButtonName03, "Экспорт имен материалов в файл .txt");

            // Добавляем SplitButton #1 на панель
            SplitButton splitButton03 = panel03.AddItem(splitButtonData03) as SplitButton;

            if (splitButton01 != null)
            {
                // Добавляем кнопки в SplitButton
                Ribbon.AddPushButtonToSplit(splitButton03, 
                    "Экспорт имен", 
                    "Экспорт имен", 
                    "asBIM.Code_ExportMaterialMapping", 
                    "asBIM.Resources.ExportMaterialMapping_16.png",
                    "asBIM.Resources.ExportMaterialMapping_32.png");
                
                // Добавляем кнопки в SplitButton
                Ribbon.AddPushButtonToSplit(splitButton03, 
                    "Импорт имен",
                    "Импорт имен", 
                    "asBIM.Code_RenameMaterialsfromMapping", 
                    "asBIM.Resources.RenameMaterialsfromMapping_16.png",
                    "asBIM.Resources.RenameMaterialsfromMapping_32.png");
            }
            // Создаём панель #1. "RA | Параметры"
            
            
            return Result.Succeeded;

        }
        
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

    }
    
}


