using System.Collections.Generic;
using System.Linq;

namespace SymbolMaker
{
    public class RotateShapeCommand : ICommand
    {
        private readonly ShapeBase Shape;
        private readonly List<PointV2D> InitialVertices; // Original vertices for undo
        private List<PointV2D> RotatedVertices; // Rotated vertices for redo
        private readonly float RotationAngle; // Angle of rotation in degrees

        public RotateShapeCommand(ShapeBase shape, float rotationAngle)
        {
            Shape = shape;
            RotationAngle = rotationAngle;

            if (Shape is PolygonShape polygon)
            {
                InitialVertices = polygon.Vertices.Select(v => new PointV2D(v.X, v.Y)).ToList();
                RotatedVertices = new List<PointV2D>(InitialVertices); // Initialized to hold rotated values
            }
        }

        public void Execute()
        {
            if (Shape is PolygonShape polygon)
            {
                polygon.Rotate(RotationAngle);
                // Store the new rotated vertices for redo
                RotatedVertices = polygon.Vertices.Select(v => new PointV2D(v.X, v.Y)).ToList();
                polygon.GetSingleShapeBounds();
            }
            else if(Shape is ArcShape arc)
            {
                arc.StartAngle = (arc.StartAngle - RotationAngle) % 360;
            }
            else if(Shape is SymbolShape sym)
            {
                sym.Rotate(RotationAngle);
            }
        }

        public void Unexecute()
        {
            if (Shape is PolygonShape polygon)
            {
                // Undo rotation by rotating by the negative angle
                polygon.Rotate(-RotationAngle);

                // Restore the original vertices to ensure consistency
                polygon.Vertices = InitialVertices.Select(v => new PointV2D(v.X, v.Y)).ToList();
                polygon.GetSingleShapeBounds();
            }
            else if (Shape is ArcShape arc)
            {
                arc.StartAngle = (arc.StartAngle + RotationAngle) % 360;
            }
            else if (Shape is SymbolShape sym)
            {
                sym.Rotate(-RotationAngle);
            }
        }
    }
}
