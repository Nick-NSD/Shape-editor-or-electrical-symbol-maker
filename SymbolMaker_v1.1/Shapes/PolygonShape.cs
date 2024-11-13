using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml.Serialization;

namespace SymbolMaker
{
    [Serializable]
    public class PolygonShape : ShapeBase
    {
        [XmlElement("PolygonVertices")]
        public List<PointV2D> Vertices = new List<PointV2D>();

        public PolygonShape(List<PointV2D> points, Color borderPenColor, Color fillBrushColor)
        {
            // Make a copy of the vertices list to avoid modifying the original list
            Vertices = new List<PointV2D>(points);
            BorderPenColor = borderPenColor;
            FillBrushColor = fillBrushColor;
            GetSingleShapeBounds();
        }

        public PolygonShape()
        {
            // Parameterless constructor required for XML serialization
        }

        public override void Draw(Graphics g)
        {
            if (Vertices.Count > 1)
            {
                PointF[] pointFArray = Vertices.Select(v => new PointF((float)v.X, (float)v.Y)).ToArray();

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
                    p.DashCap = DashCap.Round;
                    p.DashStyle = DashStyle.Custom;
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

                    if (!IsSelected)
                    {
                        g.FillPolygon(sb, pointFArray);
                        if (!(HatchStyl == CustomHatchStyle.None)) g.FillPolygon(hb, pointFArray);
                        g.DrawPolygon(pen, pointFArray);
                    }
                    if (IsSelected)
                    {
                        g.FillPolygon(sb, pointFArray);
                        if (!(HatchStyl == CustomHatchStyle.None)) g.FillPolygon(hb, pointFArray);
                        g.DrawPolygon(p, pointFArray);
                    }
                }
            }
        }

