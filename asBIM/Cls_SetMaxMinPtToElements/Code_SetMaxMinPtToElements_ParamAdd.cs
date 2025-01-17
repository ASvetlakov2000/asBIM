﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asBIM.Properties;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.Xaml;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Windows;
using asBIM.ViewModel;
using Notifications.Wpf;
using CommunityToolkit.Mvvm.Input;
using Nice3point.Revit.Extensions;
using asBIM;

namespace asBIM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Code_SetMaxMinPtToElements_ParamAdd : IExternalCommand
    {
        // Имя общего параметра "PRO_ID группы в пространстве
        string shParamTopPtName = "PRO_Отметка верха";
        // Имя общего параметра "PRO_ID группы в пространстве
        string shParamBotPtName = "PRO_Отметка низа";
        // Имя общего параметра "PRO_ID группы в пространстве
        string shParamStName = "PRO_Отметка в начале";
        // Имя общего параметра "PRO_ID группы в пространстве
        string shParamEndName = "PRO_Отметка в конце";
        // Путь до ФОП выбором файла
        string filePathFprFOP =  OpenFile.OpenSingleFile("Выберите ФОП [PRO_SharedParametr] для добавления параметра", "txt");
        
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            
            // ОСНОВНОЙ КОД ПЛАГИНА // НАЧАЛО  
            // Добавление параметров для Элементов
            AddSharedParamTBPt(doc);
            // Добавление параметров для Линейных
            AddSharedParamStEndPt(doc);
            // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ

            return Result.Succeeded;
        }

        private void AddSharedParamTBPt(Document doc)
        {
            try
            {
                // Добавление категорий для общих параметров [PRO_Отметка верха] и [PRO_Отметка низа]
                List<Category> categoriesForElem = new List<Category>
                {
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Walls), // Стены
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Floors), // Полы
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Ceilings), // Потолки
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Columns), // Колонны
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralColumns), // Конструктивные колонны
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Roofs), // Крыши
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Doors), // Двери
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Windows), // Окна
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Stairs), // Лестницы
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StairsRailing), // Ограждения лестниц
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Ramps), // Пандусы
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Furniture), // Мебель
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_CurtainWallMullions), // Импосты витражей
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_CurtainWallPanels), // Панели витражей
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_GenericModel), // Общие модели
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_MechanicalEquipment), // Механическое оборудование
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipeFitting), // Фитинги труб
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctFitting), // Фитинги воздуховодов
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PlumbingFixtures), // Сантехнические приборы
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_LightingFixtures), // Светильники
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_ElectricalEquipment), // Электрооборудование
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_ElectricalFixtures), // Электроприборы
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Casework), // Корпусная мебель
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_CommunicationDevices), // Устройства связи
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_FireAlarmDevices), // Устройства пожарной сигнализации
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DataDevices), // Устройства передачи данных
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_NurseCallDevices), // Устройства вызова медсестры
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_SecurityDevices), // Устройства безопасности
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_FurnitureSystems), // Системы мебели
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_SpecialityEquipment), // Специальное оборудование
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_LightingDevices), // Осветительные устройства
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Parking), // Парковочные места
                // doc.Settings.Categories.get_Item(BuiltInCategory.OST_Railings), // Ограждения
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Topography), // Топография
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Planting), // Растения
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Entourage), // Окружение
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFraming), // Каркасы
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralTruss), // Фермы
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFoundation), // Конструктивные фундаменты
                // doc.Settings.Categories.get_Item(BuiltInCategory.OST_Cameras), // Камеры
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Parts), // Части
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFraming), // Каркас несущий
                };
                
                // Добавление общего параметра [PRO_Отметка верха]
                SharedParameterHelper.AddSharedParameterFromFOP(
                    doc,
                    // путь к ФОП
                    filePathFprFOP,
                    // Имя создаваемого параметра
                    shParamTopPtName,
                    // Куда относится параметр
                    BuiltInParameterGroup.PG_IDENTITY_DATA,
                    // Параметр для экземпляра
                    true,
                    // ссылка на список с категориями
                    categoriesForElem);
                
                // Добавление общего параметра [PRO_Отметка низа]
                SharedParameterHelper.AddSharedParameterFromFOP(
                    doc,
                    // путь к ФОП
                    filePathFprFOP,
                    // Имя создаваемого параметра
                    shParamBotPtName,
                    // Куда относится параметр
                    BuiltInParameterGroup.PG_IDENTITY_DATA,
                    // Параметр для экземпляра
                    true,
                    // ссылка на список с категориями
                    categoriesForElem);
                    
                // Уведомление. "Общий параметр добавлен!"
                NotificationManagerWPF.MessageInfo(
                    "Параметры для Элементов добавлены!",
                    "\n(￢‿￢ )" +
                    "\n\nПараметры: " 
                    +
                    "\n[PRO_Отметка верха]"
                    +
                    "\n[PRO_Отметка низа]",
                    NotificationType.Success);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", ex.Message);
                throw;
            }
        }
        
        private void AddSharedParamStEndPt(Document doc)
        {
            try
            {
                // Добавление категорий для общих параметров [PRO_Отметка в начале] и [PRO_Отметка в конце]
                List<Category> categoriesForLinear = new List<Category>
                {
                    doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipeCurves), // Трубы
                    doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctCurves), // Воздуховоды
                    doc.Settings.Categories.get_Item(BuiltInCategory.OST_CableTray), // Кабельные лотки
                    doc.Settings.Categories.get_Item(BuiltInCategory.OST_Conduit), // Короба
                    doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipeInsulations), // Трубы - Изоляция
                    doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctInsulations) // Воздуховоды - Изоляция
                };
                
                // Добавление общего параметра [PRO_Отметка в начале]
                SharedParameterHelper.AddSharedParameterFromFOP(
                    doc,
                    // путь к ФОП
                    filePathFprFOP,
                    // Имя создаваемого параметра
                    shParamStName,
                    // Куда относится параметр
                    BuiltInParameterGroup.PG_IDENTITY_DATA,
                    // Параметр для экземпляра
                    true,
                    // ссылка на список с категориями
                    categoriesForLinear);
                
                // Добавление общего параметра [PRO_Отметка в конце]
                SharedParameterHelper.AddSharedParameterFromFOP(
                    doc,
                    // путь к ФОП
                    filePathFprFOP,
                    // Имя создаваемого параметра
                    shParamEndName,
                    // Куда относится параметр
                    BuiltInParameterGroup.PG_IDENTITY_DATA,
                    // Параметр для экземпляра
                    true,
                    // ссылка на список с категориями
                    categoriesForLinear);
                    
                // Уведомление. "Общий параметр добавлен!"
                NotificationManagerWPF.MessageInfo(
                    "Параметры для Линейных добавлены!",
                    "\n(￢‿￢ )" +
                    "\n\nПараметры: " 
                    +
                    "\n[PRO_Отметка в начале]"
                    +
                    "\n[PRO_Отметка в конце]",
                    NotificationType.Success);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", ex.Message);
                throw;
            }
        }
        
    }
}