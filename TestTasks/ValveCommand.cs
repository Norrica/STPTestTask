using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTasks
{
    [Transaction(TransactionMode.Manual)]
    internal class ValveCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitAPISingleton api = RevitAPISingleton.getInstance(commandData);
            Reference valveRef = api.uidoc.Selection.PickObject(ObjectType.Element, new SingleValveFilter());
            var valve = api.doc.GetElement(valveRef);
            if (valve != null)
            {
                var pt = new PipeTraversal(valve as FamilyInstance);
                api.uidoc.Selection.SetElementIds(pt.Visited);

                //TaskDialog.Show("VAlve", $"{pt.TraversedPipes.Count}\n {pt.TraversedValves.Count}", TaskDialogCommonButtons.Ok);
            }
            return Result.Succeeded;
        }
    }

    internal class SingleValveFilter : ISelectionFilter
    {

        public bool AllowElement(Element elem)
        {
                return elem.Category.Name == "Арматура трубопроводов";
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
