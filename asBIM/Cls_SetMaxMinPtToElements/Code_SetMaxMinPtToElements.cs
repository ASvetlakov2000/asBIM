using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using asBIM.Helpers;
using Notifications.Wpf;
using asBIM;


namespace asBIM
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Code_SetMaxMinPtToElements : IExternalCommand
    {
        // Имена общих парамтеров
        private string shParamTopPtName = "PRO_Отметка верха";
        private string shParamBotPtName = "PRO_Отметка низа";
        private string shParamStName = "PRO_Отметка в начале";
        private string shParamEndName = "PRO_Отметка в конце";
        
        // Объявление общих параметров для записи в проект
        private Guid shParamTopPtGuid = new Guid("22c86588-f717-403e-b1c6-1607cac39965"); // PRO_Отметка верха
        private Guid shParamBotPtGuid = new Guid("b0ed44c1-724e-4301-b489-8d89c02acec5"); // PRO_Отметка низа
        private Guid shParamStGuid = new Guid("c7f57780-cff9-467e-a05e-47cc3f261064"); // PRO_Отметка в начале
        private Guid shParamEndGuid = new Guid("ea5f8c68-e31b-4882-bdca-a8a12200d347"); // PRO_Отметка в конце

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
            Stopwatch time = new Stopwatch();
            time.Start();
            
            // Запись отметок верха и низа
            SetElementsTBPoints(doc);
            
            // Запись отметок начала и конца
            SetLinearElemTBPoints(doc);

            time.Stop();
            // Время выполнения
            var timeInSecForCommand = time.Elapsed.TotalSeconds;
            double timeInMin = TimeOfWorkConverter.ConvertTime(timeInSecForCommand).timeInMinOutput;
            double timeInSec = TimeOfWorkConverter.ConvertTime(timeInSecForCommand).timeInSecOutput;
            
            // Уведомление. "Время работы"
            NotificationManagerWPF.TimeOfWork("Время работы",
                timeInSec: "\n \u231b Время выполнения: " +
                           Convert.ToString(Math.Round(Convert.ToDouble(timeInMin), 0,
                               MidpointRounding.AwayFromZero) + " мин") + " " +
                           Convert.ToString(Math.Round(Convert.ToDouble(timeInSec), 0,
                               MidpointRounding.AwayFromZero) + " сек"),
                NotificationType.Information);  
            
            // Высота Базовой точки
            double bpElevation = Math.Round
            (UnitUtils.ConvertFromInternalUnits
                (BasePtPosition.GetBasePointHeight(doc).Z, UnitTypeId.Millimeters), 0, MidpointRounding.AwayFromZero);
            
            if (bpElevation != 0.0)
            {
                // Уведомление. "Высота Базовой Точки"
                NotificationManagerWPF.MessageInfoSmile("Внимание!\nБазовая точка не в нулевой позиции",
                    "\u00af\u00af\u00af\u00af\u00af\u00af\\_(ツ)_/\u00af\u00af\u00af" +
                    "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af" +
                    "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af",
                     $"\n\nБазовая Точка на высоте: " + bpElevation.ToString() + "мм"
                    +
                    "\nПроверьте значения заполненных параметров!",
                    NotificationType.Warning);
            }
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
            string elemCountStrSucc;
            // Счетчик для количества элементов. Не Удачно
            string elemCountStrErr;
            
            // Список свободных элементов для обработки
            List<ElementId> freeElements = new List<ElementId>();
            // Список занятых элементов для обработки
            List<ElementId> occupElements = new List<ElementId>();
            
            foreach (Element el in groupedElements["Element"])
            {
                if (WorksharingUtils.GetCheckoutStatus(doc, el.Id) == CheckoutStatus.OwnedByOtherUser)
                {
                    // Передача в список occupElements типа ElementId только тех элементов, которые заняты другими пользователями
                    occupElements.Add(el.Id);
                    continue;
                }
                // Передача в список freeElements типа ElementId только тех элементов, которые не заняты другими пользователями
                freeElements.Add(el.Id);
            }

            // Высота Базовой точки
            double bpElevation = Math.Round
                (UnitUtils.ConvertFromInternalUnits
                    (BasePtPosition.GetBasePointHeight(doc).Z, UnitTypeId.Millimeters), 0, MidpointRounding.AwayFromZero);

            //Транзакция
            using (Transaction tr = new Transaction(doc, "Запись параметров отметок Верха и Низа"))
            {
                tr.Start();
                // Проверка на наличие в документе общих параметров, перед заполнением. Если нет, то уведомление, если да, то выполнение ниже.
                if (SharedParameterHelper.FindSharedParameterByGUID(doc, shParamTopPtGuid) &&
                    SharedParameterHelper.FindSharedParameterByGUID(doc, shParamBotPtGuid))
                {
                    // Цикл с перебором всех элементов в коллекции
                    foreach (ElementId elemincollector in freeElements)
                    {
                        try
                        {
                            Element elem = doc.GetElement(elemincollector);
                            //Получение "Общего параметра по Guid" из Revit в переменную topPointParam из эл в коллекции. Отметка верха
                            Parameter topPointParam = elem.LookupParameter(shParamTopPtName);
                            //Получение "Общего параметра по Guid" из Revit в переменную bottomPointParam параметра из эл в коллекции. Отметка низа
                            Parameter bottomPointParam = elem.LookupParameter(shParamBotPtName);

                            // Проверка на null "Общего параметра по Guid" из Revit
                            if (topPointParam != null && bottomPointParam != null)
                            {
                                // Фильтр на Уровни. Выборка из Документа только уровней.
                                FilteredElementCollector levelsCollector =
                                    new FilteredElementCollector(doc).OfClass(typeof(Level));
                                // Список levels с типом Level
                                List<Level> levels = levelsCollector.Cast<Level>().OrderBy(level => level.Elevation)
                                    .ToList();
                                // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ ВЕРХА
                                // Получение отметки Верха элемента с каждого элемента в Документе.
                                double elementTopPointElevationSm =
                                    ElementTopBottomPt.GetElementTopPoint(elem).Z;
                                // Присвоение closestLevelForTop результата метода FindBottomElemLevel
                                // В метод FindBottomElemLevel подаются отметки Низа всех эл из groupedElements["AR"] и отсортированный по возрастанию список уровней
                                Level closestLevelForTop =
                                    LevelInfo.FindBottomElemLevel(elementTopPointElevationSm, levels);
                                // Вычисление расстояние от Отметки Верха до Отметки Уровня
                                double elemTopPtElevFromLevel =
                                    Math.Abs(closestLevelForTop.Elevation - elementTopPointElevationSm);
                                // Конвертация из футов в мм
                                double elemTopPtElevFromLevelSm =
                                    UnitUtils.ConvertFromInternalUnits(elemTopPtElevFromLevel, UnitTypeId.Millimeters);
                                
                                // Отладка
                                if (bpElevation <= 0)
                                {
                                    double elTopPtElev = Math.Round(elemTopPtElevFromLevelSm + Math.Abs(bpElevation));
                                    
                                    // Запись Имени Нижнего этажа.
                                    topPointParam.Set(closestLevelForTop != null
                                        ? elTopPtElev + " от " + closestLevelForTop.Name.Split('_').Last()
                                        : "Не определено");
                                    // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ ВЕРХА
                                }
                                if (bpElevation > 0)
                                {
                                    double elTopPtElev = Math.Round(elemTopPtElevFromLevelSm - Math.Abs(bpElevation));
                                    
                                    // Запись Имени Нижнего этажа.
                                    topPointParam.Set(closestLevelForTop != null
                                        ? elTopPtElev + " от " + closestLevelForTop.Name.Split('_').Last()
                                        : "Не определено");
                                    // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ ВЕРХА
                                }
                                // Отладка

                                // ЗАПИСЬ УРОВНЯ ДЛЯ ОТМЕТКИ НИЗА
                                // Получение отметки Низа элемента с каждого элемента в Документе.
                                double elementBottomPointElevationSm =
                                    ElementTopBottomPt.GetElementBottomPoint(elem).Z;
                                // Присвоение closestLevelForBot результата метода FindBottomElemLevel
                                // В метод FindBottomElemLevel подаются отметки Низа всех эл из groupedElements["AR"] и отсортированный по убыванию список уровней
                                Level closestLevelForBot =
                                    LevelInfo.FindBottomElemLevel(elementBottomPointElevationSm, levels);
                                // Вычисление расстояние от Отметки Низа до Отметки Уровня
                                double elemBotPtElevFromLevel =
                                    Math.Abs(closestLevelForBot.Elevation - elementBottomPointElevationSm);
                                
                                // Конвертация из футов в мм
                                double elemBotPtElevFromLevelSm =
                                    UnitUtils.ConvertFromInternalUnits(elemBotPtElevFromLevel, UnitTypeId.Millimeters);
                                
                                // Отладка
                                if (bpElevation <= 0)
                                {
                                    double elBotPtElev = Math.Round(elemBotPtElevFromLevelSm + Math.Abs(bpElevation));
                                    
                                    // Запись Имени Нижнего этажа.
                                    bottomPointParam.Set(closestLevelForBot != null
                                        ? elBotPtElev + " от " + closestLevelForBot.Name.Split('_').Last()
                                        : "Не определено");
                                    // Запись Имени Нижнего этажа.
                                }
                                if (bpElevation > 0)
                                {
                                    double elBotPtElev = Math.Round(elemBotPtElevFromLevelSm - Math.Abs(bpElevation));
                                    
                                    // Запись Имени Нижнего этажа.
                                    bottomPointParam.Set(closestLevelForBot != null
                                        ? elBotPtElev + " от " + closestLevelForBot.Name.Split('_').Last()
                                        : "Не определено");
                                    // Запись Имени Нижнего этажа.
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show("Ошибка", ex.Message);
                        }
                    }
                    tr.Commit();
                    
                    elemCountStrSucc = $"\n\nКол-во обработанных элементов: {freeElements.Count.ToString()}";
                    elemCountStrErr = $"\n\nКол-во пропущенных элементов: {occupElements.Count.ToString()}";
                    
                    NotificationManagerWPF.MessageSmileInfo(
                        "Запись параметров для Элементов",
                        "\n(￢‿￢)", elemCountStrSucc,
                        NotificationType.Success);
                    
                    NotificationManagerWPF.MessageSmileInfo(
                        "Пропущенные элементы",
                        "\n\u00af\\_(ツ)_/\u00af", elemCountStrErr,
                        NotificationType.Warning);
                }
                else
                {
                    NotificationManagerWPF.MessageSmileInfo(
                        "Отсутствие параметров для Элементов",
                        "\u00af\u00af\u00af\u00af\u00af\u00af\\_(ツ)_/\u00af\u00af\u00af\u00af" +
                        "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af" +
                        "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af",
                        "\n\nДобавьте общие параметры:\n[PRO_Отметка верха]\n[PRO_Отметка низа]",
                        NotificationType.Error);
                }
            }
        }


        /// <summary>
        /// Метод SetLinearElemTBPoints - записывает в параметры Отметка в Начале и Отметка в Конце
        /// <param name = "doc" > Документ </param>
        /// </summary>
        public void SetLinearElemTBPoints(Document doc)
        {
            var groupedElements = RevitAPI_Sort_ByCategory.SortElementByCategory(doc);

            // Счетчик для количества элементов. Удачно
            string linearCountStrSucc;
            // Счетчик для количества элементов. Не Удачно
            string linearCountStrErr;
            
            // Список свободных элементов для обработки
            List<ElementId> freeLinear = new List<ElementId>();
            // Список занятых элементов для обработки
            List<ElementId> occupLinear = new List<ElementId>();
            
            foreach (Element el in groupedElements["Linear"])
            {
                if (WorksharingUtils.GetCheckoutStatus(doc, el.Id) == CheckoutStatus.OwnedByOtherUser)
                {
                    // Передача в список occupElements типа ElementId только тех элементов, которые заняты другими пользователями
                    occupLinear.Add(el.Id);
                    continue;
                }
                // Передача в список freeElements типа ElementId только тех элементов, которые не заняты другими пользователями
                freeLinear.Add(el.Id);
            }
            
            // Высота Базовой точки
            double bpElevation = Math.Round
            (UnitUtils.ConvertFromInternalUnits
                (BasePtPosition.GetBasePointHeight(doc).Z, UnitTypeId.Millimeters), 0, MidpointRounding.AwayFromZero);

            using (Transaction tr = new Transaction(doc, "Запись параметров отметок Верха и Низа"))
            {
                tr.Start();
                // Проверка на наличие в документе общих параметров, перед заполнением. Если нет, то уведомление, если да, то выполнение ниже.
                if (SharedParameterHelper.FindSharedParameterByGUID(doc, shParamStGuid) &&
                    SharedParameterHelper.FindSharedParameterByGUID(doc, shParamEndGuid))
                {

                    foreach (ElementId elemincollector in freeLinear)
                    {
                        try
                        {
                            Element linear = doc.GetElement(elemincollector);
                            //Получение "Общего параметра по Guid" из Revit в переменную topPointParam из эл в коллекции. Отметка верха
                            Parameter startPointParam = linear.LookupParameter(shParamStName);
                            //Получение "Общего параметра по Guid" из Revit в переменную bottomPointParam параметра из эл в коллекции. Отметка низа
                            Parameter endtPointParam = linear.LookupParameter(shParamEndName);

                            // Проверка на null "Общего параметра по Guid" из Revit
                            if (startPointParam != null && endtPointParam != null)
                            {

                                // Фильтр на Уровни. Выборка из Документа только уровней.
                                FilteredElementCollector levelsCollector =
                                    new FilteredElementCollector(doc).OfClass(typeof(Level));
                                // Список levels с типом Level
                                List<Level> levels = levelsCollector.Cast<Level>().OrderBy(level => level.Elevation)
                                    .ToList();
                                // Получение Отметки в Начале для линейных обьектов 
                                double linearElemStartPtElev =
                                    ElementTopBottomPt.GetLinearPoint(linear as MEPCurve, true).Z;
                                // В метод FindBottomElemLevel подаются отметки Низа всех эл из groupedElements["AR"] и отсортированный по возрастанию список уровней
                                Level closestLevelForTop = LevelInfo.FindBottomElemLevel(linearElemStartPtElev, levels);
                                // Вычисление расстояние от Отметки Верха до Отметки Уровня
                                double linearElemStartPtElevFromLev =
                                    Math.Abs(closestLevelForTop.Elevation - linearElemStartPtElev);
                                // Конвертация из футов в мм
                                double linearElemStartPtElevFromLevSm =
                                    UnitUtils.ConvertFromInternalUnits(linearElemStartPtElevFromLev, UnitTypeId.Millimeters);
                                
                                // Отладка
                                if (bpElevation <= 0)
                                {
                                    double elStartPtElev =
                                        Math.Round(linearElemStartPtElevFromLevSm + Math.Abs(bpElevation));

                                    // ЗАПИСЬ ОТМЕТКИ В НАЧАЛЕ ДЛЯ ЛИНЕЙНЫХ ЭЛЕМЕНТОВ
                                    startPointParam.Set(closestLevelForTop != null
                                        ? elStartPtElev + " от " +
                                          closestLevelForTop.Name.Split('_').Last()
                                        : "Не определено");
                                }
                                if (bpElevation > 0)
                                {
                                    double elStartPtElev =
                                        Math.Round(linearElemStartPtElevFromLevSm - Math.Abs(bpElevation));

                                    // ЗАПИСЬ ОТМЕТКИ В НАЧАЛЕ ДЛЯ ЛИНЕЙНЫХ ЭЛЕМЕНТОВ
                                    startPointParam.Set(closestLevelForTop != null
                                        ? elStartPtElev + " от " +
                                          closestLevelForTop.Name.Split('_').Last()
                                        : "Не определено");
                                }
                                // Отладка

                                // Получение Отметки в Начале для линейных обьектов 
                                double linearElemEndPtElev =
                                    ElementTopBottomPt.GetLinearPoint(linear as MEPCurve, false).Z;
                                // В метод FindBottomElemLevel подаются отметки Низа всех эл из groupedElements["AR"] и отсортированный по возрастанию список уровней
                                Level closestLevelForBottom =
                                    LevelInfo.FindBottomElemLevel(linearElemEndPtElev, levels);
                                // Вычисление расстояние от Отметки Верха до Отметки Уровня
                                double linearElemEndPtElevFromLev =
                                    Math.Abs(closestLevelForBottom.Elevation - linearElemEndPtElev);
                                
                                // Конвертация из футов в мм
                                double linearElemEndPtElevFromLevSm =
                                    UnitUtils.ConvertFromInternalUnits(linearElemEndPtElevFromLev, UnitTypeId.Millimeters);
                                
                                // Отладка
                                if (bpElevation <= 0)
                                {
                                    double elEndPtElev =
                                        Math.Round(linearElemEndPtElevFromLevSm + Math.Abs(bpElevation));
                                    
                                    endtPointParam.Set(closestLevelForBottom != null
                                        ? elEndPtElev + " от " +
                                          closestLevelForBottom.Name.Split('_').Last()
                                        : "Не определено");
                                    // ЗАПИСЬ ОТМЕТКИ В КОНЦЕ ДЛЯ ЛИНЕЙНЫХ ЭЛЕМЕНТОВ
                                }
                                if (bpElevation > 0)
                                {
                                    double elEndPtElev =
                                        Math.Round(linearElemEndPtElevFromLevSm - Math.Abs(bpElevation));
                                    
                                    endtPointParam.Set(closestLevelForBottom != null
                                        ? elEndPtElev + " от " +
                                          closestLevelForBottom.Name.Split('_').Last()
                                        : "Не определено");
                                    // ЗАПИСЬ ОТМЕТКИ В КОНЦЕ ДЛЯ ЛИНЕЙНЫХ ЭЛЕМЕНТОВ
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show("Ошибка", ex.Message);
                        }
                    }
                    tr.Commit();
                    
                    linearCountStrSucc = $"\n\nКол-во обработанных элементов: {freeLinear.Count.ToString()}";
                    linearCountStrErr = $"\n\nКол-во пропущенных элементов: {occupLinear.Count.ToString()}";
                    
                    NotificationManagerWPF.MessageSmileInfo(
                        "Запись параметров для Линейных",
                        "\n(￢‿￢)", linearCountStrSucc,
                        NotificationType.Success);
                    
                    NotificationManagerWPF.MessageSmileInfo(
                        "Пропущенные линейные элементы",
                        "\n\u00af\\_(ツ)_/\u00af", linearCountStrErr,
                        NotificationType.Warning);
                }
                else
                {
                    NotificationManagerWPF.MessageSmileInfo(
                        "Отсутствие параметров для Линейных",
                        "\u00af\u00af\u00af\u00af\u00af\u00af\\_(ツ)_/\u00af\u00af\u00af\u00af" +
                        "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af" +
                        "\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af\u00af",
                    "\n\nДобавьте общие параметры:\n[PRO_Отметка в начале]\n[PRO_Отметка в конце]",
                        NotificationType.Error);
                }
            }
        }
    }
}

