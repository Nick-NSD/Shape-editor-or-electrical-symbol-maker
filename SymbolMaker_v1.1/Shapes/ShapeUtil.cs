using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using static SymbolMaker.ShapeBase;

namespace SymbolMaker
{
    public class ShapeUtil
    {
        public static bool IsMouseNearEdge(Rectangle Rect, PointV2D mousePoint, int edgeTolerance)
        {
            return
                IsMouseNearLine(mousePoint, new PointV2D(Rect.Left, Rect.Top), new PointV2D(Rect.Right, Rect.Top), edgeTolerance) || // Top edge
                IsMouseNearLine(mousePoint, new PointV2D(Rect.Left, Rect.Bottom), new PointV2D(Rect.Right, Rect.Bottom), edgeTolerance) || // Bottom edge
                IsMouseNearLine(mousePoint, new PointV2D(Rect.Left, Rect.Top), new PointV2D(Rect.Left, Rect.Bottom), edgeTolerance) || // Left edge
                IsMouseNearLine(mousePoint, new PointV2D(Rect.Right, Rect.Top), new PointV2D(Rect.Right, Rect.Bottom), edgeTolerance); // Right edge
        }

        public static bool IsMouseNearLine(PointV2D mousePoint, PointV2D lineStart, PointV2D lineEnd, int tolerance)
        {
            double distance = Math.Abs((lineEnd.Y - lineStart.Y) * mousePoint.X -
                                       (lineEnd.X - lineStart.X) * mousePoint.Y +
                                       lineEnd.X * lineStart.Y -
                                       lineEnd.Y * lineStart.X) /
                              Math.Sqrt(Math.Pow(lineEnd.Y - lineStart.Y, 2) +
                                        Math.Pow(lineEnd.X - lineStart.X, 2));

            bool isWithinBounds = mousePoint.X >= Math.Min(lineStart.X, lineEnd.X) - tolerance &&
                                  mousePoint.X <= Math.Max(lineStart.X, lineEnd.X) + tolerance &&
                                  mousePoint.Y >= Math.Min(lineStart.Y, lineEnd.Y) - tolerance &&
                                  mousePoint.Y <= Math.Max(lineStart.Y, lineEnd.Y) + tolerance;

            return distance <= tolerance && isWithinBounds;
        }

        public static ShapeEdge GetEdgeUnderMouse(PointV2D StartPoint, PointV2D EndPoint, PointV2D mousePoint, int edgeTolerance)
        {
            // Normalize the rectangle
            double left = Math.Min(StartPoint.X, EndPoint.X);
            double top = Math.Min(StartPoint.Y, EndPoint.Y);
            double right = Math.Max(StartPoint.X, EndPoint.X);
            double bottom = Math.Max(StartPoint.Y, EndPoint.Y);

            // Check for corner detection first
            if (IsPointNear(mousePoint, new PointV2D(left, top), edgeTolerance)) return ShapeEdge.TopLeft;
            if (IsPointNear(mousePoint, new PointV2D(right, top), edgeTolerance)) return ShapeEdge.TopRight;
            if (IsPointNear(mousePoint, new PointV2D(left, bottom), edgeTolerance)) return ShapeEdge.BottomLeft;
            if (IsPointNear(mousePoint, new PointV2D(right, bottom), edgeTolerance)) return ShapeEdge.BottomRight;

            // Check for edge detection
            if (Math.Abs(mousePoint.X - left) <= edgeTolerance && mousePoint.Y >= top && mousePoint.Y <= bottom) return ShapeEdge.Left;
            if (Math.Abs(mousePoint.X - right) <= edgeTolerance && mousePoint.Y >= top && mousePoint.Y <= bottom) return ShapeEdge.Right;
            if (Math.Abs(mousePoint.Y - top) <= edgeTolerance && mousePoint.X >= left && mousePoint.X <= right) return ShapeEdge.Top;
            if (Math.Abs(mousePoint.Y - bottom) <= edgeTolerance && mousePoint.X >= left && mousePoint.X <= right) return ShapeEdge.Bottom;

            return ShapeEdge.None;
        }

        public static bool IsPointNear(PointV2D p1, PointV2D p2, int tolerance)
        {
            return Math.Abs(p1.X - p2.X) <= tolerance && Math.Abs(p1.Y - p2.Y) <= tolerance;
        }

