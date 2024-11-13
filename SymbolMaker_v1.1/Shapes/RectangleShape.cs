using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;

namespace SymbolMaker
{
    [Serializable]
    public class RectangleShape : ShapeBase
    {
        public RectangleShape(PointV2D startPoint, PointV2D endPoint, Color borderPenColor, Color fillBrushColor)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            BorderPenColor = borderPenColor;
            FillBrushColor = fillBrushColor;
            GetSingleShapeBounds();
        }

        public RectangleShape()
        {
            // Parameterless constructor required for XML serialization
        }

        public override void Draw(Graphics g)
        {
            GetSingleShapeBounds();
            using (Pen pen = new Pen(BorderPenColor, PenThickness))
            using (Pen p = new Pen(Color.Black, 0.15f))
            using (SolidBrush sb = new SolidBrush(FillBrushColor))
            using (HatchBrush hb = new HatchBrush(ShapeUtil.ConvertToHatchStyle(HatchStyl), BorderPenColor, FillBrushColor))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;
                pen.DashCap = DashCap.Flat;

                p.StartCap = LineCap.Round;
                p.EndCap = LineCap.Round;
                p.LineJoin = LineJoin.Round;
                p.DashPattern = new float[] { 2, 2 };
                p.DashCap = DashCap.Round;
                p.DashStyle = DashStyle.Custom;
                p.Width = 0.25F;

                float[] dashPattern = ShapeUtil.GetPenDashPattern(LinStyle);

                if (dashPattern == null)
                {
                    pen.DashStyle = DashStyle.Solid;
                }
                else
                {
                    pen.DashPattern = dashPattern;
                }

                if (!IsSelected)
                {
                    g.FillRectangle(sb, Rect);
                    if (!(HatchStyl == CustomHatchStyle.None)) g.FillRectangle(hb, Rect);
                    g.DrawRectangle(pen, Rect.X, Rect.Y, Rect.Width, Rect.Height);

                }
                if (IsSelected)
                {
                    g.FillRectangle(sb, Rect);
                    if (!(HatchStyl == CustomHatchStyle.None)) g.FillRectangle(hb, Rect);
                    g.DrawRectangle(p, Rect.X, Rect.Y, Rect.Width, Rect.Height);
                }
            }
        }

        private bool IsHatchStyleNone(HatchStyle hatchStyle)
        {
            // Check if the HatchStyl corresponds to CustomHatchStyle.None
            return ShapeUtil.GetCustomHatchStyle(hatchStyle) == CustomHatchStyle.None;
        }

        public override void Move(double deltaX, double deltaY)
        {
            StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y + deltaY);
            EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y + deltaY);
            GetSingleShapeBounds();
        }

        private bool IsReverseDrawing()
        {
            // Adjust StartPoint and EndPoint by zoom factor if necessary
            PointV2D adjustedStartPoint = new PointV2D(StartPoint.X / ZoomFactor, StartPoint.Y / ZoomFactor);
            PointV2D adjustedEndPoint = new PointV2D(EndPoint.X / ZoomFactor, EndPoint.Y / ZoomFactor);

            return adjustedStartPoint.X > adjustedEndPoint.X || adjustedStartPoint.Y > adjustedEndPoint.Y;
        }

        public override void Rotate(float angle)
        {
            double centerX = (StartPoint.X + EndPoint.X) / 2f;
            double centerY = (StartPoint.Y + EndPoint.Y) / 2f;

            // Convert angle to radians
            double angleRad = Math.PI * angle / 180.0;

            // Calculate the four corners of the rectangle
            PointV2D[] corners = new PointV2D[]
            {
                new PointV2D(StartPoint.X, StartPoint.Y),   // Top-left
                new PointV2D(EndPoint.X, StartPoint.Y),     // Top-right
                new PointV2D(EndPoint.X, EndPoint.Y),       // Bottom-right
                new PointV2D(StartPoint.X, EndPoint.Y)      // Bottom-left
            };

            // Rotate each corner around the center
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = ShapeUtil.RotatePointAtCenter(corners[i], centerX, centerY, angleRad);
            }

            // Update StartPoint and EndPoint based on the new rotated corners
            StartPoint = new PointV2D(corners[0].X, corners[0].Y);
            EndPoint = new PointV2D(corners[2].X, corners[2].Y); // Use bottom-right corner
            GetSingleShapeBounds(); // Update the bounds after rotation
        }

        public override void Flip(bool flipHorizontally)
        {
            //throw new NotImplementedException();
        }

        public override ShapeBase Clone()
        {
            var clonedRectangle = new RectangleShape();
            clonedRectangle.StartPoint = new PointV2D(
               StartPoint.X,
               StartPoint.Y
           );
            clonedRectangle.EndPoint = new PointV2D(
                EndPoint.X,
                EndPoint.Y
            );

            CloneCommonProperties(clonedRectangle);
            return clonedRectangle;
        }
    }
}


