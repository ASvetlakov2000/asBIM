using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Autodesk.Revit.DB.Plumbing;

namespace asBIM
{
    internal static class RevitAPI
    {
        public static void InfoDialog(this string message)
        {
            TaskDialog.Show("Info", message);
        } //Вывод сообщения

        public static string Joiner(this IEnumerable<string> lst, string sep)
        {
            return string.Join(sep, lst);
        }

        public static Parameter GetParameter(this Element e, string name)
        {
            return e.LookupParameter(name);
        }
        public static Parameter GetParameter(this Element e, BuiltInParameter bip)
        {
            return e.get_Parameter(bip);
        }
        public static Parameter GetParameter(this Element e, Guid guid)
        {
            return e.get_Parameter(guid);
        }

        public static Parameter GetParameter<T>(this Element e, T key)
        {
            if (typeof(T) == typeof(Guid) && key is Guid guid)
                return e.get_Parameter(guid);
            if (typeof(T) == typeof(string) && key is string str)
                return e.LookupParameter(str);
            if (typeof(T) == typeof(BuiltInParameter) && key is BuiltInParameter bip)
                return e.get_Parameter(bip);
            return null;
        }
    }


    /// <summary>
    /// Класс для сортировки всех ЭЭлементов в Документе.
    /// Класс собирает все элементы В Коллекцию.
    /// Коллекции сортируются и собираются в Словари по Категориям Revit
    /// </summary>
    internal static class RevitAPI_Sort_ByCategory
    {
        // Сбор элементов в коллекцию из документа с группировкой BuiltInCategory категорий.
        public static Dictionary<string, List<Element>> SortElementByCategory(Document doc)
        {
            // Инициализация обьекта collector класса FilteredElementCollector для запроса элементов из документа.
            FilteredElementCollector collectorMain = new FilteredElementCollector(doc);
            // Сбор всех элементов из модели в список IList<Element> allElementsInDoc. 
            // Создание обьекта нового списка allElementsInDoc со всеми элементами в документе
            IList<Element> allElementsInDoc = new List<Element>();
            // Передача списку allElementsInDoc всех элементов из документа. 
            // Только экземпляры (не типы). Только элементы.
            allElementsInDoc = collectorMain.WhereElementIsNotElementType().ToElements();
            
            // Создание списка "Element" с группами
            List<BuiltInCategory> group_Element_Cat = new List<BuiltInCategory>()
                {
                    // Перечисление всех категорий для АР
                    BuiltInCategory.OST_Walls, // Стены
                    BuiltInCategory.OST_Floors, // Полы
                    BuiltInCategory.OST_Ceilings, // Потолки
                    BuiltInCategory.OST_Columns, // Колонны
                    BuiltInCategory.OST_StructuralColumns, // Конструктивные колонны
                    BuiltInCategory.OST_Roofs, // Крыши
                    BuiltInCategory.OST_Doors, // Двери
                    BuiltInCategory.OST_Windows, // Окна
                    BuiltInCategory.OST_Stairs, // Лестницы
                    BuiltInCategory.OST_StairsRailing, // Ограждения лестниц
                    BuiltInCategory.OST_Ramps, // Пандусы
                    BuiltInCategory.OST_Furniture, // Мебель
                    BuiltInCategory.OST_CurtainWallMullions, // Импосты витражей
                    BuiltInCategory.OST_CurtainWallPanels, // Панели витражей
                    BuiltInCategory.OST_GenericModel, // Общие модели
                    BuiltInCategory.OST_MechanicalEquipment, // Механическое оборудование
                    BuiltInCategory.OST_PipeFitting, // Фитинги труб
                    BuiltInCategory.OST_DuctFitting, // Фитинги воздуховодов
                    BuiltInCategory.OST_PlumbingFixtures, // Сантехнические приборы
                    BuiltInCategory.OST_LightingFixtures, // Светильники
                    BuiltInCategory.OST_ElectricalEquipment, // Электрооборудование
                    BuiltInCategory.OST_ElectricalFixtures, // Электроприборы
                    BuiltInCategory.OST_Casework, // Корпусная мебель
                    BuiltInCategory.OST_CommunicationDevices, // Устройства связи
                    BuiltInCategory.OST_FireAlarmDevices, // Устройства пожарной сигнализации
                    BuiltInCategory.OST_DataDevices, // Устройства передачи данных
                    BuiltInCategory.OST_NurseCallDevices, // Устройства вызова медсестры
                    BuiltInCategory.OST_SecurityDevices, // Устройства безопасности
                    BuiltInCategory.OST_FurnitureSystems, // Системы мебели
                    BuiltInCategory.OST_SpecialityEquipment, // Специальное оборудование
                    BuiltInCategory.OST_LightingDevices, // Осветительные устройства
                    BuiltInCategory.OST_Parking, // Парковочные места
                    BuiltInCategory.OST_Railings, // Ограждения
                    BuiltInCategory.OST_Topography, // Топография
                    BuiltInCategory.OST_Planting, // Растения
                    BuiltInCategory.OST_Entourage, // Окружение
                    BuiltInCategory.OST_StructuralFraming, // Каркасы
                    BuiltInCategory.OST_StructuralTruss, // Фермы
                    BuiltInCategory.OST_StructuralFoundation, // Конструктивные фундаменты
                    BuiltInCategory.OST_Cameras, // Камеры
                    BuiltInCategory.OST_Parts, // Части
                    BuiltInCategory.OST_StructuralFraming, // Каркас несущий
                    
                };

            // Создание списка "Linear" с группами
            List<BuiltInCategory> group_Linear_Cat = new List<BuiltInCategory>()
            {
                
                BuiltInCategory.OST_PipeCurves, // Трубы
                BuiltInCategory.OST_DuctCurves, // Воздуховоды
                BuiltInCategory.OST_CableTray, // Кабельные лотки
                BuiltInCategory.OST_Conduit, // Короба
                BuiltInCategory.OST_PipeInsulations, // Трубы - Изоляция
                BuiltInCategory.OST_DuctInsulations // Воздуховоды - Изоляция

            };

            // Создание словаря с Группами Элементов из Документа
            Dictionary<string, List<Element>> groupedElementsDict = new Dictionary<string, List<Element>>
            {
                ["Element"] = new List<Element>(),
                ["Linear"] = new List<Element>(),
            };

            // Группировка элементов с сортировкой по категориям. 
            // Цикл foreach перебирает ЭЭлементы в Коллекторе и при совпадении категории ЭЭлемента относит к Группе (АР, КР и т.д)
            foreach (var element in allElementsInDoc)
            {
                // Присвоение переменной elemCategory типа Category Категории ЭЭлемента
                Category elemCategory = element.Category;

                // Проверка Категории ЭЭлемента на ноль
                if (elemCategory != null)
                {
                    // Если в ЭЭлемент коллекции collectorMain (doc) относится к Категории попадающей в список group_AR_Cat,
                    // то элемент добавиться в словарь groupedElementsDict по ключу "АР"
                    if (group_Element_Cat.Contains((BuiltInCategory)elemCategory.Id.IntegerValue))
                    {
                        groupedElementsDict["Element"].Add(element);
                    }

                    // Если в ЭЭлемент коллекции collectorMain (doc) относится к Категории попадающей в список group_AR02_Cat,
                    // то элемент добавиться в словарь groupedElementsDict по ключу "АР02"
                    if (group_Linear_Cat.Contains((BuiltInCategory)elemCategory.Id.IntegerValue))
                    {
                        groupedElementsDict["Linear"].Add(element);
                    }
                }

            }
            // 
            return groupedElementsDict;
        }
    }

