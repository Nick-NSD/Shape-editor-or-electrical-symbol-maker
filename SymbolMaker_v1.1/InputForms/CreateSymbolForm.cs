using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UserControls;

namespace SymbolMaker
{
    public partial class CreateSymbolForm : Form
    {
        public string InputName { get; set; }
        public bool InputVisible { get; set; }
        public Font InputTextFont { get; set; }
        public Color InputFontColor { get; set; }
        public string InputSymbolType { get; set; }
        public TextAlignment InputTextAlignment { get; set; }
        public TextRotation InputTextRotation { get; set; }


        public CreateSymbolForm(string currentName, bool visible, Font currentFont, TextAlignment currentAlignment, TextRotation currentRotation, Color currentColor, SymbolShape symbol)
        {
            InitializeComponent();
            textBox1.Validating += textBox1_Validating;


            textBox1.Text = currentName;
            checkBox1.Checked = visible;
            fontControl1.ValueFont = currentFont;
            fontControl1.ValueFontColor = currentColor;
            fontControl1.ValueFontSize = FontSizeToMillimeters(currentFont.Size);//ConvertpointToMm(pointSize)
            fontControl1.ValueAlignment = currentAlignment;//for symbol name
            fontControl1.ValueRotation = currentRotation;//for symbol name

            comboBox1.SelectedIndex = 0;
            Image img = GraphExtension.GenerateThumbnail(symbol);
            pictureBox1.Image = img;
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
            InputName = textBox1.Text;
            InputVisible = checkBox1.Checked;
            // here we need to convert font size from millimeter to points because Font need emSize for font size
            InputTextFont = new Font(
                fontControl1.ValueFont.Name,
                MillimetersToPoints(fontControl1.ValueFontSize),
                fontControl1.ValueStyle);
            InputFontColor = fontControl1.ValueFontColor;
            InputTextAlignment = fontControl1.ValueAlignment;
            InputTextRotation = fontControl1.ValueRotation;
            InputSymbolType = comboBox1.SelectedItem.ToString();

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
            InputTextFont = new Font(
                fontControl1.ValueFont.Name,
                MillimetersToPoints(fontControl1.ValueFontSize),
                fontControl1.ValueStyle);
            InputFontColor = fontControl1.ValueFontColor;
            InputTextAlignment = fontControl1.ValueAlignment;
            InputTextRotation = fontControl1.ValueRotation;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            InputSymbolType = comboBox1.SelectedItem.ToString();
            string suggestedName = string.Empty;

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    suggestedName = "K";
                    break;
                case 1:
                    suggestedName = "S";
                    break;
                case 2:
                    suggestedName = "S";
                    break;
                case 3:
                    suggestedName = "B";
                    break;
                case 4:
                    suggestedName = "g";
                    break;
                case 5:
                    suggestedName = "M";
                    break;
                case 6:
                    suggestedName = "G";
                    break;
                case 7:
                    suggestedName = "D";
                    break;
                case 8:
                    suggestedName = "T";
                    break;
                case 9:
                    suggestedName = "L";
                    break;
                case 10:
                    suggestedName = "Cap";
                    break;
                case 11:
                    suggestedName = "C";
                    break;
                case 12:
                    suggestedName = "Ter";
                    break;
                case 13:
                    suggestedName = "Graph";
                    break;
                case 14:
                    suggestedName = "Sign";
                    break;
                case 15:
                    suggestedName = "F";
                    break;
            }

            //if (IsReservedFileName(suggestedName))
            //{
            //    MessageBox.Show($"The name '{suggestedName}' is a reserved name and cannot be used. It will be renamed to '{suggestedName}_symbol'.",
            //                    "Reserved Name Warning",
            //                    MessageBoxButtons.OK,
            //                    MessageBoxIcon.Warning);
            //    suggestedName += "_symbol"; // Modify to ensure it's not a reserved name
            //}

            textBox1.Text = suggestedName;
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            string name = textBox1.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("The name cannot be empty.", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true; 
                textBox1.Focus();
                return;
            }

            // Check if the entered name is a reserved file name
            if (IsReservedFileName(textBox1.Text))
            {
                MessageBox.Show($"The name '{textBox1.Text}' is a reserved name and cannot be used. Please enter a different name.",
                                "Reserved Name Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                e.Cancel = true;
                textBox1.Focus(); 
                textBox1.SelectAll(); // Select all text for easy correction
            }
        }


        private bool IsReservedFileName(string fileName)
        {
            // List of reserved device names in Windows
            string[] reservedNames = new string[]
            {
        "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            };

            // Compare case-insensitive
            return reservedNames.Contains(fileName.ToUpper());
        }



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            InputVisible = checkBox1.Checked;
        }
    }
}
