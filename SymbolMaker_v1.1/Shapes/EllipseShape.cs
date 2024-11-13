using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;

namespace SymbolMaker
{
    [Serializable]
    public class EllipseShape : ShapeBase
    {
        public float RotationAngle = 0;

        public EllipseShape(PointV2D startPoint, PointV2D endPoint, Color borderPenColor, Color fillBrushColor)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            BorderPenColor = borderPenColor;
            FillBrushColor = fillBrushColor;
            GetSingleShapeBounds();
        }

        public EllipseShape()
        {
            // Parameterless constructor required for XML serialization
        }

        public override void Draw(Graphics g)
        {
            using (Pen pen = new Pen(BorderPenColor, PenThickness))
            using (Pen p = new Pen(Color.Black, 0.15f))
            using (SolidBrush sb = new SolidBrush(FillBrushColor))
            using (HatchBrush hb = new HatchBrush(ShapeUtil.ConvertToHatchStyle(HatchStyl), BorderPenColor, FillBrushColor))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;
                pen.DashCap = DashCap.Flat;

                p.DashPattern = new float[] { 2, 2 };
                p.DashCap = DashCap.Round;
                p.DashStyle = DashStyle.Custom;
                p.LineJoin = LineJoin.Round;

                float[] dashPattern = ShapeUtil.GetPenDashPattern(LinStyle); // Get the pattern

                if (dashPattern == null)
                {
                    pen.DashStyle = DashStyle.Solid; // Solid line if no custom dash pattern
                }
                else
                {
                    pen.DashPattern = dashPattern;
                }

                if (!IsSelected)
                {
                    g.FillEllipse(sb, Rect);
                    if (!(HatchStyl == CustomHatchStyle.None)) g.FillEllipse(hb, Rect);
                    g.DrawEllipse(pen, Rect);
                }
                if (IsSelected)
                {
                    g.DrawRectangle(p, Rect.X, Rect.Y, Rect.Width, Rect.Height);
                    g.FillEllipse(sb, Rect);
                    if (!(HatchStyl == CustomHatchStyle.None)) g.FillEllipse(hb, Rect);
                    g.DrawEllipse(p, Rect);
                }
            }
        }

        public override void Move(double deltaX, double deltaY)
        {
            StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y + deltaY);
            EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y + deltaY);
            GetSingleShapeBounds();
        }

        public override void Rotate(float angle)
        {
            // Get the center of the bounding box (Rect)
            float centerX = Rect.X + Rect.Width / 2f;
            float centerY = Rect.Y + Rect.Height / 2f;

            // Convert angle to radians
            double angleRad = Math.PI * angle / 180.0;

            // To rotate, you could just store the rotation angle and apply the transformation during drawing
            RotationAngle += angle; // RotationAngle is a new property you'll need to add to store the current angle
        }

        public override void Flip(bool flipHorizontally)
        {
            // Get the center of the bounding box (Rect)
            float centerX = Rect.X + Rect.Width / 2f;
            float centerY = Rect.Y + Rect.Height / 2f;

            if (flipHorizontally)
            {
                // Flip horizontally: Swap the left and right sides of the bounding box
                Rect = new RectangleF((2 * centerX - Rect.Right), Rect.Y, Rect.Width, Rect.Height);
            }
            else
            {
                // Flip vertically: Swap the top and bottom sides of the bounding box
                Rect = new RectangleF(Rect.X, (2 * centerY - Rect.Bottom), Rect.Width, Rect.Height);
            }
        }

        public override ShapeBase Clone()
        {
            var clonedEllipse = new EllipseShape();
            clonedEllipse.StartPoint = new PointV2D(
              StartPoint.X,
              StartPoint.Y
          );
            clonedEllipse.EndPoint = new PointV2D(
                EndPoint.X,
                EndPoint.Y
            );
            clonedEllipse.Rect = new RectangleF((float)(StartPoint.X), (float)(StartPoint.Y), Rect.Width, Rect.Height);
            CloneCommonProperties(clonedEllipse);
            return clonedEllipse;
        }
    }
}
