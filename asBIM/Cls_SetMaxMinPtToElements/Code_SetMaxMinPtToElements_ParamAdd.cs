using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using asBIM.Helpers;
using Notifications.Wpf;

namespace asBIM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Code_SetMaxMinPtToElements_ParamAdd : IExternalCommand
    {
        // Имя общего параметра "PRO_ID группы в пространстве
        private string shParamTopPtName = "PRO_Отметка верха";
        // Имя общего параметра "PRO_ID группы в пространстве
        private string shParamBotPtName = "PRO_Отметка низа";
        // Имя общего параметра "PRO_ID группы в пространстве
        private string shParamStName = "PRO_Отметка в начале";
        // Имя общего параметра "PRO_ID группы в пространстве
        private string shParamEndName = "PRO_Отметка в конце";
        
        // GUID общих параметров
        private Guid shParamTopPtGuid = new Guid("22c86588-f717-403e-b1c6-1607cac39965"); // PRO_Отметка верха
        private Guid shParamBotPtGuid = new Guid("b0ed44c1-724e-4301-b489-8d89c02acec5"); // PRO_Отметка низа
        private Guid shParamStGuid = new Guid("c7f57780-cff9-467e-a05e-47cc3f261064"); // PRO_Отметка в начале
        private Guid shParamEndGuid = new Guid("ea5f8c68-e31b-4882-bdca-a8a12200d347"); // PRO_Отметка в конце
        
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
            
            // Задание функции "Параметр изменяется по экземплярам групп"
            SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(doc, shParamTopPtGuid, true);
            SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(doc, shParamBotPtGuid, true);
            SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(doc, shParamStGuid, true);
            SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(doc, shParamEndGuid, true);
            
            
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
                // doc.Settings.Categories.get_Item(BuiltInCategory.OST_Parking), // Парковочные места
                // doc.Settings.Categories.get_Item(BuiltInCategory.OST_Railings), // Ограждения
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Topography), // Топография
                // doc.Settings.Categories.get_Item(BuiltInCategory.OST_Planting), // Растения
                // doc.Settings.Categories.get_Item(BuiltInCategory.OST_Entourage), // Окружение
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFraming), // Каркасы
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralTruss), // Фермы
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFoundation), // Конструктивные фундаменты
                // doc.Settings.Categories.get_Item(BuiltInCategory.OST_Cameras), // Камеры
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Parts), // Части
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFraming), // Каркас несущий
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipeAccessory), // Арматура труб
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctAccessory), // Арматура воздуховодов
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PlumbingEquipment), // Сантехническое оборудование
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctTerminal), // Воздухораспределители
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_CableTrayFitting), // Соединительные детали кабельных лотков
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_ConduitFitting), // Соединительные детали коробов
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Sprinklers), // Спринклеры
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_TelephoneDevices), // Телефонные устройства
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
                    categoriesForElem,
                    true);
                
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
                    categoriesForElem,
                    true);
                    
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
                    categoriesForLinear,
                    true);
                
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
                    categoriesForLinear,
                    true);

            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", ex.Message);
                throw;
            }
        }
        
    }
}