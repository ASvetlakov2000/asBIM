using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace asBIM
{
    internal class AllUsedCategoryList
    {
        public IEnumerable<BuiltInCategory> categoryList = new BuiltInCategory[] {
            BuiltInCategory.OST_PipeAccessory, //Арматура трубопроводов 
            BuiltInCategory.OST_MechanicalEquipment, //Оборудование 
            BuiltInCategory.OST_PlumbingFixtures, //Сантехнические приборы 
            BuiltInCategory.OST_PipeFitting, //Соединительные детали трубопроводов 
            BuiltInCategory.OST_PipeInsulations, //Материалы изоляции труб 
            BuiltInCategory.OST_DuctCurves, //Воздуховоды 
            BuiltInCategory.OST_DuctFitting, //Соединительные детали воздуховодов 
            BuiltInCategory.OST_DuctAccessory, //Арматура воздуховодов 
            BuiltInCategory.OST_DuctTerminal, //Воздухораспределители 
            BuiltInCategory.OST_CableTray, //Кабельные лотки 
            BuiltInCategory.OST_Conduit, //Короба 
            BuiltInCategory.OST_CableTrayFitting, //Соединительные детали кабельных лотков 
            BuiltInCategory.OST_ConduitFitting, //Соединительные детали коробов 
            BuiltInCategory.OST_FireAlarmDevices, //Пожарная сигнализация 
            BuiltInCategory.OST_DataDevices, //Датчики 
            BuiltInCategory.OST_ElectricalEquipment, //Электрооборудование 
            BuiltInCategory.OST_ElectricalFixtures, //Электрические приборы 
            BuiltInCategory.OST_LightingFixtures,  //Осветительные приборы 
            BuiltInCategory.OST_DuctInsulations, //Материалы изоляции воздуховодов 
            BuiltInCategory.OST_LinksAnalytical, //Аналитические связи
            BuiltInCategory.OST_Casework, //Мебельные изделия
            BuiltInCategory.OST_Ceilings, //Потолки
            BuiltInCategory.OST_Columns, //Колонны
            BuiltInCategory.OST_CommunicationDevices, //Устройства связи
            BuiltInCategory.OST_CurtainWallPanels, //Панели навесных стен
            BuiltInCategory.OST_CurtaSystem, //Системы навесных стен
            BuiltInCategory.OST_CurtainWallMullions, //Импосты навесных стен
            BuiltInCategory.OST_DetailComponents, //Деталировочные компоненты
            BuiltInCategory.OST_Doors, //Двери
            BuiltInCategory.OST_ElectricalCircuit, //Электрические цепи
            BuiltInCategory.OST_Entourage, //Окружение
            BuiltInCategory.OST_FlexDuctCurves, //Гибкие воздуховоды
            BuiltInCategory.OST_FlexPipeCurves, //Гибкие трубы
            BuiltInCategory.OST_Floors, //Полы
            BuiltInCategory.OST_Furniture, //Мебель
            BuiltInCategory.OST_FurnitureSystems, //Мебельные системы
            BuiltInCategory.OST_GenericModel, //Обобщённые модели
            BuiltInCategory.OST_LightingDevices, //Световые устройства
            BuiltInCategory.OST_Mass, //Массы
            BuiltInCategory.OST_NurseCallDevices, //Устройства вызова медсестры
            BuiltInCategory.OST_Parking, //Парковка
            BuiltInCategory.OST_Parts, //Части
            BuiltInCategory.OST_PipeCurves, //Трубы
            BuiltInCategory.OST_StairsRailing, //Ограждения лестниц
            BuiltInCategory.OST_Ramps, //Пандусы
            BuiltInCategory.OST_RebarShape, //Формы арматуры
            BuiltInCategory.OST_Roofs, //Крыши
            BuiltInCategory.OST_SecurityDevices, //Устройства безопасности
            BuiltInCategory.OST_ShaftOpening, //Шахты
            BuiltInCategory.OST_Site, //Участки
            BuiltInCategory.OST_MEPSpaces, //Пространства MEP
            BuiltInCategory.OST_SpecialityEquipment, //Специальное оборудование
            BuiltInCategory.OST_Sprinklers, //Спринклеры
            BuiltInCategory.OST_Stairs, //Лестницы
            BuiltInCategory.OST_AreaRein, //Области армирования
            BuiltInCategory.OST_StructuralFramingSystem, //Системы конструктивного каркаса
            BuiltInCategory.OST_StructuralColumns, //Конструктивные колонны
            BuiltInCategory.OST_StructConnections, //Конструктивные соединения
            BuiltInCategory.OST_FabricAreas, //Армирующие сетки
            BuiltInCategory.OST_FabricReinforcement, //Сетчатое армирование
            BuiltInCategory.OST_StructuralFoundation, //Фундаменты
            BuiltInCategory.OST_StructuralFraming, //Конструктивный каркас
            BuiltInCategory.OST_PathRein, //Пути армирования
            BuiltInCategory.OST_Rebar, //Арматура
            BuiltInCategory.OST_StructuralStiffener, //Жёсткости
            BuiltInCategory.OST_StructuralTruss, //Фермы
            BuiltInCategory.OST_SwitchSystem, //Системы переключения
            BuiltInCategory.OST_TelephoneDevices, //Телефонные устройства
            BuiltInCategory.OST_Walls, //Стены
            BuiltInCategory.OST_Windows, //Окна
            BuiltInCategory.OST_Wire, //Провод
        };
    }
}
