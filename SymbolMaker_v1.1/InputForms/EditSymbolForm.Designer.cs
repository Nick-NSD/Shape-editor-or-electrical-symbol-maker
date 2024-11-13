namespace SymbolMaker
{
    partial class EditSymbolForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditSymbolForm));
            this.txtName = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.listViewConnections = new System.Windows.Forms.ListView();
            this.btnRename = new System.Windows.Forms.Button();
            this.textBoxRename = new System.Windows.Forms.TextBox();
            this.ckNameVisible = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.fontControl2 = new UserControls.FontControl();
            this.fontControl1 = new UserControls.FontControl();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(71, 9);
            this.txtName.Margin = new System.Windows.Forms.Padding(2);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(212, 27);
            this.txtName.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(503, 306);
            this.btnSave.Margin = new System.Windows.Forms.Padding(2);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 31);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "OK";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(595, 306);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 31);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name";
            // 
            // listViewConnections
            // 
            this.listViewConnections.GridLines = true;
            this.listViewConnections.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewConnections.HideSelection = false;
            this.listViewConnections.Location = new System.Drawing.Point(5, 122);
            this.listViewConnections.Margin = new System.Windows.Forms.Padding(2);
            this.listViewConnections.Name = "listViewConnections";
            this.listViewConnections.Size = new System.Drawing.Size(344, 216);
            this.listViewConnections.TabIndex = 4;
            this.listViewConnections.UseCompatibleStateImageBehavior = false;
            this.listViewConnections.SelectedIndexChanged += new System.EventHandler(this.ListViewConnections_SelectedIndexChanged);
            // 
            // btnRename
            // 
            this.btnRename.Location = new System.Drawing.Point(377, 234);
            this.btnRename.Margin = new System.Windows.Forms.Padding(2);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(192, 31);
            this.btnRename.TabIndex = 5;
            this.btnRename.Text = "Rename connection";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // textBoxRename
            // 
            this.textBoxRename.Location = new System.Drawing.Point(377, 204);
            this.textBoxRename.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxRename.Name = "textBoxRename";
            this.textBoxRename.Size = new System.Drawing.Size(192, 27);
            this.textBoxRename.TabIndex = 6;
            // 
            // ckNameVisible
            // 
            this.ckNameVisible.AutoSize = true;
            this.ckNameVisible.Location = new System.Drawing.Point(289, 10);
            this.ckNameVisible.Name = "ckNameVisible";
            this.ckNameVisible.Size = new System.Drawing.Size(81, 24);
            this.ckNameVisible.TabIndex = 8;
            this.ckNameVisible.Text = "Visible";
            this.toolTip1.SetToolTip(this.ckNameVisible, "Visible");
            this.ckNameVisible.UseVisualStyleBackColor = true;
            this.ckNameVisible.CheckedChanged += new System.EventHandler(this.ckBoxNameVisible_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(6, 122);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(657, 24);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.Text = "                                              ";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.Location = new System.Drawing.Point(6, 152);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(657, 24);
            this.checkBox2.TabIndex = 14;
            this.checkBox2.Text = "                                            ";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(653, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 20);
            this.label2.TabIndex = 15;
            this.label2.Text = "mm";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(653, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 20);
            this.label3.TabIndex = 16;
            this.label3.Text = "mm";
            // 
            // fontControl2
            // 
            this.fontControl2.Location = new System.Drawing.Point(377, 122);
            this.fontControl2.MinimumSize = new System.Drawing.Size(270, 44);
            this.fontControl2.Name = "fontControl2";
            this.fontControl2.Size = new System.Drawing.Size(272, 77);
            this.fontControl2.TabIndex = 7;
            this.fontControl2.ValueAlignment = UserControls.TextAlignment.LeftAlign;
            this.fontControl2.ValueFont = null;
            this.fontControl2.ValueFontColor = System.Drawing.Color.Black;
            this.fontControl2.ValueFontSize = 4F;
            this.fontControl2.ValueRotation = UserControls.TextRotation.RotationZero;
            this.fontControl2.ValueStyle = System.Drawing.FontStyle.Regular;
            this.fontControl2.ValueChanged += new System.EventHandler(this.fontControl2_ValueChanged);
            // 
            // fontControl1
            // 
            this.fontControl1.Location = new System.Drawing.Point(377, 8);
            this.fontControl1.Margin = new System.Windows.Forms.Padding(2);
            this.fontControl1.MinimumSize = new System.Drawing.Size(270, 44);
            this.fontControl1.Name = "fontControl1";
            this.fontControl1.Size = new System.Drawing.Size(272, 77);
            this.fontControl1.TabIndex = 3;
            this.fontControl1.ValueAlignment = UserControls.TextAlignment.RightAlign;
            this.fontControl1.ValueFont = new System.Drawing.Font("Arial", 11.33858F);
            this.fontControl1.ValueFontColor = System.Drawing.Color.Black;
            this.fontControl1.ValueFontSize = 4F;
            this.fontControl1.ValueRotation = UserControls.TextRotation.RotationZero;
            this.fontControl1.ValueStyle = System.Drawing.FontStyle.Regular;
            this.fontControl1.ValueChanged += new System.EventHandler(this.fontControl1_ValueChanged);
            // 
            // EditSymbolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 345);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ckNameVisible);
            this.Controls.Add(this.fontControl2);
            this.Controls.Add(this.textBoxRename);
            this.Controls.Add(this.btnRename);
            this.Controls.Add(this.listViewConnections);
            this.Controls.Add(this.fontControl1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.checkBox2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditSymbolForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Symbol";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private UserControls.FontControl fontControl1;
        private System.Windows.Forms.ListView listViewConnections;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.TextBox textBoxRename;
        private UserControls.FontControl fontControl2;
        private System.Windows.Forms.CheckBox ckNameVisible;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}