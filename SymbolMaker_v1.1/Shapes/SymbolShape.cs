using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Xml.Serialization;
using UserControls;

namespace SymbolMaker
{
    [Serializable]
    public class SymbolShape : ShapeBase
    {
        [XmlElement("SymbolID")]
        public string SymbolID { get; set; }

        [XmlElement("SymbolName")]
        public TextShape SymbolName { get; set; }

        [XmlElement("SymbolNameVisibility")]
        public bool SymbolNameVisible { get; set; } = true;

        [XmlElement("SymConnectionVisibility")]
        public bool SymbolConnectionVisible { get; set; } = false;

        [XmlElement("SymbolNameLocation")]
        public PointV2D NamePosition { get; set; } = new PointV2D();

        [XmlArray("SymbolInternalShapes")]
        public List<ShapeBase> InternalShapes { get; set; } = new List<ShapeBase>();


        private ShapeBase detectedInternalShape = null;

        public SymbolShape(PointV2D namePosition, string name, TextAlignment nameAlignment, TextRotation nameRotation, Font textFont, Color fontColor, string symbolID, List<ShapeBase> internalShapes)
        {
            NamePosition = namePosition;
            SymbolName = new TextShape(namePosition, 2, name, nameAlignment, nameRotation, textFont, fontColor);
            SymbolName.ShapeID = "s";
            SymbolID = symbolID;
            InternalShapes = internalShapes;
            InternalShapes.Add(SymbolName);
            UpdateBoundingBox();
        }

        public List<ConnectionShape> GetAllConnections()
        {
            return InternalShapes.OfType<ConnectionShape>().ToList();
        }

        public SymbolShape()
        {
            ZoomFactorChanged += (sender, e) => ApplyZoomToInternalShapes();
        }

        public override void Draw(Graphics g)
        {
            // Iterate over all internal shapes
            foreach (var shape in InternalShapes)
            {
                if (shape is ConnectionShape csh && SymbolConnectionVisible)
                {
                    g.DrawRectangle(Pens.Crimson, Rectangle.Round(csh.Rect));//show all connection
                }
                using (Pen p = new Pen(Color.Black, 0.15f))
                {
                    p.DashPattern = new float[] { 2, 2 };
                    p.DashStyle = DashStyle.Custom;
                    p.DashCap = DashCap.Round;
                    p.LineJoin = LineJoin.Round;
                    p.StartCap = LineCap.Round;
                    p.EndCap = LineCap.Round;

                    //Propagate the symbol's selection state to internal shapes for highlighting
                    if (!(detectedInternalShape is TextShape))
                    {
                        shape.IsSelected = IsSelected;//select all geometrical shapes
                    }
                    else shape.IsSelected = false;

                    if (detectedInternalShape is TextShape sn && sn.TextType == 2 && SymbolNameVisible)
                    {
                        g.DrawRectangle(p, Rectangle.Round(sn.Rect));//draw bounding box if shape is symbol name
                    }

                    if (detectedInternalShape is TextShape textShape && textShape.TextType == 1)
                    {
                        if (shape is ConnectionShape cn)
                        {
                            if (cn.ConnectionNameVisible)
                            {
                                g.DrawRectangle(p, Rectangle.Round(textShape.Rect));
                            }
                        }
                    }

                    if (detectedInternalShape is ConnectionShape cs)
                    {
                        g.FillRectangle(Brushes.Crimson, cs.Rect);//show detected connection
                    }

                    // Draw each internal shape only once
                    if (shape is ConnectionShape con)
                    {
                        con.Draw(g);
                        if (con.ConnectionNameVisible)
                        {
                            con.ConnectionName.Draw(g);//Draw connection name text
                        }
                    }

                    else if (shape is TextShape syn && syn.TextType == 2 && SymbolNameVisible)
                    {
                        syn.Draw(g);//Draw symbol name text
                    }
                    else if (shape is TextShape txt && txt.TextType == 0)
                    {
                        txt.Draw(g);//Draw text witch is normal text
                    }
                    else if (!(shape is TextShape))
                    {
                        // Draw geometrical shapes that are not texts
                        shape.Draw(g);
                    }
                }
            }
        }

        public override void Move(double deltaX, double deltaY)
        {
            foreach (var shape in InternalShapes)
            {
                shape.Move(deltaX, deltaY);
            }
            UpdateBoundingBox();
        }

