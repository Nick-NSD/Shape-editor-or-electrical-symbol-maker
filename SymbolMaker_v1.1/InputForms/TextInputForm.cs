using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserControls;

namespace SymbolMaker
{
    public partial class TextInputForm : Form
    {
        public string InputText { get; set; }
        public Font TextFont { get; set; }
        public Color FontColor { get; set; }
        public TextAlignment NewTextAlignment { get; set; }
        public TextRotation NewTextRotation { get; set; }



        public TextInputForm(string currentName, Font currentFont, Color currentColor, TextAlignment currentAlignment, TextRotation currentRotation)
        {
            InitializeComponent();
            textBox1.Text = currentName;
            fontControl1.ValueFont = currentFont;
            fontControl1.ValueFontColor = currentColor;
            fontControl1.ValueFontSize = FontSizeToMillimeters(currentFont.Size);//ConvertpointToMm(pointSize)
            fontControl1.ValueStyle = currentFont.Style;
            fontControl1.ValueAlignment = currentAlignment;
            fontControl1.ValueRotation = currentRotation;
        }

        public float FontSizeToMillimeters(float fontSizeInPoints)
        {
            // Convert points to inches, then inches to millimeters
            return (fontSizeInPoints / 72.0f) * 25.4f;
        }

        public float MillimetersToPoints(float fontSizeInMillimeters)
        {
            // Convert millimeters to inches, then inches to points
            return (fontSizeInMillimeters / 25.4f) * 72.0f;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("If the text is empty, text placement is canceled.", "Cancel", MessageBoxButtons.OK, MessageBoxIcon.None);
                DialogResult = DialogResult.Cancel;
                btnCancel.Focus();
                return;
            }
            InputText = textBox1.Text;
            //TextFont = new Font(fontControl1.ValueFont.Name, fontControl1.ValueFontSize, fontControl1.ValueStyle);
            //Debug.WriteLine("fontControl1.ValueFontSize = " + fontControl1.ValueFontSize);
            //FontColor = fontControl1.ValueFontColor;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void fontControl1_ValueChanged(object sender, EventArgs e)
        {
            TextFont = new Font(fontControl1.ValueFont.Name, MillimetersToPoints(fontControl1.ValueFontSize), fontControl1.ValueStyle);
            FontColor = fontControl1.ValueFontColor;
            NewTextAlignment = fontControl1.ValueAlignment;
            NewTextRotation = fontControl1.ValueRotation;
        }
    }
}
