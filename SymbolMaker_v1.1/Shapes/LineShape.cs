using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SymbolMaker
{
    [Serializable]
    public class LineShape : ShapeBase
    {
        public LineShape(PointV2D startPoint, PointV2D endPoint, Color borderPenColor)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            BorderPenColor = borderPenColor;
            GetSingleShapeBounds();
        }

        public LineShape()
        {
            // Parameterless constructor required for XML serialization
        }

        public override void Draw(Graphics g)
        {
            using (Pen pen = new Pen(BorderPenColor, PenThickness))
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
                    pen.DashStyle = DashStyle.Solid; // Solid line if no custom dash pattern
                }
                else
                {
                    pen.DashPattern = dashPattern; // Apply the custom dash pattern
                }

                if (!IsSelected)
                {
                    // Here we need to convert PointV2D to System PointF in order to draw without any offset
                    g.DrawLine(pen, StartPoint.ToPointF, EndPoint.ToPointF);
                }
                if (IsSelected)
                {
                    // Here we need to convert PointV2D to System PointF in order to draw without any offset
                    g.DrawLine(p, StartPoint.ToPointF, EndPoint.ToPointF);
                }
            }
        }

        public override void Move(double deltaX, double deltaY)
        {
            StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y + deltaY);
            EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y + deltaY);
            GetSingleShapeBounds();
        }

        public override void Resize(PointV2D newStartPoint, PointV2D newEndPoint)
        {
            StartPoint = newStartPoint;
            EndPoint = newEndPoint;
            GetSingleShapeBounds();
        }

        public override bool IsMouseOnEndsOfLine(PointV2D mousePoint, int tolerance)
        {
            // Check if the adjusted mouse point is near the start or end points
            if (IsPointNear(mousePoint, StartPoint, tolerance))
            {
                return true; // Mouse is near the start point
            }

            if (IsPointNear(mousePoint, EndPoint, tolerance))
            {
                return true; // Mouse is near the end point
            }

            return false; // Mouse is not near any ends of the line
        }

        public bool IsPointNear(PointV2D p1, PointV2D p2, int tolerance)
        {
            PointV2D mp = new PointV2D(p1.X / ZoomFactor, p1.Y / ZoomFactor);
            int adjustedTolerance = Math.Max((int)(tolerance / ZoomFactor), 5);

            return Math.Abs(mp.X - p2.X) <= adjustedTolerance && Math.Abs(mp.Y - p2.Y) <= adjustedTolerance;
        }

        public bool IsMouseNearLine(PointV2D mousePoint, PointV2D lineStart, PointV2D lineEnd, int tolerance)
        {
            // Adjust mousePoint, lineStart, and lineEnd by zoom factor
            PointV2D adjustedMousePoint = new PointV2D(mousePoint.X / ZoomFactor, mousePoint.Y / ZoomFactor);

            // Adjust the edge tolerance for the zoom factor, ensuring it doesn't go below a minimum threshold
            int adjustedTolerance = Math.Max((int)(tolerance / ZoomFactor), 3); // Set a minimum tolerance of 3

            // Calculate the distance from the point to the line
            double distance = DistanceFromPointToLine(adjustedMousePoint, lineStart, lineEnd);
            return distance <= adjustedTolerance;
        }

        private double DistanceFromPointToLine(PointV2D point, PointV2D lineStart, PointV2D lineEnd)
        {
            double A = point.X - lineStart.X;
            double B = point.Y - lineStart.Y;
            double C = lineEnd.X - lineStart.X;
            double D = lineEnd.Y - lineStart.Y;

            double dot = A * C + B * D;
            double len_sq = C * C + D * D;
            double param = dot / len_sq;

            double xx, yy;

            if (param < 0 || (lineStart.X == lineEnd.X && lineStart.Y == lineEnd.Y))
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
                xx = lineStart.X + param * C;
                yy = lineStart.Y + param * D;
            }

            double dx = point.X - xx;
            double dy = point.Y - yy;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public override bool IsMouseNearLineBody(PointV2D mousePoint, int tolerance)
        {
            // Adjust the mouse point and tolerance for the zoom factor
            PointV2D adjustedMousePoint = new PointV2D(mousePoint.X / ZoomFactor, mousePoint.Y / ZoomFactor);
            int adjustedTolerance = Math.Max((int)(tolerance / ZoomFactor), 3); // Set a minimum tolerance of 3

            // Check if the mouse is near the body of the line
            return DistanceFromPointToLineSegment(adjustedMousePoint, StartPoint, EndPoint) <= adjustedTolerance;
        }

        private double DistanceFromPointToLineSegment(PointV2D point, PointV2D lineStart, PointV2D lineEnd)
        {
            // Line vector (end - start)
            double lineDX = lineEnd.X - lineStart.X;
            double lineDY = lineEnd.Y - lineStart.Y;

            // Vector from start to the point
            double pointDX = point.X - lineStart.X;
            double pointDY = point.Y - lineStart.Y;

            // Project point onto the line, scaling the projection parameter by the length of the line segment
            double lineLengthSquared = lineDX * lineDX + lineDY * lineDY;
            double t = (pointDX * lineDX + pointDY * lineDY) / lineLengthSquared;

            // Ensure t is within the range [0,1] to find the closest point on the line segment
            t = Math.Max(0, Math.Min(1, t));

            // Find the closest point on the line segment
            double closestX = lineStart.X + t * lineDX;
            double closestY = lineStart.Y + t * lineDY;

            // Return the distance from the mouse point to the closest point on the line
            double distance = Math.Sqrt((point.X - closestX) * (point.X - closestX) + (point.Y - closestY) * (point.Y - closestY));
            return distance;
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
            var clonedLine = new LineShape();

            clonedLine.StartPoint = new PointV2D(StartPoint.X, StartPoint.Y);
            clonedLine.EndPoint = new PointV2D(EndPoint.X, EndPoint.Y);

            clonedLine.Rect = new RectangleF(
                  (float)(StartPoint.X),
                  (float)(StartPoint.Y),
                  Rect.Width,
                  Rect.Height
              );
            CloneCommonProperties(clonedLine);
            return clonedLine;
        }
    }
}
