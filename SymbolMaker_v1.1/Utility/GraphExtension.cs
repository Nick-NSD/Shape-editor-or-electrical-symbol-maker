using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;

namespace SymbolMaker
{
    public class GraphExtension
    {
        public static Image GenerateThumbnail(SymbolShape symbol, int thumbnailWidth = 216, int thumbnailHeight = 216)
        {
            // Create a new bitmap with the specified thumbnail size
            Bitmap thumbnail = new Bitmap(thumbnailWidth, thumbnailHeight);

            using (Graphics g = Graphics.FromImage(thumbnail))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(Color.White); // Clear the background with white
                symbol.UpdateBoundingBox();

                // Determine the bounding rectangle of the symbol
                RectangleF symbolBounds = symbol.Rect; // Get the bounding box of the symbol
                symbolBounds.Inflate(5, 5);
                // Calculate the scale factor to fit the symbol within the thumbnail dimensions
                float scaleX = thumbnailWidth / symbolBounds.Width;
                float scaleY = thumbnailHeight / symbolBounds.Height;
                float scale = Math.Min(scaleX, scaleY);

                // Calculate the position to center the symbol in the thumbnail
                float offsetX = (thumbnailWidth - (symbolBounds.Width * scale)) / 2.0f;
                float offsetY = (thumbnailHeight - (symbolBounds.Height * scale)) / 2.0f;

                // Translate the drawing origin to ensure the symbol is centered
                g.TranslateTransform(offsetX - symbolBounds.Left * scale, offsetY - symbolBounds.Top * scale);
                g.ScaleTransform(scale, scale);

                // Draw the symbol onto the graphics object (scaled and centered)
                symbol.Draw(g);
            }
            return thumbnail;
        }

        public static RectangleF GetShapesBounds(List<ShapeBase> shapes, string symID)
        {
            // Filter the shapes based on the provided symID. Below is the LINQ statement.
            //var filteredShapes = shapes.Where(shape => shape.ShapeID == symID).ToList();

            List<ShapeBase> filteredShapes = new List<ShapeBase>();

            foreach (var shape in shapes)
            {
                if (shape.ShapeID == symID)
                {
                    filteredShapes.Add(shape);
                }
            }

            if (filteredShapes == null || filteredShapes.Count == 0)
            {
                return Rectangle.Empty;
            }

            var firstShape = filteredShapes[0];
            float minX = firstShape.Rect.Left;
            float minY = firstShape.Rect.Top;
            float maxX = firstShape.Rect.Right;
            float maxY = firstShape.Rect.Bottom;

            // Loop through the filtered shapes to find the minimum and maximum coordinates
            foreach (var shape in filteredShapes)
            {
                if (shape.Rect.Left < minX) minX = shape.Rect.Left;
                if (shape.Rect.Top < minY) minY = shape.Rect.Top;
                if (shape.Rect.Right > maxX) maxX = shape.Rect.Right;
                if (shape.Rect.Bottom > maxY) maxY = shape.Rect.Bottom;
            }

            // Calculate the width and height based on the min/max coordinates
            float width = maxX - minX;
            float height = maxY - minY;

            // Return the bounding rectangle
            return new RectangleF(minX, minY, width, height);
        }
    }
}
