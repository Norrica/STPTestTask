using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace TestTasks
{
    public static class Extensions
    {
        public static double GetLength(this Pipe pipe)
        {
            return pipe.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
        }
    }


}
