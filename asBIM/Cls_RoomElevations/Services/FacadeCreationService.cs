using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using asBIM.External;
using asBIM.Models;
using System;
using System.Linq;

namespace asBIM.Services
{
    public class FacadeCreationService
    {
        public static FacadeCreationService Instance { get; } = new FacadeCreationService();
        private FacadeCreationService() { }

        public void StartFacadeCreation(UIApplication uiApp, FacadeCreationSettings settings)
        {
            UIDocument uidoc = uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Reference lineRef = uidoc.Selection.PickObject(ObjectType.Element, new LineSelectionFilter(), "Выберите линию для развертки");
                if (lineRef == null) return;

                Element lineElem = doc.GetElement(lineRef);
                Curve curve = null;

                if (lineElem is ModelLine modelLine) curve = modelLine.GeometryCurve;
                else if (lineElem is DetailLine detailLine) curve = detailLine.GeometryCurve;

                if (curve == null)
                {
                    TaskDialog.Show("Ошибка", "Выбранный элемент не является линией.");
                    return;
                }

                XYZ pt1 = curve.GetEndPoint(0);
                XYZ pt2 = curve.GetEndPoint(1);

                Level baseLevel = doc.GetElement(doc.ActiveView.GenLevel.Id) as Level;
                double bottom = baseLevel.Elevation + settings.BottomHeight;
                double top = baseLevel.Elevation + settings.TopHeight;

                ViewSection facadeView = CreateElevationAlongLine(doc, pt1, pt2, bottom, top, settings);

                if (!string.IsNullOrEmpty(settings.SelectedViewTemplate))
                    ApplyViewTemplate(doc, facadeView, settings.SelectedViewTemplate);

                PlaceViewsOnSheet(doc, facadeView, settings);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                TaskDialog.Show("Отмена", "Выбор линии отменён пользователем.");
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", ex.Message);
            }
        }

        private ViewSection CreateElevationAlongLine(Document doc, XYZ pt1, XYZ pt2, double bottom, double top, FacadeCreationSettings settings)
        {
            XYZ center = (pt1 + pt2) / 2.0;

            using (Transaction t = new Transaction(doc, "Создать развертку"))
            {
                t.Start();

                double markerHeight = top - bottom;
                ElevationMarker marker = ElevationMarker.CreateElevationMarker(
                    doc,
                    doc.GetDefaultElementTypeId(ElementTypeGroup.ViewTypeElevation),
                    center,
                    (int)markerHeight
                );

                ViewSection view = marker.CreateElevation(doc, doc.ActiveView.Id, 0);
                view.Name = $"Развертка помещения №{settings.RoomNumber}. Вид {pt1.X:F0}-{pt2.X:F0}";

                BoundingBoxXYZ crop = view.CropBox;
                crop.Min = new XYZ(crop.Min.X, crop.Min.Y, bottom);
                crop.Max = new XYZ(crop.Max.X, crop.Max.Y, top);
                view.CropBox = crop;

                t.Commit();
                return view;
            }
        }

        private void ApplyViewTemplate(Document doc, View view, string templateName)
        {
            var templates = new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .OfType<View>()
                .Where(v => v.IsTemplate && v.ViewType == ViewType.Elevation);

            foreach (var v in templates)
            {
                if (v.Name.Equals(templateName))
                {
                    view.ViewTemplateId = v.Id;
                    break;
                }
            }
        }

        private void PlaceViewsOnSheet(Document doc, ViewSection view, FacadeCreationSettings settings)
        {
            int sheetNumber;
            double offset = 0.0;

            if (!int.TryParse(settings.SheetNumber, out sheetNumber)) return;
            if (!double.TryParse(settings.ViewsOffset, out offset)) offset = 0.1;

            var sheets = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet))
                .OfType<ViewSheet>()
                .ToList();

            ViewSheet sheet = sheets.FirstOrDefault(s => s.SheetNumber == sheetNumber.ToString());
            if (sheet == null)
            {
                TaskDialog.Show("Ошибка", $"Лист №{sheetNumber} не найден");
                return;
            }

            using (Transaction t = new Transaction(doc, "Разместить вид на листе"))
            {
                t.Start();
                XYZ location = new XYZ(offset, 0, 0);
                Viewport.Create(doc, sheet.Id, view.Id, location);
                t.Commit();
            }
        }
    }
}
