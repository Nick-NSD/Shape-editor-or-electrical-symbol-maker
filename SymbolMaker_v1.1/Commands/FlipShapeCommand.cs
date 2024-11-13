using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolMaker
{
    public class FlipShapeCommand : ICommand
    {
        private readonly ShapeBase Shape;
        private readonly bool FlipHorizontally;
        private List<PointV2D> InitialVertices; // Store the initial vertices
        private List<PointV2D> FlippedVertices; // Store the flipped vertices for redo

        public FlipShapeCommand(ShapeBase shape, bool flipHorizontally)
        {
            Shape = shape;
            FlipHorizontally = flipHorizontally;

            if (Shape is PolygonShape polygon)
            {
                // Store the original vertices for undo
                InitialVertices = polygon.Vertices.Select(v => new PointV2D(v.X, v.Y)).ToList();
                FlippedVertices = new List<PointV2D>();
            }
        }

        public void Execute()
        {
            if (Shape is PolygonShape polygon)
            {
                polygon.Flip(FlipHorizontally); // Perform the flip
                                                // Store the new flipped vertices after flip operation
                FlippedVertices = polygon.Vertices.Select(v => new PointV2D(v.X, v.Y)).ToList();
            }
            else if (Shape is ArcShape arc)
            {
                arc.Flip(FlipHorizontally);
                arc.GetSingleShapeBounds();
            }
            else if (Shape is SymbolShape sym)
            {
                sym.Flip(FlipHorizontally);
                sym.GetSingleShapeBounds();
            }
        }

        public void Unexecute()
        {
            if (Shape is PolygonShape polygon)
            {
                // Revert to the initial vertices
                polygon.Vertices.Clear();
                polygon.Vertices.AddRange(InitialVertices.Select(v => new PointV2D(v.X, v.Y)));
                polygon.GetSingleShapeBounds(); // Recalculate the bounding box
            }

            else if (Shape is ArcShape arc)
            {
                arc.Flip(FlipHorizontally);
                arc.GetSingleShapeBounds();
            }

            else if (Shape is SymbolShape sym)
            {
                sym.Flip(FlipHorizontally);
                sym.GetSingleShapeBounds();
            }
        }
    }
}
