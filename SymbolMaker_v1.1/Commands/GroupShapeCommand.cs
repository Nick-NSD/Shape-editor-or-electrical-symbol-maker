using System.Collections.Generic;

namespace SymbolMaker
{
    public class GroupShapeCommand : ICommand
    {
        private readonly List<ShapeBase> Shapes;
        private readonly List<ShapeBase> groupedShapes;
        private GroupShape groupShape;

        public GroupShapeCommand(List<ShapeBase> shapes, List<ShapeBase> selectedShapes)
        {
            Shapes = shapes;
            groupedShapes = new List<ShapeBase>(selectedShapes);
        }

        public void Execute()
        {
            // Create a new GroupShape from the selected shapes
            groupShape = new GroupShape(groupedShapes);
            groupShape.GetSingleShapeBounds();

            // Remove the individual shapes from the main shapes list
            foreach (var shape in groupedShapes)
            {
                Shapes.Remove(shape);
            }

            // Add the new GroupShape to the main shapes list
            Shapes.Add(groupShape);
        }

        public void Unexecute()
        {
            // Remove the GroupShape from the main shapes list
            Shapes.Remove(groupShape);

            // Re-add the individual shapes to the main shapes list
            foreach (var shape in groupedShapes)
            {
                Shapes.Add(shape);
            }
        }
    }
}
