using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace TestTasks
{
    public class ValveInfo
    {
        private double lenghtFromInitial;

        public FamilyInstance Valve { get; }
        public double LenghtFromInitial
        {
            get => lenghtFromInitial; set
            {
                lenghtFromInitial = UnitUtils.Convert(value, UnitTypeId.Feet, UnitTypeId.Millimeters);
            }
        }
        public List<ElementId> PipePathIds { get; }

        public ValveInfo(FamilyInstance member, double lenghtFromInitial, List<ElementId> pipePathIds)
        {
            Valve = member;
            LenghtFromInitial = lenghtFromInitial;
            PipePathIds = pipePathIds;
        }
    }
}
