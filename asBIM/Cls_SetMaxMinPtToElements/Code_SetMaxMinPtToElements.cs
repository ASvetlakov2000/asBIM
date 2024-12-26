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
using Notifications.Wpf;


namespace asBIM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Code_SetMaxMinPtToElements : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            // ОСНОВНОЙ КОД ПЛАГИНА // НАЧАЛО  
            Form_SetMaxMinPtToElements form = new Form_SetMaxMinPtToElements();
            form.ShowDialog();

            if (form.DialogResult == true)
            {
                SetElementsTBPoints(doc);
                // Оповещение об успешной отработке команды
                NotificationManagerWPF_SetMaxMinPtToElements.Success(success: message);
            }
            if (form.DialogResult == false)
            {
                SetElementsTBPoints_Null(doc);
                // Оповещение об успешной отработке команды
                NotificationManagerWPF_SetMaxMinPtToElements.Error(error: message);
            }
            
            // TODO: 4. Сделать вывод инфо: 1. Необработанные элементы, 2. Обработанные элементы, 3. Время работы команды
            
            // Оповещение об ошибке
            //NotificationManagerWPF_SetMaxMinPtToElements.Error(error: message);
            // Оповещение о времени работы команды
            
            

            // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ  
            return Result.Succeeded;
        }


        // Метод SetElementsTBPoints
        // TODO: 5. ОПИСАНИЕ ДЕЙСТВИЯ МЕТОДА "SetElementsTBPoints" С ЦИКЛОМ РИЧ ДОПОЛНИТЬ !!!
        internal void SetElementsTBPoints(Document doc)
        {
            // Коллекция + словать с именем переменной groupedElements с фильтром на категорию и Элементы
            var groupedElements = RevitAPI_Sort_ByCategory.SortElementByCategory(doc);

            //Транзакция
            using (Transaction tr = new Transaction(doc, "Запись параметров отметок Верха и Низа"))
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
                        
                        // Проверка на null "Общего параметра по Guid" из Revit
                        if (topPointParam != null && bottomPointParam != null)
                        {
                            // Фильтр на Уровни. Выборка из Документа только уровней.
                            FilteredElementCollector levelsCollector = new FilteredElementCollector(doc).OfClass(typeof(Level));
                            // Список levels с типом Level
                            List<Level> levels = levelsCollector.Cast<Level>().OrderBy(level => level.Elevation).ToList();
                            
                            // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ ВЕРХА
                            // Получение отметки Верха элемента с каждого элемента в Документе.
                            double elementTopPointElevationSm = ElementTopBottomPt.GetElementTopPoint(elemincollector).Z;
                            // Присвоение closestLevelForTop результата метода FindBottomElemLevel
                            // В метод FindBottomElemLevel подаются отметки Низа всех эл из groupedElements["AR"] и отсортированный по возрастанию??? список уровней
                            Level closestLevelForTop = LevelInfo.FindBottomElemLevel(elementTopPointElevationSm, levels);

                            double elemTopPtElevFromLevel = Math.Abs(closestLevelForTop.Elevation - elementTopPointElevationSm);
                            
                            double elemTopPtElevFromLevelSm = UnitUtils.ConvertFromInternalUnits(elemTopPtElevFromLevel, UnitTypeId.Millimeters);
                            // Округление topVal до 2 знаков после запятой
                            double elemTopPtElevFromLevelSmRound = Math.Round(elemTopPtElevFromLevelSm, 2, MidpointRounding.AwayFromZero);
                            // Запись Имени Нижнего этажа.
                            topPointParam.Set(closestLevelForTop != null ? elemTopPtElevFromLevelSmRound  + " от " + closestLevelForTop.Name : "Не определено");
                            // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ ВЕРХА
                            
                            
                            // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ НИЗА
                            // Получение отметки Низа элемента с каждого элемента в Документе.
                            double elementBottomPointElevationSm = ElementTopBottomPt.GetElementBottomPoint(elemincollector).Z;
                            // Присвоение closestLevelForBot результата метода FindBottomElemLevel
                            // В метод FindBottomElemLevel подаются отметки Низа всех эл из groupedElements["AR"] и отсортированный по возрастанию??? список уровней
                            Level closestLevelForBot = LevelInfo.FindBottomElemLevel(elementBottomPointElevationSm, levels);
                            
                            double elemBotPtElevFromLevel = Math.Abs(closestLevelForBot.Elevation - elementBottomPointElevationSm);
                            
                            double elemBotPtElevFromLevelSm = UnitUtils.ConvertFromInternalUnits(elemBotPtElevFromLevel, UnitTypeId.Millimeters);
                            // Округление botVal до 2 знаков после запятой
                            double elemBotPtElevFromLevelSmRound = Math.Round(elemBotPtElevFromLevelSm, 2, MidpointRounding.AwayFromZero);
                            
                            // Запись Имени Нижнего этажа.
                            bottomPointParam.Set(closestLevelForBot != null ? elemBotPtElevFromLevelSmRound  + " от " + closestLevelForBot.Name : "Не определено");
                            // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ НИЗА
                            
                            
                            
                            // TODO: 6. Добавить конвертер наименований в Этаж №
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
        
        
        //Метод для обнуления значений
        internal void SetElementsTBPoints_Null(Document doc)
        {
            // Коллекция + словать с именем переменной groupedElements с фильтром на категорию и Элементы
            var groupedElements = RevitAPI_Sort_ByCategory.SortElementByCategory(doc);
        
            //Транзакция
            using (Transaction tr = new Transaction(doc, "Запись параметров отметок Верха и Низа"))
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
                        
                        // Проверка на null "Общего параметра по Guid" из Revit
                        if (topPointParam != null && bottomPointParam != null)
                        {
                            // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ ВЕРХА
                            topPointParam.Set("");
                            
                            // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ НИЗА
                            bottomPointParam.Set("");
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

