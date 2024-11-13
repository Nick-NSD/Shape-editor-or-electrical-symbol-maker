using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using UserControls;

namespace SymbolMaker
{
    [Serializable]
    public class TextShape : ShapeBase
    {
        [XmlElement("TextShapeText")]
        public string StringText { get; set; }

        [XmlElement("TextVisibility")]
        public bool TextVisible { get; set; } = true;

        [XmlElement("TextShapeType")]
        public int TextType { get; set; }//0 = Text, 1 = Connection name, 2 = Symbol name

        [XmlIgnore]
        public Font TextFont { get; set; }

        [XmlElement("TextShapeFont")]
        public FontInfo FontAsXml
        {
            get { return FontInfo.FromFont(TextFont); }
            set { TextFont = value.ToFont(); }
        }

        [XmlIgnore]
        public Color TextColor { get; set; }

        [XmlElement("TextShapeColor")]
        public int BrushColorAsArgb
        {
            get { return TextColor.ToArgb(); }
            set { TextColor = Color.FromArgb(value); }
        }

        [XmlElement("TextShapeAlignment")]
        public TextAlignment TextShapeAlign { get; set; }

        [XmlElement("TextShapeRotation")]
        public TextRotation TextShapeRotation { get; set; }

        public RectangleF RotatedRectangle;// { get; set; }

        private PointV2D PivotPoint;
        private PointV2D[] corners;
        private int angle = 0;

        public TextShape(PointV2D startPoint, int type, string text, TextAlignment textAlignment, TextRotation textRotation, Font font, Color textColor)
        {
            StartPoint = startPoint;
            TextType = type;
            StringText = text;
            TextShapeAlign = textAlignment;
            TextShapeRotation = textRotation;
            TextFont = font;
            TextColor = textColor;
            GetSingleShapeBounds();
            PivotPoint = new PointV2D(StartPoint.X, StartPoint.Y);
        }

        public TextShape()
        {
            // Parameterless constructor required for XML serialization
        }

        public override void Draw(Graphics g)
        {
            using (SolidBrush sb = new SolidBrush(TextColor))
            using (Pen p = new Pen(Color.Black, 0.15f))//pen for highlight (draw bounding box)
            using (StringFormat format = new StringFormat(StringFormatFlags.NoClip))
            {
                p.DashPattern = new float[] { 2, 2 };
                p.DashStyle = DashStyle.Custom;
                p.DashCap = DashCap.Round;
                p.LineJoin = LineJoin.Round;

                switch (TextShapeAlign)
                {
                    case TextAlignment.LeftAlign:
                        format.Alignment = StringAlignment.Near;
                        format.LineAlignment = StringAlignment.Near;
                        break;
                    case TextAlignment.CenterAlign:
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Near;
                        break;
                    case TextAlignment.RightAlign:
                        format.Alignment = StringAlignment.Far;
                        format.LineAlignment = StringAlignment.Near;
                        break;
                }

                switch (TextShapeRotation)
                {
                    case TextRotation.RotationZero://no rotation. Only draw string
                        angle = 0;
                        g.DrawString(StringText, TextFont, sb, (float)StartPoint.X, (float)StartPoint.Y, format);
                        GetSingleShapeBounds();
                        RotatedRectangle = new RectangleF(Rect.X, Rect.Y, Rect.Width, Rect.Height);
                        break;

                    case TextRotation.Rotation90://90 deg trigonometric
                        angle = -90;
                        RotateText(angle, g, p, StringText, TextFont, sb, format);
                        break;

                    case TextRotation.Rotation270://270 deg trigonometric
                        angle = 90;
                        RotateText(angle, g, p, StringText, TextFont, sb, format);
                        break;
                }

                // Draw bounding box if selected
                if (IsSelected)
                {
                    if (ShapeID == null)//for simple TextShapes
                    {
                        g.DrawRectangle(p, Rect.X, Rect.Y, Rect.Width, Rect.Height);
                        // Draw pivot point if selected - Optional
                        //g.FillEllipse(Brushes.Black, (float)StartPoint.X - 0.5f, (float)StartPoint.Y - 0.5f, 1f, 1f);
                    }
                }
            }
        }

