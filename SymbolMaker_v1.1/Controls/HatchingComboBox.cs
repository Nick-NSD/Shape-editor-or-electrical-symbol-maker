using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static SymbolMaker.ShapeBase;

namespace SymbolMaker
{
    public class HatchingComboBox : ComboBox
    {
        public HatchingComboBox()
        {
            // Set up the control to use owner-draw mode
            Items.Clear();
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            ItemHeight = 20; // Set item height for custom drawing

            // Populate the ComboBox with all CustomHatchStyle enum values
            foreach (CustomHatchStyle hatch in Enum.GetValues(typeof(CustomHatchStyle)))
            {
                Items.Add(hatch);
            }

            // Subscribe to the DrawItem event to customize drawing
            DrawItem += new DrawItemEventHandler(HatchingComboBox_DrawItem);

            // Optionally set the default to "None"
            SelectedIndex = 0; // "None" will be at index 0
        }

        // Access the selected hatch style
        public CustomHatchStyle? SelectedHatchStyle
        {
            get
            {
                // Check if the selected item is "None" (no hatch)
                if (SelectedItem is string && (string)SelectedItem == "None")
                {
                    return null; // Return null to indicate "no hatch"
                }

                // Otherwise, return the selected HatchStyle
                return (CustomHatchStyle)SelectedItem;
            }
            set
            {
                if (value == null)
                {
                    // Set "None" as the selected item if value is null
                    SelectedItem = "None";
                }
                else
                {
                    SelectedItem = value;
                }
            }
        }

        // Override the OnDrawItem method or use DrawItem event for custom drawing
        private void HatchingComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            // Draw the background
            e.DrawBackground();

            // Handle the "None" case
            if (Items[e.Index] is string && (string)Items[e.Index] == "None")
            {
                // Draw "None" as a text option
                using (Brush textBrush = new SolidBrush(e.ForeColor))
                {
                    e.Graphics.DrawString("None", e.Font, textBrush, e.Bounds.X, e.Bounds.Y);
                }
            }
            else
            {
                // Otherwise, handle the HatchStyle drawing
                CustomHatchStyle hatchStyle = (CustomHatchStyle)Items[e.Index];

                // Create a HatchBrush with the current HatchStyle
                using (HatchBrush brush = new HatchBrush(ShapeUtil.ConvertToHatchStyle(hatchStyle), Color.Black, Color.White))
                {
                    // Define the rectangle where the hatch pattern will be drawn
                    Rectangle hatchRectangle = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Height - 4, e.Bounds.Height - 4);
                    e.Graphics.FillRectangle(brush, hatchRectangle);
                }

                // Draw the name of the hatch style as text next to the pattern
                string hatchName = hatchStyle.ToString();
                using (Brush textBrush = new SolidBrush(e.ForeColor))
                {
                    e.Graphics.DrawString(hatchName, e.Font, textBrush, e.Bounds.Height + 5, e.Bounds.Y);
                }
            }

            // Draw focus rectangle if needed
            e.DrawFocusRectangle();
        }
    }
}
