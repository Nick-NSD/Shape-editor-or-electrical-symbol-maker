using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SymbolMaker.ShapeBase;

namespace SymbolMaker.InputForms
{
    public partial class CreateArcForm : Form
    {
        public Color PenColor { get; set; }
        public Color FillColor { get; set; }
        public float ArcRadius { get; set; }
        public CustomHatchStyle HatchStyl { get; set; }
        public float StartAngle { get; set; }
        public float SweepAngle { get; set; }

        private LineStyle linStyle;

        public LineStyle LinStyle
        {
            get { return linStyle; }
            set
            {
                linStyle = value;
                // Update the ComboBox index based on the new LinStyle value
                switch (linStyle)
                {
                    case LineStyle.Solid:
                        lineStyleComboBox1.SelectedIndex = 0;
                        break;
                    case LineStyle.Custom1:
                        lineStyleComboBox1.SelectedIndex = 1;
                        break;
                    case LineStyle.Custom2:
                        lineStyleComboBox1.SelectedIndex = 2;
                        break;
                    case LineStyle.Custom3:
                        lineStyleComboBox1.SelectedIndex = 3;
                        break;
                    case LineStyle.Custom4:
                        lineStyleComboBox1.SelectedIndex = 4;
                        break;
                    case LineStyle.Custom5:
                        lineStyleComboBox1.SelectedIndex = 5;
                        break;
                    default:
                        lineStyleComboBox1.SelectedIndex = 0;
                        break;
                }
            }
        }

        private float thickness;
        public float PenThickness//in mm
        {
            get
            {
                return thickness;
            }

            set
            {
                if (value >= 0.01f && value <= 100f)
                {
                    thickness = value;
                    nudPenThickness.Value = (decimal)value;
                }
                Invalidate();
            }
        }

        public float MillimetersToPixels(float mm)
        {
            // Convert millimeters to inches, then inches to pixels
            return (mm / 25.4f) * 96;
        }

        public float PixelsToMillimeters(float pixels)
        {
            // Convert pixels to inches, then inches to millimeters
            return pixels / 96 * 25.4f;
        }

        public CreateArcForm(Color penColor, LineStyle linStyle, float penThickness, Color fillColor, CustomHatchStyle hatchStyle, float startAngle, float sweepAngle, float arcRadius)
        {
            InitializeComponent();

            btnPenColor.BackColor = penColor;
            btnFillColor.BackColor = fillColor;
            PenThickness = PixelsToMillimeters(penThickness);//nud is updated in properties
            LinStyle = linStyle;//also for LinStyle
            HatchStyl = hatchStyle;
            hatchingComboBox1.SelectedHatchStyle = hatchStyle;
            nudStartAngle.Value = (decimal)startAngle;//nud is numericUpDown
            nudEndAngle.Value = (decimal)sweepAngle;
            nudRadius.Value = (decimal)PixelsToMillimeters(arcRadius);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            PenColor = btnPenColor.BackColor;
            FillColor = btnFillColor.BackColor;
            ArcRadius = (float)nudRadius.Value;//mm
            StartAngle = (float)nudStartAngle.Value;
            SweepAngle = (float)nudEndAngle.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void panelFillColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    btnFillColor.BackColor = cd.Color;
                }
            }
        }

        private void btnFillTransp_Click(object sender, EventArgs e)
        {
            btnFillColor.BackColor = Color.Transparent;
        }

        private void lineStyleComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (lineStyleComboBox1.SelectedIndex)
            {
                case 0:
                    LinStyle = LineStyle.Solid;
                    break;

                case 1:
                    LinStyle = LineStyle.Custom1;
                    break;

                case 2:
                    LinStyle = LineStyle.Custom2;
                    break;

                case 3:
                    LinStyle = LineStyle.Custom3;
                    break;

                case 4:
                    LinStyle = LineStyle.Custom4;
                    break;
                case 5:
                    LinStyle = LineStyle.Custom5;
                    break;
                default:
                    LinStyle = LineStyle.Solid;
                    break;
            }
        }

        private void btnPenColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    btnPenColor.BackColor = cd.Color;
                }
            }
        }

        private void btnFillColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    btnFillColor.BackColor = cd.Color;
                }
            }
        }

        private void nudPenThickness_ValueChanged(object sender, EventArgs e)
        {
            //This value is in millimeters
            PenThickness = (float)nudPenThickness.Value;
        }

        private void hatchingComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected item as a string
            string selectedValue = hatchingComboBox1.SelectedItem.ToString();

            // Check for "None" option first
            if (selectedValue == "None")
            {
                HatchStyl = CustomHatchStyle.None; // Handle "None" case explicitly
            }
            else
            {
                // Try parsing the string to the corresponding CustomHatchStyle enum
                if (Enum.TryParse(selectedValue, out CustomHatchStyle selectedHatchStyle))
                {
                    HatchStyl = selectedHatchStyle;
                    Debug.WriteLine("HatchStyl = " + HatchStyl);
                }
                else
                {
                    HatchStyl = CustomHatchStyle.None;
                }
            }

        }

        private void nudStartAngle_ValueChanged(object sender, EventArgs e)
        {
            StartAngle = (float)nudStartAngle.Value;
            picBoxArcPreview.Invalidate();
        }

        private void nudSweepAngle_ValueChanged(object sender, EventArgs e)
        {
            SweepAngle = (float)nudEndAngle.Value;
            picBoxArcPreview.Invalidate();
        }

        private void picBoxArcPreview_Paint(object sender, PaintEventArgs e)
        {
            Rectangle r = new Rectangle(3, 3, picBoxArcPreview.ClientRectangle.Width - 6, picBoxArcPreview.ClientRectangle.Height - 6);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int cx = picBoxArcPreview.ClientRectangle.Width / 2;
            int cy = picBoxArcPreview.ClientRectangle.Height / 2;
            int w = picBoxArcPreview.ClientRectangle.Width;
            int h = picBoxArcPreview.ClientRectangle.Height;
            e.Graphics.DrawEllipse(Pens.White, r);
            e.Graphics.DrawLine(Pens.White, 0, cy, w, cy);
            e.Graphics.DrawLine(Pens.White, cx, 0, cx, h);
            e.Graphics.DrawArc(Pens.Black, r, (float)nudStartAngle.Value, (float)nudEndAngle.Value);
        }

        private void button1_Click(object sender, EventArgs e)//Reset
        {
            PenThickness = 0.25f;
            nudPenThickness.Value = 0.25m;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    btnFillColor.BackColor = cd.Color;
                }
            }
        }
    }
}