        private void RotateText(int angle, Graphics g, Pen p, string StringText, Font TextFont, SolidBrush sb, StringFormat format)
        {
            GetSingleShapeBounds();//returns horizontal bounding box
            GraphicsState state = g.Save();

            // Translate to the pivot point based on the alignment
            // and get the rotated rectangle bounding box
            switch (TextShapeAlign)
            {
                case TextAlignment.LeftAlign:
                    PivotPoint = new PointV2D(StartPoint.X, StartPoint.Y);
                    g.TranslateTransform((float)StartPoint.X, (float)StartPoint.Y);
                    break;
                case TextAlignment.CenterAlign:
                    if (angle == 0) PivotPoint = new PointV2D(StartPoint.X + Rect.Width / 2, StartPoint.Y + Rect.Height / 2);
                    else
                    {
                        PivotPoint = new PointV2D(Rect.Left + Rect.Width / 2, Rect.Top);
                    }
                    g.TranslateTransform((float)PivotPoint.X, (float)PivotPoint.Y);
                    break;
                case TextAlignment.RightAlign:
                    PivotPoint = new PointV2D(StartPoint.X, StartPoint.Y);
                    g.TranslateTransform((float)StartPoint.X, (float)StartPoint.Y);
                    break;
            }

            // Apply rotation and draw the text
            g.RotateTransform(angle);
            g.DrawString(StringText, TextFont, sb, 0, 0, format);

            g.Restore(state);

            // Calculate and store the rotated bounding box for detecting
            corners = ShapeUtil.GetRotatedRectangleCorners(Rectangle.Round(Rect), PivotPoint, angle);
            Rect = ShapeUtil.GetBoundingBox(corners);
            RotatedRectangle = new RectangleF(Rect.X, Rect.Y, Rect.Width, Rect.Height);
        }

        public override void Move(double deltaX, double deltaY)
        {
            StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y + deltaY);
            EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y + deltaY);
            // Avoid to draw unrotated rectangle returned by GetSingleShapeBounds() when angle != 0
            if (angle == 0) GetSingleShapeBounds();
        }

        public override void GetSingleShapeBounds()
        {
            // This gets always horizontal rectangle
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            using (StringFormat format = new StringFormat(StringFormatFlags.NoClip))
            {
                // Measure the size of the text
                SizeF size = g.MeasureString(StringText, TextFont);

                // Here I set explicitly EndPoint to avoid null ref exception
                EndPoint = new PointV2D(StartPoint.X + size.Width, StartPoint.Y + size.Height);

                // Create a rectangle for the text bounding box based on the alignment
                switch (TextShapeAlign)
                {
                    case TextAlignment.LeftAlign:
                        Rect = new RectangleF((float)StartPoint.X, (float)StartPoint.Y, size.Width, size.Height);
                        break;
                    case TextAlignment.CenterAlign:
                        Rect = new RectangleF((float)StartPoint.X - size.Width / 2, (float)StartPoint.Y, size.Width, size.Height);
                        break;
                    case TextAlignment.RightAlign:
                        Rect = new RectangleF((float)StartPoint.X - size.Width, (float)StartPoint.Y, size.Width, size.Height);
                        break;
                    default:
                        Rect = new RectangleF((float)StartPoint.X, (float)StartPoint.Y, size.Width, size.Height);
                        break;
                }
            }
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
            var clonedText = new TextShape();

            clonedText.ShapeID = ShapeID;
            clonedText.StringText = StringText;
            clonedText.TextVisible = TextVisible;
            clonedText.TextType = TextType;
            clonedText.TextFont = new Font(TextFont.FontFamily, TextFont.Size, TextFont.Style);
            clonedText.TextColor = TextColor;
            clonedText.TextShapeAlign = TextShapeAlign;
            clonedText.TextShapeRotation = TextShapeRotation;
            clonedText.StartPoint = new PointV2D(
              StartPoint.X,
              StartPoint.Y
          );

            clonedText.GetSingleShapeBounds();
            return clonedText;
        }
    }
}
