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
using System.Xaml;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Windows;
using asBIM.ViewModel;
using Notifications.Wpf;
using CommunityToolkit.Mvvm.Input;


namespace asBIM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Code_PlaceGroupsInSpacesTX : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            // ОСНОВНОЙ КОД ПЛАГИНА // НАЧАЛО  

            
            // GetSpacesFromDoc(doc);
            // GetTxGroupsFromDoc(doc);
            
            PlacementOfGroupsInSpaces(doc, GetSpacesFromDoc(doc), GetTxGroupsFromDoc(doc));
            

            // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ  
            return Result.Succeeded;
        }


        /// <summary>
        /// Метод для сбора Пространств 
        /// <param name = "doc" > Документ </param>
        /// <returns>centerOfSpace - список с Пространствами, отсортированный по возрастанию значению номера (целое)</returns>
        /// </summary>
        public List<SpatialElement> GetSpacesFromDoc(Document doc)
        {
            // Сбор Пространств в коллекцию с сортировкой по категории и выбор только элементов
            FilteredElementCollector collectorSpaces = new FilteredElementCollector(doc);
            collectorSpaces.OfCategory(BuiltInCategory.OST_MEPSpaces).ToElements();
            // Сбор Пространств в список.
            // Сортировка Пространств в списке по Номеру Пространств
            List<SpatialElement> spacesList = collectorSpaces.Cast<SpatialElement>()
                // Конвертация номера Пространств из строки в целое
                .OrderBy(space => Convert.ToInt32(space.Number))
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
        public XYZ GetSpaceCenter(Element elem)
        {
            BoundingBoxXYZ bounding = elem.get_BoundingBox(null);
            if (bounding != null)
            {
                return (bounding.Max + bounding.Min) * 0.5;
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
            if (!spacesList.Any())
            {
                TaskDialog.Show("Ошибка!", "В проекте отсутствуют пространства.");
                return;
            }
            if (!groupsList.Any())
            {
                TaskDialog.Show("Ошибка!", "В проекте отсутствуют группы.");
                return;
            }

            StringBuilder sb = new StringBuilder();

            using (Transaction tr = new Transaction(doc, "Расстановка групп в пространства"))
            {
                tr.Start();
                foreach (SpatialElement space in spacesList)
                {
                    // Получаем имя пространства
                    string spaceName = space.Name.Split(' ').FirstOrDefault();

                    // TODO: 1. Отредактировать проверку на пустое пространство
                    // Проверяем, что имя пространства не пустое
                    if (string.IsNullOrEmpty(spaceName)) 
                    {
                        sb.AppendLine("Пространство с отсутствующим или некорректным именем пропущено.");
                        
                        // Мое. Продолжить
                        sb.AppendLine(space.Number.ToString());
                        TaskDialog.Show("Ошибка", "Пространство №" + sb.ToString() + "без имени.");
                        // Мое. Продолжить
                        
                        continue; // Переходим к следующему пространству
                    }

                    // Сопоставляем пространство с группами
                    var matchingGroups = groupsList
                        .Where(group => !string.IsNullOrEmpty(group.Name) && // Проверка имени группы
                                        group.Name.Equals(spaceName, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (matchingGroups.Any())
                    {
                        foreach (var group in matchingGroups)
                        {
                            XYZ placementPoint = GetSpaceCenter(space);
                            if (placementPoint != null)
                            {
                                try
                                {
                                    doc.Create.PlaceGroup(placementPoint, group.GroupType);
                                    sb.AppendLine($"Группа '{group.Name}' размещена в пространстве '{spaceName}'.");
                                }
                                catch (Exception ex)
                                {
                                    sb.AppendLine($"Ошибка при размещении группы '{group.Name}': {ex.Message}");
                                }
                            }
                        }
                    }
                    else
                    {
                        sb.AppendLine($"Пространство '{spaceName}' не совпадает ни с одной группой.");
                    }
                }
                tr.Commit();
            }

            TaskDialog.Show("Результаты сопоставления", sb.ToString());
        }
        
    }
} 