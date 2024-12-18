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
            
            LevelInfo.GetLevelInfoInDoc(doc);

            // TaskDialog.Show("Запись параметра", "Че-то сделал, а че ХЗ");

            // ОСНОВНОЙ КОД ПЛАГИНА // КОНЕЦ  
            return Result.Succeeded;
        }


        // Метод SetElementsTBPoints 
        //  ОПИСАНИЕ ДЕЙСТВИЯ МЕТОДА С ЦИКЛОМ РИЧ ДОПОЛНИТЬ !!!
        public void SetElementsTBPoints(Document doc)
        {
            // Коллекция + словать с именем переменной groupedElements с фильтром на категорию и Элементы  // Сменить комент под новый способ
            var groupedElements = RevitAPI_Sort_ByCategory.SortElementByCategory(doc);

            //Транзакция
            using (Transaction tr = new Transaction(doc, "Запись параметра"))
            {
                tr.Start();
                // Цикл с перебором всех элементов в коллекции
                foreach (Element elemincollector in groupedElements["AR"])
                {
                    try
                    {
                        //Присваивание переменной topPointParam параметра по Guid из эл в коллекции. Отметка верха
                        Parameter topPointParam = elemincollector.get_Parameter(new Guid("22c86588-f717-403e-b1c6-1607cac39965"));
                        //Присваивание переменной bottomPointParam параметра по Guid из эл в коллекции. Отметка низа
                        Parameter bottomPointParam = elemincollector.get_Parameter(new Guid("b0ed44c1-724e-4301-b489-8d89c02acec5"));

                        // Для записи уровня в Назначение
                        Parameter levelInfo = elemincollector.get_Parameter(new Guid("efa0466a-d957-4708-a947-916133c9d4f8"));

                        if (topPointParam != null && bottomPointParam != null && levelInfo != null)
                        {
                            // Конвертация единиц из Футов в ММ. Для точки Верха
                            var top = UnitUtils.ConvertFromInternalUnits(ElementTopBottomPt.GetElementTopPoint(elemincollector).Z, UnitTypeId.Millimeters);
                            // Конвертация единиц из Футов в ММ. Для точки Низа
                            var bot = UnitUtils.ConvertFromInternalUnits(ElementTopBottomPt.GetElementBottomPoint(elemincollector).Z, UnitTypeId.Millimeters);
                            // Округление top до 2 знаков после запятой
                            var topVal = Math.Round(top, 2, MidpointRounding.AwayFromZero);
                            // Округление bot до 2 знаков после запятой
                            var botVal = Math.Round(bot, 2, MidpointRounding.AwayFromZero);

                            // Запись значения из GetElementTopPoint(elemincollector).Z в параметр topPointParam - отметки Верха
                            topPointParam.Set(Convert.ToString(topVal));
                            // Запись значения из GetElementBottomPoint(elemincollector).Z в параметр bottomPointParam - отметки Низа
                            bottomPointParam.Set(Convert.ToString(botVal));


                            FilteredElementCollector levelsCollector = new FilteredElementCollector(doc).OfClass(typeof(Level));
                            // Список levels с типом Element
                            List<Level> levels = levelsCollector.Cast<Level>().OrderBy(level => level.Elevation).ToList();

                            // Получение отметки Верха элемента.
                            double elementBottomPointElevetionSm = ElementTopBottomPt.GetElementBottomPoint(elemincollector).Z;

                            Level closestLevel = LevelInfo.FindBottomLevel(elementBottomPointElevetionSm, levels);
                            // Запись Имени этажа. Тест.

                            //var value = "Не определено";
                            //if (closestLevel != null)
                            //    value = closestLevel.Name;
                            levelInfo.Set(closestLevel != null ? closestLevel.Name : "Не определено");
                            

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

