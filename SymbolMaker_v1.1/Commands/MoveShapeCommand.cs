using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolMaker
{
    //public class MoveShapeCommand : ICommand
    //{
    //    private readonly ShapeBase Shape;
    //    private double DeltaX;
    //    private double DeltaY;

    //    public MoveShapeCommand(ShapeBase shape, double deltaX, double deltaY)
    //    {
    //        Shape = shape;
    //        DeltaX = deltaX;
    //        DeltaY = deltaY;
    //    }

    //    public void Execute()
    //    {
    //        Shape.Move(DeltaX, DeltaY); // Move by delta
    //    }

    //    public void Unexecute()
    //    {
    //        Shape.Move(-DeltaX, -DeltaY); // Reverse by -delta
    //    }

    //    public void UpdateFinalDelta(double finalDeltaX, double finalDeltaY)
    //    {
    //        DeltaX = finalDeltaX;
    //        DeltaY = finalDeltaY;
    //    }
    //}

    public class MoveShapeCommand : ICommand
    {
        private readonly ShapeBase Shape;
        private readonly PointV2D InitialStartPoint;
        private readonly PointV2D InitialEndPoint;
        private PointV2D NewStartPoint;
        private PointV2D NewEndPoint;
        private double DeltaX;
        private double DeltaY;
        private readonly List<PointV2D> InitialVertices;
        private List<PointV2D> NewVertices;

        public MoveShapeCommand(ShapeBase shape)
        {
            Shape = shape;

            // Capture initial position (start, end points, or vertices for polygons)
            InitialStartPoint = new PointV2D(shape.StartPoint.X, shape.StartPoint.Y);
            InitialEndPoint = new PointV2D(shape.EndPoint.X, shape.EndPoint.Y);

            if (shape is PolygonShape polygon)
            {
                InitialVertices = polygon.Vertices.Select(v => new PointV2D(v.X, v.Y)).ToList();
                NewVertices = polygon.Vertices.Select(v => new PointV2D(v.X, v.Y)).ToList();
            }
        }

        public void Execute()
        {
            // Move shape to the new position
            Shape.StartPoint = NewStartPoint;
            Shape.EndPoint = NewEndPoint;

            if (Shape is PolygonShape polygon)
            {
                polygon.Vertices.Clear();
                polygon.Vertices.AddRange(NewVertices);
            }

            else if (Shape is SymbolShape || Shape is GroupShape)
            {
                Shape.Move(DeltaX, DeltaY);
            }
            Shape.GetSingleShapeBounds();
        }

        public void Unexecute()
        {
            // Move shape back to the initial position
            Shape.StartPoint = InitialStartPoint;
            Shape.EndPoint = InitialEndPoint;

            if (Shape is PolygonShape polygon)
            {
                polygon.Vertices.Clear();
                polygon.Vertices.AddRange(InitialVertices);
            }

            else if (Shape is SymbolShape || Shape is GroupShape)
            {
                Shape.Move(-DeltaX, -DeltaY);
            }
            Shape.GetSingleShapeBounds();
        }

        public void UpdateNewPosition(PointV2D newStartPoint, PointV2D newEndPoint)
        {
            NewStartPoint = newStartPoint;
            NewEndPoint = newEndPoint;
            Shape.GetSingleShapeBounds();

            if (Shape is PolygonShape polygon)
            {
                // Update NewVertices to reflect the new position
                NewVertices = polygon.Vertices.Select(v => new PointV2D(v.X, v.Y)).ToList();
            }
        }

        public void UpdateFinalDelta(double finalDeltaX, double finalDeltaY)
        {
            DeltaX = finalDeltaX;
            DeltaY = finalDeltaY;
        }
    }
}
