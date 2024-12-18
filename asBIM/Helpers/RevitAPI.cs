using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

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

    // Класс для сортировки всех ЭЭлементов в Документе
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
            // Создание списка "АР" с группами
            List<BuiltInCategory> group_AR_Cat = new List<BuiltInCategory>()
                {
                    // Перечисление всех категорий для АР
                    BuiltInCategory.OST_Walls, //Стены
                    BuiltInCategory.OST_Floors, //Полы
                    BuiltInCategory.OST_Ceilings, //Потолки
                    BuiltInCategory.OST_Columns, //Колонны
                    BuiltInCategory.OST_StructuralColumns, //Конструктивные колонны
                    BuiltInCategory.OST_Roofs, //Крыши
                    BuiltInCategory.OST_Doors, //Двери
                    BuiltInCategory.OST_Windows, //Окна
                    BuiltInCategory.OST_Stairs, //Лестницы
                    BuiltInCategory.OST_StairsRailing, //Ограждения лестниц
                    BuiltInCategory.OST_Ramps, //Пандусы

                    BuiltInCategory.OST_CurtainWallPanels, //Панели навесных стен
                    BuiltInCategory.OST_CurtaSystem, //Системы навесных стен
                    BuiltInCategory.OST_CurtainWallMullions, //Импосты навесных стен
                
                    BuiltInCategory.OST_GenericModel, //Обобщённые модели
                    BuiltInCategory.OST_Furniture, //Мебель
                    BuiltInCategory.OST_FurnitureSystems, //Мебельные системы
               
                    BuiltInCategory.OST_SpecialityEquipment, //Специальное оборудование
                
                    BuiltInCategory.OST_Parts, //Части
                };

            // Создание списка "КР" с группами
            List<BuiltInCategory> group_KR_Cat = new List<BuiltInCategory>()
            {

            };

            // TODO: 5. Продолжить списки.

            // TODO: 4. Продолжить списки с категориями для:
            // TODO: 4.1 ЭОМ
            // TODO: 4.2 ВК
            // TODO: 4.3 ОВ1
            // TODO: 4.4 ОВ2
            // TODO: 4.5 СС
            // TODO: 4.6 АИ
            // TODO: 4.7 ПТ

            // Создание словаря с Группами Элементов из Документа
            Dictionary<string, List<Element>> groupedElementsDict = new Dictionary<string, List<Element>>
            {
                ["AR"] = new List<Element>(),
                ["KR"] = new List<Element>(),
                // TODO: 6. Продолжить словарь 
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
                    if (group_AR_Cat.Contains((BuiltInCategory)elemCategory.Id.IntegerValue))
                    {
                        groupedElementsDict["AR"].Add(element);
                    }

                    // Если в ЭЭлемент коллекции collectorMain (doc) относится к Категории попадающей в список group_AR02_Cat,
                    // то элемент добавиться в словарь groupedElementsDict по ключу "АР02"
                    if (group_KR_Cat.Contains((BuiltInCategory)elemCategory.Id.IntegerValue))
                    {
                        groupedElementsDict["AR02"].Add(element);
                    }
                }

            }
            // 
            return groupedElementsDict;
        }
    }

    internal static class ElementTopBottomPt
    {
        // Метод GetElementTopPoint 
        // принимающий элемент, возращающий верхнюю точку BoundingBoxXYZ
        public static XYZ GetElementTopPoint(Element element)
        {
            // Получение BoundingBox элемента
            BoundingBoxXYZ bounding = element.get_BoundingBox(null);
            // Присвоение перем. elemTopPointFeet_XYZ значения отметки Верхней точки BoundingBox элемента 
            // Без вычитания расстояния от Начала Координат (НК)
            XYZ elemTopPointFeet_XYZ = bounding.Max;

            // Возрашает XYZ elemTopPointSm_XYZ. Значения XYZ отметки Верха в см
            return elemTopPointFeet_XYZ;
        }

        // Метод GetElementBottomPoint 
        // принимающий элемент, возращающий нижнюю точку boundingboxxyz
        public static XYZ GetElementBottomPoint(Element element)
        {
            // Получение BoundingBox элемента
            BoundingBoxXYZ bounding = element.get_BoundingBox(null);
            // Присвоение перем. elemBottomPointFeet_XYZ значения отметки Нижней точки BoundingBox элемента 
            // Без вычитания расстояния от Начала Координат (НК)
            XYZ elemBottomPointFeet_XYZ = bounding.Min;

            // Возрашает XYZ elemBottomPointSm_XYZ. Значения XYZ отметки Низа в см
            return elemBottomPointFeet_XYZ;
        }
    }

    // Класс для определения уровня элемента. Для верхнего уровня и нижнего.
    internal static class LevelInfo
    {
        // МЕТОДЫ
        // Метод для Получения значений Имени и Отметки Уровней в Документе
        public static Level GetLevelInfoInDoc(Document doc)
        {
            // Обьявление переменных для хранения Имени, Отметки.
            string levelName;           // Имя
            double levelElevationFeet;  // Отметка в футах
            double levelElevationSm;    // Отметка в см
            StringBuilder levelInformation = new StringBuilder();

            // Фильтр на Level
            FilteredElementCollector levelsCollector = new FilteredElementCollector(doc).OfClass(typeof(Level));
            // Список levels с типом Element
            List<Level> levels = levelsCollector.Cast<Level>().OrderBy(level => level.Elevation).ToList();
            // Создание контейнера типа Level
            Level levelBox = null;

            // Перебор уровней. Получение имени и отметки
            foreach (Element element in levels)
            {
                Level level = element as Level;
                if (null != level)
                {
                    // Получение Имени уровня
                    levelName = level.Name;
                    // Получение Отметки уровня в футах
                    levelElevationFeet = level.Elevation;
                    // Получение Отметки уровня в см
                    levelElevationSm = levelElevationFeet * 304.8;
                    // 
                    levelBox = level;

                    // Вывод информации Имени уровня и Отметки уровня
                    levelInformation.Append("\n" + level.Name + "__на отм__" + "[" + levelElevationSm + "]" + "\n");
                }
            }
            TaskDialog.Show("Информация по уровням", levelInformation.ToString());

            return levelBox;
        }


        public static Level FindBottomLevel(double elementBottomPointElevetionSm, IEnumerable<Level> levels)
        {

            //StringBuilder levelInformation = new StringBuilder();

            Level closestBottomLevel = null;

            foreach (var level in levels)
            {
                var elev = level.Elevation;
                var upper_Level = level.FindTop(levels);
                var lower_Level = level.FindBot(levels);

                if (elementBottomPointElevetionSm < upper_Level.Elevation && elementBottomPointElevetionSm >= lower_Level.Elevation)
                {
                    closestBottomLevel = lower_Level;
                }
                if (elementBottomPointElevetionSm < lower_Level.Elevation && lower_Level.Id == level.Id)
                {
                    closestBottomLevel = lower_Level;
                }
                if (elementBottomPointElevetionSm > upper_Level.Elevation && upper_Level.Id == level.Id)
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
        public static Level FindTop(this Level level, IEnumerable<Level> levels)
        {
            Level topLevel = null;
            var odered = levels.OrderBy(l => l.Elevation);
            var oLevels = odered.Where(x => x.Elevation >= level.Elevation);
            topLevel = oLevels.Count() >= 2 ? oLevels.Skip(1).FirstOrDefault() : oLevels.FirstOrDefault();
            var sb = new StringBuilder();

            //sb.AppendLine(level.Name);
            //sb.AppendLine("-------");
            //oLevels.Select(x => x.Name).ToList().ForEach(y => sb.AppendLine(y));
            //sb.AppendLine("-------");
            //sb.AppendLine(topLevel.Name);
            //TaskDialog.Show("check", sb.ToString());

            return topLevel;
        }


        /// <summary>
        /// Метод берет первый элемент который в списке по убыванию. В данном случае берет самый близкий Нижний уровень 
        /// </summary>
        /// <param name="level">Текущий уровень</param>
        /// <param name="levels">Все уровни</param>
        /// <returns></returns>
        public static Level FindBot(this Level level, IEnumerable<Level> levels)
        {
            Level topLevel = null;
            var oLevels = levels.OrderByDescending(l => l.Elevation).Where(x => x.Elevation <= level.Elevation);
            topLevel = oLevels.Count() >= 2 ? oLevels.Skip(1).FirstOrDefault() : oLevels.FirstOrDefault();

            //var sb = new StringBuilder();
            //sb.AppendLine(level.Name);
            //sb.AppendLine("-------");
            //oLevels.Select(x => x.Name).ToList().ForEach(y => sb.AppendLine(y));
            //sb.AppendLine("-------");
            //sb.AppendLine(topLevel.Name);
            //TaskDialog.Show("check", sb.ToString());

            return topLevel;
        }
        // МЕТОДЫ
    }
}
