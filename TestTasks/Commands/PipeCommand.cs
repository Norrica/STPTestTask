using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace TestTasks
{
    [Transaction(TransactionMode.Manual)]
    internal class PipeCommand : IExternalCommand
    {
        private RevitAPISingleton Api { get; set; }
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Api = RevitAPISingleton.getInstance(commandData);

            UIDocument uidoc = Api.Uidoc;
            try
            {
                // TODO Понять что значит "Есть вход но выход пустой" и реализовать ошибку на зацикленность
                List<Element> filteredElements = uidoc.Selection.PickElementsByRectangle(new PipeSelectionFilter(), "Выберите трубы") as List<Element>;

                double totalLength = 0;
                HashSet<ElementId> mepIds = new HashSet<ElementId>();
                List<ElementId> failureIds = new List<ElementId>();
                foreach (Pipe item in filteredElements)
                {
                    if (item.MEPSystem is null)
                    {
                        failureIds.Add(item.Id);
                    }
                    else
                    {
                        mepIds.Add(item.MEPSystem?.Id);
                        if (mepIds.Count > 1)
                        {
                            TaskDialog.Show("Ошибка", "Трубы в разных системах", TaskDialogCommonButtons.Cancel);
                            return Result.Cancelled;
                        }
                        totalLength += item.GetLength();
                    }
                }

                if (failureIds.Count > 0)
                {
                    uidoc.Selection.SetElementIds(failureIds);
                    TaskDialog.Show("Ошибка", $"Для данных труб не определен Тип Системы", TaskDialogCommonButtons.Cancel);
                    return Result.Cancelled;
                }
                totalLength = UnitUtils.Convert(totalLength, UnitTypeId.Feet, UnitTypeId.Millimeters);
                TaskDialog.Show("Общая длина", $"{totalLength} mm", TaskDialogCommonButtons.Ok);
                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
        }

    }

    internal class PipeSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            try
            {
                return elem is Pipe;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}