using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
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
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class PickObject : IExternalCommand
    {
        /// <summary>
        /// PickObject Element Single
        /// Show Information Element
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region Init

            RevitAPISingleton api = RevitAPISingleton.getInstance(commandData);
            UIDocument uidoc = api.uidoc;
            Document doc = api.doc;
            #endregion

            try
            {
                Reference reference = uidoc.Selection.PickObject(ObjectType.Element, new SingleValveFilter());
                Element element = doc.GetElement(reference);
                var ele = element as FamilyInstance;
                var connectors = ele.MEPModel.ConnectorManager.Connectors;
                foreach (Connector connector in connectors)
                {
                    GetElementAtConnector(connector);
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException e)
            {
                TaskDialog.Show($"You Has Press Esc\n{e}", "Warning", TaskDialogCommonButtons.Ok);
                //Pressed Esc
            }

            return Result.Succeeded;
        }
        public void GetElementAtConnector(Autodesk.Revit.DB.Connector connector)
        {

            string message = "Connector is owned by: " + connector.Owner.Name;

            if (connector.IsConnected == true)
            {
                ConnectorSet connectorSet = connector.AllRefs;
                ConnectorSetIterator csi = connectorSet.ForwardIterator();
                while (csi.MoveNext())
                {
                    Connector connected = csi.Current as Connector;
                    if (null != connected)
                    {
                        // look for physical connections
                        if (connected.ConnectorType == ConnectorType.End ||
                            connected.ConnectorType == ConnectorType.Curve ||
                            connected.ConnectorType == ConnectorType.Physical)
                        {
                            message += "\nConnector is connected to: " + connected.Owner.Name;
                            message += "\nConnection type is: " + connected.ConnectorType;
                        }
                    }
                }
            }
            else
            {
                message += "\nConnector is not connected to anything.";
            }

            TaskDialog.Show("Revit", message);

        }
    }
}
