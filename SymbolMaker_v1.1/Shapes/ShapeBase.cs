using System;
using System.Drawing;
using System.Xml.Serialization;

namespace SymbolMaker
{
    [XmlInclude(typeof(ArcShape))]
    [XmlInclude(typeof(ConnectionShape))]
    [XmlInclude(typeof(EllipseShape))]
    [XmlInclude(typeof(LineShape))]
    [XmlInclude(typeof(PolygonShape))]
    [XmlInclude(typeof(RectangleShape))]
    [XmlInclude(typeof(SymbolShape))]
    [XmlInclude(typeof(TextShape))]
    [XmlInclude(typeof(DotShape))]

    public abstract class ShapeBase
    {
        public enum ShapeEdge
        {
            None,
            Top,
            Bottom,
            Left,
            Right,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public enum RectangleDrawingDirection
        {
            TopLeftToBottomRight,
            BottomLeftToTopRight,
            TopRightToBottomLeft,
            BottomRightToTopLeft
        }

        public enum LineStyle
        {
            Solid,
            Custom1,
            Custom2,
            Custom3,
            Custom4,
            Custom5
        }

        public enum CustomHatchStyle
        {
            None,
            Horizontal,
            Vertical,
            Cross,
            Diagonal,
            BackDiagonal,
            DiagCross,
            CheckerBoard
        }

        [XmlElement("ShapeName")]
        public string ShapeName { get; set; }

        [XmlElement("ShapeID")]
        public string ShapeID { get; set; }

        [XmlElement("Rectangle")]
        public RectangleF Rect { get; set; }

        [XmlElement("StartPoint")]
        public PointV2D StartPoint { get; set; } = new PointV2D();

        [XmlElement("EndPoint")]
        public PointV2D EndPoint { get; set; } = new PointV2D();


        private double zoomFactor;
        private double viewportOffsetX;//unused
        private double viewportOffsetY;//unused

        [XmlIgnore]
        public virtual double ZoomFactor
        {
            get => zoomFactor;
            set
            {
                zoomFactor = value;
                OnZoomFactorChanged();  // Trigger event when ZoomFactor is modified
            }
        }
        [XmlIgnore]
        public double ViewportOffsetX
        {
            get => viewportOffsetX;
            set => viewportOffsetX = value;
        }
        [XmlIgnore]
        public double ViewportOffsetY
        {
            get => viewportOffsetY;
            set => viewportOffsetY = value;
        }

        public double Width { get; set; }
        public double Height { get; set; }

        [XmlElement("PenThickness")]
        public float PenThickness { get; set; } = 0.94881f;//=0.25mm

        [XmlElement("PenLineStyle")]
        public LineStyle LinStyle { get; set; } = LineStyle.Solid;

        [XmlElement("HatchStyle")]
        public CustomHatchStyle HatchStyl { get; set; } = CustomHatchStyle.None;

        [XmlIgnore]
        public Color BorderPenColor { get; set; } = Color.Black;

        [XmlElement("ShapeBorderColor")]
        public int BorderPenColorAsArgb
        {
            get { return BorderPenColor.ToArgb(); }
            set { BorderPenColor = Color.FromArgb(value); }
        }

        [XmlIgnore]
        public Color FillBrushColor { get; set; } = Color.Transparent;

        [XmlElement("ShapeFillColor")]
        public int FillBrushColorAsArgb
        {
            get { return FillBrushColor.ToArgb(); }
            set { FillBrushColor = Color.FromArgb(value); }
        }

        [XmlIgnore]
        public bool IsSelected { get; set; }

        public override string ToString()
        {
            return ShapeName ?? " Unnamed Shape";
        }

        protected void CloneCommonProperties(ShapeBase clone)
        {
            clone.BorderPenColor = BorderPenColor;
            clone.FillBrushColor = FillBrushColor;
            clone.PenThickness = PenThickness;
            clone.LinStyle = LinStyle;
            clone.HatchStyl = HatchStyl;
            clone.ShapeID = ShapeID;
            clone.ShapeName = ShapeName;
            clone.GetSingleShapeBounds();
        }

        public abstract ShapeBase Clone();

        // Event for ZoomFactor change
        public event EventHandler ZoomFactorChanged;

        // Method to raise the event. Change zoom factor for internalshapes for symbol
        // and for connection name text
        protected virtual void OnZoomFactorChanged()
        {
            ZoomFactorChanged?.Invoke(this, EventArgs.Empty);
        }

        public abstract void Draw(Graphics g);
        public abstract void Move(double deltaX, double deltaY);
        public abstract void Flip(bool flipHorizontally);
        public abstract void Rotate(float angle);