        //This is read from MouseMove event in Form1 class - DetectShapeUnderMouse method
        public ShapeBase DetectShapeAtPoint(PointV2D mousePoint, int rtolerance)
        {
            detectedInternalShape = null;
            foreach (var shape in InternalShapes)
            {

                if (shape is EllipseShape ellipse)
                {
                    if (shape.IsMouseNearEdge(mousePoint, rtolerance))
                    {
                        return ellipse;
                    }
                }

                if (shape is ArcShape arc)
                {
                    if (shape.IsMouseNearEdge(mousePoint, rtolerance))
                    {
                        return arc;
                    }
                }

                if (shape is RectangleShape rect)
                {
                    if (shape.IsMouseNearEdge(mousePoint, rtolerance))
                    {
                        return rect;
                    }
                }

                else if (shape is LineShape line)
                {
                    if (line.IsMouseNearLineBody(mousePoint, rtolerance))
                    {
                        return line;
                    }
                }

                else if (shape is TextShape textShape && textShape.TextType == 0)//text
                {
                    if (textShape.IsMouseInsideShape(mousePoint) || textShape.IsMouseNearEdge(mousePoint, rtolerance))
                    {
                        detectedInternalShape = textShape;
                        return textShape;
                    }
                }

                else if (shape is TextShape symName && symName.TextType == 2)//symbol name
                {
                    if (symName.IsMouseInsideShape(mousePoint) || symName.IsMouseNearEdge(mousePoint, rtolerance))
                    {
                        SymbolName = symName;
                        detectedInternalShape = symName;
                        return symName;
                    }
                }

                else if (shape is ConnectionShape connection)
                {
                    // Check if the mouse is near the connection dot edge itself (tolerance = fixed value 10) 
                    if (connection.IsMouseNearEdge(mousePoint, rtolerance) || connection.IsMouseInsideShape(mousePoint))
                    {
                        detectedInternalShape = connection;
                        return connection; // Select the connection (dot) 
                    }

                    //Check if the mouse is over the connection's name (TextShape)
                    if (connection.IsMouseOverName(mousePoint))
                    {
                        detectedInternalShape = connection.ConnectionName;
                        return connection.ConnectionName; // Select the TextShape
                    }
                }

                else if (shape is PolygonShape poly)
                {
                    if (poly.IsMouseNearVertex(mousePoint, rtolerance, out int vertexIndex))
                    {
                        return poly;
                    }
                    if (poly.IsMouseNearEdge(mousePoint, rtolerance))
                    {
                        return poly;
                    }
                }
            }
            return null; // No shape detected at the given point
        }

        public override bool IsMouseInsideShape(PointV2D mousePoint)
        {
            GetSingleShapeBounds();
            // Adjust the mouse point based on the current zoom factor
            PointV2D adjustedPointF = new PointV2D(mousePoint.X / ZoomFactor, mousePoint.Y / ZoomFactor);

            // Check if the adjusted point is inside the original unscaled rectangle
            Rectangle r = Rectangle.Round(Rect);
            return r.Contains(adjustedPointF);
        }

