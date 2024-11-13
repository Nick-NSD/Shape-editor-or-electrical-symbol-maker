using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;

namespace SymbolMaker
{
    [Serializable]
    public class ArcShape : ShapeBase
    {
        [XmlElement("ArcStartAngle")]
        public float StartAngle { get; set; } = 0;

        [XmlElement("ArcSweepAngle")]
        public float SweepAngle { get; set; } = -270;

        public ArcShape(PointV2D startPoint, PointV2D endPoint, float startAngle, float sweepAngle, Color borderPenColor, Color fillBrushColor)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            StartAngle = startAngle;
            SweepAngle = sweepAngle;
            BorderPenColor = borderPenColor;
            FillBrushColor = fillBrushColor;
            GetSingleShapeBounds();
        }

        public ArcShape()
        {
            // Parameterless constructor required for XML serialization
        }

        public override void Draw(Graphics g)
        {
            using (Pen pen = new Pen(BorderPenColor, PenThickness))
            using (SolidBrush sb = new SolidBrush(FillBrushColor))
            using (HatchBrush hb = new HatchBrush(ShapeUtil.ConvertToHatchStyle(HatchStyl), BorderPenColor, FillBrushColor))
            using (Pen p = new Pen(Color.Black, 0.15f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;
                pen.DashCap = DashCap.Flat;

                p.DashPattern = new float[] { 2, 2 };
                p.DashStyle = DashStyle.Custom;
                p.DashCap = DashCap.Round;
                p.LineJoin = LineJoin.Round;

                float[] dashPattern = ShapeUtil.GetPenDashPattern(LinStyle); // Get the pattern

                if (dashPattern == null)
                {
                    pen.DashStyle = DashStyle.Solid;
                }
                else
                {
                    pen.DashPattern = dashPattern;
                }

                if (Rect.Width > 0 && Rect.Height > 0)
                {
                    if (!IsSelected)
                    {
                        g.FillPie(sb, Rect.X, Rect.Y, Rect.Width, Rect.Height, StartAngle, SweepAngle);
                        if (!(HatchStyl == CustomHatchStyle.None)) g.FillPie(hb, Rect.X, Rect.Y, Rect.Width, Rect.Height, StartAngle, SweepAngle);
                        g.DrawArc(pen, Rect, StartAngle, SweepAngle);
                    }

                    if (IsSelected)
                    {
                        g.DrawRectangle(p, Rect.X, Rect.Y, Rect.Width, Rect.Height);
                        g.FillPie(sb, Rect.X, Rect.Y, Rect.Width, Rect.Height, StartAngle, SweepAngle);
                        if (!(HatchStyl == CustomHatchStyle.None)) g.FillPie(hb, Rect.X, Rect.Y, Rect.Width, Rect.Height, StartAngle, SweepAngle);
                        g.DrawArc(p, Rect, StartAngle, SweepAngle);
                    }
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
            // Rotate the bounding box and adjust StartAngle
            StartAngle = (StartAngle - angle) % 360;
        }

        public override void Flip(bool flipHorizontally)
        {
            if (flipHorizontally)
            {
                // Flip horizontally: Reflect the StartAngle
                StartAngle = (180 - (StartAngle + SweepAngle)) % 360;
            }
            else
            {
                // Flip vertically: Reflect the StartAngle
                StartAngle = (360 - (StartAngle + SweepAngle)) % 360;
            }
        }

        public override ShapeBase Clone()
        {
            var clonedArc = new ArcShape();

            clonedArc.StartPoint = new PointV2D(StartPoint.X, StartPoint.Y);
            clonedArc.EndPoint = new PointV2D(EndPoint.X, EndPoint.Y);
            clonedArc.Rect = new RectangleF((float)(StartPoint.X), (float)(StartPoint.Y), Rect.Width, Rect.Height);
            clonedArc.StartAngle = StartAngle;
            clonedArc.SweepAngle = SweepAngle;

            CloneCommonProperties(clonedArc);
            return clonedArc;
        }
    }
}