        public virtual void Resize(double deltaX, double deltaY, ShapeEdge edge)
        {
            RectangleDrawingDirection direction = GetDrawingDirection();
            var ReturnedPoints = ShapeUtil.ResizeNormal(deltaX, deltaY, edge, StartPoint, EndPoint);
            switch (direction)
            {
                case RectangleDrawingDirection.TopLeftToBottomRight:
                    ReturnedPoints = ShapeUtil.ResizeNormal(deltaX, deltaY, edge, StartPoint, EndPoint);
                    break;
                case RectangleDrawingDirection.BottomLeftToTopRight:
                    ReturnedPoints = ShapeUtil.ResizeBottomLeftToTopRight(deltaX, deltaY, edge, StartPoint, EndPoint);
                    break;
                case RectangleDrawingDirection.TopRightToBottomLeft:
                    ReturnedPoints = ShapeUtil.ResizeTopRightToBottomLeft(deltaX, deltaY, edge, StartPoint, EndPoint);
                    break;
                case RectangleDrawingDirection.BottomRightToTopLeft:
                    ReturnedPoints = ShapeUtil.ResizeBottomRightToTopLeft(deltaX, deltaY, edge, StartPoint, EndPoint);
                    break;
            }
            StartPoint = ReturnedPoints[0];
            EndPoint = ReturnedPoints[1];
            GetSingleShapeBounds();
        }

        private RectangleDrawingDirection GetDrawingDirection()
        {
            if (StartPoint.X < EndPoint.X && StartPoint.Y < EndPoint.Y)
                return RectangleDrawingDirection.TopLeftToBottomRight;
            else if (StartPoint.X < EndPoint.X && StartPoint.Y > EndPoint.Y)
                return RectangleDrawingDirection.BottomLeftToTopRight;
            else if (StartPoint.X > EndPoint.X && StartPoint.Y < EndPoint.Y)
                return RectangleDrawingDirection.TopRightToBottomLeft;
            else
                return RectangleDrawingDirection.BottomRightToTopLeft;
        }

        public virtual void Resize(PointV2D newStartPoint, PointV2D newEndPoint) { }//for LineShape

        public virtual ShapeEdge GetEdgeUnderMouse(PointV2D mousePoint, int edgeTolerance)
        {
            PointV2D transformedMousePoint = mousePoint / ZoomFactor;

            // Ensure the edge tolerance is scaled appropriately
            int adjustedEdgeTolerance = Math.Max((int)(edgeTolerance / ZoomFactor), 3);

            // Call the utility method with transformed points
            return ShapeUtil.GetEdgeUnderMouse(StartPoint, EndPoint, transformedMousePoint, adjustedEdgeTolerance);
        }

        public virtual bool IsMouseNearEdge(PointV2D mousePoint, int edgeTolerance)
        {
            PointV2D adjustedPointF = new PointV2D(mousePoint.X / ZoomFactor, mousePoint.Y / ZoomFactor);

            // Adjust the edge tolerance for the zoom factor, ensuring it doesn't go below a minimum threshold
            int adjustedEdgeTolerance = Math.Max((int)(edgeTolerance / ZoomFactor), 3); // Set a minimum tolerance of 3

            return ShapeUtil.IsMouseNearEdge(Rectangle.Round(Rect), adjustedPointF, adjustedEdgeTolerance);

        }

        public virtual bool IsMouseInsideShape(PointV2D mousePoint)
        {
            // Adjust the mouse point based on the current zoom factor
            PointV2D adjustedPointF = new PointV2D(mousePoint.X / ZoomFactor, mousePoint.Y / ZoomFactor);

            //Point adjustedPoint = Point.Round(adjustedPointF);

            // Check if the adjusted point is inside the original unscaled rectangle
            return Rectangle.Round(Rect).Contains(adjustedPointF);
        }

        public virtual bool IsMouseOnEndsOfLine(PointV2D mousePoint, int edgeTolerance) { return false; }//for LineShape

        public virtual bool IsMouseNearLineBody(PointV2D mousePoint, int tolerance) { return false; }//for LineShape

        public virtual void GetSingleShapeBounds()
        {
            // Normalize the StartPoint and EndPoint to get the actual bounds of the rectangle
            float left = Math.Min(StartPoint.ToPointF.X, EndPoint.ToPointF.X);
            float top = Math.Min(StartPoint.ToPointF.Y, EndPoint.ToPointF.Y);
            float right = Math.Max(StartPoint.ToPointF.X, EndPoint.ToPointF.X);
            float bottom = Math.Max(StartPoint.ToPointF.Y, EndPoint.ToPointF.Y);

            // Set the rectangle bounds using RectangleF
            Rect = new RectangleF(left, top, right - left, bottom - top);
        }
    }
}