    internal static class ElementTopBottomPt
    {
        /// <summary>
        /// Метод возвращающий верхнюю точку BoundingBoxXYZ
        /// </summary>
        /// <param name="element">Принимает все Элементы как элементы из Документа</param>
        /// <returns></returns>
        public static XYZ GetElementTopPoint(Element element)
        {
            // Получение BoundingBox элемента
            BoundingBoxXYZ bounding = element.get_BoundingBox(null);
            // Присвоение перем. elemTopPointFeet_XYZ значения отметки Верхней точки BoundingBox элемента 
            // Без вычитания расстояния от Начала Координат (НК)
            XYZ elemTopPointFeet_XYZ = bounding.Max;

            // Возращает XYZ elemTopPointFeet_XYZ. Значения XYZ отметки Верха в футах
            return elemTopPointFeet_XYZ;
        }

        /// <summary>
        /// Метод возвращающий нижнюю точку BoundingBoxXYZ
        /// </summary>
        /// <param name="element">Принимает все Элементы как элементы из Документа</param>
        /// <returns></returns>
        public static XYZ GetElementBottomPoint(Element element)
        {
            // Получение BoundingBox элемента
            BoundingBoxXYZ bounding = element.get_BoundingBox(null);
            // Присвоение перем. elemBottomPointFeet_XYZ значения отметки Нижней точки BoundingBox элемента 
            // Без вычитания расстояния от Начала Координат (НК)
            XYZ elemBottomPointFeet_XYZ = bounding.Min;

            // Возвращает XYZ elemBottomPointFeet_XYZ. Значения XYZ отметки Низа в футах
            return elemBottomPointFeet_XYZ;
        }
        
