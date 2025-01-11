using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCRevitRibbonUtil;
using Autodesk.Revit.UI;
using System.Net.NetworkInformation;
using System.Resources;
using asBIM.Properties;
using asBIM;

namespace asBIM
{
    internal class MainPanel : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            //Создание вкладки "asBIM"
            var tab = Ribbon.GetApplicationRibbon(a).Tab("asBIM");

            // Кнопка №1chfh
            //Создание панели "Инфо"
          //   var panel1 = tab.Panel("Инфо");
          //   //Создание кнопки "Инфо"
          //   panel1.CreateButton<Info>("Функционал",
          // "О плагине\nasBIM", b =>
          // {
          //     b.SetLargeImage(Resources.);
          //     b.SetSmallImage();
          //     b.SetLongDescription("Описание функционала плагина asBIM");
          // });

            //Кнопка №2
            //Создание панели "Запись"
            var panel2 = tab.Panel("Параметризация");
            panel2.CreateButton<Code_SetMaxMinPtToElements>("Отметки \nВерха/Низа", "Отметки \nВерха/Низа", b =>
            {
                b.SetLargeImage(Resources.TB_Points_32);
                b.SetSmallImage(Resources.TB_Points_16);
                b.SetLongDescription("Параметризация для Гравиона\n\nЗапись значений в параметры: \nPRO_Отметка верха \nPRO_Отметка низа\n\nОтметки записываются от уровня на котором находится элемент");
            });

             
            //Создание панели "Создать"
            var panel3 = tab.Panel("Разместить");
            //Создание кнопки "Создать"
            panel3.CreateButton<Code_PlaceGroupsInSpacesTX>("Разместить группы",
            "Группы ТХ", b =>
            {
                b.SetLargeImage(Resources.PlaceGroupsInSpacesTX_32);
                b.SetSmallImage(Resources.PlaceGroupsInSpacesTX_16);
                b.SetLongDescription("Размещение групп в пространствах ТХ");
            });

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}

