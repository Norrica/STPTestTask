using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System.Collections.Generic;

namespace TestTasks
{
    internal class PipeTraversal
    {
        public List<ElementId> Visited = new List<ElementId>();
        public List<ValveInfo> Valves = new List<ValveInfo>();
        private readonly double TraversedLength = 0;

        public PipeTraversal(FamilyInstance valve)
        {
            var pipePath = new List<ElementId>();
            SimpleTraversal(valve, TraversedLength, pipePath);
        }

        public void SimpleTraversal(Element f, double length, List<ElementId> currentPipePath)
        {
            if (f is Pipe p)
            {
                var updatedPipePath = new List<ElementId>(currentPipePath) { p.Id };

                foreach (Connector conn in p.ConnectorManager.Connectors)
                    foreach (Connector reff in conn.AllRefs)
                    {
                        if (Visited.Contains(reff.Owner.Id))
                            continue;
                        Visited.Add(reff.Owner.Id);
                        SimpleTraversal(reff.Owner, length + p.GetLength(), updatedPipePath);
                    }
            }
            if (f is FamilyInstance)
            {
                var fi = f as FamilyInstance;
                foreach (Connector conn in fi.MEPModel.ConnectorManager.Connectors)
                    foreach (Connector reff in conn.AllRefs)
                    {
                        if (Visited.Contains(reff.Owner.Id))
                            continue;
                        Visited.Add(reff.Owner.Id);
                        if (fi.Category.Name == "Арматура трубопроводов")
                            Valves.Add(new ValveInfo(fi, length, new List<ElementId>(currentPipePath)));
                        SimpleTraversal(reff.Owner, length, currentPipePath);
                    }
            }
        }
    }
}
