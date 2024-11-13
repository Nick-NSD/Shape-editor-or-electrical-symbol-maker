namespace UserControls
{
    partial class FontControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FontControl));
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.rbLeft = new System.Windows.Forms.RadioButton();
            this.rbCenter = new System.Windows.Forms.RadioButton();
            this.rbRight = new System.Windows.Forms.RadioButton();
            this.rb270deg = new System.Windows.Forms.RadioButton();
            this.rb90deg = new System.Windows.Forms.RadioButton();
            this.rb0deg = new System.Windows.Forms.RadioButton();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(0, 1);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(191, 24);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox2.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox2.Image = ((System.Drawing.Image)(resources.GetObject("checkBox2.Image")));
            this.checkBox2.Location = new System.Drawing.Point(22, 33);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(24, 23);
            this.checkBox2.TabIndex = 1;
            this.toolTip1.SetToolTip(this.checkBox2, "Italic");
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox1.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox1.Image = ((System.Drawing.Image)(resources.GetObject("checkBox1.Image")));
            this.checkBox1.Location = new System.Drawing.Point(-1, 33);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(24, 23);
            this.checkBox1.TabIndex = 1;
            this.toolTip1.SetToolTip(this.checkBox1, "Bold");
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox3.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox3.Image = ((System.Drawing.Image)(resources.GetObject("checkBox3.Image")));
            this.checkBox3.Location = new System.Drawing.Point(45, 33);
            this.checkBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(24, 23);
            this.checkBox3.TabIndex = 1;
            this.toolTip1.SetToolTip(this.checkBox3, "Underline");
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox4.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox4.Image = ((System.Drawing.Image)(resources.GetObject("checkBox4.Image")));
            this.checkBox4.Location = new System.Drawing.Point(68, 33);
            this.checkBox4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(24, 23);
            this.checkBox4.TabIndex = 1;
            this.toolTip1.SetToolTip(this.checkBox4, "Strikeout");
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(92, 33);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 23);
            this.button1.TabIndex = 5;
            this.toolTip1.SetToolTip(this.button1, "Font color");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // rbLeft
            // 
            this.rbLeft.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbLeft.Image = ((System.Drawing.Image)(resources.GetObject("rbLeft.Image")));
            this.rbLeft.Location = new System.Drawing.Point(0, 0);
            this.rbLeft.Name = "rbLeft";
            this.rbLeft.Size = new System.Drawing.Size(24, 23);
            this.rbLeft.TabIndex = 7;
            this.rbLeft.Tag = "6";
            this.toolTip1.SetToolTip(this.rbLeft, "Left align");
            this.rbLeft.UseVisualStyleBackColor = true;
            this.rbLeft.CheckedChanged += new System.EventHandler(this.radioButtonAlign_CheckedChanged);
            // 
            // rbCenter
            // 
            this.rbCenter.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbCenter.Image = ((System.Drawing.Image)(resources.GetObject("rbCenter.Image")));
            this.rbCenter.Location = new System.Drawing.Point(24, 0);
            this.rbCenter.Name = "rbCenter";
            this.rbCenter.Size = new System.Drawing.Size(24, 23);
            this.rbCenter.TabIndex = 7;
            this.rbCenter.Tag = "7";
            this.toolTip1.SetToolTip(this.rbCenter, "Center align");
            this.rbCenter.UseVisualStyleBackColor = true;
            this.rbCenter.CheckedChanged += new System.EventHandler(this.radioButtonAlign_CheckedChanged);
            // 
            // rbRight
            // 
            this.rbRight.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbRight.Image = ((System.Drawing.Image)(resources.GetObject("rbRight.Image")));
            this.rbRight.Location = new System.Drawing.Point(48, 0);
            this.rbRight.Name = "rbRight";
            this.rbRight.Size = new System.Drawing.Size(24, 23);
            this.rbRight.TabIndex = 7;
            this.rbRight.Tag = "8";
            this.toolTip1.SetToolTip(this.rbRight, "Right align");
            this.rbRight.UseVisualStyleBackColor = true;
            this.rbRight.CheckedChanged += new System.EventHandler(this.radioButtonAlign_CheckedChanged);
            // 
            // rb270deg
            // 
            this.rb270deg.Appearance = System.Windows.Forms.Appearance.Button;
            this.rb270deg.Image = ((System.Drawing.Image)(resources.GetObject("rb270deg.Image")));
            this.rb270deg.Location = new System.Drawing.Point(48, 0);
            this.rb270deg.Name = "rb270deg";
            this.rb270deg.Size = new System.Drawing.Size(24, 23);
            this.rb270deg.TabIndex = 8;
            this.rb270deg.Tag = "11";
            this.toolTip1.SetToolTip(this.rb270deg, "Rotation 270°");
            this.rb270deg.UseVisualStyleBackColor = true;
            this.rb270deg.CheckedChanged += new System.EventHandler(this.radioButtonRotate_CheckedChanged);
            // 
            // rb90deg
            // 
            this.rb90deg.Appearance = System.Windows.Forms.Appearance.Button;
            this.rb90deg.Image = ((System.Drawing.Image)(resources.GetObject("rb90deg.Image")));
            this.rb90deg.Location = new System.Drawing.Point(24, 0);
            this.rb90deg.Name = "rb90deg";
            this.rb90deg.Size = new System.Drawing.Size(24, 23);
            this.rb90deg.TabIndex = 9;
            this.rb90deg.Tag = "10";
            this.toolTip1.SetToolTip(this.rb90deg, "Rotation 90°");
            this.rb90deg.UseVisualStyleBackColor = true;
            this.rb90deg.CheckedChanged += new System.EventHandler(this.radioButtonRotate_CheckedChanged);
            // 
            // rb0deg
            // 
            this.rb0deg.Appearance = System.Windows.Forms.Appearance.Button;
            this.rb0deg.Image = ((System.Drawing.Image)(resources.GetObject("rb0deg.Image")));
            this.rb0deg.Location = new System.Drawing.Point(0, 0);
            this.rb0deg.Name = "rb0deg";
            this.rb0deg.Size = new System.Drawing.Size(24, 23);
            this.rb0deg.TabIndex = 10;
            this.rb0deg.Tag = "9";
            this.toolTip1.SetToolTip(this.rb0deg, "Rotation 0°");
            this.rb0deg.UseVisualStyleBackColor = true;
            this.rb0deg.CheckedChanged += new System.EventHandler(this.radioButtonRotate_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(93, 56);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(21, 7);
            this.panel1.TabIndex = 6;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel2.Controls.Add(this.rbLeft);
            this.panel2.Controls.Add(this.rbRight);
            this.panel2.Controls.Add(this.rbCenter);
            this.panel2.Location = new System.Drawing.Point(119, 33);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(72, 30);
            this.panel2.TabIndex = 11;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel3.Controls.Add(this.rb0deg);
            this.panel3.Controls.Add(this.rb270deg);
            this.panel3.Controls.Add(this.rb90deg);
            this.panel3.Location = new System.Drawing.Point(195, 33);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(74, 30);
            this.panel3.TabIndex = 12;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DecimalPlaces = 1;
            this.numericUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown1.Location = new System.Drawing.Point(197, 2);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(72, 22);
            this.numericUpDown1.TabIndex = 13;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDown1.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // FontControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.panel2);
            this.Name = "FontControl";
            this.Size = new System.Drawing.Size(293, 67);
            this.Load += new System.EventHandler(this.FontControl_Load);
            this.Resize += new System.EventHandler(this.FontControl_Resize);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbLeft;
        private System.Windows.Forms.RadioButton rbCenter;
        private System.Windows.Forms.RadioButton rbRight;
        private System.Windows.Forms.RadioButton rb270deg;
        private System.Windows.Forms.RadioButton rb90deg;
        private System.Windows.Forms.RadioButton rb0deg;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
    }
}
