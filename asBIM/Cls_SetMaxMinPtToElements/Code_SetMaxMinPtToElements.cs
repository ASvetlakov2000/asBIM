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

    public class Code_SetMaxMinPtToElements : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            
            // ОСНОВНОЙ КОД ПЛАГИНА // НАЧАЛО  

            // Вызов UI
            // var vm = new SetMaxMinPtToElements_ViewModel(uiapp);
            // // в view передается null из-за проблем в XAML
            // var view = new Form_SetMaxMinPtToElements(vm);
            // view.Show();
            
            // Тест для настройки окон
            // Form_SetMaxMinPtToElements form = new Form_SetMaxMinPtToElements();
            // form.Show();
            // Вызов UI
            
            // Без UI
            
            // Время выполнения
            Stopwatch swPlaceGroups = new Stopwatch();
            swPlaceGroups.Start();
            
            // TODO: 3. Добавить проверку на наличие параметра Отметки Верха и Низа
            // Тест для настройки команд
            SetElementsTBPoints(doc);
            
            // TODO: 4. Добавить проверку на наличие параметра Отметки в Начале и в Конце
            // Тест для настройки команд
            SetLinearElemTBPoints(doc);
            
            swPlaceGroups.Stop();
            // Время выполнения
            var timeInSecForCommand = swPlaceGroups.Elapsed.TotalSeconds;
            // Уведомление. "Время работы"
            NotificationManagerWPF.TimeOfWork("Время работы", 
                timeInSec:"\nВремя выполнения: " + Convert.ToString(Math.Round(Convert.ToDouble(timeInSecForCommand), 0, MidpointRounding.AwayFromZero) + " сек"),
                NotificationType.Information);
            
            // Без UI
            
            // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ  
            return Result.Succeeded;
        }


        /// <summary>
        /// Метод для записи Отметок Верха и Низа для Элементов по BoundingBox
        /// <param name = "doc" > Документ </param>
        /// </summary>
        public void SetElementsTBPoints(Document doc)
        {
            // Коллекция + словать с именем переменной groupedElements с фильтром на категорию и Элементы
            var groupedElements = RevitAPI_Sort_ByCategory.SortElementByCategory(doc);
            
            // Счетчик для количества элементов. Удачно
            string elemCountStr;
            IList<Element> elemCount = new List<Element>();

            //Транзакция
            using (Transaction tr = new Transaction(doc, "Запись параметров отметок Верха и Низа"))
            {
                tr.Start();
                // Цикл с перебором всех элементов в коллекции
                foreach (Element elemincollector in groupedElements["Element"])
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
                            // В метод FindBottomElemLevel подаются отметки Низа всех эл из groupedElements["AR"] и отсортированный по возрастанию список уровней
                            Level closestLevelForTop = LevelInfo.FindBottomElemLevel(elementTopPointElevationSm, levels);
                            // Вычисление расстояние от Отметки Верха до Отметки Уровня
                            double elemTopPtElevFromLevel = Math.Abs(closestLevelForTop.Elevation - elementTopPointElevationSm);
                            // Конвертация из футов в мм
                            double elemTopPtElevFromLevelSm = UnitUtils.ConvertFromInternalUnits(elemTopPtElevFromLevel, UnitTypeId.Millimeters);
                            // Округление topVal до 0 знаков после запятой
                            double elemTopPtElevFromLevelSmRound = Math.Round(elemTopPtElevFromLevelSm, 0, MidpointRounding.AwayFromZero);
                            
                            // Запись Имени Нижнего этажа.
                            topPointParam.Set(closestLevelForTop != null ? elemTopPtElevFromLevelSmRound  + " от " + closestLevelForTop.Name.Split('_').Last() : "Не определено");
                            // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ ВЕРХА
                            
                            
                            // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ НИЗА
                            // Получение отметки Низа элемента с каждого элемента в Документе.
                            double elementBottomPointElevationSm = ElementTopBottomPt.GetElementBottomPoint(elemincollector).Z;
                            // Присвоение closestLevelForBot результата метода FindBottomElemLevel
                            // В метод FindBottomElemLevel подаются отметки Низа всех эл из groupedElements["AR"] и отсортированный по убыванию список уровней
                            Level closestLevelForBot = LevelInfo.FindBottomElemLevel(elementBottomPointElevationSm, levels);
                            // Вычисление расстояние от Отметки Низа до Отметки Уровня
                            double elemBotPtElevFromLevel = Math.Abs(closestLevelForBot.Elevation - elementBottomPointElevationSm);
                            // Конвертация из футов в мм
                            double elemBotPtElevFromLevelSm = UnitUtils.ConvertFromInternalUnits(elemBotPtElevFromLevel, UnitTypeId.Millimeters);
                            // Округление botVal до 0 знаков после запятой
                            double elemBotPtElevFromLevelSmRound = Math.Round(elemBotPtElevFromLevelSm, 0, MidpointRounding.AwayFromZero);
                            
                            // Запись Имени Нижнего этажа.
                            bottomPointParam.Set(closestLevelForBot != null ? elemBotPtElevFromLevelSmRound  + " от " + closestLevelForBot.Name.Split('_').Last() : "Не определено");
                            
                            // Счетчик элементов 
                            elemCount.Add(elemincollector);
                            
                            // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ НИЗА
                            
                        }
                    }

                    catch (Exception ex)
                    {
                        TaskDialog.Show("Ошибка", ex.Message);
                    }
                }

                elemCountStr = $"\n\nКол-во обработанных элементов: {elemCount.Count.ToString()}";
                NotificationManagerWPF.MessageSmileInfo(
                    "Запись параметров для Элементов", 
                    "\n(￢‿￢ )",elemCountStr,
                    NotificationType.Success);
                
                tr.Commit();
            }

        }
        
        
        //Метод для обнуления значений
        // public void SetElementsTBPoints_Null(Document doc)
        // {
        //     // Коллекция + словать с именем переменной groupedElements с фильтром на категорию и Элементы
        //     var groupedElements = RevitAPI_Sort_ByCategory.SortElementByCategory(doc);
        //
        //     //Транзакция
        //     using (Transaction tr = new Transaction(doc, "Запись параметров отметок Верха и Низа"))
        //     {
        //         tr.Start();
        //         // Цикл с перебором всех элементов в коллекции
        //         foreach (Element elemincollector in groupedElements["Element"])
        //         {
        //             try
        //             {
        //                 //Получение "Общего параметра по Guid" из Revit в переменную topPointParam из эл в коллекции. Отметка верха
        //                 Parameter topPointParam = elemincollector.get_Parameter(new Guid("22c86588-f717-403e-b1c6-1607cac39965"));
        //                 //Получение "Общего параметра по Guid" из Revit в переменную bottomPointParam параметра из эл в коллекции. Отметка низа
        //                 Parameter bottomPointParam = elemincollector.get_Parameter(new Guid("b0ed44c1-724e-4301-b489-8d89c02acec5")); 
        //                 
        //                 // Проверка на null "Общего параметра по Guid" из Revit
        //                 if (topPointParam != null && bottomPointParam != null)
        //                 {
        //                     // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ ВЕРХА. Удаление значений
        //                     topPointParam.Set("");
        //                     
        //                     // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ НИЗА. Удаление значений
        //                     bottomPointParam.Set("");
        //                 }
        //             }
        //
        //             catch (Exception ex)
        //             {
        //                 TaskDialog.Show("Ошибка", ex.Message);
        //             }
        //         }
        //         tr.Commit();
        //     }
        // }
        
        
        /// <summary>
        /// Метод SetLinearElemTBPoints - записывает в параметры Отметка в Начале и Отметка в Конце
        /// <param name = "doc" > Документ </param>
        /// </summary>
        public void SetLinearElemTBPoints(Document doc)
        {
            var groupedElements = RevitAPI_Sort_ByCategory.SortElementByCategory(doc);
            
            // Счетчик для количества элементов. Удачно
            string elemCountStr;
            IList<Element> elemCount = new List<Element>();
            
            using (Transaction tr = new Transaction(doc, "Запись параметров отметок Верха и Низа"))
            {
                tr.Start();
                foreach (MEPCurve elemincollector in groupedElements["Linear"])
                {
                    try
                    {
                        //Получение "Общего параметра по Guid" из Revit в переменную topPointParam из эл в коллекции. Отметка верха
                        Parameter startPointParam = elemincollector.get_Parameter(new Guid("c7f57780-cff9-467e-a05e-47cc3f261064"));
                        //Получение "Общего параметра по Guid" из Revit в переменную bottomPointParam параметра из эл в коллекции. Отметка низа
                        Parameter endtPointParam = elemincollector.get_Parameter(new Guid("ea5f8c68-e31b-4882-bdca-a8a12200d347")); 
                        
                        // Проверка на null "Общего параметра по Guid" из Revit
                        if (startPointParam != null && endtPointParam != null)
                        { 
                            
                            // Фильтр на Уровни. Выборка из Документа только уровней.
                            FilteredElementCollector levelsCollector = new FilteredElementCollector(doc).OfClass(typeof(Level));
                            // Список levels с типом Level
                            List<Level> levels = levelsCollector.Cast<Level>().OrderBy(level => level.Elevation).ToList();
                            
                            // Получение Отметки в Начале для линейных обьектов 
                            double linearElemStartPtElev = ElementTopBottomPt.GetLinearPoint(elemincollector, true).Z;
                            // В метод FindBottomElemLevel подаются отметки Низа всех эл из groupedElements["AR"] и отсортированный по возрастанию список уровней
                            Level closestLevelForTop = LevelInfo.FindBottomElemLevel(linearElemStartPtElev, levels);
                            // Вычисление расстояние от Отметки Верха до Отметки Уровня
                            double linearElemStartPtElevFromLev = Math.Abs(closestLevelForTop.Elevation - linearElemStartPtElev);
                            // Конвертация из футов в мм
                            double linearElemStartPtElevFromLevSm = UnitUtils.ConvertFromInternalUnits(linearElemStartPtElevFromLev, UnitTypeId.Millimeters);
                            // Округление topVal до 0 знаков после запятой
                            double linearElemStartPtElevFromLevSmRound = Math.Round(linearElemStartPtElevFromLevSm, 0, MidpointRounding.AwayFromZero);
                            
                            // ЗАПИСЬ ОТМЕТКИ В НАЧАЛЕ ДЛЯ ЛИНЕЙНЫХ ЭЛЕМЕНТОВ
                            startPointParam.Set(closestLevelForTop != null ? linearElemStartPtElevFromLevSmRound  + " от " + closestLevelForTop.Name.Split('_').Last() : "Не определено");
                            
                            // Получение Отметки в Начале для линейных обьектов 
                            double linearElemEndPtElev = ElementTopBottomPt.GetLinearPoint(elemincollector, false).Z;
                            // В метод FindBottomElemLevel подаются отметки Низа всех эл из groupedElements["AR"] и отсортированный по возрастанию список уровней
                            Level closestLevelForBottom = LevelInfo.FindBottomElemLevel(linearElemEndPtElev, levels);
                            // Вычисление расстояние от Отметки Верха до Отметки Уровня
                            double linearElemEndPtElevFromLev = Math.Abs(closestLevelForBottom.Elevation - linearElemEndPtElev);
                            // Конвертация из футов в мм
                            double linearElemEndPtElevFromLevSm = UnitUtils.ConvertFromInternalUnits(linearElemEndPtElevFromLev, UnitTypeId.Millimeters);
                            // Округление topVal до 0 знаков после запятой
                            double linearElemEndPtElevFromLevSmRound = Math.Round(linearElemEndPtElevFromLevSm, 0, MidpointRounding.AwayFromZero);
                            
                            // Счетчик элементов 
                            elemCount.Add(elemincollector);
                            endtPointParam.Set(closestLevelForBottom != null ? linearElemEndPtElevFromLevSmRound  + " от " + closestLevelForBottom.Name.Split('_').Last() : "Не определено");
                            // ЗАПИСЬ ОТМЕТКИ В КОНЦЕ ДЛЯ ЛИНЕЙНЫХ ЭЛЕМЕНТОВ
                        }
                    }
        
                    catch (Exception ex)
                    {
                        TaskDialog.Show("Ошибка", ex.Message);
                    }
                }
                
                elemCountStr = $"\n\nКол-во обработанных элементов: {elemCount.Count.ToString()}";
                NotificationManagerWPF.MessageSmileInfo(
                    "Запись параметров для Линейных", 
                    "\n(￢‿￢ )",elemCountStr,
                    NotificationType.Success);
                
                tr.Commit();
            }
        }
    }
}

