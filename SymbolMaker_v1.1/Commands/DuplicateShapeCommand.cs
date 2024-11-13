using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace SymbolMaker
{
    public class DuplicateShapeCommand : ICommand
    {
        private readonly List<ShapeBase> ShapeList;
        private readonly ShapeBase OriginalShape;
        private ShapeBase ClonedShape;
        private readonly float Offset;

        public DuplicateShapeCommand(List<ShapeBase> shapeList, ShapeBase originalShape, float offset)
        {
            ShapeList = shapeList;
            OriginalShape = originalShape;
            Offset = offset;
        }

        public void Execute()
        {
            // Clone the original shape and shift its position
            ClonedShape = OriginalShape.Clone();

            if (ClonedShape is GroupShape groupShape)
            {
                // Assign unique connection numbers to any ConnectionShape within the group
                AssignConnectionNumbersInGroup(groupShape);
            }

            if (ClonedShape is ConnectionShape con)
            {
                Form1.connectionNumber++;
                con.ConnectionNumber = Form1.connectionNumber;
                Debug.WriteLine("ConnectionNumber = " + con.ConnectionNumber);
            }
            ShiftShape(ClonedShape, Offset);

            // Add the cloned shape to the list
            ShapeList.Add(ClonedShape);
        }

        private void AssignConnectionNumbersInGroup(GroupShape group)
        {
            foreach (var shape in group.Shapes)
            {
                if (shape is ConnectionShape connectionShape)
                {
                    Form1.connectionNumber++;
                    connectionShape.ConnectionNumber = Form1.connectionNumber;
                    Debug.WriteLine("Assigned ConnectionNumber in Group = " + connectionShape.ConnectionNumber);
                }
                else if (shape is GroupShape nestedGroup)
                {
                    // Recursively handle nested groups
                    AssignConnectionNumbersInGroup(nestedGroup);
                }
            }
        }


        public void Unexecute()
        {
            // Remove the cloned shape from the list
            ShapeList.Remove(ClonedShape);
        }

        private void ShiftShape(ShapeBase shape, float offset)
        {
            // Apply offset depending on shape type, similar to your DuplicateShape method
            if (shape is ArcShape arc)
            {
                arc.StartPoint = new PointV2D(arc.StartPoint.X + offset, arc.StartPoint.Y + offset);
                arc.EndPoint = new PointV2D(arc.EndPoint.X + offset, arc.EndPoint.Y + offset);
            }
            else if (shape is EllipseShape eli)
            {
                eli.StartPoint = new PointV2D(eli.StartPoint.X + offset, eli.StartPoint.Y + offset);
                eli.EndPoint = new PointV2D(eli.EndPoint.X + offset, eli.EndPoint.Y + offset);
                eli.Rect = new RectangleF(eli.Rect.X + offset, eli.Rect.Y + offset, eli.Rect.Width, eli.Rect.Height);
            }
            else if (shape is LineShape line)
            {
                line.StartPoint = new PointV2D(line.StartPoint.X + offset, line.StartPoint.Y + offset);
                line.EndPoint = new PointV2D(line.EndPoint.X + offset, line.EndPoint.Y + offset);
                line.Rect = new RectangleF(line.Rect.X + offset, line.Rect.Y + offset, line.Rect.Width, line.Rect.Height);
            }
            else if (shape is RectangleShape rect)
            {
                rect.StartPoint = new PointV2D(rect.StartPoint.X + offset, rect.StartPoint.Y + offset);
                rect.EndPoint = new PointV2D(rect.EndPoint.X + offset, rect.EndPoint.Y + offset);
            }
            else if (shape is DotShape dot)
            {
                dot.StartPoint = new PointV2D(dot.StartPoint.X + offset, dot.StartPoint.Y + offset);
                dot.EndPoint = new PointV2D(dot.StartPoint.X + offset, dot.StartPoint.Y + offset);
            }
            else if (shape is TextShape txt)
            {
                txt.StartPoint = new PointV2D(txt.StartPoint.X + offset, txt.StartPoint.Y + offset);
                txt.EndPoint = new PointV2D(txt.EndPoint.X + offset, txt.EndPoint.Y + offset);
                txt.Rect = new RectangleF(txt.Rect.X + offset - 4, txt.Rect.Y + offset - 4, txt.Rect.Width, txt.Rect.Height);
            }
            else if (shape is ConnectionShape cs)
            {
                cs.StartPoint = new PointV2D(cs.StartPoint.X + offset, cs.StartPoint.Y + offset);
                cs.EndPoint = new PointV2D(cs.EndPoint.X + offset, cs.EndPoint.Y + offset);
                cs.Rect = new RectangleF(cs.Rect.X + offset - 4, cs.Rect.Y + offset - 4, cs.Rect.Width, cs.Rect.Height);
                cs.ConnectionName.StartPoint = new PointV2D(cs.ConnectionName.StartPoint.X + offset, cs.ConnectionName.StartPoint.Y + offset);
            }
            else if (shape is PolygonShape poly)
            {
                var shiftedVertices = new List<PointV2D>();
                foreach (var vertex in poly.Vertices)
                {
                    shiftedVertices.Add(new PointV2D(vertex.X + offset, vertex.Y + offset));
                }
                poly.Vertices = shiftedVertices;
            }
            else if (shape is GroupShape grp)
            {
                foreach (var shp in grp.Shapes)
                {
                    ShiftShape(shp, offset); // Recursively shift each shape in the group
                }
                grp.UpdateBoundingBox();
            }
            else if (shape is SymbolShape sym)
            {
                sym.SymbolID = Guid.NewGuid().ToString();
                foreach (var sh in sym.InternalShapes)
                {
                    if (!(sh is TextShape))//shift only geometrical shapes
                    {
                        sh.StartPoint = new PointV2D(sh.StartPoint.X + offset, sh.StartPoint.Y + offset);
                        sh.EndPoint = new PointV2D(sh.EndPoint.X + offset, sh.EndPoint.Y + offset);

                        // Special case for EllipseShape
                        sh.Rect = new RectangleF(sh.Rect.X + offset, sh.Rect.Y + offset, sh.Rect.Width, sh.Rect.Height);
                        sh.GetSingleShapeBounds();
                    }

                    if (sh is ConnectionShape ics)//shift connection dot and name is not necessary
                    {
                        // Here the shift is not necessary because it has already been executed
                        // in the declaration above where only TextShape is excluded
                        ics.StartPoint = new PointV2D(ics.StartPoint.X, ics.StartPoint.Y);
                        ics.EndPoint = new PointV2D(ics.EndPoint.X, ics.EndPoint.Y);
                        ics.Rect = new RectangleF((float)ics.StartPoint.X - 4, (float)ics.StartPoint.Y - 4, ics.Rect.Width, ics.Rect.Height);

                        ics.ConnectionName.StartPoint = new PointV2D(ics.ConnectionName.StartPoint.X + offset, ics.ConnectionName.StartPoint.Y + offset);
                        ics.ConnectionName.ShapeID = "";
                        ics.ConnectionName.GetSingleShapeBounds();
                    }
                    else if (sh is TextShape itxt)//shift all texts
                    {
                        itxt.StartPoint = new PointV2D(itxt.StartPoint.X + offset, itxt.StartPoint.Y + offset);
                        itxt.EndPoint = new PointV2D(itxt.EndPoint.X + offset, itxt.EndPoint.Y + offset);
                        itxt.Rect = new RectangleF(itxt.Rect.X + offset, itxt.Rect.Y + offset, itxt.Rect.Width, itxt.Rect.Height);
                        itxt.GetSingleShapeBounds();
                    }
                    if (sh is PolygonShape pol)//special case polygon
                    {
                        var shiftedVertices = new List<PointV2D>();

                        foreach (var vertex in pol.Vertices)
                        {
                            shiftedVertices.Add(new PointV2D(vertex.X + offset, vertex.Y + offset));
                        }
                        pol.Vertices = new List<PointV2D>(shiftedVertices);
                        pol.GetSingleShapeBounds();
                    }
                }
                sym.UpdateBoundingBox();
            }
            shape.GetSingleShapeBounds();
        }
    }
}
