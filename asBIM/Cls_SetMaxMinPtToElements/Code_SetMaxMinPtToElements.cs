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
using System.Xaml;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Windows;


namespace asBIM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Code_SetMaxMinPtToElements : IExternalCommand
    {

        public InternalOrigin internalOrigin;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            // ОСНОВНОЙ КОД ПЛАГИНА // НАЧАЛО  

            SetElementsTBPoints(doc);
            
            // // TODO: 12. В релизе убрать вывод инфо об Уровнях
            LevelInfo.GetLevelInfoInDoc(doc);

            // TODO: 8. Сделать вывод инфо: 1. Необработанные элементы, 2. Обработанные элементы, 3. Время работы команды

            // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ  
            return Result.Succeeded;
        }


        // Метод SetElementsTBPoints
        // TODO: 9. ОПИСАНИЕ ДЕЙСТВИЯ МЕТОДА "SetElementsTBPoints" С ЦИКЛОМ РИЧ ДОПОЛНИТЬ !!!
        public void SetElementsTBPoints(Document doc)
        {
            // Коллекция + словать с именем переменной groupedElements с фильтром на категорию и Элементы
            var groupedElements = RevitAPI_Sort_ByCategory.SortElementByCategory(doc);
            // TODO: 10. Вынести транзакцию из режима "для каждого элемента"
            //Транзакция
            using (Transaction tr = new Transaction(doc, "Запись параметра"))
            {
                tr.Start();
                // Цикл с перебором всех элементов в коллекции
                foreach (Element elemincollector in groupedElements["AR"])
                {
                    try
                    {
                        //Получение "Общего параметра по Guid" из Revit в переменную topPointParam из эл в коллекции. Отметка верха
                        Parameter topPointParam = elemincollector.get_Parameter(new Guid("22c86588-f717-403e-b1c6-1607cac39965"));
                        //Получение "Общего параметра по Guid" из Revit в переменную bottomPointParam параметра из эл в коллекции. Отметка низа
                        Parameter bottomPointParam = elemincollector.get_Parameter(new Guid("b0ed44c1-724e-4301-b489-8d89c02acec5")); 
                        // TODO: 11. Добавить запись уровня для отметки Верха. Протестировать.
                        
                        //Получение "Общего параметра по Guid" из Revit в переменную levelInfo параметра из эл в коллекции. Уровень для отметки Низа
                        Parameter levelInfo = elemincollector.get_Parameter(new Guid("efa0466a-d957-4708-a947-916133c9d4f8"));
                        
                        // Проверка на null "Общего параметра по Guid" из Revit
                        if (topPointParam != null && bottomPointParam != null && levelInfo != null)
                        {
                            // Конвертация единиц из Футов в ММ. Для точки Верха
                            var top = UnitUtils.ConvertFromInternalUnits(ElementTopBottomPt.GetElementTopPoint(elemincollector).Z, UnitTypeId.Millimeters);
                            // Конвертация единиц из Футов в ММ. Для точки Низа
                            var bot = UnitUtils.ConvertFromInternalUnits(ElementTopBottomPt.GetElementBottomPoint(elemincollector).Z, UnitTypeId.Millimeters);
                            // Округление topVal до 2 знаков после запятой
                            var topVal = Math.Round(top, 2, MidpointRounding.AwayFromZero);
                            // Округление botVal до 2 знаков после запятой
                            var botVal = Math.Round(bot, 2, MidpointRounding.AwayFromZero);

                            // Запись значения из GetElementTopPoint(elemincollector).Z в параметр topPointParam - отметки Верха с конвертацией в строку
                            topPointParam.Set(Convert.ToString(topVal));
                            // Запись значения из GetElementBottomPoint(elemincollector).Z в параметр bottomPointParam - отметки Низа с конвертацией в строку
                            bottomPointParam.Set(Convert.ToString(botVal));
                            
                            // Фильтр на Уровни. Выборка из Документа только уровней.
                            FilteredElementCollector levelsCollector = new FilteredElementCollector(doc).OfClass(typeof(Level));
                            // Список levels с типом Level
                            List<Level> levels = levelsCollector.Cast<Level>().OrderBy(level => level.Elevation).ToList();

                            // Получение отметки Верха элемента с каждого элемента в Документе.
                            double elementBottomPointElevationSm = ElementTopBottomPt.GetElementBottomPoint(elemincollector).Z;
                            // Присвоение closestLevel значения имени Нижнего этажа
                            // В метод FindBottomElemLevel подаются отметки Верха для всех эл из groupedElements["AR"]
                            Level closestLevel = LevelInfo.FindBottomElemLevel(elementBottomPointElevationSm, levels);
                            // Запись Имени этажа. Тест.
                            
                            //var value = "Не определено";
                            //if (closestLevel != null)
                            //    value = closestLevel.Name;
                            levelInfo.Set(closestLevel != null ? closestLevel.Name : "Не определено");
                            
                            // TODO: 13. Добавить обнулятор значений на время тестов
                        }
                    }

                    catch (Exception ex)
                    {
                        TaskDialog.Show("Ошибка", ex.Message);
                    }
                }
                tr.Commit();
            }

        }

    }
}

