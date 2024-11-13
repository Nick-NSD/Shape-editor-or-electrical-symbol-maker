using SymbolMaker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UserControls
{
    public enum TextAlignment
    {
        LeftAlign,
        CenterAlign,
        RightAlign
    }

    public enum TextRotation
    {
        RotationZero,
        Rotation90,
        Rotation270
    }


    //set the default event for this user control
    [DefaultEvent("ValueChanged")]


    public partial class FontControl : UserControl
    {
        //set the initial values
        private Size controlSize = new Size(240, 84);
        private Color valueFontColor;
        private Font valueFont;
        private FontStyle valueStyle;
        private float valueFontSize;//in mm
        private TextAlignment valueAlignment;
        private TextRotation valueRotation;

        [Category("Font Control")]
        public TextAlignment ValueAlignment
        {
            get
            {
                return valueAlignment;
            }
            set
            {
                switch (value)
                {
                    case TextAlignment.LeftAlign:
                        rbLeft.Checked = true;
                        break;
                    case TextAlignment.CenterAlign:
                        rbCenter.Checked = true;
                        break;
                    case TextAlignment.RightAlign:
                        rbRight.Checked = true;
                        break;
                }
                valueAlignment = value;
                Invalidate();
            }
        }


        [Category("Font Control")]
        public TextRotation ValueRotation
        {
            get
            {
                return valueRotation;
            }
            set
            {
                switch (value)
                {
                    case TextRotation.RotationZero:
                        rb0deg.Checked = true;
                        break;
                    case TextRotation.Rotation90:
                        rb90deg.Checked = true;
                        break;
                    case TextRotation.Rotation270:
                        rb270deg.Checked = true;
                        break;
                }
                valueRotation = value;
                Invalidate();
            }
        }

        //set the event handler for this user control = ValueChanged
        public event EventHandler ValueChanged;

        //set the properties for this user control
        #region properties
        [Category("Font Control")]
        public Font ValueFont
        {
            get { return valueFont; }
            set
            {
                valueFont = value;
                if (valueFont != null)
                {
                    // Automatically set the ComboBox to the font name when the ValueFont is assigned
                    SetComboBoxToFontName(valueFont.Name);
                    Invalidate();
                }
            }
        }

        [Category("Font Control")]
        public FontStyle ValueStyle
        {
            get { return valueStyle; }
            set
            {
                valueStyle = value;
                checkBox1.Checked = value.HasFlag(FontStyle.Bold);
                checkBox2.Checked = value.HasFlag(FontStyle.Italic);
                checkBox3.Checked = value.HasFlag(FontStyle.Underline);
                checkBox4.Checked = value.HasFlag(FontStyle.Strikeout);
                Invalidate();
            }
        }

        [Category("Font Control")]
        public Color ValueFontColor
        {
            get
            {
                return valueFontColor;
            }

            set
            {
                valueFontColor = value;
                panel1.BackColor = value;
                Invalidate();
            }
        }

        [Category("Font Control")]
        public float ValueFontSize//in mm
        {
            get
            {
                return valueFontSize;
            }

            set
            {
                // Check if the value is a number and within the allowed range (1 to 100 mm)
                if (value >= 1 && value <= 100)
                {
                    numericUpDown1.Value = (decimal)value;
                    valueFontSize = value;
                    
                }
                Invalidate(); // Trigger a repaint to reflect changes
            }
        }
        #endregion


        //constructor
        public FontControl()
        {
            InitializeComponent();

            // Alignment RadioButtons
            rbLeft.Tag = TextAlignment.LeftAlign;
            rbCenter.Tag = TextAlignment.CenterAlign;
            rbRight.Tag = TextAlignment.RightAlign;

            // Rotation RadioButtons
            rb0deg.Tag = TextRotation.RotationZero;
            rb90deg.Tag = TextRotation.Rotation90;
            rb270deg.Tag = TextRotation.Rotation270;
            //comboBox2.Text = ValueFontSize.ToString();
        }

        public float MillimetersToPoints(float fontSizeInMillimeters)
        {
            // Convert millimeters to inches, then inches to points
            return (fontSizeInMillimeters / 25.4f) * 72.0f;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            ValueFont = new Font(comboBox1.SelectedItem.ToString(), MillimetersToPoints(ValueFontSize));
            OnValueChanged();
        }

        protected virtual void OnValueChanged()
        {
            valueStyle = FontStyle.Regular;
            if (checkBox1.Checked) valueStyle |= FontStyle.Bold;
            if (checkBox2.Checked) valueStyle |= FontStyle.Italic;
            if (checkBox3.Checked) valueStyle |= FontStyle.Underline;
            if (checkBox4.Checked) valueStyle |= FontStyle.Strikeout;

            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateControlLayout()
        {
            comboBox1.Width = controlSize.Width - numericUpDown1.Width - 7;
            comboBox1.DropDownWidth = controlSize.Width;

            numericUpDown1.Location = new Point(comboBox1.Right + 7, comboBox1.Top);
            numericUpDown1.Width = controlSize.Width - comboBox1.Width - 7;

            checkBox1.Height = comboBox1.Height;
            checkBox2.Height = comboBox1.Height;
            checkBox3.Height = comboBox1.Height;
            checkBox4.Height = comboBox1.Height;
            button1.Height = comboBox1.Height;
            rbLeft.Height = comboBox1.Height;
            rbCenter.Height = comboBox1.Height;
            rbRight.Height = comboBox1.Height;
            rb0deg.Height = comboBox1.Height;
            rb90deg.Height = comboBox1.Height;
            rb270deg.Height = comboBox1.Height;
            panel2.Height = comboBox1.Height;
            panel3.Height = comboBox1.Height;

            checkBox1.Location = new Point(0, comboBox1.Bottom + 5);
            checkBox2.Location = new Point(checkBox1.Right, comboBox1.Bottom + 5);
            checkBox3.Location = new Point(checkBox2.Right, comboBox1.Bottom + 5);
            checkBox4.Location = new Point(checkBox3.Right, comboBox1.Bottom + 5);
            button1.Location = new Point(checkBox4.Right, comboBox1.Bottom + 5);
            panel1.Location = new Point(button1.Left + 1, button1.Bottom);
            panel1.Width = button1.Width - 2;
            panel1.Height = 7;
            panel2.Location = new Point(button1.Right + 3, comboBox1.Bottom + 5);
            panel3.Location = new Point(panel2.Right + 3, panel2.Location.Y);
        }

        private void FontControl_Resize(object sender, EventArgs e)
        {
            controlSize = new Size(Width, Height);
            MinimumSize = new Size(270, 44);
            UpdateControlLayout();
        }

        private void FontControl_Load(object sender, EventArgs e)
        {
            MinimumSize = new Size(270, 44);
            UpdateControlLayout();

            //List<string> SystemFonts = FontFamily.Families
            //.Where(ff => ff.IsStyleAvailable(FontStyle.Regular))
            //.Select(ff => ff.Name)
            //.ToList();

            //comboBox1.Items.AddRange(SystemFonts.ToArray());

            foreach (FontFamily ff in FontFamily.Families)
            {
                if (ff.IsStyleAvailable(FontStyle.Regular))
                {
                    comboBox1.Items.Add(ff.Name.ToString());
                }
            }

            // Ensure ValueFont is not null before setting the ComboBox selected item
            if (ValueFont != null && comboBox1.Items.Contains(ValueFont.Name))
            {
                SetComboBoxToFontName(ValueFont.Name);
            }
        }

        public void SetComboBoxToFontName(string fontName)
        {
            //Find the index of the item in the ComboBox that matches the font name
            int index = comboBox1.Items.IndexOf(fontName);

            // If the font name is found in the ComboBox, set the selected index
            if (index != -1)
            {
                comboBox1.SelectedIndex = index;
            }
            else
            {
                // Optionally handle the case where the font name isn't found
                comboBox1.SelectedIndex = -1; // Set to no selection
            }
        }


        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            OnValueChanged();
            panel1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //font color
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                panel1.BackColor = colorDialog1.Color;
                ValueFontColor = colorDialog1.Color;
            }
            OnValueChanged();
            panel1.Focus();
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            panel1.BackColor = this.Enabled ? valueFontColor : Color.Silver;
        }

        private void radioButtonAlign_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null && radioButton.Checked)
            {
                ValueAlignment = (TextAlignment)radioButton.Tag;
                OnValueChanged();
            }
        }

        private void radioButtonRotate_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null && radioButton.Checked)
            {
                ValueRotation = (TextRotation)radioButton.Tag;
                OnValueChanged();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            //this is font size in mm
            ValueFontSize = (float)numericUpDown1.Value;
            OnValueChanged();
        }
    }
}
