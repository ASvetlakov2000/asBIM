using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Nice3point.Revit.Extensions;
using Notifications.Wpf;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Binding = Autodesk.Revit.DB.Binding;


namespace asBIM
{
    internal static class RevitAPI
    {
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
                BuiltInCategory.OST_PipeAccessory, // Арматура труб
                BuiltInCategory.OST_DuctAccessory, // Арматура воздуховодов
                BuiltInCategory.OST_PipeFitting, // Фитинги труб
                BuiltInCategory.OST_DuctFitting, // Фитинги воздуховодов
                BuiltInCategory.OST_PlumbingFixtures, // Сантехнические приборы
                BuiltInCategory.OST_PlumbingEquipment, // Сантехническое оборудование
                BuiltInCategory.OST_DuctTerminal, // Воздухораспределители
                BuiltInCategory.OST_CableTrayFitting, // Соединительные детали кабельных лотков
                BuiltInCategory.OST_ConduitFitting, // Соединительные детали коробов
                BuiltInCategory.OST_Sprinklers, // Спринклеры
                BuiltInCategory.OST_TelephoneDevices, // Телефонные устройства
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
                // BuiltInCategory.OST_Parking, // Парковочные места
                // BuiltInCategory.OST_Railings, // Ограждения
                BuiltInCategory.OST_Topography, // Топография
                // BuiltInCategory.OST_Planting, // Растения
                // BuiltInCategory.OST_Entourage, // Окружение
                BuiltInCategory.OST_StructuralFraming, // Каркасы
                BuiltInCategory.OST_StructuralTruss, // Фермы
                BuiltInCategory.OST_StructuralFoundation, // Конструктивные фундаменты
                // BuiltInCategory.OST_Cameras, // Камеры
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

    /// <summary>
    /// Класс для определения отметок верха и низа элемента по Bounding Box
    /// </summary>
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
        

        /// <summary>
        /// Метод XYZ GetLinearPoint для получения Отметок в Начале и Отметок в Конце
        /// </summary>
        /// <param name="curve">Принимает обьекты на основе кривой</param>
        /// <param name="end">Принимает конечную точку кривой, для проверки на начало или конец</param>
        /// <returns>XYZ locationCurve?.GetEndPoint((bool)end ? 1 : 0)</returns>>
        public static XYZ GetLinearPoint(MEPCurve curve, bool? end = null)
        {
            var location = curve.Location;
            var locationCurve = (location as LocationCurve)?.Curve;
            return locationCurve?.GetEndPoint((bool)end ? 1 : 0);
        }

        // Способ 2
        public static XYZ GetPoint(Element e, bool? max = null)
        {
            if (e is null) return new XYZ();
            var location = e.Location;
            switch (location)
            {
                case LocationCurve locationCurve:
                    var top = locationCurve.Curve.GetEndPoint(0);
                    var bottom = locationCurve.Curve.GetEndPoint(1);
                    if (max is bool tr)
                    {
                        if (!tr)
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

    /// <summary>
    /// Класс для определения принадлежности уровня элемента
    /// </summary>
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
                .Where(x => x.Elevation <=
                            level.Elevation); // Коллекция уровней oLevels, отсортированная от самого высокого к низкому, но не выше текущего уровня.
            // Запись Уровня в botLevel с условием (если в перечислении уровней oLevels число уровней больше 2-ух, то - пропустить 1-й и взять первый, если нет - взять первый)
            botLevel = oLevels.Count() >= 2 ? oLevels.Skip(1).FirstOrDefault() : oLevels.FirstOrDefault();

            return botLevel;
        }


    }

    public abstract class BasePtPosition
    {
        public static double GetBasePointHeight(Document doc)
        {
            // Найти базовую точку проекта
            var basePoint = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_ProjectBasePoint) // Категория базовой точки
                .OfClass(typeof(BasePoint))                       // Убедиться, что это BasePoint
                .Cast<BasePoint>()
                .FirstOrDefault();                               // Получить первую найденную

            if (basePoint == null)
            {
                throw new InvalidOperationException("Базовая точка проекта не найдена.");
            }

            // Получить параметр высоты (Elevation)
            Parameter elevationParam = basePoint.get_Parameter(BuiltInParameter.BASEPOINT_ELEVATION_PARAM);

            if (elevationParam == null || !elevationParam.HasValue)
            {
                throw new InvalidOperationException("Не удалось получить параметр высоты базовой точки.");
            }

            // Преобразовать значение параметра в тип double (в футах)
            double elevation = elevationParam.AsDouble();

            // Вернуть высоту
            return elevation;
        }
        
    }


