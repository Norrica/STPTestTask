using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using ValveDistanceApp;

namespace TestTasks
{
    [Transaction(TransactionMode.Manual)]
    internal class ValveCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitAPISingleton api = RevitAPISingleton.getInstance(commandData);
            Reference valveRef = api.Uidoc.Selection.PickObject(ObjectType.Element, new SingleValveFilter());
            var valve = api.Doc.GetElement(valveRef);
            if (valve != null)
            {
                var pt = new PipeTraversal(valve as FamilyInstance);
                var selectionList = new List<ElementId>();
                foreach (var ptObj in pt.Valves)
                {
                    selectionList.AddRange(ptObj.PipePathIds);
                }
                var viewModel = new ValveDistanceViewModel(pt.Valves, valve as FamilyInstance);
                api.Uidoc.Selection.SetElementIds(
                    new List<ElementId> { viewModel.ClosestValve.Valve.Id, viewModel.FurthestValve.Valve.Id }
                    );
                var window = new InfoDisplayWindow(viewModel);
                window.ShowDialog();
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
