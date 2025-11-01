using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace asBIM.External
{
    public class LineSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is ModelLine || elem is DetailLine;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}