        public override bool IsMouseNearEdge(PointV2D mousePoint, int edgeTolerance)
        {
            // Check if the mouse is near the edge of any of the internal shapes
            foreach (var shape in InternalShapes)
            {
                if (shape.IsMouseNearEdge(mousePoint, edgeTolerance))
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateBoundingBox()
        {
            if (InternalShapes.Count == 0)
            {
                Rect = Rectangle.Empty;
                return;
            }

            // Initialize the min and max values
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            // Calculate bounding box based on internal shapes
            foreach (var shape in InternalShapes)
            {
                if (!(shape is TextShape sn && sn.TextType == 2))
                {
                    if (shape.Rect != Rectangle.Empty)
                    {
                        shape.GetSingleShapeBounds();
                        minX = Math.Min(minX, shape.Rect.Left);
                        minY = Math.Min(minY, shape.Rect.Top);
                        maxX = Math.Max(maxX, shape.Rect.Right);
                        maxY = Math.Max(maxY, shape.Rect.Bottom);
                    }
                }
            }

            // Validate the bounding box
            if (minX <= maxX && minY <= maxY)
            {
                StartPoint = new PointV2D(minX, minY);
                EndPoint = new PointV2D(maxX, maxY);
                Width = maxX - minX;
                Height = maxY - minY;
                Rect = new RectangleF(minX, minY, (float)Width, (float)Height);
            }
            else
            {
                Rect = Rectangle.Empty;
            }
        }

        public void ApplyZoomToInternalShapes()
        {
            foreach (var shape in InternalShapes)
            {
                shape.ZoomFactor = ZoomFactor;
                shape.ViewportOffsetX = ViewportOffsetX;
                shape.ViewportOffsetY = ViewportOffsetY;

                // If the internal shape is a connection shape or has its own internal shapes, handle it recursively
                if (shape is ConnectionShape conShape)
                {
                    conShape.ConnectionName.ZoomFactor = ZoomFactor;
                }
                if (shape is TextShape txtShape && txtShape.TextType == 2)
                {
                    txtShape.ZoomFactor = ZoomFactor;
                }
            }
        }

        #region Flip / Rotate
        public override void Flip(bool flipHorizontally)
        {
            // Get all shapes with the same ShapeID
            var groupedShapes = InternalShapes;

            if (groupedShapes == null || groupedShapes.Count == 0) return;

            // Get all connection shapes with the same ShapeID
            var connectionShapes = groupedShapes.OfType<ConnectionShape>().ToList();
            if (connectionShapes.Count == 0) return; // No connections to base the flip axis on

            // Calculate the bounding box for only connection shapes
            RectangleF connectionBounds = GetConnectionShapesBounds(connectionShapes);
            if (connectionBounds == Rectangle.Empty) return; // No valid bounding box found for connections

            // Calculate the flipping axis based on the connection bounding box
            float centerX = connectionBounds.X + connectionBounds.Width / 2f;
            float centerY = connectionBounds.Y + connectionBounds.Height / 2f;

            // Flip each shape in the group based on the connection center
            foreach (var shape in groupedShapes)
            {
                if (shape is PolygonShape polygon)
                {
                    // Flip each vertex of the polygon
                    for (int i = 0; i < polygon.Vertices.Count; i++)
                    {
                        PointV2D vertex = polygon.Vertices[i];

                        if (flipHorizontally)
                        {
                            // Flip horizontally around the center X-axis (of connections)
                            double newX = 2 * centerX - vertex.X;
                            polygon.Vertices[i] = new PointV2D(newX, vertex.Y);
                        }
                        else
                        {
                            // Flip vertically around the center Y-axis (of connections)
                            double newY = 2 * centerY - vertex.Y;
                            polygon.Vertices[i] = new PointV2D(vertex.X, newY);
                        }
                    }

                    // Recalculate the polygon's bounding box after flipping
                    polygon.GetSingleShapeBounds();
                }

                else
                {
                    //Handle non-polygon shapes (e.g., lines)
                    if (flipHorizontally)
                    {
                        //Flip horizontally around the center X-axis (of connections)
                        double newX1 = 2 * centerX - shape.StartPoint.X;
                        double newX2 = 2 * centerX - shape.EndPoint.X;
                        shape.StartPoint = new PointV2D(newX1, shape.StartPoint.Y);
                        shape.EndPoint = new PointV2D(newX2, shape.EndPoint.Y);
                    }
                    else
                    {
                        //Flip vertically around the center Y-axis (of connections)
                        double newY1 = 2 * centerY - shape.StartPoint.Y;
                        double newY2 = 2 * centerY - shape.EndPoint.Y;
                        shape.StartPoint = new PointV2D(shape.StartPoint.X, newY1);
                        shape.EndPoint = new PointV2D(shape.EndPoint.X, newY2);
                    }
                    if (shape is ArcShape arc)
                    {
                        if (flipHorizontally)
                        {
                            // Flip horizontally: Reflect the StartAngle
                            arc.StartAngle = 180 - (arc.StartAngle + arc.SweepAngle) % 360;
                        }
                        else
                        {
                            // Flip vertically: Reflect the StartAngle
                            arc.StartAngle = 360 - (arc.StartAngle + arc.SweepAngle) % 360;
                        }
                    }
                    // Recalculate shape bounds after flipping
                    shape.GetSingleShapeBounds();
                }
            }

            // Update the bounding box after flipping all shapes
            UpdateBoundingBox();
        }

        public static RectangleF GetConnectionShapesBounds(List<ConnectionShape> connectionShapes)
        {
            if (connectionShapes == null || connectionShapes.Count == 0)
                return Rectangle.Empty;

            // Initialize with the bounds of the first connection shape
            RectangleF bounds = connectionShapes[0].Rect;

            // Expand the bounds to include all connection shapes
            foreach (var connection in connectionShapes.Skip(1))
            {
                bounds = RectangleF.Union(bounds, connection.Rect);
            }
            return bounds;
        }

        public override void Rotate(float angle)
        {
            // Get all shapes with the same ShapeID
            var groupedShapes = InternalShapes;

            if (groupedShapes.Count == 0) return;

            // Get all connection shapes with the same ShapeID
            var connectionShapes = groupedShapes.OfType<ConnectionShape>().ToList();
            if (connectionShapes.Count == 0) return; // No connections to base the rotation axis on

            // Calculate the bounding box for only connection shapes
            RectangleF connectionBounds = GetConnectionShapesBounds(connectionShapes);
            if (connectionBounds == Rectangle.Empty) return; // No valid bounding box found for connections

            // Calculate the center of rotation (based on the connection bounding box)
            float centerX = connectionBounds.X + connectionBounds.Width / 2f;
            float centerY = connectionBounds.Y + connectionBounds.Height / 2f;

            // Convert angle to radians for trigonometric functions
            double angleRad = -Math.PI * angle / 180.0;

            foreach (var shape in groupedShapes)
            {
                // Check if the shape is a polygon
                if (shape is PolygonShape polygon)
                {
                    // Rotate each vertex of the polygon
                    for (int i = 0; i < polygon.Vertices.Count; i++)
                    {
                        PointV2D vertex = polygon.Vertices[i];

                        // Translate the vertex relative to the center
                        double deltaX = vertex.X - centerX;
                        double deltaY = vertex.Y - centerY;

                        // Apply the 2D rotation matrix
                        double rotatedX = (deltaX * Math.Cos(angleRad) - deltaY * Math.Sin(angleRad));
                        double rotatedY = (deltaX * Math.Sin(angleRad) + deltaY * Math.Cos(angleRad));

                        // Update the vertex with the new rotated position
                        polygon.Vertices[i] = new PointV2D(rotatedX + centerX, rotatedY + centerY);
                    }
                    polygon.GetSingleShapeBounds();
                }
                else
                {
                    // Rotate StartPoint and EndPoint for non-polygon shapes (like lines)
                    double deltaX1 = shape.StartPoint.X - centerX;
                    double deltaY1 = shape.StartPoint.Y - centerY;
                    double rotatedX1 = (deltaX1 * Math.Cos(angleRad) - deltaY1 * Math.Sin(angleRad));
                    double rotatedY1 = (deltaX1 * Math.Sin(angleRad) + deltaY1 * Math.Cos(angleRad));

                    double deltaX2 = shape.EndPoint.X - centerX;
                    double deltaY2 = shape.EndPoint.Y - centerY;
                    double rotatedX2 = (deltaX2 * Math.Cos(angleRad) - deltaY2 * Math.Sin(angleRad));
                    double rotatedY2 = (deltaX2 * Math.Sin(angleRad) + deltaY2 * Math.Cos(angleRad));

                    // Assign rotated points back to the shape
                    shape.StartPoint = new PointV2D(rotatedX1 + centerX, rotatedY1 + centerY);
                    shape.EndPoint = new PointV2D(rotatedX2 + centerX, rotatedY2 + centerY);

                    if (shape is ArcShape arc)
                    {
                        arc.StartAngle = (arc.StartAngle - angle) % 360;
                        arc.GetSingleShapeBounds();
                    }
                    shape.GetSingleShapeBounds();
                }
            }
            UpdateBoundingBox();
        }
        #endregion

        public override ShapeBase Clone()
        {
            var clonedSymbol = new SymbolShape();

            clonedSymbol.SymbolID = SymbolID;
            clonedSymbol.SymbolNameVisible = SymbolNameVisible;
            clonedSymbol.SymbolConnectionVisible = SymbolConnectionVisible;
            clonedSymbol.NamePosition = new PointV2D(NamePosition.X, NamePosition.Y);
            clonedSymbol.SymbolName = SymbolName.Clone() as TextShape;

            // Clone internal shapes
            clonedSymbol.InternalShapes = new List<ShapeBase>();
            foreach (var shape in InternalShapes)
            {
                var clonedShape = shape.Clone();
                clonedSymbol.InternalShapes.Add(clonedShape);
            }
            clonedSymbol.UpdateBoundingBox();
            return clonedSymbol;
        }
    }
}