        public static PointV2D[] Resize(PointV2D StartPoint, PointV2D EndPoint, int deltaX, int deltaY, ShapeEdge edge)
        {
            switch (edge)
            {
                case ShapeEdge.Left:
                    StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y); // Move left edge by adjusting StartPoint.X
                    break;

                case ShapeEdge.Right:
                    EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y); // Move right edge by adjusting EndPoint.X
                    break;

                case ShapeEdge.Top:
                    StartPoint = new PointV2D(StartPoint.X, StartPoint.Y + deltaY); // Move top edge by adjusting StartPoint.Y
                    break;

                case ShapeEdge.Bottom:
                    EndPoint = new PointV2D(EndPoint.X, EndPoint.Y + deltaY); // Move bottom edge by adjusting EndPoint.Y
                    break;

                case ShapeEdge.TopLeft:
                    StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y + deltaY); // Move both left and top
                    break;

                case ShapeEdge.TopRight:
                    StartPoint = new PointV2D(StartPoint.X, StartPoint.Y + deltaY); // Move top
                    EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y); // Move right
                    break;

                case ShapeEdge.BottomLeft:
                    StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y); // Move left
                    EndPoint = new PointV2D(EndPoint.X, EndPoint.Y + deltaY); // Move bottom
                    break;

                case ShapeEdge.BottomRight:
                    EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y + deltaY); // Move both right and bottom
                    break;
            }
            return new PointV2D[] { StartPoint, EndPoint };
        }

        public static PointV2D[] GetRotatedRectangleCorners(Rectangle rect, PointV2D pivotPoint, float angle)
        {
            // Calculate the four corners of the rectangle
            PointV2D[] corners = new PointV2D[4];
            corners[0] = new PointV2D(rect.Left, rect.Top);         // Top-left
            corners[1] = new PointV2D(rect.Right, rect.Top);        // Top-right
            corners[2] = new PointV2D(rect.Right, rect.Bottom);     // Bottom-right
            corners[3] = new PointV2D(rect.Left, rect.Bottom);      // Bottom-left

            // Rotate each corner around the custom pivot point
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = RotatePoint(corners[i], pivotPoint, angle);
            }
            return corners;
        }

        // Rotate a point around a pivot point
        public static PointV2D RotatePoint(PointV2D point, PointV2D pivot, float angle)
        {
            // Convert angle to radians
            float radians = angle * (float)(Math.PI / 180.0);

            // Translate point to the origin (relative to the pivot)
            float translatedX = (float)(point.X - pivot.X);
            float translatedY = (float)(point.Y - pivot.Y);

            // Apply rotation matrix
            float rotatedX = (float)(translatedX * Math.Cos(radians) - translatedY * Math.Sin(radians));
            float rotatedY = (float)(translatedX * Math.Sin(radians) + translatedY * Math.Cos(radians));

            // Translate point back relative to the pivot
            return new PointV2D(rotatedX + pivot.X, rotatedY + pivot.Y);
        }

        // Helper method to rotate a point around a center bounding box
        public static PointV2D RotatePointAtCenter(PointV2D point, double centerX, double centerY, double angleRad)
        {
            double deltaX = point.X - centerX;
            double deltaY = point.Y - centerY;

            double rotatedX = (deltaX * Math.Cos(angleRad) - deltaY * Math.Sin(angleRad)) + centerX;
            double rotatedY = (deltaX * Math.Sin(angleRad) + deltaY * Math.Cos(angleRad)) + centerY;

            return new PointV2D(rotatedX, rotatedY);
        }


        // Helper method to calculate the bounding box of the rotated rectangle
        //public static RectangleF GetBoundingBox(PointV2D[] corners)
        //{
        //    float minX = (float)corners.Min(corner => corner.X);
        //    float minY = (float)corners.Min(corner => corner.Y);
        //    float maxX = (float)corners.Max(corner => corner.X);
        //    float maxY = (float)corners.Max(corner => corner.Y);
        //    return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        //}

        public static RectangleF GetBoundingBox(PointV2D[] points)
        {
            float minX = points.Min(p => (float)p.X);
            float minY = points.Min(p => (float)p.Y);
            float maxX = points.Max(p => (float)p.X);
            float maxY = points.Max(p => (float)p.Y);

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }




        public static float[] GetPenDashPattern(LineStyle linStyle)
        {
            switch (linStyle)
            {
                case LineStyle.Solid:
                    return null; // Solid lines don't need a custom DashPattern, returning null is fine.

                case LineStyle.Custom1:
                    return new float[] { 4, 2 }; // Custom Dash Style 1

                case LineStyle.Custom2:
                    return new float[] { 4, 4 }; // Custom Dash Style 2

                case LineStyle.Custom3:
                    return new float[] { 6, 4 }; // Custom Dash Style 3

                case LineStyle.Custom4:
                    return new float[] { 6, 6 }; // Custom Dash Style 4

                case LineStyle.Custom5:
                    return new float[] { 6, 3, 1, 3, 6 }; // Custom Dash Style 5

                default:
                    return null; // Default to a solid line if no other match
            }
        }


        public static CustomHatchStyle GetCustomHatchStyle(HatchStyle hatchStyle)
        {
            switch (hatchStyle)
            {
                case HatchStyle.Horizontal:
                    return CustomHatchStyle.Horizontal;
                case HatchStyle.Vertical:
                    return CustomHatchStyle.Vertical;
                case HatchStyle.Cross:
                    return CustomHatchStyle.Cross;
                case HatchStyle.ForwardDiagonal:
                    return CustomHatchStyle.Diagonal;
                default:
                    return CustomHatchStyle.None; // Fallback to None
            }
        }

        public static HatchStyle ConvertToHatchStyle(CustomHatchStyle customHatchStyle)
        {
            switch (customHatchStyle)
            {
                case CustomHatchStyle.Horizontal:
                    return HatchStyle.Horizontal;
                case CustomHatchStyle.Vertical:
                    return HatchStyle.Vertical;
                case CustomHatchStyle.Cross:
                    return HatchStyle.Cross;
                case CustomHatchStyle.Diagonal:
                    return HatchStyle.ForwardDiagonal;
                case CustomHatchStyle.BackDiagonal:
                    return HatchStyle.BackwardDiagonal;
                case CustomHatchStyle.DiagCross:
                    return HatchStyle.DiagonalCross;
                case CustomHatchStyle.CheckerBoard:
                    return HatchStyle.LargeCheckerBoard;
                case CustomHatchStyle.None:
                    return HatchStyle.Percent05; // Treat None as Percent05
                default:
                    return HatchStyle.Percent05;
            }
        }


        public static PointV2D[] ResizeNormal(double deltaX, double deltaY, ShapeEdge edge, PointV2D StartPoint, PointV2D EndPoint)
        {
            const int MinSize = 2; // Minimum width and height in pixels

            // Calculate the current width and height
            double currentWidth = Math.Abs(EndPoint.X - StartPoint.X);
            double currentHeight = Math.Abs(EndPoint.Y - StartPoint.Y);

            switch (edge)
            {
                case ShapeEdge.Left:
                    if (currentWidth - deltaX >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y);
                    }
                    break;

                case ShapeEdge.Right:
                    if (currentWidth + deltaX >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y);
                    }
                    break;

                case ShapeEdge.Top:
                    if (currentHeight - deltaY >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X, StartPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.Bottom:
                    if (currentHeight + deltaY >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X, EndPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.TopLeft:
                    if (currentWidth - deltaX >= MinSize && currentHeight - deltaY >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.TopRight:
                    if (currentWidth + deltaX >= MinSize && currentHeight - deltaY >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X, StartPoint.Y + deltaY);
                        EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y);
                    }
                    break;

                case ShapeEdge.BottomLeft:
                    if (currentWidth - deltaX >= MinSize && currentHeight + deltaY >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y);
                        EndPoint = new PointV2D(EndPoint.X, EndPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.BottomRight:
                    if (currentWidth + deltaX >= MinSize && currentHeight + deltaY >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y + deltaY);
                    }
                    break;
            }
            return new PointV2D[] { StartPoint, EndPoint };
        }

        public static PointV2D[] ResizeBottomLeftToTopRight(double deltaX, double deltaY, ShapeEdge edge, PointV2D StartPoint, PointV2D EndPoint)
        {
            const int MinSize = 5; // Minimum width and height in pixels

            // Calculate the current width and height
            double currentWidth = Math.Abs(EndPoint.X - StartPoint.X);
            double currentHeight = Math.Abs(EndPoint.Y - StartPoint.Y);

            switch (edge)
            {
                case ShapeEdge.Left:
                    if (currentWidth - deltaX >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y);
                    }
                    break;

                case ShapeEdge.Right:
                    if (currentWidth + deltaX >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y);
                    }
                    break;

                case ShapeEdge.Top:
                    if (currentHeight - deltaY >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X, EndPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.Bottom:
                    if (currentHeight + deltaY >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X, StartPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.TopLeft:
                    if (currentWidth - deltaX >= MinSize && currentHeight - deltaY >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y);
                        EndPoint = new PointV2D(EndPoint.X, EndPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.TopRight:
                    if (currentWidth + deltaX >= MinSize && currentHeight - deltaY >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y + deltaY); // Adjust top-right
                    }
                    break;

                case ShapeEdge.BottomLeft:
                    if (currentWidth - deltaX >= MinSize && currentHeight + deltaY >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y + deltaY); // Adjust bottom-left
                    }
                    break;

                case ShapeEdge.BottomRight:
                    if (currentWidth + deltaX >= MinSize && currentHeight + deltaY >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y); // Adjust right side
                        StartPoint = new PointV2D(StartPoint.X, StartPoint.Y + deltaY);  // Adjust bottom side
                    }
                    break;
            }
            return new PointV2D[] { StartPoint, EndPoint };
        }

        public static PointV2D[] ResizeTopRightToBottomLeft(double deltaX, double deltaY, ShapeEdge edge, PointV2D StartPoint, PointV2D EndPoint)
        {
            const int MinSize = 5; // Minimum width and height in pixels

            // Calculate the current width and height
            double currentWidth = Math.Abs(EndPoint.X - StartPoint.X);
            double currentHeight = Math.Abs(EndPoint.Y - StartPoint.Y);

            switch (edge)
            {
                case ShapeEdge.Left:
                    if (currentWidth - deltaX >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y);
                    }
                    break;

                case ShapeEdge.Right:
                    if (currentWidth + deltaX >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y);
                    }
                    break;

                case ShapeEdge.Top:
                    if (currentHeight - deltaY >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X, StartPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.Bottom:
                    if (currentHeight + deltaY >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X, EndPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.TopLeft:
                    if (currentWidth - deltaX >= MinSize && currentHeight - deltaY >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y);
                        StartPoint = new PointV2D(StartPoint.X, StartPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.TopRight:
                    if (currentWidth + deltaX >= MinSize && currentHeight - deltaY >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.BottomLeft:
                    if (currentWidth - deltaX >= MinSize && currentHeight + deltaY >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.BottomRight:
                    if (currentWidth + deltaX >= MinSize && currentHeight + deltaY >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y);
                        EndPoint = new PointV2D(EndPoint.X, EndPoint.Y + deltaY);
                    }
                    break;
            }
            return new PointV2D[] { StartPoint, EndPoint };
        }

        public static PointV2D[] ResizeBottomRightToTopLeft(double deltaX, double deltaY, ShapeEdge edge, PointV2D StartPoint, PointV2D EndPoint)
        {
            const int MinSize = 5; // Minimum width and height in pixels

            // Calculate the current width and height
            double currentWidth = Math.Abs(EndPoint.X - StartPoint.X);
            double currentHeight = Math.Abs(EndPoint.Y - StartPoint.Y);

            switch (edge)
            {
                case ShapeEdge.Left:
                    if (currentWidth - deltaX >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y);
                    }
                    break;

                case ShapeEdge.Right:
                    if (currentWidth + deltaX >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y);
                    }
                    break;

                case ShapeEdge.Top:
                    if (currentHeight - deltaY >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X, EndPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.Bottom:
                    if (currentHeight + deltaY >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X, StartPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.TopLeft:
                    if (currentWidth - deltaX >= MinSize && currentHeight - deltaY >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.TopRight:
                    if (currentWidth + deltaX >= MinSize && currentHeight - deltaY >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y);
                        EndPoint = new PointV2D(EndPoint.X, EndPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.BottomLeft:
                    if (currentWidth - deltaX >= MinSize && currentHeight + deltaY >= MinSize)
                    {
                        EndPoint = new PointV2D(EndPoint.X + deltaX, EndPoint.Y);
                        StartPoint = new PointV2D(StartPoint.X, StartPoint.Y + deltaY);
                    }
                    break;

                case ShapeEdge.BottomRight:
                    if (currentWidth + deltaX >= MinSize && currentHeight + deltaY >= MinSize)
                    {
                        StartPoint = new PointV2D(StartPoint.X + deltaX, StartPoint.Y + deltaY);
                    }
                    break;
            }
            return new PointV2D[] { StartPoint, EndPoint };
        }
    }
}