        public override void Move(double deltaX, double deltaY)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new PointV2D(Vertices[i].X + deltaX, Vertices[i].Y + deltaY);
            }
            GetSingleShapeBounds();  // Update the bounding rectangle after moving
        }

        // Move a specific vertex
        public void MoveVertex(int index, PointV2D newPosition)
        {
            if (index >= 0 && index < Vertices.Count)
            {
                Vertices[index] = newPosition;
                GetSingleShapeBounds();  // Recalculate bounds if necessary
            }
        }

        public override void GetSingleShapeBounds()
        {
            if (Vertices.Count == 0)
            {
                Rect = Rectangle.Empty;
                return;
            }

            double left = Vertices.Min(p => p.X);
            double top = Vertices.Min(p => p.Y);
            double right = Vertices.Max(p => p.X);
            double bottom = Vertices.Max(p => p.Y);

            Rect = new RectangleF((float)left, (float)top, (float)(right - left), (float)(bottom - top));
        }

        public override bool IsMouseInsideShape(PointV2D mousePoint)
        {
            // Adjust the mouse point based on the current zoom factor
            PointV2D adjustedPointF = new PointV2D(mousePoint.X / ZoomFactor, mousePoint.Y / ZoomFactor);

            // Adjust the vertices of the polygon for zoom
            List<PointV2D> adjustedVertices = Vertices
                .Select(v => new PointV2D(v.X / ZoomFactor, v.Y / ZoomFactor))
                .ToList();

            // Check if the adjusted point is inside the polygon
            return IsPointInPolygon(adjustedPointF, adjustedVertices);
        }

        private bool IsPointInPolygon(PointV2D point, List<PointV2D> polygon)
        {
            bool result = false;
            int j = polygon.Count - 1;
            for (int i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].Y < point.Y && polygon[j].Y >= point.Y || polygon[j].Y < point.Y && polygon[i].Y >= point.Y)
                {
                    if (polygon[i].X + (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < point.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public override bool IsMouseNearEdge(PointV2D mousePosition, int tolerance)
        {
            // Adjust the edge tolerance for the zoom factor, ensuring it doesn't go below a minimum threshold
            int adjustedTolerance = Math.Max((int)(tolerance / ZoomFactor), 3); // Set a minimum tolerance of 3

            // Convert the mouse position to match the zoomed world space
            PointV2D adjustedMousePointF = new PointV2D(mousePosition.X / ZoomFactor, mousePosition.Y / ZoomFactor);

            // Check if the adjusted mouse is near any of the polygon's edges
            for (int i = 0; i < Vertices.Count; i++)
            {
                PointV2D p1 = Vertices[i];
                PointV2D p2 = Vertices[(i + 1) % Vertices.Count];  // Wrap around to the first point
                if (IsPointNearLine(adjustedMousePointF, p1, p2, adjustedTolerance))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsPointNearLine(PointV2D point, PointV2D lineStart, PointV2D lineEnd, int tolerance)
        {
            // Calculate distance from point to the line segment
            double distance = DistanceFromPointToLine(point, lineStart, lineEnd);
            return distance <= tolerance;
        }

        private double DistanceFromPointToLine(PointV2D point, PointV2D lineStart, PointV2D lineEnd)
        {
            double a = point.X - lineStart.X;
            double b = point.Y - lineStart.Y;
            double c = lineEnd.X - lineStart.X;
            double d = lineEnd.Y - lineStart.Y;

            double dot = a * c + b * d;
            double len_sq = c * c + d * d;
            double param = (len_sq != 0) ? dot / len_sq : -1;

            double xx, yy;

            if (param < 0)
            {
                xx = lineStart.X;
                yy = lineStart.Y;
            }
            else if (param > 1)
            {
                xx = lineEnd.X;
                yy = lineEnd.Y;
            }
            else
            {
                xx = lineStart.X + param * c;
                yy = lineStart.Y + param * d;
            }

            double dx = point.X - xx;
            double dy = point.Y - yy;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public bool IsMouseNearVertex(PointV2D mousePosition, int tolerance, out int vertexIndex)
        {
            // Adjust the mouse point and tolerance based on the current zoom factor
            PointV2D adjustedMousePointF = new PointV2D(mousePosition.X / ZoomFactor, mousePosition.Y / ZoomFactor);
            // Adjust the edge tolerance for the zoom factor, ensuring it doesn't go below a minimum threshold
            int adjustedTolerance = Math.Max((int)(tolerance / ZoomFactor), 3); // Set a minimum tolerance of 3

            // Detect if the mouse is near any vertex
            for (int i = 0; i < Vertices.Count; i++)
            {
                var vertex = Vertices[i];
                double distance = Math.Sqrt(Math.Pow(vertex.X - adjustedMousePointF.X, 2) + Math.Pow(vertex.Y - adjustedMousePointF.Y, 2));
                if (distance <= adjustedTolerance)
                {
                    vertexIndex = i;
                    return true;
                }
            }
            vertexIndex = -1;
            return false;
        }

        public override void Flip(bool flipHorizontally)
        {
            // Your polygon flip logic here
            float centerX = Rect.X + Rect.Width / 2f;
            float centerY = Rect.Y + Rect.Height / 2f;

            for (int i = 0; i < Vertices.Count; i++)
            {
                PointV2D vertex = Vertices[i];
                if (flipHorizontally)
                {
                    double newX = 2 * centerX - vertex.X;
                    Vertices[i] = new PointV2D(newX, vertex.Y);
                }
                else
                {
                    double newY = 2 * centerY - vertex.Y;
                    Vertices[i] = new PointV2D(vertex.X, newY);
                }
            }
            GetSingleShapeBounds();
        }

        public override void Rotate(float angle)
        {
            float centerX = Rect.X + Rect.Width / 2f;
            float centerY = Rect.Y + Rect.Height / 2f;
            double angleRad = Math.PI * angle / 180.0;

            for (int i = 0; i < Vertices.Count; i++)
            {
                PointV2D vertex = Vertices[i];
                double rotatedX = centerX + (vertex.X - centerX) * Math.Cos(angleRad) - (vertex.Y - centerY) * Math.Sin(angleRad);
                double rotatedY = centerY + (vertex.X - centerX) * Math.Sin(angleRad) + (vertex.Y - centerY) * Math.Cos(angleRad);
                Vertices[i] = new PointV2D(rotatedX, rotatedY);
            }
            GetSingleShapeBounds();
        }

        public override ShapeBase Clone()
        {
            var clonedPolygon = new PolygonShape();
            clonedPolygon.Vertices = new List<PointV2D>();
            foreach (var vertex in Vertices)
            {
                clonedPolygon.Vertices.Add(new PointV2D(vertex.X, vertex.Y));
            }
            CloneCommonProperties(clonedPolygon);
            return clonedPolygon;
        }
    }
}

