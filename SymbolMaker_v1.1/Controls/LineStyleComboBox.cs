using System.Drawing;
using System.Windows.Forms;

namespace SymbolMaker
{
    public class LineStyleComboBox : ComboBox
    {
        public LineStyleComboBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            Items.Clear();
            // Add items for different line styles
            // Ensure items are only added once
            if (Items.Count == 0)
            {
                Items.AddRange(new object[]
                {
                "Solid",
                "Custom1",
                "Custom2",
                "Custom3",
                "Custom4",
                "Custom5"
                });
            }
        }
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();
            Point p1 = new Point(e.Bounds.Left + 5, e.Bounds.Y + 5);
            Point p2 = new Point(e.Bounds.Right - 5, e.Bounds.Y + 5);

            switch (e.Index)
            {
                case 0:
                    using (Pen SolidmyPen = new Pen(e.ForeColor, 1))
                        e.Graphics.DrawLine(SolidmyPen, p1, p2);
                    break;
                case 1:
                    using (Pen Custom1 = new Pen(e.ForeColor, 1))
                    {
                        Custom1.DashPattern = new float[] { 4, 2 }; // Custom Dash Style 1
                        e.Graphics.DrawLine(Custom1, p1, p2);
                    }
                    break;
                case 2:
                    using (Pen Custom2 = new Pen(e.ForeColor, 1))
                    {
                        Custom2.DashPattern = new float[] { 4, 4 }; // Custom Dash Style 2
                        e.Graphics.DrawLine(Custom2, p1, p2);
                    }
                    break;
                case 3:
                    using (Pen Custom3 = new Pen(e.ForeColor, 1))
                    {
                        Custom3.DashPattern = new float[] { 6, 4 }; // Custom Dash Style 3
                        e.Graphics.DrawLine(Custom3, p1, p2);
                    }
                    break;
                case 4:
                    using (Pen Custom4 = new Pen(e.ForeColor, 1))
                    {
                        Custom4.DashPattern = new float[] { 6, 6 }; // Custom Dash Style 4
                        e.Graphics.DrawLine(Custom4, p1, p2);
                    }
                    break;
                case 5:
                    using (Pen Custom5 = new Pen(e.ForeColor, 1))
                    {
                        Custom5.DashPattern = new float[] { 10, 5, 2, 5, 10 }; // Custom Dash Style 5
                        e.Graphics.DrawLine(Custom5, p1, p2);
                    }
                    break;
            }
            e.DrawFocusRectangle();
        }
    }
}
