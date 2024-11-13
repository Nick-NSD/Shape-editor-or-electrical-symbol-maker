namespace SymbolMaker.InputForms
{
    partial class CreateArcForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.nudStartAngle = new System.Windows.Forms.NumericUpDown();
            this.nudEndAngle = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudRadius = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lineStyleComboBox1 = new SymbolMaker.LineStyleComboBox();
            this.picBoxArcPreview = new System.Windows.Forms.PictureBox();
            this.hatchingComboBox1 = new SymbolMaker.HatchingComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.btnPenColor = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.nudPenThickness = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btnFillColor = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEndAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxArcPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPenThickness)).BeginInit();
            this.SuspendLayout();
            // 
            // nudStartAngle
            // 
            this.nudStartAngle.DecimalPlaces = 1;
            this.nudStartAngle.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudStartAngle.Location = new System.Drawing.Point(118, 13);
            this.nudStartAngle.Margin = new System.Windows.Forms.Padding(4);
            this.nudStartAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudStartAngle.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.nudStartAngle.Name = "nudStartAngle";
            this.nudStartAngle.Size = new System.Drawing.Size(75, 27);
            this.nudStartAngle.TabIndex = 0;
            this.nudStartAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudStartAngle.ValueChanged += new System.EventHandler(this.nudStartAngle_ValueChanged);
            // 
            // nudEndAngle
            // 
            this.nudEndAngle.DecimalPlaces = 1;
            this.nudEndAngle.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudEndAngle.Location = new System.Drawing.Point(118, 52);
            this.nudEndAngle.Margin = new System.Windows.Forms.Padding(4);
            this.nudEndAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudEndAngle.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.nudEndAngle.Name = "nudEndAngle";
            this.nudEndAngle.Size = new System.Drawing.Size(75, 27);
            this.nudEndAngle.TabIndex = 0;
            this.nudEndAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudEndAngle.Value = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudEndAngle.ValueChanged += new System.EventHandler(this.nudSweepAngle_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Start angle";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "End angle";
            // 
            // nudRadius
            // 
            this.nudRadius.DecimalPlaces = 2;
            this.nudRadius.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudRadius.Location = new System.Drawing.Point(118, 91);
            this.nudRadius.Margin = new System.Windows.Forms.Padding(4);
            this.nudRadius.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudRadius.Name = "nudRadius";
            this.nudRadius.Size = new System.Drawing.Size(75, 27);
            this.nudRadius.TabIndex = 0;
            this.nudRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudRadius.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 20);
            this.label3.TabIndex = 1;
            this.label3.Text = "Radius";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(196, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 20);
            this.label4.TabIndex = 1;
            this.label4.Text = "Degree";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(196, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 20);
            this.label5.TabIndex = 1;
            this.label5.Text = "Degree";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(196, 93);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 20);
            this.label6.TabIndex = 1;
            this.label6.Text = "mm";
            // 
            // lineStyleComboBox1
            // 
            this.lineStyleComboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lineStyleComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lineStyleComboBox1.FormattingEnabled = true;
            this.lineStyleComboBox1.Items.AddRange(new object[] {
            });
            this.lineStyleComboBox1.Location = new System.Drawing.Point(118, 193);
            this.lineStyleComboBox1.Name = "lineStyleComboBox1";
            this.lineStyleComboBox1.Size = new System.Drawing.Size(279, 28);
            this.lineStyleComboBox1.TabIndex = 3;
            this.lineStyleComboBox1.SelectedIndexChanged += new System.EventHandler(this.lineStyleComboBox1_SelectedIndexChanged);
            // 
            // picBoxArcPreview
            // 
            this.picBoxArcPreview.Location = new System.Drawing.Point(276, 12);
            this.picBoxArcPreview.Name = "picBoxArcPreview";
            this.picBoxArcPreview.Size = new System.Drawing.Size(120, 120);
            this.picBoxArcPreview.TabIndex = 2;
            this.picBoxArcPreview.TabStop = false;
            this.picBoxArcPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.picBoxArcPreview_Paint);
            // 
            // hatchingComboBox1
            // 
            this.hatchingComboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.hatchingComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hatchingComboBox1.FormattingEnabled = true;
            this.hatchingComboBox1.ItemHeight = 20;
            this.hatchingComboBox1.Items.AddRange(new object[] {
           });
            this.hatchingComboBox1.Location = new System.Drawing.Point(118, 327);
            this.hatchingComboBox1.Name = "hatchingComboBox1";
            this.hatchingComboBox1.SelectedHatchStyle = SymbolMaker.ShapeBase.CustomHatchStyle.None;
            this.hatchingComboBox1.Size = new System.Drawing.Size(279, 26);
            this.hatchingComboBox1.TabIndex = 4;
            this.hatchingComboBox1.SelectedIndexChanged += new System.EventHandler(this.hatchingComboBox1_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 196);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 20);
            this.label7.TabIndex = 1;
            this.label7.Text = "Line style";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 330);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 20);
            this.label8.TabIndex = 1;
            this.label8.Text = "Hatch style";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(197, 367);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 32);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(299, 367);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 32);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 151);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 20);
            this.label9.TabIndex = 6;
            this.label9.Text = "Line color";
            // 
            // btnPenColor
            // 
            this.btnPenColor.ForeColor = System.Drawing.Color.Silver;
            this.btnPenColor.Location = new System.Drawing.Point(118, 146);
            this.btnPenColor.Name = "btnPenColor";
            this.btnPenColor.Size = new System.Drawing.Size(279, 31);
            this.btnPenColor.TabIndex = 7;
            this.btnPenColor.Text = "Choose a color";
            this.btnPenColor.UseVisualStyleBackColor = true;
            this.btnPenColor.Click += new System.EventHandler(this.btnPenColor_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 239);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(85, 20);
            this.label10.TabIndex = 1;
            this.label10.Text = "Thickness";
            // 
            // nudPenThickness
            // 
            this.nudPenThickness.DecimalPlaces = 2;
            this.nudPenThickness.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudPenThickness.Location = new System.Drawing.Point(118, 237);
            this.nudPenThickness.Margin = new System.Windows.Forms.Padding(4);
            this.nudPenThickness.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudPenThickness.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudPenThickness.Name = "nudPenThickness";
            this.nudPenThickness.Size = new System.Drawing.Size(75, 27);
            this.nudPenThickness.TabIndex = 0;
            this.nudPenThickness.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudPenThickness.Value = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.nudPenThickness.ValueChanged += new System.EventHandler(this.nudPenThickness_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(196, 238);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 20);
            this.label11.TabIndex = 1;
            this.label11.Text = "mm";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 291);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(73, 20);
            this.label12.TabIndex = 6;
            this.label12.Text = "Fill color";
            // 
            // btnFillColor
            // 
            this.btnFillColor.ForeColor = System.Drawing.Color.Silver;
            this.btnFillColor.Location = new System.Drawing.Point(118, 280);
            this.btnFillColor.Name = "btnFillColor";
            this.btnFillColor.Size = new System.Drawing.Size(75, 31);
            this.btnFillColor.TabIndex = 7;
            this.btnFillColor.UseVisualStyleBackColor = true;
            this.btnFillColor.Click += new System.EventHandler(this.btnFillColor_Click);
            // 
            // button3
            // 
            this.button3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button3.Location = new System.Drawing.Point(197, 280);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(200, 31);
            this.button3.TabIndex = 7;
            this.button3.Text = "Transparent";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnFillTransp_Click);
            // 
            // CreateArcForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 410);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnFillColor);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.btnPenColor);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.hatchingComboBox1);
            this.Controls.Add(this.lineStyleComboBox1);
            this.Controls.Add(this.picBoxArcPreview);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudPenThickness);
            this.Controls.Add(this.nudRadius);
            this.Controls.Add(this.nudEndAngle);
            this.Controls.Add(this.nudStartAngle);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateArcForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Place arc or circle";
            ((System.ComponentModel.ISupportInitialize)(this.nudStartAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEndAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxArcPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPenThickness)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown nudStartAngle;
        private System.Windows.Forms.NumericUpDown nudEndAngle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox picBoxArcPreview;
        private System.Windows.Forms.NumericUpDown nudRadius;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private LineStyleComboBox lineStyleComboBox1;
        private HatchingComboBox hatchingComboBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnPenColor;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nudPenThickness;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnFillColor;
        private System.Windows.Forms.Button button3;
    }
}