        // Было
        // public static XYZ GetLinearStartPoint (Element element)
        // {
        //     StringBuilder sb = new StringBuilder();
        //     
        //     // Получаем Location элемента
        //     Location location = element.Location;
        //
        //     LocationCurve locationCurve = null;
        //     // Получаем кривую линейного обьекта
        //     if (!(location is LocationCurve curve)) return XYZ.Zero;
        //     // Получаем начальную и конечную точки
        //     XYZ startPoint = curve.Curve.GetEndPoint(0);
        //     //XYZ endPoint = curve.GetEndPoint(1);
        //
        //     string test = startPoint.ToString();
        //
        //     sb.Append("\n" + test);
        //
        //     return startPoint;
        // }

        
        
        /// Метод XYZ GetLinearPoint для получения Отметок в Начале и Отметок в Конце 
        public static XYZ GetLinearPoint(MEPCurve curve, bool? end = null)
        {
            var location = curve.Location;
            var locationCurve = (location as LocationCurve)?.Curve;
            return locationCurve?.GetEndPoint((bool)end ? 1: 0);
        }

        // Способ 2
        public static XYZ GetPoint(Element e, bool? max = null)
        {
            if(e is null) return new XYZ();
            var location = e.Location;
            switch (location)
            {
                case LocationCurve locationCurve:
                    var top = locationCurve.Curve.GetEndPoint(0);
                    var bottom = locationCurve.Curve.GetEndPoint(1);
                    if (max is bool tr)
                    {
                        if(!tr)
                            return top.Z > bottom.Z ? top : bottom;
                    }
                    return top.Z < bottom.Z ? top : bottom;
                case LocationPoint locationPoint:
                    return locationPoint.Point;
                default:
                    throw new ArgumentOutOfRangeException(nameof(location));
            }
        }


    }

    // Класс для определения уровня элемента. Для верхнего уровня и нижнего.
    internal static class LevelInfo
    {
        /// <summary>
        /// Метод для Получения значений Имени и Отметки Уровней в Документе
        /// </summary>
        /// <param name="doc">Все уровни из документа</param>
        public static void GetLevelInfoInDoc(Document doc)
        {
            StringBuilder levelInformation = new StringBuilder();
            // Фильтр на Level
            FilteredElementCollector levelsCollector = new FilteredElementCollector(doc).OfClass(typeof(Level));
            // Список levels с типом Element
            List<Level> levels = levelsCollector.Cast<Level>().OrderBy(level => level.Elevation).ToList();
            // Перебор уровней. Получение имени и отметки
            foreach (Level level in levels)
            {
                if (null != level)
                {
                    // Получение Имени уровня
                    string levelName = level.Name;
                    // Получение Отметки уровня в футах
                    double levelElevationFeet = level.Elevation;
                    // Получение Отметки уровня в см
                    double levelElevationSm = levelElevationFeet * 304.8;

                    // Вывод информации Имени уровня и Отметки уровня
                    levelInformation.Append("\n" + levelName + "__на отм__" + "[" + levelElevationSm + "]" + "\n");
                    levelInformation.Append("---------------------------------------------");
                }
            }
            TaskDialog.Show("Информация по уровням", levelInformation.ToString());
        }
        
        
        /// <summary>
        /// Метод FindBottomElemLevel для определения Нижнего уровня отметки Низа элемента
        /// </summary>
        /// <param name="elementBottomPointElevationSm">Отметка Низа элемента из Документа</param>
        /// <param name="levels">Список с Уровнями из Документа</param>
        /// <returns></returns>
        public static Level FindBottomElemLevel(double elementBottomPointElevationSm, IEnumerable<Level> levels)
        {	
			// Бокс для хранения результата метода
            Level closestBottomLevel = null;

            foreach (var level in levels)
            {
                var elev = level.Elevation;
                var upper_Level = level.FindTopLevel(levels);
                var lower_Level = level.FindBotLevel(levels);
                
                // Если Нижняя точка меньше Верхнего уровня && Нижняя точка больше или равна Нижнему уровню,
                // то запишется значение уровня ниже точки.
                // Условие для точек между уровнями, но не границах
                if (elementBottomPointElevationSm < upper_Level.Elevation && 
                    elementBottomPointElevationSm + 0.003 >= lower_Level.Elevation) // + 0.003 - погрешность BoundingBox
                {
                    closestBottomLevel = lower_Level;
                }
                
                // Если Нижняя точка ниже Самого низкого уровня, то запишется значение самого низкого уровня в проекте.
                // Граничное условие для точек ниже самого низкого уровня
                if (elementBottomPointElevationSm < lower_Level.Elevation && 
                    lower_Level.Id == level.Id)
                {
                    closestBottomLevel = lower_Level;
                }
                
                // Если Нижняя точка больше или равна отметки самого высокого уровня,
                // то запишется самый высокий уровень в проекте
                // РЕШЕНИЕ - ">=" а не ">"
                if (elementBottomPointElevationSm + 0.003 >= upper_Level.Elevation && 
                    upper_Level.Id == level.Id) // + 0.003 - погрешность BoundingBox
                {
                     closestBottomLevel = upper_Level;
                }
            }

            return closestBottomLevel;
        }


