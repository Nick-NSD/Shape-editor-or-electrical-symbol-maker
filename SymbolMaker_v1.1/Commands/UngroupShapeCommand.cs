using System.Collections.Generic;

namespace SymbolMaker
{
    public class UngroupShapeCommand : ICommand
    {
        private readonly List<ShapeBase> shapes;
        private readonly GroupShape groupShape;
        private readonly List<ShapeBase> ungroupedShapes;

        public UngroupShapeCommand(List<ShapeBase> shapes, GroupShape groupShape)
        {
            this.shapes = shapes;
            this.groupShape = groupShape;
            this.ungroupedShapes = new List<ShapeBase>(groupShape.Shapes);
        }

        public void Execute()
        {
            // Remove the GroupShape from the shapes list
            shapes.Remove(groupShape);

            // Add the individual shapes back to the main shapes list
            foreach (var shape in ungroupedShapes)
            {
                shapes.Add(shape);
            }
        }

        public void Unexecute()
        {
            // Remove the individual shapes from the shapes list
            foreach (var shape in ungroupedShapes)
            {
                shapes.Remove(shape);
            }

            // Re-add the GroupShape to the shapes list
            shapes.Add(groupShape);
        }
    }

}
