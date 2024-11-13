using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SymbolMaker.ShapeBase;

namespace SymbolMaker
{
    public class ResizeShapeCommand : ICommand
    {
        private readonly ShapeBase Shape;
        private readonly PointV2D InitialStartPoint;
        private readonly PointV2D InitialEndPoint;
        private PointV2D NewStartPoint;
        private PointV2D NewEndPoint;
        private List<PointV2D> OldVertices; // Store original vertices for undo
        private List<PointV2D> NewVertices; // Store new vertices for redo

        public ResizeShapeCommand(ShapeBase shape)
        {
            Shape = shape;
            InitialStartPoint = new PointV2D(Shape.StartPoint.X, Shape.StartPoint.Y);
            InitialEndPoint = new PointV2D(Shape.EndPoint.X, Shape.EndPoint.Y);
            NewStartPoint = InitialStartPoint;
            NewEndPoint = InitialEndPoint;

            if (shape is PolygonShape polygon)
            {
                // Store original vertices
                OldVertices = polygon.Vertices.Select(v => new PointV2D(v.X, v.Y)).ToList();
                NewVertices = new List<PointV2D>(OldVertices); // Initialize NewVertices with old vertices
            }
        }

        public void Execute()
        {
            Shape.StartPoint = NewStartPoint;
            Shape.EndPoint = NewEndPoint;
            Shape.GetSingleShapeBounds(); // Update bounds

            if (Shape is PolygonShape poly && poly.Vertices.Count > 0)
            {
                // Clear and set the new vertices
                poly.Vertices.Clear();
                poly.Vertices.AddRange(NewVertices.Select(v => new PointV2D(v.X, v.Y)));
                poly.GetSingleShapeBounds(); // Recalculate bounds

            }
        }

        public void Unexecute()
        {
            // Undo the resize by setting back to the old points
            Shape.StartPoint = InitialStartPoint;
            Shape.EndPoint = InitialEndPoint;
            Shape.GetSingleShapeBounds(); // Update bounds after reverting

            if (Shape is PolygonShape polygon)
            {
                // Restore the original vertices
                polygon.Vertices.Clear();
                polygon.Vertices.AddRange(OldVertices.Select(v => new PointV2D(v.X, v.Y)));
                polygon.GetSingleShapeBounds(); // Recalculate bounds
            }
        }

        public void UpdateNewPosition(PointV2D newStartPoint, PointV2D newEndPoint)
        {
            NewStartPoint = newStartPoint; // Update the new start position
            NewEndPoint = newEndPoint; // Update the new end position
            Shape.GetSingleShapeBounds(); // Update the boundingbox of shape
        }

        public void UpdateNewPolygonPosition(List<PointV2D> newVertices)
        {
            if (newVertices != null && newVertices.Count > 0)
            {
                NewVertices = newVertices; // Update the list with the new vertices
                Shape.GetSingleShapeBounds(); // Update the bounding box
            }
        }
    }

    //public class ResizeShapeCommand : ICommand
    //{
    //    private readonly ShapeBase Shape;
    //    private readonly PointV2D InitialStartPoint;
    //    private readonly PointV2D InitialEndPoint;
    //    private PointV2D NewStartPoint;
    //    private PointV2D NewEndPoint;
    //    private List<PointV2D> NewVertices;
    //    private List<PointV2D> OldVertices;

    //    public ResizeShapeCommand(ShapeBase shape)
    //    {
    //        Shape = shape;

    //        // Capture initial positions
    //        InitialStartPoint = new PointV2D(Shape.StartPoint.X, Shape.StartPoint.Y);
    //        InitialEndPoint = new PointV2D(Shape.EndPoint.X, Shape.EndPoint.Y);
    //        NewStartPoint = InitialStartPoint;
    //        NewEndPoint = InitialEndPoint;

    //        if (shape is PolygonShape polygon)
    //        {
    //            // Store original vertices
    //            OldVertices = polygon.Vertices.Select(v => new PointV2D(v.X, v.Y)).ToList();
    //            NewVertices = new List<PointV2D>(OldVertices); // Initialize NewVertices with old vertices
    //        }
    //    }

    //    public void Execute()
    //    {
    //        Shape.StartPoint = NewStartPoint;
    //        Shape.EndPoint = NewEndPoint;
    //        Shape.GetSingleShapeBounds(); // Update bounds

    //        if (Shape is PolygonShape poly && poly.Vertices.Count > 0)
    //        {
    //            // Clear and set the new vertices
    //            poly.Vertices.Clear();
    //            poly.Vertices.AddRange(NewVertices.Select(v => new PointV2D(v.X, v.Y)));
    //            poly.GetSingleShapeBounds(); // Recalculate bounds
    //        }
    //    }

    //    public void Unexecute()
    //    {
    //        // Use the initial positions to simulate "undoing" the resize
    //        Shape.StartPoint = InitialStartPoint;
    //        Shape.EndPoint = InitialEndPoint;

    //        if (Shape is PolygonShape polygon)
    //        {
    //            // Restore the original vertices
    //            polygon.Vertices.Clear();
    //            polygon.Vertices.AddRange(OldVertices.Select(v => new PointV2D(v.X, v.Y)));
    //            polygon.GetSingleShapeBounds(); // Recalculate bounds
    //        }
    //    }

    //    public void UpdateNewPosition(PointV2D newStartPoint, PointV2D newEndPoint)
    //    {
    //        NewStartPoint = newStartPoint; // Update the new start position
    //        NewEndPoint = newEndPoint; // Update the new end position
    //        Shape.GetSingleShapeBounds(); // Update the boundingbox of shape
    //    }

    //    public void UpdateNewPolygonPosition(List<PointV2D> newVertices)
    //    {
    //        if (newVertices != null && newVertices.Count > 0)
    //        {
    //            NewVertices = newVertices; // Update the list with the new vertices
    //            Shape.GetSingleShapeBounds(); // Update the bounding box
    //        }
    //    }
    //}
}


