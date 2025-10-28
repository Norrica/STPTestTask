using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using TestTasks;

namespace ValveDistanceApp
{
    public class ValveDistanceViewModel
    {
        public ValveInfo ClosestValve { get; }
        public ValveInfo FurthestValve { get; }
        public int TotalValves { get; }

        // Formatted pipe path strings for display
        public string ClosestValvePipePath => GetPipePathString(ClosestValve);
        public string FurthestValvePipePath => GetPipePathString(FurthestValve);

        public ValveDistanceViewModel(List<ValveInfo> valves, FamilyInstance initialValve)
        {
            if (valves?.Count > 0)
            {
                ClosestValve = valves.OrderBy(v => v.LenghtFromInitial).ElementAt(1);//Closest is at len 0
                FurthestValve = valves.OrderByDescending(v => v.LenghtFromInitial).First();
                TotalValves = valves.Count; 
            }
        }

        private string GetPipePathString(ValveInfo valve)
        {
            if (valve?.PipePathIds == null || valve.PipePathIds.Count == 0)
                return "Ну удалось найти путь";

            return string.Join(" -> ", valve.PipePathIds.Select(id => id.IntegerValue));
        }
    }
}