using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using UserControls;

namespace SymbolMaker
{
    public partial class EditSymbolForm : Form
    {
        public string NewTextName { get; set; }
        public Font NewTextFont { get; set; }
        public Color NewTextColor { get; set; }
        private bool nameVisible;
        public bool SetAllSymbolConnections { get; set; }
        public bool SetAllSymbolsConnections { get; set; }
        public bool SetAllSymbolNames { get; set; }
        public TextAlignment NewTextAlignment { get; set; }
        public TextRotation NewTextRotation { get; set; }
        public List<ConnectionShape> NewConnectionList { get; set; }

        public bool NewNameVisible
        {
            get
            {
                return nameVisible;
            }

            set
            {
                nameVisible = value;
                ckNameVisible.Checked = value;
            }
        }

        public EditSymbolForm(string currentName, bool nameVis, bool ckBoxEnabled, Font currentFont, Color currentColor, TextAlignment currentAlignment, TextRotation currentRotation, List<ConnectionShape> connections, string checkBox1Text, string checkBox2Text, bool setAllSymbolConnections, bool setAllSymbolsConnections, bool setAllSymbolNames)
        {
            InitializeComponent();
            txtName.Text = currentName;
            NewNameVisible = nameVis;
            ckNameVisible.Enabled = ckBoxEnabled;
            fontControl1.ValueFont = currentFont;
            fontControl1.ValueFontColor = currentColor;
            fontControl1.ValueFontSize = FontSizeToMillimeters(currentFont.Size);
            fontControl1.ValueStyle = currentFont.Style;
            fontControl1.ValueAlignment = currentAlignment;
            fontControl1.ValueRotation = currentRotation;
            NewConnectionList = connections;

            checkBox1.Text = checkBox1Text;
            checkBox2.Text = checkBox2Text;
            //checkBox3.Text = checkBox3Text;

            SetAllSymbolConnections = setAllSymbolConnections;
            if (checkBox1.Text != "Apply these settings to all symbol names") checkBox1.Checked = setAllSymbolConnections;
            else checkBox1.Checked = setAllSymbolNames;

            SetAllSymbolsConnections = setAllSymbolsConnections;
            checkBox2.Checked = setAllSymbolsConnections;

            SetAllSymbolNames = setAllSymbolNames;
            //checkBox3.Checked = setAllSymbolNames;
            InitializeListView();
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            NewTextName = txtName.Text;
            NewTextFont = new Font(
               fontControl1.ValueFont.Name,
                MillimetersToPoints(fontControl1.ValueFontSize),
               fontControl1.ValueStyle
               );
            NewTextAlignment = fontControl1.ValueAlignment;
            NewTextRotation = fontControl1.ValueRotation;
            NewTextColor = fontControl1.ValueFontColor;
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
            NewTextColor = fontControl1.ValueFontColor;
            NewTextAlignment = fontControl1.ValueAlignment;
            NewTextRotation = fontControl1.ValueRotation;
            NewTextFont = new Font(
                fontControl1.ValueFont.Name,
                 MillimetersToPoints(fontControl1.ValueFontSize),
                fontControl1.ValueStyle
                );

        }

        private void InitializeListView()
        {
            listViewConnections.FullRowSelect = true;
            listViewConnections.GridLines = true;
            listViewConnections.View = View.Details;
            listViewConnections.Columns.Clear();
            listViewConnections.Columns.Add("Visible", 70);
            listViewConnections.Columns.Add("Connection Name", 150);
            listViewConnections.Columns.Add("No.", 50);
            listViewConnections.BackColor = Color.Gainsboro;

            listViewConnections.CheckBoxes = true;

            // Subscribe to the ItemChecked event
            listViewConnections.ItemChecked += new ItemCheckedEventHandler(listViewConnections_ItemChecked);

            PopulateConnectionsList();

            // Handle the selection change event
            listViewConnections.SelectedIndexChanged += ListViewConnections_SelectedIndexChanged;
        }

        // Event handler for checkbox state changes
        private void listViewConnections_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // Get the checked item
            ListViewItem item = e.Item;

            ConnectionShape connection = item.Tag as ConnectionShape;

            if (connection != null)
            {
                // Toggle visibility of the connection's name based on checkbox state
                connection.ConnectionNameVisible = item.Checked;
            }
        }


