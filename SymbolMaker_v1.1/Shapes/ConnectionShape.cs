using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Serialization;
using UserControls;

namespace SymbolMaker
{
    [Serializable]
    public class ConnectionShape : ShapeBase
    {
        [XmlElement("ConnectionName")]
        public TextShape ConnectionName { get; set; }

        [XmlElement("ConnectionNumber")]
        public int ConnectionNumber { get; set; }

        [XmlIgnore]
        public Font TextFont { get; set; }

        [XmlElement("ConnectionFont")]
        public FontInfo FontAsXml
        {
            get { return FontInfo.FromFont(TextFont); }
            set { TextFont = value.ToFont(); }
        }

        [XmlIgnore]
        public string Type { get; set; } // Input or Output

        [XmlElement("ConnectionLocation")]
        public PointV2D NamePosition { get; set; }

        [XmlElement("ConnectionVisibility")]
        public bool ConnectionVisible { get; set; }// = true;  // Default to true

        [XmlElement("ConnectionNameVisibility")]
        public bool ConnectionNameVisible { get; set; }

        [XmlIgnore]
        public override double ZoomFactor
        {
            get => base.ZoomFactor;
            set
            {
                base.ZoomFactor = value;
                ApplyZoomToConnectionName();
            }
        }

        public ConnectionShape(int connectionNumber, PointV2D startPoint, string name, TextAlignment textAlign, TextRotation textRot, Font textFont, Color fontColor, PointV2D namePosition, string type)
        {
            ConnectionNumber = connectionNumber;
            StartPoint = startPoint;
            EndPoint = startPoint;
            TextFont = textFont;
            NamePosition = namePosition;
            Type = type;
            ConnectionName = new TextShape(namePosition, 1, name, textAlign, textRot, textFont, fontColor);
            GetSingleShapeBounds();
            ConnectionName.GetSingleShapeBounds();
        }

        public ConnectionShape()
        {
            ZoomFactorChanged += (sender, e) => ApplyZoomToConnectionName();
        }

        private void ApplyZoomToConnectionName()
        {
            ConnectionName.ZoomFactor = ZoomFactor;
            //ConnectionName.ViewportOffsetX = ViewportOffsetX;
            //ConnectionName.ViewportOffsetY = ViewportOffsetY;
        }

        public override void Draw(Graphics g)
        {
            if (!ConnectionVisible)
            {
                return;  // Skip drawing red dot if not visible
            }

            // The name is draw only at Creation connection or when the symbol is break
            if (ConnectionName.TextVisible && ConnectionName.ShapeID == null) ConnectionName.Draw(g);

            if (!IsSelected)
            {
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(130, Color.Red)))
                {
                    g.FillEllipse(sb, (float)StartPoint.X - 4, (float)StartPoint.Y - 4, 8, 8);
                }
            }
            if (IsSelected)
            {
                using (Pen p = new Pen(Color.Black, 0.15f))
                {
                    g.DrawRectangle(p, (float)StartPoint.X - 4, (float)StartPoint.Y - 4, 8, 8);
                }
            }
        }

        public override void GetSingleShapeBounds()
        {
            Rect = new RectangleF((float)StartPoint.X - 4, (float)StartPoint.Y - 4, 8, 8);
        }

        public override void Move(double deltaX, double deltaY)
        {
            // Move the connection's point
            StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y + deltaY);
            EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y + deltaY);
            GetSingleShapeBounds(); // Update the connection's rectangle

            // Move the connection name (TextShape)
            if (ConnectionName != null)
            {
                ConnectionName.Move(deltaX, deltaY);
            }
            ConnectionName.GetSingleShapeBounds(); // Update the name's bounding rectangle
        }

        // Method to detect if the mouse is over the connection name
        public bool IsMouseOverName(PointV2D mousePoint)
        {
            return ConnectionName != null && ConnectionName.IsMouseInsideShape(mousePoint);
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
            var clonedConnectionShape = new ConnectionShape();

            // Copy common properties
            CloneCommonProperties(clonedConnectionShape);

            // Copy specific properties
            clonedConnectionShape.ConnectionNumber = ConnectionNumber;
            clonedConnectionShape.TextFont = new Font(TextFont.FontFamily, TextFont.Size, TextFont.Style);
            clonedConnectionShape.Type = Type;
            clonedConnectionShape.NamePosition = new PointV2D(NamePosition.X, NamePosition.Y);
            clonedConnectionShape.ConnectionVisible = ConnectionVisible;
            clonedConnectionShape.ConnectionNameVisible = ConnectionNameVisible;
            clonedConnectionShape.ConnectionName = ConnectionName.Clone() as TextShape;
            clonedConnectionShape.StartPoint = new PointV2D(StartPoint.X, StartPoint.Y);
            clonedConnectionShape.Rect = new RectangleF((float)(StartPoint.X - 4), (float)(StartPoint.Y - 4), Rect.Width, Rect.Height);
            clonedConnectionShape.ConnectionName.ShapeID = ShapeID;
            clonedConnectionShape.GetSingleShapeBounds();
            clonedConnectionShape.ConnectionName.GetSingleShapeBounds();
            return clonedConnectionShape;
        }
    }
}