    /// <summary>
    /// Добавляет общий параметр в проект Revit.
    /// </summary>
    public static class SharedParameterHelper
    {
        /// <summary>
        /// Добавляет общий параметр в проект Revit с дополнительными настройками. Используется когда в ФОП нет параметра
        /// </summary>
        /// <param name="doc">Текущий документ Revit.</param>
        /// <param name="parameterName">Имя общего параметра.</param>
        /// <param name="parameterGroup">Группа общего параметра (например, Data или Structural).</param>
        /// <param name="specTypeId">Тип спецификации параметра (например, SpecTypeId.StringText, SpecTypeId.Area).</param>
        /// <param name="isInstance">True, если параметр экземплярный, иначе типовой.</param>
        /// <param name="isReadOnly">True, если параметр только для чтения, иначе False.</param>
        /// <param name="allowValuesPerGroup">True, если значения могут изменяться по экземпляру группы.</param>
        /// <param name="categories">Список категорий, к которым нужно привязать параметр.</param>
        public static void AddSharedParameter(
            Document doc,
            string parameterName,
            BuiltInParameterGroup parameterGroup,
            ForgeTypeId specTypeId,
            bool isInstance,
            // bool isReadOnly,
            bool allowValuesPerGroup,
            List<Category> categories)
        {
            // Проверка на наличие общего файла параметров
            Application app = doc.Application;
            DefinitionFile sharedParameterFile = app.OpenSharedParameterFile();

            if (sharedParameterFile == null)
            {
                TaskDialog.Show("Ошибка", "Общий файл параметров не настроен. Укажите общий файл параметров в настройках Revit.");
                return;
            }

            // Поиск или создание группы в общем файле параметров
            DefinitionGroup group = sharedParameterFile.Groups.get_Item("Параметры для asBim")
                ?? sharedParameterFile.Groups.Create("Параметры для asBim");

            // Поиск или создание общего параметра
            ExternalDefinition definition = group.Definitions.get_Item(parameterName) as ExternalDefinition;

            if (definition == null)
            {
                ExternalDefinitionCreationOptions options = new ExternalDefinitionCreationOptions(parameterName, specTypeId)
                {
                    Description = "Общий параметр, добавленный через API. asBim",
                    Visible = true,
                };
                definition = group.Definitions.Create(options) as ExternalDefinition;
            }

            // Проверка на существование параметра в проекте по имени
            ElementId existingParamId = GetParameterElementIdByName(doc, parameterName);
            if (existingParamId != null)
            {
                // РАСКОММЕНТИТЬ TaskDialog ДЛЯ ПРОВЕРКИ НА СУЩЕСТВОВАНИЕ ПАРАМЕТРА
                TaskDialog.Show("Информация", $"Параметр [{parameterName}] уже существует в проекте.");
                return;
            }
            
            // Проверка, существует ли параметр с таким GUID в проекте
            ElementId existingParamName = GetParameterElementId(doc, definition);
            if (existingParamName != null)
            {
                TaskDialog.Show("Информация", $"Параметр [{parameterName}] уже существует в проекте.");
                return;
            }
            
            // Создание категории для привязки
            CategorySet categorySet = new CategorySet();
            foreach (Category category in categories)
            {
                categorySet.Insert(category);
            }

            // Создание привязки
            Binding binding = isInstance ? (Binding)new InstanceBinding(categorySet) : new TypeBinding(categorySet);

            using (Transaction transaction = new Transaction(doc, "Добавление общего параметра"))
            {
                transaction.Start();

                // Добавление параметра в BindingMap
                BindingMap bindingMap = doc.ParameterBindings;
                if (!bindingMap.Insert(definition, binding, parameterGroup))
                {
                    TaskDialog.Show("Ошибка", $"Не удалось добавить общий параметр [{parameterName}].");
                    transaction.RollBack();
                    return;
                }

                // Получение ID параметра
                ElementId paramElementId = GetParameterElementId(doc, definition);

                if (paramElementId != null)
                {
                    // TODO: 4. Обновление кода для смены "Значения могут изменяться по экземпляру группы"
                    
                    // Настройка "Значения могут изменяться по экземпляру группы"
                    ParameterElement parameterElement = doc.GetElement(paramElementId) as ParameterElement;
                    if (parameterElement != null && parameterElement is SharedParameterElement sharedParamElement)
                    {
                        sharedParamElement.GetDefinition().SetAllowVaryBetweenGroups(doc, allowValuesPerGroup);
                        
                        // Настройка "Только для чтения"
                        // parameterElement.SetUserModifiable(!isReadOnly);
                    }
                }

                transaction.Commit();
            }
            
            TaskDialog.Show("Успех", $"Общий параметр '{parameterName}' успешно добавлен.");
        }

