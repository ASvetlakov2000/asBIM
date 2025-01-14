using Autodesk.Revit.Attributes;
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
using System.Globalization;
using System.Xaml;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Windows;
using asBIM.ViewModel;
using Notifications.Wpf;
using CommunityToolkit.Mvvm.Input;
using Nice3point.Revit.Extensions;


namespace asBIM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Code_PlaceGroupsInSpacesTX : IExternalCommand
    {
        // Guid общего параметра "PRO_ID группы в пространстве"
        Guid shParamGuid = new Guid("bae21547-43fa-423f-91f9-ff8b42d50560");
        // Имя общего параметра "PRO_ID группы в пространстве
        string shParamName = "PRO_ID группы в пространстве";
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            // ОСНОВНОЙ КОД ПЛАГИНА // НАЧАЛО  

            // Добавление общего параметра [PRO_ТХ_Группа в пространстве]
            
            /*
             List<Category> categories = new List<Category>
            {
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_MEPSpaces),
            };

            // TODO: Вызывается каждый раз при выполнении. Исправить
            SharedParameterHelper.AddSharedParameterFromFOP(
                doc,
                // путь к ФОП
                OpenFile.OpenSingleFile("Выберите ФОП [PRO_SharedParametr] для добавления параметра", "txt"),
                // Имя создаваемого параметра
                shParamName,
                // Куда относится параметр
                BuiltInParameterGroup.PG_IDENTITY_DATA,
                // Параметр для экземпляра
                true,
                // ссылка на список с категориями
                categories);

            // Уведомление. "Общий параметр добавлен!"
            NotificationManagerWPF.Message(
                "Общий параметр добавлен!",
                "\n(￢‿￢ )" +
                "\n\nПараметр: " +
                "\n[PRO_ID группы в пространстве] добавлен для Пространств!" +
                "\n\nВ параметр записывается ID группы, которая была добавлена в пространство" +
                "\n\nПример заполнения: 010101");
                */
            
            // Добавление общего параметра [PRO_ТХ_Группа в пространстве]
            

            // Установка значения "Изменяется по экз групп" для общего параметра "PRO_ID группы в пространстве"
            // TODO: Не работает
            // SharedParameterHelper.SetInstanceParamVaryBetweenGroupsBehaviour(doc, shParamGuid, true);

            // Время выполнения
            Stopwatch swPlaceGroups = new Stopwatch();
            swPlaceGroups.Start();
        
            // Размещение групп по одноименным пространствам
            PlacementOfGroupsInSpaces(doc, GetSpacesFromDoc(doc), GetTxGroupsFromDoc(doc));
        
            swPlaceGroups.Stop();
            var timeInSecForCommand = swPlaceGroups.Elapsed.TotalSeconds;
        
            // Уведомление. "Время работы"
            NotificationManagerWPF.TimeOfWork("Время работы", 
                timeInSec:"\nВремя выполнения " + Convert.ToString(Math.Round(Convert.ToDouble(timeInSecForCommand), 0, MidpointRounding.AwayFromZero) + " сек"),
                NotificationType.Information);
            
            
            // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ  
            return Result.Succeeded;
        }
        
        /// <summary>
        /// Метод для проверки наличия общего параметра в проекте
        /// <param name = "doc" > Документ </param>
        /// <param name = "shParamName" > Guid общего параметра</param>
        /// <returns>True - если параметр найден, False - если параметр не найден</returns>
        /// </summary>
        public bool SharedParametrValidChecker(Document doc, string shParamName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IEnumerable<SharedParameterElement> sharedParameters = collector
                .OfClass(typeof(SharedParameterElement))
                .Cast<SharedParameterElement>();
            string paramName = sharedParameters.First().Name;
            return sharedParameters.Any(param => paramName == shParamName);
        }

        /// <summary>
        /// Метод для сбора Пространств 
        /// <param name = "doc" > Документ </param>
        /// <returns>centerOfSpace - список с Пространствами, отсортированный по возрастанию значению номера (целое)</returns>
        /// </summary>
        public List<SpatialElement> GetSpacesFromDoc(Document doc)
        {
            // Замена "," на "."
            NumberFormatInfo format = new NumberFormatInfo {NumberDecimalSeparator = "."};
            // Сбор Пространств в коллекцию с сортировкой по категории и выбор только элементов
            FilteredElementCollector collectorSpaces = new FilteredElementCollector(doc);
            collectorSpaces.OfCategory(BuiltInCategory.OST_MEPSpaces).ToElements();
            // Сбор Пространств в список.
            // Сортировка Пространств в списке по Номеру Пространств
            List<SpatialElement> spacesList = collectorSpaces.Cast<SpatialElement>()
                // Конвертация номера Пространств из строки в целое
                .OrderBy(space => Convert.ToDouble(space.Number, format))
                .ToList();
            
            // ОТЛАДКА 2
            // string spaceName = null;
            //
            // StringBuilder sb = new StringBuilder();
            // // Перебираем все найденные Пространства и выводим информацию по ним
            // foreach (Element space in spacesList)
            // {
            //     // Приведение Element к SpatialElement
            //     SpatialElement spatialElement = space as SpatialElement;
            //     // Проверка на null
            //     if (spatialElement != null)
            //     {
            //         // Получение имени Пространств. Берется из списка типа Element
            //         string spNames = space.Name;
            //         // Получение только Имени Пространства путем удаления Номера через разделитель "пробел".
            //         spaceName = spNames.Split(' ').First();
            //         
            //         // ОТЛАДКА 1
            //         // Вывод списка Пространств через StringBuilder
            //         sb.AppendLine(spaceName);
            //         sb.AppendLine("-----------------");
            //     }
            // }
            // if (spacesList.Count > 0)
            // {
            //     TaskDialog.Show("Отладка. Перечень пространств", sb.ToString());
            // }
            // else
            // {
            //     TaskDialog.Show("Упс!", "В проекте отсутствуют пространства!");
            // }
            // // ОТЛАДКА 2
            
            // ОТЛАДКА 3. координат Пространств
            // GetSpaceCenter(spacesList);
            // ОТЛАДКА 3. координат Пространств
            
            return spacesList;
        }
        
        /// <summary>
        /// Метод GetTxGroupsFromDoc для сбора Групп
        /// <param name = "doc" > Документ </param>
        /// <returns>centerOfSpace - список с Группами, отсортированный по возрастанию значения имени (строки)</returns>
        /// </summary>
        public List<Group> GetTxGroupsFromDoc(Document doc)
        {
            // Сбор Групп в коллекцию с сортировкой по категории и выбор только элементов
            FilteredElementCollector collectorGroups = new FilteredElementCollector(doc);
            collectorGroups.OfClass(typeof(Group)).ToElements();
            // Сбор Групп в список.
            // Сортировка Групп в списке по Номеру Групп
            List<Group> groupsList = collectorGroups.Cast<Group>()
                // Конвертация номера Групп из строки в целое
                .OrderBy(group => group.Name)
                .ToList();
            
            // // ОТЛАДКА 2
            // StringBuilder sb = new StringBuilder();
            //
            // // Перебираем все найденные Группы и выводим информацию по ним
            // foreach (Element group in groupsList)
            // {
            //     // Приведение Element к Group
            //     Group g = group as Group;
            //     // Проверка на null
            //     if (g != null)
            //     {
            //         // Получение имени Групп. Берется из списка типа Element
            //         string groupNames = g.Name;
            //         
            //         // ОТЛАДКА 1
            //         sb.AppendLine(groupNames);
            //         sb.AppendLine("---------------");
            //     }
            // }
            // if (groupsList.Count > 0)
            // {
            //     TaskDialog.Show("Отладка. Перечень групп",sb.ToString());
            // }
            // else
            // {
            //     TaskDialog.Show("Упс!","В проекте отсутствуют группы!");
            // }
            // // ОТЛАДКА 2

            // ОТЛАДКА 3. координат Групп
            //GetSpaceCenter(groupsList);
            // ОТЛАДКА 3. координат Групп
            
            
            return groupsList;
        }
        
        /// <summary>
        /// Возвращает координаты центра элемента
        /// </summary>
        /// <param name="elem">Элемент</param>
        /// <returns>Координаты центра</returns>
        public XYZ GetSpaceBottomCenter(Element elem)
        {
            BoundingBoxXYZ bounding = elem.get_BoundingBox(null);
            if (bounding != null)
            {
                XYZ center = (bounding.Max + bounding.Min) * 0.5;
                LocationPoint loc = elem.Location as LocationPoint;
                XYZ spaceCenter  = new XYZ(center.X, center.Y, loc.Point.Z);
                return spaceCenter;
            }
            return null;
        }
        
        /// <summary>
        /// Сопоставляет Имена Пространств и Групп
        /// <param name = "doc" > Документ </param>
        /// <param name = "spacesList" > Список с Пространствами </param>
        /// <param name = "groupsList" > Список с Группами </param>
        /// </summary>
        public void PlacementOfGroupsInSpaces(Document doc, List<SpatialElement> spacesList, List<Group> groupsList)
        {
            // Счетчик для количества элементов. Удачно
            string placedGroupCountStr;
            IList<Group> placedGroupCount = new List<Group>();
            
            // Счетчик для количества элементов. Не дачно
            string placedErrGroupCountStr;
            IList<Group> placedErrGroupCount = new List<Group>();
            
            // Проверка на отсутствие Пространств
            if (!spacesList.Any())
            {
                TaskDialog.Show("Ошибка!", "В проекте отсутствуют пространства.");
                return;
            }
            
            // Проверка на отсутствие Групп
            if (!groupsList.Any())
            {
                TaskDialog.Show("Ошибка!", "В проекте отсутствуют группы.");
                return;
            }
            
            StringBuilder sb = new StringBuilder();
            // StringBuilder sbSpacesWithName = new StringBuilder(); // Для хранения ID пространств с именем. Удачно
            // StringBuilder sbSpacesWithoutName = new StringBuilder(); // Для хранения ID пространств без имени. Не удачно

            using (Transaction tr = new Transaction(doc, "Расстановка групп в пространства"))
            {
                tr.Start();
                foreach (SpatialElement space in spacesList)
                {
                    // Получаем имя пространства
                    string spaceName = space.Name.Split(' ').FirstOrDefault();
                    string spaceID = space.Id.ToString();

                    // GUID общего параметра из ФОП [PRO_ТХ_Группа в пространстве]
                    Guid shParamGuid = new Guid("bae21547-43fa-423f-91f9-ff8b42d50560");
                    
                    Parameter placedGropupsChecker =
                        space.get_Parameter(shParamGuid);
                    
                    if (placedGropupsChecker != null && !placedGropupsChecker.HasValue)
                    {
                        // TODO: 1. Отредактировать проверку на пустое пространство
                        // TODO: 2. Добавить проверку на пустое имя пространства через ID

                        // TODO: // НЕ РАБОТАЕТ
                        // Проверяем, что имя пространства не пустое
                        // if (string.IsNullOrWhiteSpace(spaceName))
                        // {
                        //     // Добавляем ID пространства в список
                        //     sbSpacesWithoutName.AppendLine($"ID: {spaceID}");
                        //     continue; // Переходим к следующему пространству
                        // }
                        // НЕ РАБОТАЕТ

                        // Сопоставляем пространство с группами
                        List<Group> matchingGroups = groupsList
                            .Where(group => !string.IsNullOrEmpty(group.Name) && // Проверка имени группы
                                            group.Name.Equals(spaceName, StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        if (matchingGroups.Any())
                        {
                            foreach (Group group in matchingGroups)
                            {
                                XYZ placementPoint = GetSpaceBottomCenter(space);
                                // string groupId = group.GroupId.ToString();
                                if (placementPoint != null)
                                {
                                    try
                                    {
                                        // Размещение групп в одноименных пространствах 
                                        Group placedGroup = doc.Create.PlaceGroup(placementPoint, group.GroupType);
                                        // Получение ID размещенной группы
                                        ElementId placedGroupId = placedGroup.Id;
                                        
                                        // Подсчет количества элементов
                                        // TODO: Подсчет количества элементов
                                        // IList<Group> placedGroupCount = new List<Group>();
                                        placedGroupCount.Add(placedGroup);
                                        
                                        
                                        //sbSpacesWithName.AppendLine($"Количество размещенных групп: {placedGroupCount.Count.ToString()}");

                                        Parameter spaceParameterID = space.get_Parameter(shParamGuid);
                                        if (spaceParameterID != null)
                                        {
                                            spaceParameterID.Set(placedGroupId.ToString());
                                        }
                                        
                                        // sb.AppendLine($"Группа с ID [{placedGroupId}] и Именем [{group.Name}] \nразмещена в пространстве [{spaceName}].");
                                    }
                                    catch (Exception ex)
                                    {
                                        sb.AppendLine($"Ошибка при размещении исходной группы с ID [{group.Id}] и Именем [{group.Name}]: {ex.Message}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            placedErrGroupCount.Add(matchingGroups.FirstOrDefault());
                            sb.AppendLine($"Пространство с Именем [{spaceName}] и ID [{spaceID}].");
                        }
                    }
                    if (placedGropupsChecker != null && placedGropupsChecker.HasValue)
                    {
                     continue;
                    }
                }
                
                placedGroupCountStr = $"\nКоличество размещенных групп: {placedGroupCount.Count.ToString()}";
                placedErrGroupCountStr = $"\nКоличество не размещенных групп: {placedErrGroupCount.Count.ToString()}";
                
                // Уведомление. Подсчет количества элементов. Не удачно
                NotificationManagerWPF.ElemCount(
                    "Не обработанные элементы",
                    elementCount: $"{placedErrGroupCountStr} \n\n{sb}", NotificationType.Error);

                // Уведомление. Подсчет количества элементов. Успех
                NotificationManagerWPF.ElemCount(
                    "Обработанные элементы",
                    elementCount: placedGroupCountStr, NotificationType.Success);
                
                tr.Commit();
            }
        }
        
    }
} 