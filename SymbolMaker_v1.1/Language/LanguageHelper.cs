using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SymbolMaker
{
    public class LanguageHelper
    {
        // Method to save control texts to a file
        public static void SaveControlTexts(Form form, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Save regular controls
                foreach (Control control in form.Controls)
                {
                    SaveControlText(control, writer);
                }

                // Save MenuStrip and ToolStrip items
                foreach (Control control in form.Controls)
                {
                    if (control is MenuStrip menuStrip)
                    {
                        SaveMenuStripItems(menuStrip.Items, writer);
                    }
                    else if (control is ToolStrip toolStrip)
                    {
                        SaveToolStripItems(toolStrip.Items, writer);
                    }
                }
            }
        }

        // Helper method to save the control text recursively
        private static void SaveControlText(Control control, StreamWriter writer)
        {
            if (!string.IsNullOrEmpty(control.Text))
            {
                writer.WriteLine($"{control.Name}={control.Text}");
            }

            // If the control contains child controls, iterate through them recursively
            foreach (Control childControl in control.Controls)
            {
                SaveControlText(childControl, writer);
            }
        }

        // Save MenuStrip items
        private static void SaveMenuStripItems(ToolStripItemCollection items, StreamWriter writer)
        {
            foreach (ToolStripItem item in items)
            {
                if (!string.IsNullOrEmpty(item.Text))
                {
                    writer.WriteLine($"{item.Name}={item.Text}");
                }

                if (!string.IsNullOrEmpty(item.ToolTipText))
                {
                    writer.WriteLine($"{item.Name}.ToolTipText={item.ToolTipText}");
                }

                // If the item is a ToolStripMenuItem and contains dropdown items, save them as well
                if (item is ToolStripMenuItem menuItem && menuItem.DropDownItems.Count > 0)
                {
                    SaveMenuStripItems(menuItem.DropDownItems, writer);
                }
            }
        }

        // Save ToolStrip items
        private static void SaveToolStripItems(ToolStripItemCollection items, StreamWriter writer)
        {
            foreach (ToolStripItem item in items)
            {
                if (!string.IsNullOrEmpty(item.Text))
                {
                    writer.WriteLine($"{item.Name}={item.Text}");
                }

                if (!string.IsNullOrEmpty(item.ToolTipText))
                {
                    writer.WriteLine($"{item.Name}.ToolTipText={item.ToolTipText}");
                }
            }
        }

        //Load file language and apply changes
        public static void LoadControlTexts(Form form, string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"File not found: {filePath}");
                return;
            }

            var lines = File.ReadAllLines(filePath);
            var translations = new Dictionary<string, string>();

            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    translations[parts[0].Trim()] = parts[1].Trim();
                }
            }

            ApplyTranslations(form.Controls, translations);
        }

        private static void ApplyTranslations(Control.ControlCollection controls, Dictionary<string, string> translations)
        {
            foreach (Control control in controls)
            {
                if (translations.ContainsKey(control.Name))
                {
                    control.Text = translations[control.Name];
                }

                if (control.Controls.Count > 0)
                {
                    ApplyTranslations(control.Controls, translations);
                }

                if (control is MenuStrip menuStrip)
                {
                    ApplyMenuStripTranslations(menuStrip.Items, translations);
                }
                else if (control is ToolStrip toolStrip)
                {
                    ApplyToolStripTranslations(toolStrip.Items, translations);
                }
            }
        }

        private static void ApplyMenuStripTranslations(ToolStripItemCollection items, Dictionary<string, string> translations)
        {
            foreach (ToolStripItem item in items)
            {
                if (translations.ContainsKey(item.Name))
                {
                    item.Text = translations[item.Name];
                }

                if (translations.ContainsKey($"{item.Name}.ToolTipText"))
                {
                    item.ToolTipText = translations[$"{item.Name}.ToolTipText"];
                }

                if (item is ToolStripMenuItem menuItem && menuItem.DropDownItems.Count > 0)
                {
                    ApplyMenuStripTranslations(menuItem.DropDownItems, translations);
                }
            }
        }

        private static void ApplyToolStripTranslations(ToolStripItemCollection items, Dictionary<string, string> translations)
        {
            foreach (ToolStripItem item in items)
            {
                if (translations.ContainsKey(item.Name))
                {
                    item.Text = translations[item.Name];
                }

                if (translations.ContainsKey($"{item.Name}.ToolTipText"))
                {
                    item.ToolTipText = translations[$"{item.Name}.ToolTipText"];
                }
            }
        }
    }
}
