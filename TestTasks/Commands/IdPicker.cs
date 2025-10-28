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
    internal class IdPicker : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitAPISingleton api = RevitAPISingleton.getInstance(commandData);
            Reference valveRef = api.Uidoc.Selection.PickObject(ObjectType.Element);
            var valve = api.Doc.GetElement(valveRef);
            if (valve != null)
            {
                TaskDialog.Show("Id", $"{valve.Id}");
            }
            return Result.Succeeded;
        }
    }

    
}
