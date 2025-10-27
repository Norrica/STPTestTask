using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;


using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Plumbing;
using System.Linq;
namespace TestTasks
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class PipeFind : IExternalCommand
    {
        IList<ElementId> listId = new List<ElementId>();
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            Reference refer = uidoc.Selection.PickObject(ObjectType.Element, "");
            Element elem = doc.GetElement(refer);
            listId.Add(elem.Id);
            Pipe pipe = elem as Pipe;
            XYZ xyz = refer.GlobalPoint;
            Curve curve = (pipe.Location as LocationCurve).Curve;
            XYZ project = curve.Project(xyz).XYZPoint;
            Connector connector = BackPipeNearConnectors(elem, xyz);
            BackAlrefsConnectors(connector);
            uidoc.Selection.SetElementIds(listId);
            return Result.Succeeded;
        }
        ///
        /// Returns the closest point to the pipe joint origin and the point at which the mouse picks the point on the pipe.
                ///
                ///
                ///
                ///
        private Connector BackPipeNearConnectors(Element elem, XYZ xyz)
        {
            Connector conn = null;
            MEPCurve mep = elem as MEPCurve;
            SortedDictionary<double, Connector> dictionary = new SortedDictionary<double, Connector>();
            //ConnectorSetIterator connectorSetIterator = mep.ConnectorManager.Connectors.ForwardIterator();
            ConnectorSetIterator connectorSetIterator = mep.ConnectorManager.Connectors.ReverseIterator();
            while (connectorSetIterator.MoveNext())
            {
                Connector connector = connectorSetIterator.Current as Connector;
                if (connector.AllRefs.Size > 0)
                {
                    dictionary.Add(connector.Origin.DistanceTo(xyz), connector);
                }
            }


            return dictionary.Values.ElementAt(0);
        }
        ///
        /// Get all the connections of the connection on the connector Get the element according to the connector Get the connector according to the element Implement recursive query
                ///
                ///
        private void BackAlrefsConnectors(Connector connector)
        {
            Element elem = null;
            //ConnectorSetIterator connectorSetIterator = connector.AllRefs.ForwardIterator();
            ConnectorSetIterator connectorSetIterator = connector.AllRefs.ReverseIterator();
            while (connectorSetIterator.MoveNext())
            {
                Connector connref = connectorSetIterator.Current as Connector;
                if (connref.Origin.IsAlmostEqualTo(connector.Origin))
                {
                    elem = connref.Owner;
                    if (elem is Pipe)
                    {
                        if (!listId.Contains(elem.Id))
                        {
                            listId.Add(elem.Id);
                        }
                        else return;

                        Connector connector1 = BackPipeConnectors(elem, connector);
                        if (connector1.IsConnected)
                        {
                            BackAlrefsConnectors(connector1);
                        }
                    }
                    else if (elem is FamilyInstance)
                    {
                        if (!listId.Contains(elem.Id))
                        {
                            listId.Add(elem.Id);
                        }
                        else return;
                        Connector connector1 = BackInstanceConnectors(elem as FamilyInstance, connector);
                        if (connector1.IsConnected)
                        {
                            BackAlrefsConnectors(connector1);
                        }
                    }
                }
            }
            

            

        }
        ///
        /// returns the specified connector on the pipe
                ///
                ///
                ///
                ///
        private Connector BackPipeConnectors(Element elem, Connector conn)
        {
            Connector connector = null;
            MEPCurve mep = elem as MEPCurve;
            //ConnectorSetIterator connectorSetIterator = mep.ConnectorManager.Connectors.ForwardIterator();
            ConnectorSetIterator connectorSetIterator = mep.ConnectorManager.Connectors.ReverseIterator();
            while (connectorSetIterator.MoveNext())
            {
                Connector currentConnector = connectorSetIterator.Current as Connector;
                if (currentConnector.AllRefs.Size > 0)
                {
                    if (!currentConnector.Origin.IsAlmostEqualTo(conn.Origin))
                    {
                        connector = currentConnector;
                        break;
                    }
                }
            }

            return connector;
        }
        ///
        /// Return to the specified connector of the elbow gate valve and other instances
                ///
                ///
                ///
                ///
        private Connector BackInstanceConnectors(FamilyInstance instance, Connector conn)
        {
            Connector connector = null;
            MEPModel model = instance.MEPModel as MEPModel;
            //ConnectorSetIterator connectorSetIterator = model.ConnectorManager.Connectors.ForwardIterator();
            ConnectorSetIterator connectorSetIterator = model.ConnectorManager.Connectors.ReverseIterator();
            while (connectorSetIterator.MoveNext())
            {
                Connector currentConnector = connectorSetIterator.Current as Connector;
                if (currentConnector.AllRefs.Size > 0)
                {
                    if (!currentConnector.Origin.IsAlmostEqualTo(conn.Origin))
                    {
                        connector = currentConnector;
                        break;
                    }
                }
            }
            return connector;
        }
    }
    public class PipeFilters : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is Pipe || elem is FamilyInstance;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
