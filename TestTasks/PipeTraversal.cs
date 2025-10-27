using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTasks
{
    public static class Extensions
    {
        public static double GetLength(this Pipe pipe)
        {
            return pipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
        }
    }
    public class MEPNetworkMemberInfo<T>
    {
        public T Member; // Valve or Pipe
        public double LenghtFromInitial;
        public ElementId Id;

        public MEPNetworkMemberInfo(T member, double lenghtFromInitial, ElementId id)
        {
            Member = member;
            LenghtFromInitial = lenghtFromInitial;
            Id = id;
        }
        // override object.Equals
        public override bool Equals(object obj)
        {

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            // TODO: write your implementation of Equals() here

            return this.Id == (obj as MEPNetworkMemberInfo<T>).Id;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here

            return base.GetHashCode();
        }

    }
    internal class PipeTraversal
    {
        private FamilyInstance InitialValve;
        public HashSet<FamilyInstance> TraversedValves = new HashSet<FamilyInstance>();
        public HashSet<Pipe> TraversedPipes = new HashSet<Pipe>();
        public List<ElementId> Visited = new List<ElementId>();
        private double TraversedLength = 0;
        public PipeTraversal(FamilyInstance valve)
        {
            //InitialValve = valve;
            TraverseFamily(valve);

        }
        private void TraverseFamily(FamilyInstance valve)
        {
            if (!Visited.Contains(valve.Id))
                Visited.Add(valve.Id);
            else return;
            var iter = GetConnectors(valve);
            while (iter.MoveNext())
            {
                var conn = iter.Current as Connector;
                if (conn.AllRefs.Size > 0)
                {
                    TraverseConnectors(conn.AllRefs, valve);
                }
            }
        }
        private void TraversePipe(Pipe pipe)
        {
            if (!Visited.Contains(pipe.Id))
                Visited.Add(pipe.Id);
            else return;
            var iter = GetConnectors(pipe);
            while (iter.MoveNext())
            {
                var conn = iter.Current as Connector;
                if (conn.AllRefs.Size > 0)
                {
                    TraverseConnectors(conn.AllRefs, pipe);
                }
            }
        }
        private void TraverseConnectors(ConnectorSet conns, Element initialElement)
        {
            var iter = GetConnectors(conns);
            while (iter.MoveNext())
            {
                var conn = iter.Current as Connector;
                
                if (conn.Owner.Id == initialElement.Id)
                    return;
                if (conn.Owner is Pipe)
                {
                    var owner = conn.Owner as Pipe;
                    Debug.Print($"{owner.Id}");
                    TraversePipe(owner);
                }
                if (conn.Owner is FamilyInstance)
                {
                    var owner = conn.Owner as FamilyInstance;
                    Debug.Print($"{owner.Id}");
                    TraverseFamily(owner);
                }
            }
        }

        private ConnectorSetIterator GetConnectors(Pipe pipe)
        {
            return pipe.MEPSystem.ConnectorManager.Connectors.ForwardIterator();
        }
        private ConnectorSetIterator GetConnectors(FamilyInstance valve)
        {

            return valve.MEPModel.ConnectorManager.Connectors.ForwardIterator();

        }
        private ConnectorSetIterator GetConnectors(ConnectorSet connectors) //AllrefsIterator
        {

            return connectors.ForwardIterator();

        }

        public void GetElementAtConnector(Connector connector)
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
