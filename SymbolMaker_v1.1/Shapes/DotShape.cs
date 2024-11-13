using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SymbolMaker
{
    [Serializable]
    public class DotShape : ShapeBase
    {
        [XmlElement("DotRadius")]
        public float DotRadius { get; set; } = 3.779527559f; //3.779527559px = 1mm at 96dpi

        [XmlIgnore]
        public Color DotColor { get; set; } = Color.Black;

        [XmlElement("DotColor")]
        public int DotColorAsArgb
        {
            get { return DotColor.ToArgb(); }
            set { DotColor = Color.FromArgb(value); }
        }

        public DotShape(PointV2D startPoint)
        {
            StartPoint = startPoint;
            EndPoint = startPoint;
            GetSingleShapeBounds();
        }

        public DotShape()
        {
            // Parameterless constructor required for XML serialization
        }

        public override void Draw(Graphics g)
        {
            if (!IsSelected)
            {
                using (SolidBrush sb = new SolidBrush(DotColor))
                {
                    g.FillEllipse(sb, (float)StartPoint.X - DotRadius, (float)StartPoint.Y - DotRadius, DotRadius + DotRadius, DotRadius + DotRadius);

                }
            }
            if (IsSelected)
            {
                using (Pen p = new Pen(Color.Gray, 0.15f))
                //using (SolidBrush selsb = new SolidBrush(Color.Silver))
                {
                    g.DrawRectangle(p, (float)StartPoint.X - DotRadius, (float)StartPoint.Y - DotRadius, DotRadius + DotRadius, DotRadius + DotRadius);
                    //g.FillEllipse(selsb, (float)StartPoint.X - DotRadius, (float)StartPoint.Y - DotRadius, DotRadius + DotRadius, DotRadius + DotRadius);
                }
            }
        }

        public override void GetSingleShapeBounds()
        {
            Rect = new RectangleF((float)StartPoint.X - DotRadius, (float)StartPoint.Y - DotRadius, DotRadius + DotRadius, DotRadius + DotRadius);
        }

        public override void Move(double deltaX, double deltaY)
        {
            // Move the connection's point
            StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y + deltaY);
            EndPoint = StartPoint;
            GetSingleShapeBounds(); // Update the connection's rectangle
        }

        public override void Rotate(float angle)
        {
            //throw new NotImplementedException();
        }

        public override void Flip(bool flipHorizontally)
        {
            //throw new NotImplementedException();
        }

        public override ShapeBase Clone()
        {
            var clonedDotShape = new DotShape();

            clonedDotShape.StartPoint = new PointV2D(StartPoint.X, StartPoint.Y);
            clonedDotShape.Rect = new RectangleF((float)(StartPoint.X - DotRadius), (float)(StartPoint.Y - DotRadius), Rect.Width, Rect.Height);
            clonedDotShape.DotRadius = DotRadius;
            clonedDotShape.DotColor = DotColor;
            clonedDotShape.GetSingleShapeBounds();
            return clonedDotShape;
        }
    }
}
