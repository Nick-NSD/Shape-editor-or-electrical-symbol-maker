using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace SymbolMaker
{
    [Serializable]
    public class GroupShape : ShapeBase
    {
        [XmlElement("GroupShapes")]
        public List<ShapeBase> Shapes { get; set; } = new List<ShapeBase>();

        public GroupShape()
        {
            // Parameterless constructor required for XML serialization
        }

        public GroupShape(List<ShapeBase> groupShapes)
        {
            Shapes = groupShapes;
            UpdateBoundingBox();
        }

        public override void Draw(Graphics g)
        {
            foreach (var shape in Shapes)
            {
                shape.Draw(g);
                shape.IsSelected = false;
            }
        }

        public override void Move(double deltaX, double deltaY)
        {
            foreach (var shape in Shapes)
            {
                // If the shape is a symbol, move it (which will move all internal shapes within the symbol)
                if (shape is SymbolShape symbol)
                {
                    symbol.Move(deltaX, deltaY);
                }
                else
                {
                    // If it's a regular shape, move it directly
                    shape.Move(deltaX, deltaY);
                }
            }

            // Refresh the bounding box for the group
            UpdateBoundingBox();
        }

        public void UpdateBoundingBox()
        {
            if (Shapes.Count > 0)
            {
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float maxX = float.MinValue;
                float maxY = float.MinValue;

                foreach (var shape in Shapes)
                {
                    minX = Math.Min(minX, shape.Rect.Left);
                    minY = Math.Min(minY, shape.Rect.Top);
                    maxX = Math.Max(maxX, shape.Rect.Right);
                    maxY = Math.Max(maxY, shape.Rect.Bottom);
                }

                StartPoint = new PointV2D(minX, minY);
                EndPoint = new PointV2D(maxX, maxY);
                Width = maxX - minX;
                Height = maxY - minY;
                Rect = new RectangleF(minX, minY, (float)Width, (float)Height);
            }
        }

        public override void Rotate(float angle)
        {
            //throw new NotImplementedException();
            //the group cannot be rotated
        }

        public override void Flip(bool flipHorizontally)
        {
            //throw new NotImplementedException()
            //the group cannot be fliped
        }

        public override ShapeBase Clone()
        {
            
            var clonedGroup = new GroupShape();

            var clonedRectangle = new RectangleShape();
            clonedRectangle.StartPoint = new PointV2D(
               StartPoint.X ,
               StartPoint.Y
           );

            clonedRectangle.EndPoint = new PointV2D(
                EndPoint.X,
                EndPoint.Y
            );

            clonedGroup.Rect = new RectangleF(
                  (float)(StartPoint.X),
                  (float)(StartPoint.Y),
                  Rect.Width,
                  Rect.Height
              );
            foreach (var shape in Shapes)
            {
                
                clonedGroup.Shapes.Add(shape.Clone());
            }

            CloneCommonProperties(clonedGroup);
            return clonedGroup;
        }
    }
}