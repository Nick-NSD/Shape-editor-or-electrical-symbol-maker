using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SymbolMaker
{
    public class XmlHelper
    {
        public static HashSet<string> loadedFiles = new HashSet<string>();


        public static void SaveShapesToFile(List<ShapeBase> shapes, bool isSymbol)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                // Determine the file extension based on whether it's a symbol or a drawing
                saveFileDialog.Filter = isSymbol ? "Symbol Files (*.sim)|*.sim"
                                                 : "Drawing Files (*.sch)|*.sch";
                saveFileDialog.Title = isSymbol ? "Save Symbol" : "Save Drawing";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    if (isSymbol)
                    {
                        // Filter to include only SymbolShape instances and cast to ShapeBase
                        List<ShapeBase> symbolShapes = shapes.OfType<SymbolShape>().Cast<ShapeBase>().ToList();

                        // Create an XmlSerializer for the List<ShapeBase> type
                        XmlSerializer serializer = new XmlSerializer(typeof(List<ShapeBase>), new Type[]
                        {
                                typeof(SymbolShape), // Only include SymbolShape in the serializer for symbols
                        });

                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            serializer.Serialize(writer, symbolShapes);
                        }
                    }
                    else
                    {
                        // Create an XmlSerializer for the List<ShapeBase> type
                        XmlSerializer serializer = new XmlSerializer(typeof(List<ShapeBase>), new Type[]
                        {
                            typeof(ArcShape),
                            typeof(ConnectionShape),
                            typeof(EllipseShape),
                            typeof(LineShape),
                            typeof(PolygonShape),
                            typeof(RectangleShape),
                            typeof(SymbolShape),
                            typeof(TextShape),
                            typeof(DotShape)
                        });

                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            serializer.Serialize(writer, shapes);
                        }
                    }
                }
            }
        }

        public static List<ShapeBase> LoadShapesFromFile(bool isSymbol)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = !isSymbol ? "Drawing Files (*.sch)|*.sch" : "Symbol Files (*.sim)|*.sim";

                openFileDialog.Title = !isSymbol ? "Open drawing" : "Open symbol";

                var shapes = new List<ShapeBase>();

                // Create an XmlSerializer for the List<ShapeBase> type with all shapes
                var serializer = new XmlSerializer(typeof(List<ShapeBase>), new Type[]
                {
                    typeof(ArcShape),
                    typeof(ConnectionShape),
                    typeof(EllipseShape),
                    typeof(LineShape),
                    typeof(PolygonShape),
                    typeof(RectangleShape),
                    typeof(SymbolShape),
                    typeof(TextShape),
                    typeof(DotShape)
                });

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    if (isSymbol) // Load symbol
                    {
                        // Create an XmlSerializer for the List<ShapeBase> type with only SymbolShape
                        serializer = new XmlSerializer(typeof(List<ShapeBase>), new Type[] { typeof(SymbolShape) });

                        using (StreamReader reader = new StreamReader(filePath))
                        {
                            try
                            {
                                shapes = (List<ShapeBase>)serializer.Deserialize(reader);
                                return shapes; // Return the loaded shapes
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Invalid file format");
                            }
                        }
                    }
                    else if (!loadedFiles.Contains(filePath)) //try to load drawing
                    {
                        using (StreamReader reader = new StreamReader(filePath))
                        {
                            try
                            {
                                shapes = (List<ShapeBase>)serializer.Deserialize(reader);
                                loadedFiles.Add(filePath);
                                return shapes; // Return the loaded shapes
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Invalid file format");
                            }
                        }
                    }

                    else
                    {
                        var result = MessageBox.Show("This file is already loaded.\nContinue?", "File Already Loaded", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (result == DialogResult.OK)
                        {
                            using (StreamReader reader = new StreamReader(filePath))
                            {
                                try
                                {
                                    shapes = (List<ShapeBase>)serializer.Deserialize(reader);
                                    loadedFiles.Add(filePath);
                                    return shapes; // Return the loaded shapes
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, "Invalid file format");
                                }
                            }

                        }
                        return null;

                    }
                }
            }
            return null; // Return null if no shapes were loaded
        }
    }
}
