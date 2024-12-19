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
            var panel1 = tab.Panel("Инфо");
            //Создание кнопки "Инфо"
            panel1.CreateButton<Info>("Функционал",
          "О плагине\nasBIM", b =>
          {
              b.SetLargeImage(Resources.Инфо_32);
              b.SetSmallImage(Resources.Инфо_16);
              b.SetLongDescription("Описание функционала плагина asBIM");
          });

            //Кнопка №2
            //Создание панели "Запись"
            var panel2 = tab.Panel("Параметризация");
            panel2.CreateButton<Code_SetMaxMinPtToElements>("Отметки \nВерха/Низа", "Отметки \nВерха/Низа", b =>
            {
                b.SetLargeImage(Resources.TB_Points_32);
                b.SetSmallImage(Resources.TB_Points_16);
                b.SetLongDescription("Параметризация для Гравиона\n\nЗапись значений в параметры: \nPRO_Отметка верха \nPRO_Отметка низа\n\nОтметки записываются от уровня на котором находится элемент");
            });

            // Кнопка № ???
            //Создание панели "Запись"
            //var panel2 = tab.Panel("Запись");
            ////Создание кнопки "Запись"
            //panel2.CreateButton<Code_SetParamertToVisibleElementInActiveView>("Запись параметров",
            //"Параметров \nв элементы", b =>
            //{
            //    b.SetLargeImage(Resources.ЗаписьПараметра_32);
            //    b.SetSmallImage(Resources.ЗаписьПараметра_16);
            //    b.SetLongDescription("Запись параметров для элементов на активном виде");
            //});

            // Кнопка в Разработке
            ////Создание панели "Создать"
            //var panel3 = tab.Panel("Создать");
            ////Создание кнопки "Создать"
            //panel3.CreateButton<CreateShaded3DViev>("Создание 3D вида",
            //"Презентационный \n3D вида", b =>
            //{
            //    b.SetLargeImage(Resources.ОбьемныйВидГрафика_32);
            //    b.SetSmallImage(Resources.ОбьемныйВидГрафика_16);
            //    b.SetLongDescription("Создание презентационного 3D вида с тенями и сглаженными линиями");
            //});

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}