        /// <summary>
        /// Метод берет первый элемент который в списке по возрастанию. В данном случае берет самый близкий Верхний уровень 
        /// </summary>
        /// <param name="level">Текущий уровень</param>
        /// <param name="levels">Все уровни</param>
        /// <returns></returns>
        public static Level FindTopLevel(this Level level, IEnumerable<Level> levels)
        {
            // Переменная для хранения найденного уровня
            Level topLevel = null;
            // В переменную ordered переданы уровни отсортированные по значению высоты. От Меньшего к Большему (Снизу -> Вверх)
            IOrderedEnumerable<Level> ordered = levels.OrderBy(l => l.Elevation);
            // Фильтрация и сортировка уровней
            IEnumerable<Level> oLevels = ordered
                //Сортирует уровни по высоте (Elevation) в порядке возрастания (от самого низкого к высокому).
                // Коллекция уровней oLevels, отсортированная от самого низкого к высокому, но не ниже текущего уровня.
                .Where(x => x.Elevation >= level.Elevation);
            // Запись Уровня в topLevel с условием (если в перечислении уровней oLevels число уровней больше 2-ух, то - пропустить 1-й и взять первый, если нет - взять первый)
            topLevel = oLevels.Count() >= 2 ? oLevels.Skip(1).FirstOrDefault() : oLevels.FirstOrDefault();
            
            return topLevel;
        }
        
        
        /// <summary>
        /// Метод берет первый элемент который в списке по убыванию
        /// В данном случае берет самый близкий Нижний уровень 
        /// </summary>
        /// <param name="level">Текущий уровень</param>
        /// <param name="levels">Все уровни</param>
        /// <returns></returns>
        public static Level FindBotLevel(this Level level, IEnumerable<Level> levels)
        {
            // Переменная для хранения найденного уровня
            Level botLevel = null;
            // Фильтрация и сортировка уровней
            var oLevels = levels.
                //Сортирует уровни по высоте (Elevation) в порядке убывания (от самого высокого к низкому).
                OrderByDescending(l => l.Elevation)
                // Фильтрует только те уровни, которые находятся ниже или на той же высоте, что и level.
                //.Where(x => x.Elevation <= level.Elevation); //
                .Where(x => x.Elevation <= level.Elevation); // Коллекция уровней oLevels, отсортированная от самого высокого к низкому, но не выше текущего уровня.
            // Запись Уровня в botLevel с условием (если в перечислении уровней oLevels число уровней больше 2-ух, то - пропустить 1-й и взять первый, если нет - взять первый)
            botLevel = oLevels.Count() >= 2 ? oLevels.Skip(1).FirstOrDefault() : oLevels.FirstOrDefault();
            
            return botLevel;
        }
        
        
    }
    
}