        // Populate the ListView with connection names
        private void PopulateConnectionsList()
        {
            if (NewConnectionList == null || NewConnectionList.Count == 1)
            {
                // Make invisible the ListView if no connections are available
                listViewConnections.Visible = false;
                textBoxRename.Visible = false;
                btnRename.Visible = false;
                checkBox1.Visible = true;
                checkBox2.Visible = true;
                fontControl2.Visible = false;
                label3.Visible = false;
                //Change Form height
                // Place a connection
                if (checkBox1.Text == "" && checkBox2.Text == "")
                {
                    checkBox1.Visible = false;
                    checkBox2.Visible = false;

                    Size = new Size(Width, fontControl1.Height * 3);
                }

                if (checkBox1.Text != "" && checkBox2.Text == "")
                {
                    Size = new Size(Width, fontControl1.Height * 4);
                    checkBox2.Visible = false;
                }

                if (checkBox1.Text != "" && checkBox2.Text != "") Size = new Size(Width, fontControl1.Height * 5);

                return;
            }

            // Clear existing items before populating
            listViewConnections.Items.Clear();

            if (NewConnectionList != null && NewConnectionList.Count > 1)
            {
                int index = 1; // Start numbering connections from 1
                checkBox1.Visible = false;
                checkBox2.Visible = false;

                foreach (var connection in NewConnectionList)
                {
                    // Create a new ListViewItem
                    ListViewItem listViewItem = new ListViewItem();

                    // First column = column 0 is for the check boxes
                    listViewItem.Checked = connection.ConnectionNameVisible;

                    // Add the connection name as the 1 column (sub-item)
                    listViewItem.SubItems.Add(connection.ConnectionName.StringText);

                    // Column 2 is the connection number
                    listViewItem.SubItems.Add(index.ToString() + ".");

                    // Store the connection object in the Tag for later use
                    listViewItem.Tag = connection;

                    // Add the item to the ListView
                    listViewConnections.Items.Add(listViewItem);

                    // Increment index for the next connection
                    index++;
                }
            }
        }

        // Handle when the user selects a connection in the ListView
        private void ListViewConnections_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewConnections.SelectedItems.Count > 0)
            {
                // Get the selected connection
                var selectedItem = listViewConnections.SelectedItems[0];
                var connection = (ConnectionShape)selectedItem.Tag;

                // Display the current name in the textbox for editing
                textBoxRename.Text = connection.ConnectionName.StringText;
                textBoxRename.Tag = connection.ConnectionName.StringText;

                // Display the current connection name in the fontControl2 for editing
                fontControl2.ValueFont = connection.ConnectionName.TextFont;
                fontControl2.ValueFontColor = connection.ConnectionName.TextColor;
                fontControl2.ValueFontSize = FontSizeToMillimeters(connection.ConnectionName.TextFont.Size);
                fontControl2.ValueStyle = connection.ConnectionName.TextFont.Style;
                fontControl2.ValueAlignment = connection.ConnectionName.TextShapeAlign;
                fontControl2.ValueRotation = connection.ConnectionName.TextShapeRotation;
                fontControl2.SetComboBoxToFontName(connection.ConnectionName.TextFont.Name);
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            // Ensure the ListView has items and the connection list is not null
            if (NewConnectionList != null && NewConnectionList.Count > 0)
            {
                // Get the new name from the TextBox
                string newName = textBoxRename.Text.Trim();

                // Ensure the name is not empty or whitespace
                if (!string.IsNullOrEmpty(newName))
                {
                    // Get the selected item in the ListView
                    if (listViewConnections.SelectedItems.Count > 0)
                    {
                        ListViewItem selectedItem = listViewConnections.SelectedItems[0];

                        // Access the associated connection object
                        var connection = (ConnectionShape)selectedItem.Tag;

                        // Set the new name for the connection
                        connection.ConnectionName.StringText = newName;

                        // Update the list view item text in the second column (index 1)
                        selectedItem.SubItems[1].Text = newName;

                        // Clear the TextBox after renaming
                        textBoxRename.Text = string.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid new name.");
                }

                // Refresh the ListView to reflect the changes in the UI
                listViewConnections.Refresh();
            }
        }

        // Change properties for selected connection: font, size, style, alignment and orientation
        private void fontControl2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void ckBoxNameVisible_CheckedChanged(object sender, EventArgs e)
        {
            NewNameVisible = ckNameVisible.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Text != "Apply these settings to all symbol names") SetAllSymbolConnections = checkBox1.Checked;
            else SetAllSymbolNames = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            SetAllSymbolsConnections = checkBox2.Checked;
        }
    }
}