        /// <summary>
        /// Получает ElementId для общего параметра.
        /// </summary>
        /// <param name="doc">Текущий документ Revit.</param>
        /// <param name="definition">Настройки общих параметров.</param>
        public static ElementId GetParameterElementId(Document doc, ExternalDefinition definition)
        {
            foreach (ParameterElement paramElement in new FilteredElementCollector(doc).OfClass(typeof(ParameterElement)))
            {
                Definition paramDef = paramElement.GetDefinition();
                if (paramDef is ExternalDefinition extDef && extDef.GUID == definition.GUID)
                {
                    return paramElement.Id;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Проверяет существование параметра с указанным именем в проекте.
        /// </summary>
        /// <param name="doc">Текущий документ Revit.</param>
        /// <param name="parameterName">Имя искомого параметра.</param>
        /// <returns>paramElement.Id</returns>>
        public static ElementId GetParameterElementIdByName(Document doc, string parameterName)
        {
            foreach (ParameterElement paramElement in new FilteredElementCollector(doc).OfClass(typeof(ParameterElement)))
            {
                Definition paramDef = paramElement.GetDefinition();
                if (paramDef != null && paramDef.Name == parameterName)
                {
                    return paramElement.Id;
                }
            }
            return null;
            
            // Пример использования
            // Пример добавления категорий для общего параметра

            // List<Category> categories = new List<Category>
            // {
            //     doc.Settings.Categories.get_Item(BuiltInCategory.OST_Walls),
            //     doc.Settings.Categories.get_Item(BuiltInCategory.OST_Floors)
            // };
            //

            // Пример Вызова

            // SharedParameterHelper.AddSharedParameter(
            // doc,
            // "Custom Parameter",
            // BuiltInParameterGroup.PG_DATA,
            // SpecTypeId.StringText,
            // isInstance: true,
            // isEditable: true,
            // isInstancePerGroupType: true,
            // categories: categories
            // );

            // Пример Вызова
            
        }
        
       
        /// <summary>
        /// Добавляет общий параметр из ФОПа в проект Revit с дополнительными настройками. Используется когда в ФОП уже создан параметр
        /// </summary>
        /// <param name="doc">Текущий документ Revit.</param>
        /// <param name="sharedParameterFilePath">Путь к ФОПу.</param>
        /// <param name="parameterName">Имя общего параметра.</param>
        /// <param name="parameterGroup">Группа общего параметра (например, Data или Structural).</param>
        /// <param name="isInstance">True, если параметр экземплярный, иначе типовой.</param>
        /// <param name="category">Список категорий, к которым нужно привязать параметр.</param>
        /// <param name="allowValuesPerGroup">True, если значения могут изменяться по экземпляру группы.</param>
        public static void AddSharedParameterFromFOP(
        Document doc, 
        string sharedParameterFilePath, 
        string parameterName, 
        BuiltInParameterGroup parameterGroup,
        bool isInstance,
        List<Category> categories,
        bool messageStandart)
        // ForgeTypeId forgeTypeId)
        {
            // Получаем приложение Revit
            Application app = doc.Application;

            // Загружаем файл общих параметров
            app.SharedParametersFilename = sharedParameterFilePath;
            DefinitionFile defFile = app.OpenSharedParameterFile();

            if (defFile == null)
            {
                TaskDialog.Show("Ошибка", "Не удалось открыть файл общих параметров.");
                return;
            }

            // Находим определение параметра по имени в ФОПе
            Definition definition = null;
            foreach (DefinitionGroup group in defFile.Groups)
            {
                definition = group.Definitions.get_Item(parameterName);
                if (definition != null)
                    break;
            }
            
            // Проверка на наличие параметра по имени. Если нет то TaskDialog с ошибкой
            if (definition == null)
            {
                TaskDialog.Show("Ошибка", $"Параметр [{parameterName}] в ФОП не найден.");
                
                // Уведомление. "Общий параметр добавлен!"
                NotificationManagerWPF.MessageInfo(
                    "Параметр не найден!",
                    "\u00af\u00af\u00af\u00af\u00af\u00af\\_(ツ)_/\u00af\u00af\u00af" +
                            "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af" +
                    "\n\nПараметры: " 
                    +
                    $"\n[{parameterName}]",
                    NotificationType.Error);
                
                return;
            }

            // Создаем параметр
            using (Transaction tx = new Transaction(doc, "Добавление общего параметра"))
            {
                tx.Start();

                // Создание категории для привязки
                CategorySet categorySet = new CategorySet();
                foreach (Category category in categories)
                {
                    categorySet.Insert(category);
                }

                // Создаем экземпляр параметра
                ExternalDefinition externalDef = definition as ExternalDefinition;

                // Устанавливаем ForgeTypeId
                // if (externalDef != null)
                // {
                //     externalDef.SetForgeTypeId(forgeTypeId);
                // }

                // Создаем привязку в зависимости от типа параметра
                Binding binding;
                if (isInstance)
                {
                    // Создаем привязку для экземпляров
                    binding = doc.Application.Create.NewInstanceBinding(categorySet);
                }
                else
                {
                    // Создаем привязку для типов
                    binding = doc.Application.Create.NewTypeBinding(categorySet);
                }
                
                // Получение ID параметра
                // ElementId paramElementId = GetParameterElementId(doc, externalDef);
                //
                // if (paramElementId != null)
                // {
                //     // TODO: 4. Обновление кода для смены "Значения могут изменяться по экземпляру группы"
                //
                //     // Настройка "Значения могут изменяться по экземпляру группы"
                //     ParameterElement parameterElement = doc.GetElement(paramElementId) as ParameterElement;
                //     // Настройка "Значения могут изменяться по экземпляру группы"
                //     if (parameterElement != null && parameterElement is SharedParameterElement sharedParamElement)
                //     {
                //         sharedParamElement.GetDefinition().SetAllowVaryBetweenGroups(doc, allowValuesPerGroup);
                //     
                //         // Настройка "Только для чтения"
                //         // parameterElement.SetUserModifiable(!isReadOnly);
                //     }
                // }

                // Добавляем параметр в проект
                doc.ParameterBindings.Insert(externalDef, binding);

                // Если true - то появится сообщение ниже, если false - то сообщение ниже не появится, но потребуется добавить специальное сообщение
                if (messageStandart)
                {
                    // Уведомление. "Общий параметр добавлен!"
                    NotificationManagerWPF.MessageInfo(
                        "Параметр добавлен!",
                        "\n(￢‿￢ )" +
                        "\n\nПараметры: "
                        +
                        $"\n[{parameterName}]",
                        NotificationType.Success);
                }

                tx.Commit();
            }
            
            /*
             ПРИМЕР ИСПОЛЬЗОВАНИЯ
             
            string sharedParameterFilePath = @"C:\Users\12kiw\Desktop\Андрей\00000_С#\00000_С#\03_Revit_Plugin\05_Helpers\ФОП_Г03.txt"; 
            string parameterName = "PRO_ТХ_Группа в пространстве"; // Укажите имя общего параметра
            BuiltInCategory category = BuiltInCategory.OST_MEPSpaces; // Укажите категорию, к которой будет применяться параметр
            bool isInstance = true; // Укажите, является ли параметр экземплярным
            bool allowValuesPerGroup = true;

            SharedParameterHelper.AddSharedParameterFromFOP(doc, sharedParameterFilePath, parameterName, category, allowValuesPerGroup, isInstance);
            
            ПРИМЕР ИСПОЛЬЗОВАНИЯ
            */
        }

        ///<summaru>
        /// Метод для поиска параметра в документе по GUID
        ///</summaru>
        /// <param name="doc">Текущий документ Revit.</param>
        /// <param name="parameterGuid">Guid параметра, который надо найти в документе.</param>
        /// <returns>true если параметр найден, false если параметр не найден</returns>>
        public static bool FindSharedParameterByGUID(Document doc, Guid parameterGuid)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IEnumerable<SharedParameterElement> sharedParameters = collector
                .OfClass(typeof(SharedParameterElement))
                .Cast<SharedParameterElement>();

            return sharedParameters.Any(param => param.GuidValue == parameterGuid);
        }

        
        /// <summary>
        /// Метод для задания значения `SetAllowVaryBetweenGroups` 
        /// option for instance binding param
        /// </summary>
        public static void SetInstanceParamVaryBetweenGroupsBehaviour(
            Document doc, 
            Guid guid, 
            bool allowVaryBetweenGroups = true)
        {
            //Транзакция
            using (Transaction tr = new Transaction(doc, "Смена значения с типа на экземляр"))
            {
                tr.Start();
                try // last resort
                {
                    SharedParameterElement sp
                        = SharedParameterElement.Lookup(doc, guid);

                    if (null == sp) return;

                    InternalDefinition def = sp.GetDefinition();

                    if (def.VariesAcrossGroups != allowVaryBetweenGroups)
                    {
                        // Must be within an outer transaction!

                        def.SetAllowVaryBetweenGroups(doc, allowVaryBetweenGroups);
                    }
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Ошибка", ex.Message);
                }
                tr.Commit();
            }
        }
        
        public static void UpdateCategoriesForSharedParameter(Document doc, string parameterName, List<BuiltInCategory> newCategories)
        {
            // Начало транзакции
            using (Transaction trans = new Transaction(doc, "Обновление категорий общего параметра"))
            {
                trans.Start();

                // Получаем BindingMap, где хранятся все связывания параметров
                BindingMap bindingMap = doc.ParameterBindings;

                // Найти параметр по имени
                Definition parameterDefinition = null;
                DefinitionBindingMapIterator iterator = bindingMap.ForwardIterator();
                iterator.MoveNext();
                foreach (var kvp in bindingMap)
                {
                    Definition definition = iterator.Key;
                    if (definition.Name == parameterName)
                    {
                        parameterDefinition = definition;
                        break;
                    }
                }

                if (parameterDefinition == null)
                {
                    TaskDialog.Show("Ошибка", $"Параметр '{parameterName}' не найден в модели.");
                    trans.RollBack();
                    return;
                }

                // Получаем текущее связывание параметра
                ElementBinding elementBinding = bindingMap.get_Item(parameterDefinition) as ElementBinding;
                if (elementBinding == null)
                {
                    TaskDialog.Show("Ошибка", $"Параметр '{parameterName}' не имеет связей с категориями.");
                    trans.RollBack();
                    return;
                }

                // Создаем обновленный набор категорий
                var currentCategories = elementBinding.Categories;
                CategorySet updatedCategorySet = doc.Application.Create.NewCategorySet();

                // Добавляем существующие категории
                foreach (Category category in currentCategories)
                    updatedCategorySet.Insert(category);

                // Добавляем новые категории, если их еще нет в наборе
                foreach (BuiltInCategory builtInCategory in newCategories)
                {
                    Category newCategory = doc.Settings.Categories.get_Item(builtInCategory);
                    if (!updatedCategorySet.Contains(newCategory))
                        updatedCategorySet.Insert(newCategory);
                }

                // Создаем новое связывание на основе типа существующего
                if (elementBinding is InstanceBinding)
                {
                    bindingMap.ReInsert(parameterDefinition, doc.Application.Create.NewInstanceBinding(updatedCategorySet));
                }
                else if (elementBinding is TypeBinding)
                {
                    bindingMap.ReInsert(parameterDefinition, doc.Application.Create.NewTypeBinding(updatedCategorySet));
                }

                trans.Commit();
            }
        }
    }
    
    
    internal class DeleteSelectionGroup : ISelectionFilter
    {
        // Guid groupGuid, Guid spaceGuid,
        public void DoSmth()
        {
            
        }

        public bool AllowElement(Element element)
        {
            return false;
        }

        public bool AllowReference(Reference reference, XYZ point)
        {
            return false;
        }
    }
}



    


    


