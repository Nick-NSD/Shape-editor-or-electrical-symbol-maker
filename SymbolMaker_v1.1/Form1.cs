/*
 04.10.2024 
 Author: Nicu Gurămultă - Delgaz Grid, Bacău, România.

 The project is free from any kind of restrictions.
 The code can be used, copied, modified without any problem by anyone.
 
 All CAD programs use vectors.
 Here is an example of creating and using vectors. Watch all the videos:
 https://www.youtube.com/watch?v=oBFdj7-rhIQ&list=PLH5F2POIsTGARn0aRGNBW5bICEgK0Jb1A&index=1
  */


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using UserControls;
using static SymbolMaker.ShapeBase;

namespace SymbolMaker
{
    public partial class Form1 : Form
    {
        public const double DPI = 96;
        private bool allowMoving;
        private bool allowDrawing;
        private bool allowSelecting;
        private bool allowDetecting;
        private bool allowResizing;
        private bool isMouseDown;
        private bool textIsEmpty;
        private bool isPanning;

        private bool IsStartPoint;
        private bool IsEndPoint;
        private bool setAllSymbolConnections;
        private bool setAllSymbolsConnections;
        private bool setAllSymbolNames;

        public static int connectionNumber = -1;
        private int selectedVertexIndex = -1;
        private int pageSetupIndex = 1;

        private double gridSize = 4;
        private double[] gridSizes = { 16.0, 8.0, 4.0, 2.0, 1.0, 0.5, 0.25, 0.125 };
        private double pixelsPerMillimeterX;
        private double pixelsPerMillimeterY;

        private int highlightWidth = 1;
        private Color highlightColor = Color.FromArgb(255, Color.White);
        private int rtolerance = 4;
        private int ltolerance = 10;

        private List<PointV2D> vertices = new List<PointV2D>();//points for polygon
        private List<ShapeBase> shapes; //list of all shapes including groups
        private List<ShapeBase> selectedShapes; //list of all shapes belong to symbol
        private List<ShapeBase> groupedShapes = new List<ShapeBase>();//list of all shapes belong to group
        private List<SymbolShape> loadedSymbols = new List<SymbolShape>();//list of all symbols loaded

        private ShapeBase detectedInternalShape = null;
        private SymbolShape detectedSymbol = null;
        private TextShape connectionName = null;
        private TextShape tempTextShape = new TextShape();
        private ConnectionShape connectionShape = null;
        private ShapeBase detectedShape = null;
        private ShapeBase currentShape = null;
        private GroupShape groupShape = new GroupShape();

        private PointV2D mouseCurrentLocation = new PointV2D();
        private PointV2D mousePoint = new PointV2D();
        private PointV2D startPoint = new PointV2D();
        private PointV2D endPoint = new PointV2D();
        private PointV2D lastMousePosition;
        private Point selRectMouse;

        private double viewportOffsetX = 0;
        private double viewportOffsetY = 0;

        private ToolStripButton[] ToolStripShapeButtons;
        private float startAngle = 0;
        private float sweepAngle = -270;
        private float inpDotRadius = 3.79524f;//pixels = 1mm

        private string inpText = "Text"; // default text
        private bool inpVisible = true;

        private FontStyle inpFontStyle = FontStyle.Regular; // default font style
        private Font inpFont = new Font("Arial", 4f); // default font family and size
        private Color inpFontCol = Color.Black;
        private TextAlignment inpTextAlignment = TextAlignment.LeftAlign;
        private TextRotation inpTextRotation = TextRotation.RotationZero;
        private TextAlignment inpConnAlignment = TextAlignment.LeftAlign;
        private TextRotation inpConnRotation = TextRotation.RotationZero;
        private TextAlignment inpSymNameAlignment = TextAlignment.RightAlign;
        private TextRotation inpSymNameRotation = TextRotation.RotationZero;

        private Color shapePenColor = Color.Black;
        private Color inpDotColor = Color.Black;
        private Color shapeFillColor = Color.Transparent;
        private int mouseNumClick = 1;
        private string tool;

        private Rectangle rect;
        private RectangleF selectionRectangle = Rectangle.Empty;
        private Rectangle textBounds;
        private ShapeEdge detectedEdge = ShapeEdge.None;
        private PrintDocument printDocument;
        private Graphics b;
        private Graphics g;
        private HashSet<string> loadedFiles = new HashSet<string>();

        public static double zoomFactor = 1.0f;

        private PictureBox thumbnailPicBox;

        private Point panOffset;

        private CommandManager commandManager;
        private MoveShapeCommand currentMoveCommand;
        private ResizeShapeCommand currentResizeCommand;
        private DeleteShapeCommand currentDeleteCommand;

        //private Cursor drawLineCursor;
        //private Cursor drawRectCursor;
        //private Cursor drawElliCursor;
        //private Cursor drawArcCursor;
        //private Cursor drawPolyCursor;
        //private Cursor drawCircCursor;
        //private Cursor drawCenterArcCursor;


        public Form1()
        {
            InitializeComponent();
            EnableDoubleBuffering(panelBkg);
            commandManager = new CommandManager();
            shapes = new List<ShapeBase>();
            selectedShapes = new List<ShapeBase>();
            detectedSymbol = new SymbolShape();
            connectionShape = new ConnectionShape();
            connectionName = new TextShape();

            inpFont = new Font("Arial", MillimetersToPoints(4), inpFontStyle);

            printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(PrintPage);

            pictureBox1.MouseWheel += new MouseEventHandler(pictureBox1_MouseWheel);
        }

        private void EnableDoubleBuffering(Panel panel)
        {
            panel.GetType().InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, panel, new object[] { true });
        }

        public float MillimetersToPoints(float fontSizeInMillimeters)
        {
            // Convert millimeters to inches, then inches to points
            return (fontSizeInMillimeters / 25.4f) * 72.0f;
        }

        #region Undo/Redo
        private void btnUndo_Click(object sender, EventArgs e)
        {
            commandManager.Undo();
            UpdateUndoRedoButtons();
            pictureBox1.Invalidate();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            commandManager.Redo();
            UpdateUndoRedoButtons();
            pictureBox1.Invalidate();
        }

        private void UpdateUndoRedoButtons()
        {
            toolUndo.Enabled = commandManager.CanUndo;
            toolRedo.Enabled = commandManager.CanRedo;
        }
        #endregion

        #region Drawing Methods
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            //g.TranslateTransform(panOffset.X, panOffset.Y);

            if (textIsEmpty)
            {
                textIsEmpty = false;
                SimulateKeyDown(Keys.Escape);
            }

            //DrawGrid(g, panOffset);
            DrawShapeBeforeMouseUp(g);
            DrawShapeAfterMouseUp(g);
            HighligthDetectedShape(g);
            DrawSelectionRectangle(g);
            DrawTextBoundBeforeClick(g);
            DrawConnectionBeforeClick(g);
            DrawDotBeforeClick(g);

            //g.TranslateTransform(-panOffset.X, -panOffset.Y);
        }

        private void DrawTextBoundBeforeClick(Graphics g)
        {
            if (tool == "Text")
            {
                g.ScaleTransform((float)zoomFactor, (float)zoomFactor);
                //textBounds is drawing when mouse is moving on screen
                using (Pen pen = new Pen(Color.Black, 0.25f))
                {
                    pen.DashPattern = new float[] { 2, 2 };
                    pen.DashStyle = DashStyle.Custom;

                    switch (inpTextAlignment)
                    {
                        case TextAlignment.LeftAlign:
                            rect = new Rectangle((int)mouseCurrentLocation.X, (int)mouseCurrentLocation.Y, textBounds.Width, textBounds.Height);
                            break;
                        case TextAlignment.CenterAlign:
                            rect = new Rectangle((int)mouseCurrentLocation.X - textBounds.Width / 2, (int)mouseCurrentLocation.Y, textBounds.Width, textBounds.Height);
                            break;
                        case TextAlignment.RightAlign:
                            rect = new Rectangle((int)mouseCurrentLocation.X - textBounds.Width, (int)mouseCurrentLocation.Y, textBounds.Width, textBounds.Height);
                            break;
                    }

                    switch (inpTextRotation)
                    {
                        case TextRotation.RotationZero:
                            g.DrawRectangle(pen, rect);
                            break;

                        case TextRotation.Rotation90:
                            RotateRectangle(-90);
                            break;

                        case TextRotation.Rotation270:
                            RotateRectangle(90);
                            break;
                    }

                    g.DrawRectangle(pen, rect);
                    g.FillEllipse(Brushes.Red, (float)mouseCurrentLocation.X - 2, (float)mouseCurrentLocation.Y - 2, 4, 4);
                }
                g.ResetTransform();
            }
        }

        private Rectangle RotateRectangle(int angle)
        {
            switch (inpTextAlignment)
            {
                case TextAlignment.LeftAlign:
                    if (angle == -90)
                        rect = new Rectangle((int)mouseCurrentLocation.X, (int)mouseCurrentLocation.Y - textBounds.Width, textBounds.Height, textBounds.Width);
                    if (angle == 90)
                        rect = new Rectangle((int)mouseCurrentLocation.X - textBounds.Height, (int)mouseCurrentLocation.Y, textBounds.Height, textBounds.Width);
                    break;
                case TextAlignment.CenterAlign:
                    if (angle == -90)
                        rect = new Rectangle((int)mouseCurrentLocation.X, (int)mouseCurrentLocation.Y - textBounds.Width / 2, textBounds.Height, textBounds.Width);
                    if (angle == 90)
                        rect = new Rectangle((int)mouseCurrentLocation.X - textBounds.Height, (int)mouseCurrentLocation.Y - textBounds.Width / 2, textBounds.Height, textBounds.Width);

                    break;
                case TextAlignment.RightAlign:
                    if (angle == -90)
                        rect = new Rectangle((int)mouseCurrentLocation.X, (int)mouseCurrentLocation.Y, textBounds.Height, textBounds.Width);
                    if (angle == 90)
                        rect = new Rectangle((int)mouseCurrentLocation.X - textBounds.Height, (int)mouseCurrentLocation.Y - textBounds.Width, textBounds.Height, textBounds.Width);
                    break;
            }
            return rect;
        }

        private void DrawConnectionBeforeClick(Graphics g)
        {
            if (tool == "Connection")
            {
                //textBounds for connection is drawing when mouse is moving on screen
                using (Pen pen = new Pen(Color.Black, 0.25f))
                {
                    g.ScaleTransform((float)zoomFactor, (float)zoomFactor);
                    g.DrawEllipse(pen, (float)mouseCurrentLocation.X - 4, (float)mouseCurrentLocation.Y - 4, 8, 8);
                    g.ResetTransform();
                }
            }
        }

        private void DrawDotBeforeClick(Graphics g)
        {
            if (tool == "Dot")
            {
                //textBounds for connection is drawing when mouse is moving on screen
                using (Pen pen = new Pen(Color.Black, 0.25f))
                {
                    g.ScaleTransform((float)zoomFactor, (float)zoomFactor);
                    g.DrawEllipse(pen, (float)mouseCurrentLocation.X - 4, (float)mouseCurrentLocation.Y - 4, 8, 8);
                    g.ResetTransform();
                }
            }
        }

        private void DrawShapeBeforeMouseUp(Graphics g)
        {
            if (allowDrawing)
            {
                // Apply zoom transformation
                g.ScaleTransform((float)zoomFactor, (float)zoomFactor);
                using (Pen p = new Pen(Color.Black, 0.15f))
                {
                    if (toolRectangle.Checked)
                    {
                        DrawRectangle(g, p);

                    }
                    if (toolEllipse.Checked)
                    {
                        float x = (float)Math.Min(startPoint.X, mouseCurrentLocation.X);
                        float y = (float)Math.Min(startPoint.Y, mouseCurrentLocation.Y);
                        float width = (float)Math.Abs(mouseCurrentLocation.X - startPoint.X);
                        float height = (float)Math.Abs(mouseCurrentLocation.Y - startPoint.Y);
                        g.DrawEllipse(p, x, y, width, height);
                    }
                    if (toolArc.Checked)
                    {
                        float x = (float)Math.Min(startPoint.X, mouseCurrentLocation.X);
                        float y = (float)Math.Min(startPoint.Y, mouseCurrentLocation.Y);
                        float width = (float)Math.Abs(mouseCurrentLocation.X - startPoint.X);
                        float height = (float)Math.Abs(mouseCurrentLocation.Y - startPoint.Y);
                        if (width > 0 && height > 0)
                            g.DrawArc(p, x, y, width, height, startAngle, sweepAngle);
                    }

                    if (toolLine.Checked)
                    {
                        g.DrawLine(p, (float)startPoint.X, (float)startPoint.Y, (float)mouseCurrentLocation.X, (float)mouseCurrentLocation.Y);
                    }
                    if (toolConnection.Checked)
                    {
                        g.FillEllipse(Brushes.Red, (float)startPoint.X - 4, (float)startPoint.Y - 4, 8, 8);
                    }

                    if (toolPolygon.Checked)
                    {
                        g.DrawLine(p, (float)startPoint.X, (float)startPoint.Y, (float)mouseCurrentLocation.X, (float)mouseCurrentLocation.Y);
                    }
                }
                g.ResetTransform();
            }
        }

        private void DrawRectangle(Graphics g, Pen p)
        {
            float x = (float)Math.Min(startPoint.X, mouseCurrentLocation.X);
            float y = (float)Math.Min(startPoint.Y, mouseCurrentLocation.Y);
            float width = (float)Math.Abs(mouseCurrentLocation.X - startPoint.X);
            float height = (float)Math.Abs(mouseCurrentLocation.Y - startPoint.Y);
            g.DrawRectangle(p, x, y, width, height);
        }

        private void DrawShapeAfterMouseUp(Graphics g)
        {
            g.ScaleTransform((float)zoomFactor, (float)zoomFactor);

            foreach (var shape in shapes)
            {
                shape.Draw(g);
            }

            // Draw the polygon being created in real-time
            if (vertices.Count > 1)
            {
                PointF[] pointFArray = vertices.Select(v => new PointF((float)v.X, (float)v.Y)).ToArray();
                using (Pen pen = new Pen(Color.FromArgb(255, Color.Black), 0.15f))
                {
                    pen.DashPattern = new float[] { 2, 2 };
                    pen.DashStyle = DashStyle.Custom;
                    g.DrawPolygon(pen, pointFArray);
                }
            }
            g.ResetTransform();
        }

        private void DrawSelectionRectangle(Graphics g)
        {
            if (allowSelecting && isMouseDown)
            {
                g.ScaleTransform((float)zoomFactor, (float)zoomFactor);
                float x = (float)Math.Min(selRectMouse.X / (float)zoomFactor, mousePoint.X / (float)zoomFactor);
                float y = (float)Math.Min(selRectMouse.Y / (float)zoomFactor, mousePoint.Y / (float)zoomFactor);
                float width = (float)Math.Abs(mousePoint.X / (float)zoomFactor - selRectMouse.X / (float)zoomFactor);
                float height = (float)Math.Abs(mousePoint.Y / (float)zoomFactor - selRectMouse.Y / (float)zoomFactor);

                selectionRectangle = new RectangleF(x, y, width, height);

                using (var pen = new Pen(Color.Black, 0.15f))
                using (SolidBrush br = new SolidBrush(Color.FromArgb(32, Color.Blue)))
                {
                    pen.DashPattern = new float[] { 2, 2 };
                    pen.DashStyle = DashStyle.Custom;
                    pen.DashCap = DashCap.Round;
                    g.DrawRectangle(pen, Rectangle.Round(selectionRectangle));
                    g.FillRectangle(br, selectionRectangle);
                }
                g.ResetTransform();
            }
        }

        private void HighligthDetectedShape(Graphics g)
        {
            if (detectedShape != null) //highlight a detected shape or group  
            {
                g.ScaleTransform((float)zoomFactor, (float)zoomFactor);
                using (Pen pen1 = new Pen(Color.White, (rtolerance == 0) ? 0 : highlightWidth))
                using (Pen pen2 = new Pen(Color.Black, (rtolerance == 0) ? 0 : highlightWidth))
                using (Pen p = new Pen(Color.Magenta, 0.25f))
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(32, Color.Blue)))
                {
                    pen2.DashPattern = new float[] { 2, 2 };
                    pen2.DashStyle = DashStyle.Custom;
                    pen2.LineJoin = LineJoin.Round;
                    pen1.LineJoin = LineJoin.Round;
                    float rectSize = 4;
                    if (gridSize == 0.125) rectSize = 1.5f;
                    if (gridSize == 0.25) rectSize = 1.5f;
                    if (gridSize == 0.5) rectSize = 2f;
                    if (gridSize == 1) rectSize = 2f;
                    if (gridSize == 2) rectSize = 4f;
                    if (gridSize >= 4) rectSize = 6f;

                    switch (detectedShape)
                    {
                        case LineShape line:
                            if (IsStartPoint || IsEndPoint)
                            {
                                float x1 = (float)line.StartPoint.X - rectSize / 2;
                                float y1 = (float)line.StartPoint.Y - rectSize / 2;
                                float x2 = (float)line.EndPoint.X - rectSize / 2;
                                float y2 = (float)line.EndPoint.Y - rectSize / 2;
                                float w = rectSize;
                                float h = rectSize;
                                RectangleF r1 = new RectangleF(x1, y1, w, h);
                                RectangleF r2 = new RectangleF(x2, y2, w, h);
                                g.DrawRectangle(p, Rectangle.Round(r1));
                                g.DrawRectangle(p, Rectangle.Round(r2));
                            }
                            break;

                        case PolygonShape poly:
                            if (selectedVertexIndex != -1)
                            {
                                g.DrawRectangle(p, (float)poly.Vertices[selectedVertexIndex].X - rectSize / 2, (float)poly.Vertices[selectedVertexIndex].Y - rectSize / 2, rectSize, rectSize);
                            }
                            break;

                        case GroupShape groupShape:
                            g.FillRectangle(sb, groupShape.Rect);
                            break;

                            //case SymbolShape sym:
                            //    sym.UpdateBoundingBox();
                            //    g.FillRectangle(sb, sym.Rect);
                            //    break;
                    }
                }
                g.ResetTransform();
            }
            else
            {
                foreach (var sha in shapes)
                {
                    sha.IsSelected = false;//unhighlight shape
                }
            }
        }
        #endregion

        #region Detect Shape Under Mouse
        private void DetectShapeUnderMouse(PointV2D mousePoint)
        {
            detectedShape = null;
            detectedSymbol = null;

            foreach (var shape in shapes)
            {
                shape.ZoomFactor = zoomFactor;

                // Priority 1: Detect if the mouse is near the edge of bounding box of shape
                if (shape is EllipseShape ellipse)
                {
                    if (shape.IsMouseNearEdge(mousePoint, rtolerance))
                    {
                        detectedShape = ellipse;
                        detectedShape.IsSelected = true;
                        HandleEdgeDetection(ellipse, mousePoint);//Get the edge
                        break;  // Stop after detecting the first edge
                    }
                }

                else if (shape is ArcShape arc)
                {
                    if (shape.IsMouseNearEdge(mousePoint, rtolerance))
                    {
                        detectedShape = arc;
                        detectedShape.IsSelected = true;
                        HandleEdgeDetection(arc, mousePoint);//Get the edge
                        break;  // Stop after detecting the first edge
                    }
                }

                else if (shape is RectangleShape rect)
                {
                    if (shape.IsMouseNearEdge(mousePoint, rtolerance))
                    {
                        detectedShape = rect;
                        detectedShape.IsSelected = true;
                        HandleEdgeDetection(rect, mousePoint);//Get the edge
                        break;  // Stop after detecting the first edge
                    }
                }

                else if (shape is DotShape dot)
                {
                    if (shape.IsMouseNearEdge(mousePoint, ltolerance) || shape.IsMouseInsideShape(mousePoint))
                    {
                        detectedShape = dot;
                        detectedShape.IsSelected = true;
                        break;
                    }
                }

                // Priority 2: Detect lines by proximity to the line body or points
                else if (shape is LineShape line)
                {
                    if (line.IsMouseOnEndsOfLine(mousePoint, ltolerance))
                    {
                        detectedShape = line;
                        detectedShape.IsSelected = true;
                        HandleLineEndsDetection(shape, mousePoint);//Get the ends point (startPoint or endPoint)
                        break;
                    }

                    if (line.IsMouseNearLineBody(mousePoint, rtolerance))
                    {
                        detectedShape = line;
                        detectedShape.IsSelected = true;
                        IsStartPoint = false;
                        IsEndPoint = false;
                        break; // Stop after detecting the first line shape
                    }
                }

                // Priority 3: Detect text shapes
                else if (shape is TextShape textShape)
                {
                    if (textShape.IsMouseInsideShape(mousePoint) || textShape.IsMouseNearEdge(mousePoint, rtolerance))
                    {
                        detectedShape = textShape;
                        detectedShape.IsSelected = true;
                        break;  // Stop after detecting the first bounding box shape
                    }
                }

                // Priority 4: Detect connection name and connection dot
                else if (shape is ConnectionShape connection)
                {
                    // Here we have 2 situation: 
                    // 1. IsMouseOverName conectionName
                    // 2. IsMouseNearEdge conectionDot || IsMouseInsideShape conectionDot

                    // Check if the mouse is over the connection's name (ConnectionNameShape)
                    if (connection.IsMouseOverName(mousePoint))
                    {
                        detectedShape = connection.ConnectionName; // Select the ConnectionNameShape
                        connection.ConnectionName.IsSelected = true; //highlight ConnectionName
                        break;
                    }

                    // Check if the mouse is near the connection edge itself (tolerance = fixed value 10) 
                    if (connection.IsMouseNearEdge(mousePoint, 10) || connection.IsMouseInsideShape(mousePoint))
                    {
                        detectedShape = connection; // Select the connection (dot)
                        detectedShape.IsSelected = true; //highlight connection dot
                        break;
                    }
                    connection.ConnectionName.IsSelected = false; //unhighlight ConnectionName
                    connection.IsSelected = false; //unhighlight connection dot
                }

                // Priority 5: Detect group shape by their bounding box (inside and edge)
                else if (shape is GroupShape group)
                {
                    if (group.IsMouseInsideShape(mousePoint) || group.IsMouseNearEdge(mousePoint, rtolerance))
                    {
                        detectedShape = group;
                        groupShape = group;
                        break;
                    }
                }

                // Priority 6: Detect polygon shape by their bounding box edge
                else if (shape is PolygonShape poly)
                {
                    if (poly.IsMouseNearVertex(mousePoint, ltolerance, out int vertexIndex))
                    {
                        detectedShape = poly;
                        detectedShape.IsSelected = true; //highlight shape
                        selectedVertexIndex = vertexIndex;
                        break;
                    }
                    if (poly.IsMouseNearEdge(mousePoint, rtolerance))
                    {
                        detectedShape = poly;
                        detectedShape.IsSelected = true; //highlight shape
                        selectedVertexIndex = -1;
                        break;
                    }
                }

                else if (shape is SymbolShape symbol)
                {
                    symbol.ZoomFactor = zoomFactor;
                    symbol.ApplyZoomToInternalShapes();

                    if (symbol.DetectShapeAtPoint(mousePoint, 5) != null)
                    {
                        detectedShape = symbol;
                        detectedSymbol = symbol;
                        detectedInternalShape = symbol.DetectShapeAtPoint(mousePoint, 5);
                        if (detectedInternalShape != null)// so we detected one symbol from many others
                        {
                            //if (detectedInternalShape is TextShape tx && tx.TextType == 0) Debug.WriteLine("Detectat = " + tx + " " + tx.ShapeID);

                            if (!(detectedInternalShape is TextShape))
                            {
                                symbol.IsSelected = true; //highlight internal geometrical shape (draw bounding box rectangle)
                            }
                            else
                            {
                                symbol.IsSelected = false;//unhighlight internal shape
                            }

                            if (detectedInternalShape is TextShape ts && ts.ShapeID != null)
                                Debug.WriteLine("detectedInternalShape = " + ts.TextType + " " + ts.StringText + " ID = " + ts.ShapeID);
                        }
                    }


                    if (detectedShape == null)
                    {
                        foreach (var sha in shapes)
                        {
                            sha.IsSelected = false;//unhighlight shape
                        }
                    }
                }
            }
            if (detectedShape == null)
            {
                mouseNumClick = 1;//reset location for duplicate shape
                foreach (var sha in shapes)
                {
                    sha.IsSelected = false;//unhighlight shape
                }
            }
        }

        private void HandleLineEndsDetection(ShapeBase shape, PointV2D mousePoint)
        {
            if (shape is LineShape line && detectedShape.ShapeID == null)//single line without ID
            {
                if (line.IsMouseOnEndsOfLine(mousePoint, ltolerance))
                {
                    // Determine if the start or end point is closer to the mouse, then resize
                    if (line.IsPointNear(mousePoint, line.StartPoint, ltolerance))
                    {
                        IsStartPoint = true;
                        IsEndPoint = false;
                    }
                    else if (line.IsPointNear(mousePoint, line.EndPoint, ltolerance))
                    {
                        IsEndPoint = true;
                        IsStartPoint = false;
                    }
                }
            }
        }

        private void HandleEdgeDetection(ShapeBase shape, PointV2D mousePoint)
        {
            if (allowResizing && detectedShape.ShapeID == null)
            {
                detectedEdge = shape.GetEdgeUnderMouse(mousePoint, rtolerance);
                switch (detectedEdge)
                {
                    case ShapeBase.ShapeEdge.Left:
                    case ShapeBase.ShapeEdge.Right:
                        pictureBox1.Cursor = Cursors.SizeWE;
                        break;

                    case ShapeBase.ShapeEdge.Top:
                    case ShapeBase.ShapeEdge.Bottom:
                        pictureBox1.Cursor = Cursors.SizeNS;
                        break;

                    case ShapeBase.ShapeEdge.TopLeft:
                    case ShapeBase.ShapeEdge.BottomRight:
                        pictureBox1.Cursor = Cursors.SizeNWSE;
                        break;

                    case ShapeBase.ShapeEdge.TopRight:
                    case ShapeBase.ShapeEdge.BottomLeft:
                        pictureBox1.Cursor = Cursors.SizeNESW;
                        break;
                }
            }
        }


        private void MoveDetectedShape(PointV2D mousePoint)
        {
            if (allowMoving && detectedShape != null)
            {
                mouseCurrentLocation = SnapToGrid(mousePoint);
                var deltaX = mouseCurrentLocation.X - startPoint.X;
                var deltaY = mouseCurrentLocation.Y - startPoint.Y;
                detectedShape.Move(deltaX, deltaY);
                startPoint = mouseCurrentLocation;
            }
        }

        private void ResizeDetectedRectShape(PointV2D mousePoint)
        {
            if (allowResizing && detectedShape != null)
            {
                mouseCurrentLocation = SnapToGrid(mousePoint);

                double deltaX = mouseCurrentLocation.X - startPoint.X;
                double deltaY = mouseCurrentLocation.Y - startPoint.Y;
                detectedShape.Resize(deltaX, deltaY, detectedEdge);
                startPoint = mouseCurrentLocation;
            }
        }

        private void ResizeLineShape(PointV2D mousePoint)
        {
            mouseCurrentLocation = SnapToGrid(mousePoint);

            if (IsStartPoint)
            {
                detectedShape.Resize(mouseCurrentLocation, detectedShape.EndPoint);
                detectedShape.StartPoint = mouseCurrentLocation;
            }
            if (IsEndPoint)
            {
                detectedShape.Resize(detectedShape.StartPoint, mouseCurrentLocation);
                detectedShape.EndPoint = mouseCurrentLocation;
            }
        }

        #endregion

        #region ====== Mouse Events
        private PointV2D initialPoint;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = SnapToGrid(e.Location);
            selRectMouse = e.Location;
            toolText.Checked = false;

            if (e.Button == MouseButtons.Middle)
            {
                isPanning = true;
                lastMousePosition = e.Location;
                pictureBox1.Cursor = Cursors.Hand;
            }

            if (e.Button == MouseButtons.Right)
            {
                if (tool == "Polygon")
                {
                    CreatePolygon();
                    UpdateUndoRedoButtons();
                    SimulateKeyDown(Keys.Escape);
                }

                if (tool == "Text" || tool == "Dot" || tool == "Connection") SimulateKeyDown(Keys.Escape);

                //Show context menu for shape
                if (detectedShape == null && tool != "Text" && tool != "Polygon")
                {
                    allowDetecting = true;
                    contextMenuStrip1.Show(pictureBox1, new PointV2D(e.Location.X, e.Location.Y));
                }
            }

            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;

                if (allowDetecting && detectedShape != null)
                {
                    if (tool == "Move")//move shape or entire symbol
                    {
                        initialPoint = startPoint;
                        currentMoveCommand = new MoveShapeCommand(detectedShape);

                        allowMoving = true;
                        allowDrawing = true;
                        allowResizing = false;
                    }

                    if (tool == "Resize" &&
                        !(detectedShape is TextShape cn && cn.TextType == 1 ||
                        detectedShape is TextShape sn && sn.TextType == 2 ||
                        detectedShape is TextShape ts && ts.TextType == 0 ||
                        detectedShape is GroupShape))
                    {
                        currentResizeCommand = new ResizeShapeCommand(detectedShape);

                        // Check if the shape is a PolygonShape to allow vertex resizing
                        if (detectedShape is PolygonShape polygon)
                        {
                            currentResizeCommand.UpdateNewPolygonPosition(polygon.Vertices.Select(v => new PointV2D(v.X, v.Y)).ToList());
                        }

                        allowResizing = true;
                        allowDrawing = true;
                        allowMoving = false;
                    }

                    if (tool == "Delete")
                    {
                        allowMoving = false;
                        allowResizing = false;
                        currentDeleteCommand = new DeleteShapeCommand(shapes, detectedShape);
                        commandManager.ExecuteCommand(currentDeleteCommand);
                        pictureBox1.Cursor = Cursors.Default;
                    }

                    if (tool == "Ungroup")
                    {
                        if (detectedShape is GroupShape) Ungroup(groupShape);
                        else
                        {
                            DialogResult result = MessageBox.Show("This is not a group!", "Error!");
                            SimulateKeyDown(Keys.Escape);
                            allowDetecting = true;
                            toolMove.Checked = true;
                            tool = "Move";
                            return;
                        }
                    }

                    if (tool == "UnpackSym" && detectedSymbol != null)
                    {
                        UnpackSymbol(detectedSymbol);
                    }

                    if (detectedShape is SymbolShape)
                    {

                        if (tool == "ModifyObject")
                        {
                            //Modify an connection that belong to a symbol
                            if (detectedInternalShape is TextShape connectionName && connectionName.TextType == 1)
                            {
                                foreach (var shape in detectedSymbol.InternalShapes)
                                {
                                    if (shape is ConnectionShape cs && cs.ConnectionName == connectionName)
                                    {
                                        EditSymbolConnectionName(detectedSymbol, cs);
                                    }
                                }
                            }

                            // Modify symbol name
                            if (detectedInternalShape == detectedSymbol.SymbolName)
                            {
                                EditSymbolName(detectedSymbol);
                            }

                            //Modify entire symbol name and connections
                            if (!(detectedInternalShape is TextShape))
                            {
                                EditSelectedSymbol(detectedSymbol);
                            }

                            if (detectedInternalShape is TextShape txs && txs.ShapeID != null)
                            {
                                EditShape(txs);//here I need a special case for symbol text field with ID="s"
                            }
                        }
                    }

                    if (tool == "FlipHoriz")
                    {
                        var flipCommand = new FlipShapeCommand(detectedShape, true);
                        commandManager.ExecuteCommand(flipCommand);
                    }

                    if (tool == "FlipVert")
                    {
                        var flipCommand = new FlipShapeCommand(detectedShape, false);
                        commandManager.ExecuteCommand(flipCommand);
                    }

                    if (tool == "Rotate")
                    {
                        if (!(detectedShape is EllipseShape && detectedShape is RectangleShape && detectedShape is LineShape))
                        {
                            var rotateCommand = new RotateShapeCommand(detectedShape, 90);
                            commandManager.ExecuteCommand(rotateCommand);
                        }
                    }

                    // Modify separate connection
                    if (tool == "ModifyObject" && detectedShape is TextShape connectName
                        && connectName.TextType == 1
                        && detectedShape.ShapeID == null)
                    {
                        foreach (var shape in shapes)
                        {
                            if (shape is ConnectionShape conn && conn.ConnectionName == connectName)
                            {
                                EditConnectionName(conn);
                            }
                        }
                    }

                    // Edit shape as geometrical shape except group, connection, text and symbol
                    if (tool == "ModifyObject" &&
                        detectedShape != null &&
                        detectedShape.ShapeID == null &&
                        !(detectedShape is GroupShape) &&
                        !(detectedShape is ConnectionShape) &&
                        !(detectedShape is TextShape con && con.TextType == 1) &&
                        !(detectedShape is SymbolShape))
                    {
                        EditShape(detectedShape);
                    }

                    if (tool == "BringToFront" && detectedShape != null)
                    {
                        BringToFront(detectedShape);
                    }

                    if (tool == "SendToBack" && detectedShape != null)
                    {
                        SendToBack(detectedShape);
                    }

                    if (tool == "Duplicate" && detectedShape != null)
                    {
                        DuplicateShape(detectedShape);
                        mouseNumClick++; //increment location for duplicate of detected shape 
                    }
                }

                if (detectedShape == null)
                {
                    allowDrawing = true;
                }

                if (toolGroup.Checked || toolCreateSym.Checked)
                {
                    allowSelecting = true;
                }

                if (toolPolygon.Checked)
                {
                    allowDrawing = true;
                    vertices.Add(startPoint);
                }

                if (toolModifyObject.Checked)
                {
                    allowDetecting = true;
                    isMouseDown = false;
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //used for draw current shape before finalizing
            mouseCurrentLocation = SnapToGrid(e.Location);
            pictureBox1.Cursor = Cursors.Default;
            mousePoint = e.Location;
            toolStripStatusLabel1.Text = string.Format("{0,0:f1},{1,0:f1}", ConvertPixelToMm((float)mouseCurrentLocation.X), ConvertPixelToMm((float)mouseCurrentLocation.Y));

            if (isPanning)
            {
                // Calculate the movement delta
                int deltaX = e.Location.X - (int)lastMousePosition.X;
                int deltaY = e.Location.Y - (int)lastMousePosition.Y;

                // Update the offset with the movement delta
                panOffset.X += deltaX;
                panOffset.Y += deltaY;

                // Store the current mouse position
                lastMousePosition = e.Location;

                // Redraw the PictureBox with the new offset
                pictureBox1.Invalidate();  // This will trigger the Paint event
            }

            if (!isMouseDown && allowDetecting)
            {
                DetectShapeUnderMouse(mousePoint);
            }

            //case rectangle ============================
            if (isMouseDown && allowDetecting && !(detectedShape is LineShape))
            {
                if (allowResizing)
                {
                    ResizeDetectedRectShape(mousePoint);
                }

                //move shapes
                if (allowMoving)
                {
                    if (detectedShape is SymbolShape symbol)
                    {
                        if (detectedInternalShape is TextShape symName && symName.TextType == 2)
                        {
                            detectedShape = symName;
                        }

                        if (detectedInternalShape is TextShape conName && conName.TextType == 1)
                        {
                            detectedShape = conName;
                        }

                        if (detectedInternalShape is TextShape txt && txt.TextType == 0)
                        {
                            detectedShape = txt;
                        }
                    }
                    MoveDetectedShape(mousePoint);
                }
            }

            //case line ========================
            if (isMouseDown && (detectedShape is LineShape))
            {
                //resize line
                if (allowResizing)
                {
                    ResizeLineShape(mousePoint);
                }

                //move line
                if (allowMoving)
                {
                    MoveDetectedShape(mousePoint);
                }
            }

            if (tool == "Text")
            {
                //update text bounds location before mouse up
                //show where the text will be submitted
                textBounds.Location = mouseCurrentLocation;
            }

            if (isMouseDown && allowResizing && detectedShape is PolygonShape poly && selectedVertexIndex != -1)
            {
                poly.MoveVertex(selectedVertexIndex, mouseCurrentLocation);
            }

            pictureBox1.Invalidate(); // redraw the shape in mouse move event
        }


        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                isPanning = false;
                pictureBox1.Cursor = Cursors.Default;
            }

            allowSelecting = false;
            isMouseDown = false;
            allowMoving = false;
            isPanning = false;

            endPoint = SnapToGrid(e.Location);

            // Update new position after MouseUp event
            if (detectedShape != null && currentMoveCommand != null)
            {
                currentMoveCommand.UpdateNewPosition(detectedShape.StartPoint, detectedShape.EndPoint);
                commandManager.ExecuteCommand(currentMoveCommand);

                // Special case for SymbolShape
                if (detectedShape is SymbolShape || detectedShape is GroupShape)
                {
                    double finalDeltaX = mouseCurrentLocation.X - initialPoint.X;
                    double finalDeltaY = mouseCurrentLocation.Y - initialPoint.Y;
                    currentMoveCommand.UpdateFinalDelta(finalDeltaX, finalDeltaY);
                }
                currentMoveCommand = null;
            }

            if (detectedShape != null && currentResizeCommand != null)
            {
                // Special case for polygon
                if (detectedShape is PolygonShape poly)
                {
                    currentResizeCommand.UpdateNewPolygonPosition(poly.Vertices.ToList());
                }
                currentResizeCommand.UpdateNewPosition(detectedShape.StartPoint, detectedShape.EndPoint);
                commandManager.ExecuteCommand(currentResizeCommand);
                currentResizeCommand = null; // Reset command after execution
            }

            if (allowDrawing)
            {
                // Creating a new shape
                switch (tool)
                {
                    case "Rectangle":
                        currentShape = new RectangleShape(startPoint, endPoint, shapePenColor, shapeFillColor);
                        currentShape.ShapeName = "Rectangle";
                        break;
                    case "Dot":
                        currentShape = new DotShape(startPoint);
                        currentShape.ShapeName = "Dot";
                        break;
                    case "Ellipse":
                        currentShape = new EllipseShape(startPoint, endPoint, shapePenColor, shapeFillColor);
                        currentShape.ShapeName = "Ellipse";
                        break;
                    case "Arc":
                        currentShape = new ArcShape(startPoint, endPoint, startAngle, sweepAngle, shapePenColor, shapeFillColor);
                        currentShape.ShapeName = "Arc";
                        break;
                    case "Line":
                        currentShape = new LineShape(startPoint, endPoint, shapePenColor);
                        currentShape.ShapeName = "Line";
                        break;
                    case "Text":
                        currentShape = new TextShape(startPoint, 0, inpText, inpTextAlignment, inpTextRotation, inpFont, inpFontCol);
                        currentShape.ShapeName = "Text";
                        break;
                    case "Group":
                        GroupSelectedShapes();
                        break;
                    case "Connection":
                        CreateConnection(startPoint);
                        break;
                    case "CreateSym":
                        CreateSymbol();
                        break;
                }

                if (currentShape != null)
                {
                    var addShapeCommand = new AddShapeCommand(shapes, currentShape);
                    commandManager.ExecuteCommand(addShapeCommand);
                    UpdateUndoRedoButtons();
                    vertices.Clear();
                    currentShape = null;
                    allowDrawing = false;
                }
            }
            detectedShape = null;
        }
        #endregion

        #region Create Text
        private void CreateTextShape()
        {
            UncheckAllButtons();
            toolText.Checked = true;
            tool = "Text";
            allowDetecting = false;
            textIsEmpty = false;
            //Open a dialog
            using (var inputForm = new TextInputForm(inpText, inpFont, inpFontCol, inpTextAlignment, inpTextRotation))
            {
                inputForm.Text = "Place a text";
                DialogResult result = inputForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    inpText = inputForm.InputText;
                    inpFont = inputForm.TextFont;
                    inpFontCol = inputForm.FontColor;
                    inpTextAlignment = inputForm.NewTextAlignment;
                    inpTextRotation = inputForm.NewTextRotation;

                    // Here I instantiated TextShape only for get textBounds to be visible on screen
                    tempTextShape = new TextShape(startPoint, 0, inpText, inpTextAlignment, inpTextRotation, inpFont, inpFontCol);
                    textBounds = Rectangle.Round(tempTextShape.Rect);

                }
                if (result == DialogResult.Cancel)
                {
                    detectedShape = null;
                    currentShape = null;
                    tempTextShape = null;
                    textIsEmpty = true;
                }
            }
        }
        #endregion

        #region Create Polygon, Connection
        private void CreatePolygon()
        {
            if (vertices.Count > 2)  // Ensure the polygon has at least 3 points
            {
                currentShape = new PolygonShape(vertices, shapePenColor, shapeFillColor);
                currentShape.ShapeName = "Polygon";
                // Add the newly created polygon to the shapes list
                if (allowDrawing && currentShape != null)
                {
                    var addShapeCommand = new AddShapeCommand(shapes, currentShape);
                    commandManager.ExecuteCommand(addShapeCommand);
                    vertices.Clear();  // Clear the vertices for the next polygon
                    currentShape = null;
                    allowDrawing = false;
                    pictureBox1.Invalidate();  // Trigger redraw
                }
            }
        }

        private void CreateConnection(PointV2D startPoint)
        {
            //Open a dialog to get the connection name, font, and color
            string label2Text = "";
            string label3Text = "";
            using (var esf = new EditSymbolForm(
                inpText,
                true,//connection name is always visible at creation - checkbox visible is checked
                false,//checkbox enabled = false
                inpFont,
                inpFontCol,
                inpConnAlignment,
                inpConnRotation, null,
                label2Text, label3Text, false, false, false))
            {
                esf.Text = "Place a connection";
                if (esf.ShowDialog() == DialogResult.OK)//there is no Undo/Redo for Connection properties
                {
                    connectionNumber++;
                    inpText = esf.NewTextName;
                    inpFont = esf.NewTextFont;
                    inpFontCol = esf.NewTextColor;
                    inpConnAlignment = esf.NewTextAlignment;
                    inpConnRotation = esf.NewTextRotation;

                    // 4 is the diameter in pixels of connection dot, so the name is shifted by 4 pixels to right
                    //and 4 pixels to botto// to avoid overlapping
                    // later, after creation we can move the name in another position if we want
                    PointV2D namePosition = new PointV2D(startPoint.X + 4, startPoint.Y + 4);
                    currentShape = new ConnectionShape(connectionNumber, startPoint, inpText, inpConnAlignment, inpConnRotation, inpFont, inpFontCol, namePosition, "Input");
                    currentShape.ShapeName = "Connection";

                    // Put visibility on true at creation
                    if (currentShape is ConnectionShape con)
                    {
                        con.ConnectionVisible = true;
                        con.ConnectionNameVisible = true;
                    }
                    pictureBox1.Invalidate();
                }
                else
                {
                    toolConnection.Checked = false;
                    tool = "";
                }
            }
        }
        #endregion

        private void EditShape(ShapeBase detectedShape)
        {
            if (detectedShape != null && detectedShape.ShapeID == null && !(detectedShape is TextShape))
            {
                bool isFilled = !(detectedShape is LineShape);

                bool isArcShape = detectedShape is ArcShape; // Check if it's an ArcShape
                if (isArcShape)
                {
                    ArcShape arcShape = detectedShape as ArcShape;
                    startAngle = arcShape.StartAngle;
                    sweepAngle = arcShape.SweepAngle;
                }
                else isArcShape = false;

                bool isDotShape = detectedShape is DotShape;
                if (isDotShape)
                {
                    DotShape dotShape = detectedShape as DotShape;
                    inpDotRadius = dotShape.DotRadius;//in pixels
                    inpDotColor = dotShape.DotColor;
                    isArcShape = false;
                    isFilled = false;
                }
                else isDotShape = false;

                //Open a dialog
                using (EditShapesForm esf = new EditShapesForm(detectedShape.BorderPenColor, detectedShape.LinStyle, detectedShape.PenThickness, detectedShape.FillBrushColor, isFilled, detectedShape.HatchStyl, isArcShape, startAngle, sweepAngle, isDotShape, inpDotRadius, inpDotColor, detectedShape.Rect.Width, detectedShape.Rect.Height))
                {
                    esf.Text = "Modify " + detectedShape;
                    DialogResult result = esf.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        Color newPenColor = esf.PenColor;
                        float newPenThickness = ConvertMmToPixels(esf.PenThickness);
                        Color newFillColor = esf.FillColor;
                        LineStyle newLineStyle = esf.LinStyle;
                        CustomHatchStyle newHatchStyle = esf.HatchStyl;

                        var command = new EditShapeCommand(
                            detectedShape, newPenColor, newPenThickness, newFillColor, newLineStyle, newHatchStyle,
                            startAngle: isArcShape ? esf.StartAngle : (float?)null,
                            sweepAngle: isArcShape ? esf.SweepAngle : (float?)null,
                            dotRadius: isDotShape ? ConvertMmToPixels(esf.DotRadius) : (float?)null,
                            dotColor: isDotShape ? esf.DotColor : (Color?)null
                            );

                        // Execute the command
                        commandManager.ExecuteCommand(command);
                        pictureBox1.Invalidate(); // Redraw the canvas
                    }
                }
            }

            if (detectedShape != null && detectedShape.ShapeID == null && detectedShape is TextShape text && text.TextType == 0)
            {
                //Open a dialog
                using (TextInputForm tif = new TextInputForm(text.StringText, text.TextFont, text.TextColor, text.TextShapeAlign, text.TextShapeRotation))
                {
                    tif.Text = "Modify a text";
                    DialogResult result = tif.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        string newStringText = tif.InputText;
                        Font newTextFont = tif.TextFont;
                        Color newTextColor = tif.FontColor;
                        TextAlignment newTextShapeAlign = tif.NewTextAlignment;
                        TextRotation newTextShapeRotation = tif.NewTextRotation;

                        var command = new EditTextCommand(
                            text,
                            newStringText,
                            newTextFont,
                            newTextColor,
                            newTextShapeAlign,
                            newTextShapeRotation
                        );
                        commandManager.ExecuteCommand(command);
                        pictureBox1.Invalidate(); // Redraw the canvas
                    }
                }
            }
        }

        #region EditSymbol&ConnectionName-EditConnectionName-EditSymbolName-EditSelectedSymbol

        private void EditSymbolConnectionName(SymbolShape detectedSymbol, ConnectionShape detectedConnection)
        {
            var connectionList = detectedSymbol.GetAllConnections();
            if (connectionList.Count > 0)
            {
                //Open a dialog
                string checkBox1Text = "Apply these settings to all connection names of this symbol";
                string checkBox2Text = "Apply these settings to all connection names of all symbols";
                using (EditSymbolForm editForm = new EditSymbolForm(
                    detectedConnection.ConnectionName.StringText,
                    detectedConnection.ConnectionNameVisible,
                    true,
                    detectedConnection.ConnectionName.TextFont,
                    detectedConnection.ConnectionName.TextColor,
                    detectedConnection.ConnectionName.TextShapeAlign,
                    detectedConnection.ConnectionName.TextShapeRotation,
                    null, checkBox1Text, checkBox2Text, setAllSymbolConnections, setAllSymbolsConnections, false))
                {
                    editForm.Text = "Modify connection";

                    DialogResult result = editForm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        var command = new EditSymConNameCommand(
                            shapes,
                            detectedConnection,
                            detectedSymbol,
                            editForm.NewTextName,
                            editForm.NewNameVisible,
                            editForm.NewTextFont,
                            editForm.NewTextColor,
                            editForm.NewTextAlignment,
                            editForm.NewTextRotation,
                            editForm.SetAllSymbolConnections,
                            editForm.SetAllSymbolsConnections
                        );

                        commandManager.ExecuteCommand(command);
                        pictureBox1.Invalidate();  // Redraw the canvas
                    }

                    if (result == DialogResult.Cancel)
                    {
                        allowDetecting = true;
                        allowMoving = false;
                        allowResizing = false;
                        detectedShape = null;
                        pictureBox1.Invalidate();  // Redraw the canvas
                    }
                }
            }
        }

        // Edit connection name after placement on the drawing
        private void EditConnectionName(ConnectionShape connection)
        {
            //Open a dialog
            string checkBox1Text = "";
            string checkBox2Text = "";
            using (EditSymbolForm editForm = new EditSymbolForm(
                connection.ConnectionName.StringText,
                true,
                false,
                connection.ConnectionName.TextFont,
                connection.ConnectionName.TextColor,
                connection.ConnectionName.TextShapeAlign,
                connection.ConnectionName.TextShapeRotation, null,
                checkBox1Text, checkBox2Text, false, false, false))
            {
                editForm.Text = "Modify connection";
                DialogResult result = editForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    connection.ConnectionName.StringText = editForm.NewTextName;
                    connection.ConnectionNameVisible = editForm.NewNameVisible;
                    connection.ConnectionName.TextFont = editForm.NewTextFont;
                    connection.ConnectionName.TextColor = editForm.NewTextColor;
                    connection.ConnectionName.TextShapeAlign = editForm.NewTextAlignment;
                    connection.ConnectionName.TextShapeRotation = editForm.NewTextRotation;
                    connection.ConnectionName.GetSingleShapeBounds();
                    pictureBox1.Invalidate();  // Redraw the canvas
                }
                if (result == DialogResult.Cancel)
                {
                    allowDetecting = true;
                    allowMoving = false;
                    allowResizing = false;
                    detectedShape = null;
                    if (detectedSymbol != null) detectedSymbol.IsSelected = false;
                    pictureBox1.Invalidate();  // Redraw the canvas
                }
            }
        }

        private void EditSymbolName(SymbolShape detectedSymbol)
        {
            //Open a dialog
            string checkBox1Text = "Apply these settings to all symbol names";
            string checkBox2Text = "";

            using (EditSymbolForm editForm = new EditSymbolForm(
                detectedSymbol.SymbolName.StringText,
                detectedSymbol.SymbolNameVisible,
                true,
                detectedSymbol.SymbolName.TextFont,
                detectedSymbol.SymbolName.TextColor,
                detectedSymbol.SymbolName.TextShapeAlign,
                detectedSymbol.SymbolName.TextShapeRotation, null,
                checkBox1Text, checkBox2Text, false, false, setAllSymbolNames))
            {
                editForm.Text = "Modify symbol name";
                DialogResult result = editForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Execute command with setAllSymbolNames flag and shapes list
                    var command = new EditSymbolNameCommand(
                        detectedSymbol,
                        editForm.NewTextName,
                        editForm.NewNameVisible,
                        editForm.NewTextFont,
                        editForm.NewTextColor,
                        editForm.NewTextAlignment,
                        editForm.NewTextRotation,
                        editForm.SetAllSymbolNames,
                        shapes);

                    commandManager.ExecuteCommand(command);
                    pictureBox1.Invalidate();  // Redraw the canvas and update symbol to reflect modification
                }

                if (result == DialogResult.Cancel)
                {
                    allowDetecting = true;
                    allowMoving = false;
                    allowResizing = false;
                    detectedShape = null;
                    pictureBox1.Invalidate();  // Redraw the canvas
                }
            }
        }

        //Edit entire symbol - name and connections
        private void EditSelectedSymbol(SymbolShape selectedSymbol)
        {
            string checkBox1Text = "";
            string checkBox2Text = "";
            using (EditSymbolForm editForm = new EditSymbolForm(
                selectedSymbol.SymbolName.StringText,
                selectedSymbol.SymbolNameVisible,
                true,
                selectedSymbol.SymbolName.TextFont,
                selectedSymbol.SymbolName.TextColor,
                selectedSymbol.SymbolName.TextShapeAlign,
                selectedSymbol.SymbolName.TextShapeRotation,
                selectedSymbol.GetAllConnections(),
                checkBox1Text, checkBox2Text, false, false, false))
            {
                editForm.Text = "Modify symbol";
                DialogResult result = editForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Update symbol name visibility
                    selectedSymbol.SymbolNameVisible = editForm.NewNameVisible;
                    selectedSymbol.SymbolName.StringText = editForm.NewTextName;
                    selectedSymbol.SymbolName.GetSingleShapeBounds();

                    foreach (var shape in selectedSymbol.InternalShapes)
                    {
                        if (shape is TextShape ts && ts.TextType == 2)
                        {
                            ts.StringText = editForm.NewTextName;
                            ts.TextFont = editForm.NewTextFont;
                            ts.TextColor = editForm.NewTextColor;
                            ts.TextShapeAlign = editForm.NewTextAlignment;
                            ts.TextShapeRotation = editForm.NewTextRotation;
                            ts.GetSingleShapeBounds();
                        }

                        // Update connection names by iterating through all connections
                        if (shape is ConnectionShape oldConnection)
                        {
                            // Find the corresponding updated connection from the editForm
                            foreach (var newConnection in editForm.NewConnectionList)
                            {
                                if (newConnection.ConnectionNumber == oldConnection.ConnectionNumber)
                                {
                                    // Set the new connection name from the updated list
                                    oldConnection.ConnectionName.StringText = newConnection.ConnectionName.StringText;
                                    // Update bounding box for new name
                                    oldConnection.ConnectionName.GetSingleShapeBounds();
                                }
                            }
                        }
                    }
                }
                if (result == DialogResult.Cancel)
                {
                    allowDetecting = true;
                    allowMoving = false;
                    allowResizing = false;
                    detectedShape = null;
                    detectedSymbol.IsSelected = false;
                    pictureBox1.Invalidate();  // Redraw the canvas
                }
            }
            pictureBox1.Invalidate();
        }


        #endregion

        #region Group / Ungroup
        private void GroupSelectedShapes()
        {
            if (selectionRectangle == Rectangle.Empty)
            {
                SimulateKeyDown(Keys.Escape);
                return;
            }

            groupedShapes = new List<ShapeBase>();
            bool isSelectionTouchingRectangleEdges = false;

            foreach (var shape in shapes)
            {
                if (shape is RectangleShape rectangleShape)
                {
                    // Check if selection rectangle reaches the edges of this outer rectangle
                    isSelectionTouchingRectangleEdges =
                        selectionRectangle.Left <= rectangleShape.Rect.Left &&
                        selectionRectangle.Right >= rectangleShape.Rect.Right &&
                        selectionRectangle.Top <= rectangleShape.Rect.Top &&
                        selectionRectangle.Bottom >= rectangleShape.Rect.Bottom;

                    // If it does touch or reach the edges, include the rectangle in selection
                    if (isSelectionTouchingRectangleEdges)
                    {
                        shape.IsSelected = true;
                        groupedShapes.Add(shape);
                    }
                }
                else
                {
                    // For other shapes like lines, select only if fully within the selection rectangle
                    RectangleF shapeBounds = shape.Rect;
                    bool isShapeCompletelyInside =
                        selectionRectangle.Contains(shapeBounds.Left, shapeBounds.Top) &&
                        selectionRectangle.Contains(shapeBounds.Right, shapeBounds.Top) &&
                        selectionRectangle.Contains(shapeBounds.Left, shapeBounds.Bottom) &&
                        selectionRectangle.Contains(shapeBounds.Right, shapeBounds.Bottom);

                    if (isShapeCompletelyInside || isSelectionTouchingRectangleEdges)
                    {
                        shape.IsSelected = true;
                        groupedShapes.Add(shape);
                    }
                }
            }

            if (groupedShapes.Count > 0)
            {
                var groupCommand = new GroupShapeCommand(shapes, groupedShapes);
                commandManager.ExecuteCommand(groupCommand);
            }

            selectionRectangle = Rectangle.Empty;
            groupedShapes = null;
            pictureBox1.Invalidate();
        }

        private void Ungroup(GroupShape group)
        {
            if (shapes.Contains(group))
            {
                var ungroupCommand = new UngroupShapeCommand(shapes, group);
                commandManager.ExecuteCommand(ungroupCommand);
            }

            groupShape = null;
            detectedShape = null;
            selectionRectangle = Rectangle.Empty;
            allowDetecting = true;
        }
        #endregion

        private void PanViewport(double deltaX, double deltaY)
        {
            viewportOffsetX += deltaX;
            viewportOffsetY += deltaY;
            pictureBox1.Invalidate(); // Redraw after panning
        }

        #region Duplicate(Clone) detected shape
        private void DuplicateShape(ShapeBase detectedShape)
        {
            float shapeShiftInPixels = ConvertMmToPixels(gridSize);
            float offset = mouseNumClick * shapeShiftInPixels;

            if (detectedShape != null)
            {
                var duplicateShapeCommand = new DuplicateShapeCommand(shapes, detectedShape, offset);
                commandManager.ExecuteCommand(duplicateShapeCommand);
                UpdateUndoRedoButtons();
            }
        }
        #endregion

        #region Create Symbol
        private void CreateSymbol()
        {
            string symbolID = null;
            connectionNumber = -1;//reset connection number to avoid increase to over range

            if (selectionRectangle == Rectangle.Empty)
            {
                SimulateKeyDown(Keys.Escape);
                return;
            }

            var selectedShapes = new List<ShapeBase>();

            // First, gather the selected shapes
            foreach (var shape in shapes)
            {
                if (selectionRectangle.IntersectsWith(Rectangle.Round(shape.Rect)))
                {
                    selectedShapes.Add(shape);

                    if (shape is ConnectionShape connection)
                    {
                        if (connection.ConnectionName != null)
                        {
                            selectedShapes.Add(connection.ConnectionName);
                        }
                    }
                }
            }

            // If no shapes were selected, exit early
            if (selectedShapes.Count == 0)
            {
                detectedShape = null;
                currentShape = null;
                groupShape = null;
                SimulateKeyDown(Keys.Escape);
                return;
            }

            var dotShapes = selectedShapes.OfType<DotShape>().ToList();
            var textShapes = selectedShapes.OfType<TextShape>().ToList();
            var symbolShapes = selectedShapes.OfType<SymbolShape>().ToList();
            var connectionShapes = selectedShapes.OfType<ConnectionShape>().ToList();
            var groupShapes = selectedShapes.OfType<GroupShape>().ToList();

            if (dotShapes.Count > 0)// Cancel creation: selected shape content a dot
            {
                MessageBox.Show("There is a dot in the selection.\nSymbols cannot contain dots.", "Cancel creating symbol");
                selectedShapes.Clear();
                DeselectSelectedShapes();
                SimulateKeyDown(Keys.Escape);
                return;
            }


            if (symbolShapes.Count > 0)// Cancel creation: selected shape content a symbol
            {
                MessageBox.Show("There is a symbol in the selection.", "Cancel creating symbol");
                selectedShapes.Clear();
                DeselectSelectedShapes();
                SimulateKeyDown(Keys.Escape);
                return;
            }

            if (groupShapes.Count > 0)// Cancel creation: selected shape content a group
            {
                MessageBox.Show("There is a group in the selection.\nSymbols cannot contain groups.", "Cancel creating symbol");
                selectedShapes.Clear();
                DeselectSelectedShapes();
                SimulateKeyDown(Keys.Escape);
                return;
            }

            if (connectionShapes.Count == 0)// Warning symbol has no connection
            {
                DialogResult result = MessageBox.Show("This symbol has no connection!\nDo you continue?", "Warning", MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel)
                {
                    selectedShapes.Clear();
                    DeselectSelectedShapes();
                    SimulateKeyDown(Keys.Escape);
                    return;
                }
            }


            //Debug.WriteLine("selectedShapes count = " + selectedShapes.Count);
            var previewSelectedShapes = new List<ShapeBase>();
            previewSelectedShapes.AddRange(selectedShapes);

            // Create temporary symbolShape only for preview and calculate the bounding box for all shapes (without name)
            var previewSymbolShape = new SymbolShape(startPoint, null, inpSymNameAlignment, inpSymNameRotation, inpFont, inpFontCol, null, previewSelectedShapes);

            // Calculate the bounding box for all shapes in order to place later the name of symbol
            RectangleF groupBounds = GraphExtension.GetShapesBounds(previewSelectedShapes, null);
            if (groupBounds == Rectangle.Empty)
            {
                //Debug.WriteLine("No shape in selectedShapes");
                return; // No valid bounding box found
            }
            //Debug.WriteLine("groupBounds = " + groupBounds);
            selectedShapes.Clear();// Prepair for creating the real symbol shape

            // If is shapes selected


            using (CreateSymbolForm csf = new CreateSymbolForm(inpText, inpVisible, inpFont, inpSymNameAlignment, inpSymNameRotation, inpFontCol, previewSymbolShape))
            {
                DialogResult result = csf.ShowDialog();

                if (result == DialogResult.Cancel)
                {
                    // Cancel the selection and symbol creation
                    foreach (var selShape in selectedShapes)
                    {
                        selShape.IsSelected = true;
                        if (selShape is ConnectionShape connection)
                        {
                            connection.ConnectionVisible = true;
                        }
                    }
                    selectedShapes.Clear();
                    DeselectSelectedShapes();
                    SimulateKeyDown(Keys.Escape);
                    return;
                }

                if (result == DialogResult.OK)
                {
                    inpText = csf.InputName; // The name of symbol
                    inpVisible = csf.InputVisible;
                    inpFont = csf.InputTextFont;
                    inpFontCol = csf.InputFontColor;
                    inpSymNameAlignment = csf.InputTextAlignment;
                    inpSymNameRotation = csf.InputTextRotation;

                    // Generate a new unique ID for this symbol instance
                    symbolID = Guid.NewGuid().ToString();
                    //symbolID = $"{inpSymbolType}_{Guid.NewGuid()}";

                    // First, gather again the selected shapes
                    foreach (var shape in shapes)
                    {
                        if (selectionRectangle.IntersectsWith(Rectangle.Round(shape.Rect)))
                        {
                            shape.IsSelected = false;
                            shape.GetSingleShapeBounds();
                            shape.ShapeID = "s"; //ID = s for symbol internal shape
                            if (shape is ConnectionShape connection)
                            {
                                // It is about connection dot

                                connection.ConnectionVisible = false;
                                connection.ShapeID = "s";
                                connection.ConnectionName.ShapeID = "s";
                                connection.ConnectionName.IsSelected = false;
                                connection.ConnectionName.GetSingleShapeBounds();
                            }
                            if (shape is TextShape txt && txt.TextType == 0)
                            {
                                txt.ShapeID = "s";
                            }
                            // Now, add shapes in selectedShapes list 
                            selectedShapes.Add(shape); //ID = s for symbol internal shape
                        }
                    }

                    // Create the SymbolName for symbol
                    // Set the startPoint for symbolName location to half of height of groupBounds
                    PointV2D symbolNameLocation = new PointV2D(groupBounds.X, groupBounds.Y + groupBounds.Height / 2);

                    // Create the real symbol witch contains the name as symbolName(TextShape) in selectedShape List
                    var symbolShape = new SymbolShape(symbolNameLocation, inpText, inpSymNameAlignment, inpSymNameRotation, inpFont, inpFontCol, symbolID, selectedShapes);

                    //symbolShape.SymbolID = Guid.NewGuid().ToString();

                    // Set visibility of symbol name according to the user's choice
                    symbolShape.SymbolNameVisible = inpVisible;

                    // Remove selected shapes for symbol. Now the symbol itself will be visible on screen 
                    foreach (var shape in shapes.ToList())
                    {
                        if (selectionRectangle.IntersectsWith(Rectangle.Round(shape.Rect)))
                        {
                            shape.IsSelected = false;
                            var deleteShapeCommand = new DeleteShapeCommand(shapes, shape);
                            commandManager.ExecuteCommand(deleteShapeCommand);
                        }
                    }

                    // Now add symbolShape to ShapeBase list of shapes
                    var addShapeCommand = new AddShapeCommand(shapes, symbolShape);
                    commandManager.ExecuteCommand(addShapeCommand);

                    // Call method to save symbol.
                    SaveSymbol(symbolShape);

                    // Activate Move tool 
                    toolMove.Checked = true;
                    tool = "Move";
                    toolMove.PerformClick();
                }
            }
        }
        #endregion

        private void DeselectSelectedShapes()
        {
            foreach (var shape in shapes)
            {
                if (selectionRectangle.IntersectsWith(Rectangle.Round(shape.Rect)))
                {
                    if (shape is ConnectionShape connection)
                    {
                        connection.ConnectionVisible = true;
                    }

                    shape.IsSelected = false;
                    pictureBox1.Invalidate();
                }
            }
        }

        #region Break symbol
        private void UnpackSymbol(SymbolShape symbol)
        {
            // Using a copy shapes in order to remove SymbolName shape from list
            foreach (var shape in symbol.InternalShapes.ToList())
            {
                shape.ShapeID = null;
                shape.IsSelected = false;

                if (shape is ConnectionShape connection)
                {
                    connection.ConnectionVisible = true;
                    connection.ShapeID = null;
                    connection.IsSelected = false;
                    //Handle nested TextShapes(such as connection names)
                    if (connection.ConnectionName != null)
                    {
                        // Clear ID of the connection name
                        connection.ConnectionName.ShapeID = null;
                        connection.ConnectionName.IsSelected = false;
                    }
                }

                // Remove symbol name
                if (shape is TextShape symName && symName.TextType == 2)
                {
                    symbol.InternalShapes.Remove(symName);
                }
            }
            selectedShapes = new List<ShapeBase>();
            foreach (var shape in symbol.InternalShapes)
            {
                selectedShapes.Add(shape);
            }
            shapes.Remove(symbol);//remove symbol from list
            shapes.AddRange(selectedShapes);//add shape to list

            //detectedShape = null;
            detectedSymbol = null;
            detectedInternalShape = null;
            SimulateKeyDown(Keys.Escape);
            DeselectSelectedShapes();
            pictureBox1.Invalidate();
        }
        #endregion

        #region Form Load
        private void Form1_Load(object sender, EventArgs e)
        {
            XmlHelper.loadedFiles.Clear();
            //drawLineCursor = CreateCursor.CreateCursorNoResize(Properties.Resources.LineCursor, 0, 0);
            //drawRectCursor = CreateCursor.CreateCursorNoResize(Properties.Resources.RectCursor, 0, 0);
            //drawElliCursor = CreateCursor.CreateCursorNoResize(Properties.Resources.EllipseCursor, 0, 0);
            //drawArcCursor = CreateCursor.CreateCursorNoResize(Properties.Resources.ArcCursor, 0, 0);
            //drawPolyCursor = CreateCursor.CreateCursorNoResize(Properties.Resources.PolyCursor, 0, 0);
            //drawCircCursor = CreateCursor.CreateCursorNoResize(Properties.Resources.CircleCursor, 0, 0);
            //drawCenterArcCursor = CreateCursor.CreateCursorNoResize(Properties.Resources.CenterArcCursor, 0, 0);
            SetPictureBoxSize();
            UpdateCanvas();
            try
            {
                LoadSymbolsAndDisplayInFlowLayoutPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error while loading symbols.");
            }

            // Prepare the shape tools. We will make the buttons act as radio buttons.
            ToolStripShapeButtons = new ToolStripButton[]
            {
                toolRectangle,
                toolEllipse,
                toolArc,
                toolLine,
                toolText,
                toolDot,
                toolConnection,
                toolPolygon,
                toolMove,
                toolResize,
                toolDelete,
                toolGroup,
                toolUngroup,
                toolFlipHoriz,
                toolFlipVert,
                toolDuplicate,
                toolRotate,
                toolCreateSym,
                toolUnpackSym,
                toolModifyObject,
                toolBringToFront,
                toolSendToBack
            };
            foreach (ToolStripButton btn in ToolStripShapeButtons)
            {
                btn.Click += ToolShape_Click;
            }

            UpdateUndoRedoButtons();
        }
        #endregion

        #region Tool shape create radio buttons
        // Allow only one shape selection at a time.
        private void ToolShape_Click(object sender, EventArgs e)
        {
            SelectToolStripButton(sender as ToolStripButton, ToolStripShapeButtons);
        }

        // Select the indicated button and deselect the others.
        private void SelectToolStripButton(ToolStripButton selectedButton, ToolStripButton[] buttons)
        {
            foreach (ToolStripButton clickButton in buttons)
            {
                clickButton.Checked = (clickButton == selectedButton);
            }
        }
        #endregion

        #region SnapToGrid
        public PointV2D SnapToGrid(Point p)
        {
            double gridSpacingPixelsX = gridSize * pixelsPerMillimeterX * zoomFactor;
            double gridSpacingPixelsY = gridSize * pixelsPerMillimeterY * zoomFactor;
            double scaledGridSizeX = gridSpacingPixelsX / zoomFactor;
            double scaledGridSizeY = gridSpacingPixelsX / zoomFactor;
            double x = Math.Round(p.X / gridSpacingPixelsX) * scaledGridSizeX;
            double y = Math.Round(p.Y / gridSpacingPixelsX) * scaledGridSizeY;
            return new PointV2D(x, y);
        }
        #endregion

        #region DPI and Convertions
        public float ConvertMmToPixels(double mm)
        {
            return (float)(mm * DPI / 25.4);
        }

        public double ConvertPixelToMm(double pixel)
        {
            return pixel * 25.4f / DPI; ;
        }

        public float FontSizeToMillimeters(float fontSizeInPoints)
        {
            // Convert points to inches, then inches to millimeters
            return (fontSizeInPoints / 72.0f) * 25.4f;
        }

        public double PixelsToMillimeters(double pixels)
        {
            // Convert pixels to inches, then inches to millimeters
            return pixels / DPI * 25.4f;
        }

        public double MillimetersToPixels(double mm)
        {
            // Convert millimeters to inches, then inches to pixels
            return (mm / 25.4) * DPI;
        }
        #endregion

        #region DrawGrid
        public void DrawGrid(Graphics g, Point panOffset)
        {
            pixelsPerMillimeterX = DPI / 25.4;
            pixelsPerMillimeterY = DPI / 25.4;

            double gridSpacingPixelsX = gridSize * pixelsPerMillimeterX * zoomFactor;
            double gridSpacingPixelsY = gridSize * pixelsPerMillimeterY * zoomFactor;

            double fixedSpacingPixelsX = 4f * pixelsPerMillimeterX * zoomFactor;
            double fixedSpacingPixelsY = 4f * pixelsPerMillimeterY * zoomFactor;

            toolStripStatusLabel1.Text = "DPI = " + g.DpiX.ToString() + " | Grid in pixels converted from millimeters x zoomFactor = " + gridSpacingPixelsX + " px";
            float crossSize = 1.6f; // Length of the cross arms
            int alpha = (int)(16 + (zoomFactor * (225 - 16))); // Clamp between 16 and 255
            alpha = Math.Max(16, Math.Min(225, alpha)); // Ensure it's between 16 and 255
            if (zoomFactor <= 1) alpha = (int)(alpha / 2 * zoomFactor);

            if (gridSize == 0.125f && zoomFactor >= 6f)
            {
                DrawLines(g, alpha, (float)gridSpacingPixelsX, (float)gridSpacingPixelsY);
            }

            if (gridSize == 0.25f && zoomFactor >= 4f)
            {
                DrawLines(g, alpha, (float)gridSpacingPixelsX, (float)gridSpacingPixelsY);
            }

            if (gridSize == 0.5f && zoomFactor >= 2f)
            {
                DrawLines(g, alpha, (float)gridSpacingPixelsX, (float)gridSpacingPixelsY);
            }

            if (gridSize >= 1f && zoomFactor >= 1.2f)
            {
                DrawLines(g, alpha, (float)gridSpacingPixelsX, (float)gridSpacingPixelsY);
            }

            using (Pen p = new Pen(Color.FromArgb(alpha, Color.SteelBlue), 0.1f))
            {
                for (float x = 0; x < g.VisibleClipBounds.Width; x += (float)fixedSpacingPixelsX)
                {
                    for (float y = 0; y < g.VisibleClipBounds.Height; y += (float)fixedSpacingPixelsY)
                    {
                        // Draw horizontal line for the cross
                        g.DrawLine(p, x - crossSize, y, x + crossSize, y);

                        // Draw vertical line for the cross
                        g.DrawLine(p, x, y - crossSize, x, y + crossSize);
                    }
                }
            }


            //pixelsPerMillimeterX = DPI / 25.4;
            //pixelsPerMillimeterY = DPI / 25.4;

            //double gridSpacingPixelsX = gridSize * pixelsPerMillimeterX * zoomFactor;
            //double gridSpacingPixelsY = gridSize * pixelsPerMillimeterY * zoomFactor;

            //double fixedSpacingPixelsX = 4f * pixelsPerMillimeterX * zoomFactor;
            //double fixedSpacingPixelsY = 4f * pixelsPerMillimeterY * zoomFactor;

            //float crossSize = 1.6f; // Length of the cross arms
            //int alpha = (int)(16 + (zoomFactor * (225 - 16)));
            //alpha = Math.Max(16, Math.Min(225, alpha));
            //if (zoomFactor <= 1) alpha = (int)(alpha / 2 * zoomFactor);

            //// Conditionally draw lines based on grid size and zoom factor
            //if ((gridSize == 0.125f && zoomFactor >= 6f) ||
            //    (gridSize == 0.25f && zoomFactor >= 4f) ||
            //    (gridSize == 0.5f && zoomFactor >= 2f) ||
            //    (gridSize >= 1f && zoomFactor >= 1.2f))
            //{
            //    DrawLines(g, alpha, (float)gridSpacingPixelsX, (float)gridSpacingPixelsY, panOffset);
            //}

            //using (Pen p = new Pen(Color.FromArgb(alpha, Color.SteelBlue), 0.1f))
            //{
            //    for (float x = panOffset.X % (float)fixedSpacingPixelsX; x < g.VisibleClipBounds.Width; x += (float)fixedSpacingPixelsX)
            //    {
            //        for (float y = panOffset.Y % (float)fixedSpacingPixelsY; y < g.VisibleClipBounds.Height; y += (float)fixedSpacingPixelsY)
            //        {
            //            // Draw horizontal line for the cross
            //            g.DrawLine(p, x - crossSize, y, x + crossSize, y);

            //            // Draw vertical line for the cross
            //            g.DrawLine(p, x, y - crossSize, x, y + crossSize);
            //        }
            //    }
            //}


            //pixelsPerMillimeterX = DPI / 25.4;
            //pixelsPerMillimeterY = DPI / 25.4;

            //double gridSpacingPixelsX = gridSize * pixelsPerMillimeterX * zoomFactor;
            //double gridSpacingPixelsY = gridSize * pixelsPerMillimeterY * zoomFactor;

            //double fixedSpacingPixelsX = 4f * pixelsPerMillimeterX * zoomFactor;
            //double fixedSpacingPixelsY = 4f * pixelsPerMillimeterY * zoomFactor;

            //float crossSize = 1.6f;
            //int alpha = (int)(16 + (zoomFactor * (225 - 16)));
            //alpha = Math.Max(16, Math.Min(225, alpha));
            //if (zoomFactor <= 1) alpha = (int)(alpha / 2 * zoomFactor);

            //// Draw finer grid lines if zoom factor is appropriate
            //if ((gridSize == 0.125f && zoomFactor >= 6f) ||
            //    (gridSize == 0.25f && zoomFactor >= 4f) ||
            //    (gridSize == 0.5f && zoomFactor >= 2f) ||
            //    (gridSize >= 1f && zoomFactor >= 1.2f))
            //{
            //    DrawLines(g, alpha, (float)gridSpacingPixelsX, (float)gridSpacingPixelsY, panOffset);
            //}

            //using (Pen p = new Pen(Color.FromArgb(alpha, Color.SteelBlue), 0.1f))
            //{
            //    // Adjust starting points based on panOffset
            //    for (float x = panOffset.X % (float)fixedSpacingPixelsX - (float)fixedSpacingPixelsX; x < g.VisibleClipBounds.Width; x += (float)fixedSpacingPixelsX)
            //    {
            //        for (float y = panOffset.Y % (float)fixedSpacingPixelsY - (float)fixedSpacingPixelsY; y < g.VisibleClipBounds.Height; y += (float)fixedSpacingPixelsY)
            //        {
            //            g.DrawLine(p, x - crossSize, y, x + crossSize, y);
            //            g.DrawLine(p, x, y - crossSize, x, y + crossSize);
            //        }
            //    }
            //}
        }

        //Draw the lines if zoom factor is appropriate
        private void DrawLines(Graphics g, int alpha, float gridSpacingPixelsX, float gridSpacingPixelsY)
        {
            using (Pen p = new Pen(Color.FromArgb(alpha, 228, 228, 255), 0.1f))
            {
                for (float x = 0; x < g.VisibleClipBounds.Width; x += gridSpacingPixelsX)
                {
                    g.DrawLine(p, x, 0, x, g.VisibleClipBounds.Height);
                }

                for (float y = 0; y < g.VisibleClipBounds.Height; y += gridSpacingPixelsY)
                {
                    g.DrawLine(p, 0, y, g.VisibleClipBounds.Width, y);
                }
            }
        }

        //private void DrawLines(Graphics g, int alpha, float gridSpacingPixelsX, float gridSpacingPixelsY, Point panOffset)
        //{
        //    using (Pen p = new Pen(Color.FromArgb(alpha, 228, 228, 255), 0.1f))
        //    {
        //        for (float x = panOffset.X % gridSpacingPixelsX; x < g.VisibleClipBounds.Width; x += gridSpacingPixelsX)
        //        {
        //            g.DrawLine(p, x, 0, x, g.VisibleClipBounds.Height);
        //        }

        //        for (float y = panOffset.Y % gridSpacingPixelsY; y < g.VisibleClipBounds.Height; y += gridSpacingPixelsY)
        //        {
        //            g.DrawLine(p, 0, y, g.VisibleClipBounds.Width, y);
        //        }
        //    }
        //}
        //private void DrawLines(Graphics g, int alpha, float gridSpacingPixelsX, float gridSpacingPixelsY, Point panOffset)
        //{
        //    using (Pen p = new Pen(Color.FromArgb(alpha, 228, 228, 255), 0.1f))
        //    {
        //        // Horizontal lines
        //        for (float x = panOffset.X % gridSpacingPixelsX - gridSpacingPixelsX; x < g.VisibleClipBounds.Width; x += gridSpacingPixelsX)
        //        {
        //            g.DrawLine(p, x, 0, x, g.VisibleClipBounds.Height);
        //        }

        //        // Vertical lines
        //        for (float y = panOffset.Y % gridSpacingPixelsY - gridSpacingPixelsY; y < g.VisibleClipBounds.Height; y += gridSpacingPixelsY)
        //        {
        //            g.DrawLine(p, 0, y, g.VisibleClipBounds.Width, y);
        //        }
        //    }
        //}
        #endregion

        private void UncheckAllButtons()
        {
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                // Check if the item is a ToolStripButton and uncheck it if true
                if (item is ToolStripButton button)
                {
                    button.Checked = false;
                    tool = "";
                    isMouseDown = false;
                    allowDetecting = false;
                    detectedShape = null;
                }
            }
        }

        #region  Tool event handlers
        private void Tool_Click(object sender, EventArgs e)
        {
            detectedEdge = ShapeBase.ShapeEdge.None;
            pictureBox1.Cursor = Cursors.Default;
            allowDrawing = false;
            ResetEventHandlersForAll();
            AssignEventHandlersForAll(false);
            ToolStripButton bt = sender as ToolStripButton;

            switch (bt.Name)
            {
                case "toolText":
                    allowDetecting = false;
                    allowMoving = false;
                    allowResizing = false;
                    CreateTextShape();
                    break;

                //code for disable detection when drawing
                //to allow drawing right on other edges of other shapes
                //so allow detection = false
                case "toolConnection":
                    tool = "Connection";
                    allowDetecting = false;
                    break;

                case "toolRectangle":
                    tool = "Rectangle";
                    allowDetecting = false;
                    toolDuplicate.Enabled = true;
                    break;

                case "toolEllipse":
                    tool = "Ellipse";
                    allowDetecting = false;
                    break;

                case "toolArc":
                    tool = "Arc";
                    allowDetecting = false;
                    break;

                case "toolLine":
                    tool = "Line";
                    allowDetecting = false;
                    toolDuplicate.Enabled = true;
                    break;

                case "toolPolygon":
                    tool = "Polygon";
                    allowDetecting = false;
                    break;

                case "toolDot":
                    tool = "Dot";
                    allowDetecting = false;
                    break;

                //from here allow detection = true
                case "toolDelete":
                    tool = "Delete";
                    allowDetecting = true;
                    allowResizing = false;
                    AssignEventHandlersForAll(true);
                    break;

                case "toolResize":
                    tool = "Resize";
                    allowDetecting = true;
                    allowResizing = true;
                    break;

                case "toolMove":
                    tool = "Move";
                    allowDetecting = true;
                    allowMoving = true;
                    allowResizing = false;
                    break;

                case "toolFlipHoriz":
                    tool = "FlipHoriz";
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    break;

                case "toolFlipVert":
                    tool = "FlipVert";
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    break;

                case "toolRotate":
                    tool = "Rotate";
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    break;

                case "toolDuplicate":
                    tool = "Duplicate";
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    break;

                case "toolGroup":
                    tool = "Group";
                    selectionRectangle = Rectangle.Empty;
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    break;

                case "toolUngroup":
                    tool = "Ungroup";
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    break;

                case "toolCreateSym":
                    tool = "CreateSym";
                    selectionRectangle = Rectangle.Empty;
                    allowDetecting = false;
                    allowResizing = false;
                    allowMoving = false;
                    break;

                case "toolUnpackSym":
                    tool = "UnpackSym";
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    break;

                case "toolModifyObject":
                    tool = "ModifyObject";
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    break;

                case "toolBringToFront":
                    tool = "BringToFront";
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    break;

                case "toolSendToBack":
                    tool = "SendToBack";
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    break;

                default:
                    tool = "";
                    allowDetecting = false;
                    allowResizing = false;
                    allowMoving = true;
                    isMouseDown = false;
                    break;
            }
            detectedShape = null;
        }
        #endregion

        private void DeleteShape(ShapeBase detectedShape)
        {
            if (detectedShape != null)
            {
                shapes.Remove(detectedShape);
                detectedShape = null;
                pictureBox1.Invalidate(); // Refresh the canvas
            }
        }

        private void SimulateKeyDown(Keys key)
        {
            tempTextShape = null;
            KeyEventArgs keyEvent = new KeyEventArgs(key);
            Form1_KeyDown(this, keyEvent);
        }

        #region Open/Save XML file
        private void openSymbolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyZoom();//update symbol dimension to zoom factor
            UncheckAllButtons();
            var loadShapes = XmlHelper.LoadShapesFromFile(true);

            // Change the shapes ID every time the same symbol is loaded.
            string newId = Guid.NewGuid().ToString();

            // Add regular shapes to the shapes list
            if (loadShapes != null)
            {
                foreach (var shape in loadShapes)
                {
                    if (shape is SymbolShape symbol)
                    {
                        symbol.SymbolID = newId;
                    }
                    shapes.Add(shape);
                }
            }
            pictureBox1.Invalidate(); // Trigger a redraw to display loaded shapes
        }

        private void openDrawingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyZoom();//update drawing dimension to zoom factor
            UncheckAllButtons();
            var loadedShapes = XmlHelper.LoadShapesFromFile(false);

            if (loadedShapes != null)
            {
                // Add regular shapes to the shapes list
                if (loadedShapes != null)
                {
                    foreach (var shape in loadedShapes)
                    {
                        shapes.Add(shape);
                    }
                }
                pictureBox1.Invalidate(); // Trigger a redraw to display loaded shapes
            }
        }

        private void saveDrawingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlHelper.SaveShapesToFile(shapes, false);
            SimulateKeyDown(Keys.Escape);
        }

        private void SaveSymbol(SymbolShape symbol)
        {
            //XmlHelper.SaveShapesToFile(shapes, true);

            SaveSymbolToFolderAndLoadedSymbolsAndAddToFlowLayout(symbol);
            SimulateKeyDown(Keys.Escape);
        }

        private void saveSymbolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlHelper.SaveShapesToFile(shapes, true);


            SimulateKeyDown(Keys.Escape);
        }

        #endregion

        #region Keyboard
        private bool isShiftPressed = false;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                isShiftPressed = true;
            }

            if (e.KeyCode == Keys.C)
            {
                // Enable tool connection
                // Connection is created after left mouse click
                UncheckAllButtons();
                tool = "";
                startPoint = new Point();
                toolConnection.Checked = true;
                tool = "Connection";
            }

            if (e.KeyCode == Keys.Delete)
            {
                UncheckAllButtons();
                toolDelete.Checked = true;
                tool = "Delete";
                allowDetecting = true;
                toolDelete.PerformClick();
                pictureBox1.Cursor = Cursors.Default;
                DeleteShape(detectedShape);
                pictureBox1.Invalidate();
            }

            if (e.KeyCode == Keys.Escape)
            {
                detectedShape = null;
                currentShape = null;
                groupShape = null;
                pictureBox1.Cursor = Cursors.Default;
                UncheckAllButtons();
                tool = "";
                vertices.Clear();
                pictureBox1.Invalidate();
            }

            if (e.KeyCode == Keys.M)
            {
                detectedShape = null;
                currentShape = null;
                groupShape = null;
                UncheckAllButtons();
                toolMove.Checked = true;
                allowDetecting = true;
                toolMove.PerformClick();
                tool = "Move";
            }

            if (e.KeyCode == Keys.E)
            {
                detectedShape = null;
                currentShape = null;
                groupShape = null;
                UncheckAllButtons();
                toolResize.Checked = true;
                toolResize.PerformClick();
                tool = "Edit";
                allowDetecting = true;
            }

            if (e.KeyCode == Keys.H)
            {
                toolFlipHoriz.Checked = true;
                tool = "FlipHoriz";
                allowDetecting = true;
                toolFlipHoriz.PerformClick();
                pictureBox1.Cursor = Cursors.Default;
                if (detectedInternalShape != null) detectedSymbol.Flip(true);
                pictureBox1.Invalidate();
            }

            if (e.KeyCode == Keys.V)
            {
                UncheckAllButtons();
                allowDetecting = true;
                toolFlipVert.Checked = true;
                tool = "FlipVert";
                toolFlipVert.PerformClick();
                pictureBox1.Cursor = Cursors.Default;
                if (detectedInternalShape != null) detectedSymbol.Flip(false);
                pictureBox1.Invalidate();
            }

            if (e.KeyCode == Keys.R)
            {
                UncheckAllButtons();
                allowDetecting = true;
                toolRotate.Checked = true;
                tool = "Rotate";
                toolRotate.PerformClick();
                pictureBox1.Cursor = Cursors.Default;
                if (detectedInternalShape != null) detectedSymbol.Rotate(90);
                pictureBox1.Invalidate();
            }

            if (isShiftPressed)
            {
                ChangeGridSize();
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            // Reset Shift key state when released
            if (e.KeyCode == Keys.ShiftKey)
            {
                isShiftPressed = false;
                gridSize = selectedGrid;
                pictureBox1.Invalidate();
            }
        }

        private void ChangeGridSize()
        {
            gridSize = 0.5f;
        }
        #endregion

        #region Document page size
        private void Form1_Resize(object sender, EventArgs e)
        {

            UpdateCanvas();
            ApplyZoom();//update drawing

        }

        private void UpdateCanvas()
        {
            SetPictureBoxSize();
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics b = Graphics.FromImage(bmp);
            DrawGrid(b, panOffset);
            pictureBox1.Image = bmp;
        }


        private void SetPictureBoxSize()
        {
            int width = 297;
            int height = 210;

            if (pageSetupIndex == 0)
            {
                width = (int)(210 * DPI / 25.4);
                height = (int)(297 * DPI / 25.4);
            }
            if (pageSetupIndex == 1)
            {
                height = (int)(210 * DPI / 25.4);
                width = (int)(297 * DPI / 25.4);
            }

            if (pageSetupIndex == 2)
            {
                width = (int)(297 * DPI / 25.4);
                height = (int)(420 * DPI / 25.4);
            }

            if (pageSetupIndex == 3)
            {
                height = (int)(297 * DPI / 25.4);
                width = (int)(429 * DPI / 25.4);
            }

            pictureBox1.Width = width;
            pictureBox1.Height = height;
            if (zoomFactor < 1)
            {
                pictureBox1.Width = (int)(width * zoomFactor);
                pictureBox1.Height = (int)(height * zoomFactor);
            }
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            int clickedItem = Convert.ToInt32((sender as ToolStripMenuItem).Tag);
            pageSetupIndex = clickedItem;
            UpdateCanvas();
        }
        #endregion

        #region Language
        private void saveLangFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                saveFileDialog.Title = "Save controls name";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = Path.Combine(desktopPath, saveFileDialog.FileName);
                    LanguageHelper.SaveControlTexts(this, filePath);
                }
            }
        }

        private void loadLangFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                ofd.DefaultExt = "txt";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    LanguageHelper.LoadControlTexts(this, ofd.FileName);
                }
            }
        }
        #endregion

        #region Print document
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }
        #endregion

        #region Save drawing as image
        private void saveAsImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap drawingBMP = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            b = Graphics.FromImage(drawingBMP);
            b.SmoothingMode = SmoothingMode.AntiAlias;
            b.TextRenderingHint = TextRenderingHint.AntiAlias;
            b.Clear(Color.White);

            foreach (var shape in shapes)
            {
                shape.Draw(b);
            }

            string saveFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            using (SaveFileDialog saveFile = new SaveFileDialog())
            {
                saveFile.InitialDirectory = saveFilePath;
                saveFile.Title = "Save shape";
                saveFile.FileName = "Drawing";

                saveFile.Filter = "Image type" +
                    "|*.jpg; *.png" +
                    "|JPEG |*.jpg" +
                    "|PNG |*.png";


                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    drawingBMP.Save(saveFile.FileName);
                }
            }
        }
        #endregion

        #region ContextMenu
        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "Move":
                    UncheckAllButtons();
                    allowDetecting = true;
                    allowResizing = false;
                    toolMove.Checked = true;
                    allowMoving = true;
                    tool = "Move";
                    break;

                case "Modify":
                    UncheckAllButtons();
                    toolModifyObject.Checked = true;
                    allowDetecting = true;
                    allowResizing = false;
                    tool = "ModifyObject";
                    break;

                case "Delete":
                    UncheckAllButtons();
                    toolDelete.Checked = true;
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    tool = "Delete";
                    break;

                case "Resize":
                    UncheckAllButtons();
                    toolResize.Checked = true;
                    allowDetecting = true;
                    allowResizing = true;
                    tool = "Resize";
                    break;

                case "Flip Horizontal"://contextMenuStrip1 text is different from button name
                    UncheckAllButtons();
                    toolFlipHoriz.Checked = true;
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    tool = "FlipHoriz";
                    break;

                case "Flip Vertical"://contextMenuStrip1 text is different from button name
                    UncheckAllButtons();
                    toolFlipVert.Checked = true;
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    tool = "FlipVert";
                    break;

                case "Rotate 90":
                    UncheckAllButtons();
                    toolRotate.Checked = true;
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    tool = "Rotate";
                    break;

                case "Bring to Front":
                    UncheckAllButtons();
                    toolBringToFront.Checked = true;
                    allowDetecting = true;
                    allowResizing = false;
                    tool = "BringToFront";
                    allowMoving = false;
                    break;

                case "Send to Back":
                    UncheckAllButtons();
                    toolSendToBack.Checked = true;
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    tool = "SendToBack";
                    break;

                case "Break Symbol":
                    UncheckAllButtons();
                    toolUnpackSym.Checked = true;
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    tool = "UnpackSym";
                    break;

                case "Duplicate":
                    UncheckAllButtons();
                    toolDuplicate.Checked = true;
                    allowDetecting = true;
                    allowResizing = false;
                    allowMoving = false;
                    tool = "Duplicate";
                    break;
            }
        }
        #endregion

        public void BringToFront(ShapeBase shape)
        {
            if (shapes.Contains(shape))
            {
                var bringToFrontCommand = new BringToFrontCommand(shapes, shape);
                commandManager.ExecuteCommand(bringToFrontCommand);
                pictureBox1.Invalidate(); // Trigger redraw
            }
        }

        public void SendToBack(ShapeBase shape)
        {
            if (shapes.Contains(shape))
            {
                var sendToBackCommand = new SendToBackCommand(shapes, shape);
                commandManager.ExecuteCommand(sendToBackCommand);
                pictureBox1.Invalidate(); // Trigger redraw
            }
        }

        private void showConnectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var showConnectionCommand = new ShowConnectionCommand(shapes);
            commandManager.ExecuteCommand(showConnectionCommand);
            pictureBox1.Invalidate();
        }

        private void hideConnectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var hideConnectionCommand = new HideConnectionCommand(shapes);
            commandManager.ExecuteCommand(hideConnectionCommand);
            pictureBox1.Invalidate();
        }

        #region Zoom area
        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }

        private const int WM_MOUSEWHEEL = 0x020A; // Mouse wheel message

        protected override void WndProc(ref Message m)
        {
            // Check if the mouse wheel event is being captured
            if (m.Msg == WM_MOUSEWHEEL)
            {
                // Get the mouse position relative to the PictureBox
                //Point mousePosition = PointToClient(Cursor.Position);

                // Check if the mouse is over the PictureBox
                //if (pictureBox1.ClientRectangle.Contains(pictureBox1.PointToClient(Cursor.Position)))
                {
                    //MouseEventArgs e = new MouseEventArgs(MouseButtons.None, 0, mousePosition.X, mousePosition.Y, (short)((m.WParam.ToInt32() >> 16) & 0xffff));

                    int wheelDelta = (short)((m.WParam.ToInt32() >> 16) & 0xffff);


                    // Call your zoom method based on the mouse wheel delta
                    if (wheelDelta > 0)
                    {
                        ZoomIn();
                    }
                    else
                    {
                        ZoomOut();
                    }

                    // Prevent further processing of the mouse wheel event
                    return;
                }
            }

            // Call the base class WndProc
            base.WndProc(ref m);
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {

            if (e.Delta > 0)
            {
                ZoomIn();
            }
            else if (e.Delta < 0)
            {
                ZoomOut();
            }

        }

        private void ZoomIn()
        {
            if (zoomFactor < 20f) // Prevent zooming in too much
            {
                zoomFactor += 0.05f;
                ApplyZoom();
            }
        }

        private void ZoomOut()
        {
            if (zoomFactor > 0.2f) // Prevent zooming out too much
            {
                zoomFactor -= 0.05f;
                ApplyZoom();
            }
        }

        private void ApplyZoom()
        {
            foreach (var shape in shapes)
            {
                shape.ZoomFactor = zoomFactor;
            }

            double percentage = 100.0 * zoomFactor;
            toolStripLabel2.Text = percentage.ToString("00") + "%";
            UpdateCanvas();
        }

        private void btnZoom1_Click(object sender, EventArgs e)
        {
            zoomFactor = 1;
            ApplyZoom();
        }
        #endregion

        // This is executed at runtime when the app starts
        private void LoadSymbolsAndDisplayInFlowLayoutPanel()
        {
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string symbolFolderPath = Path.Combine(documentsFolder, "Symbol Maker", "Symbols");
            Debug.WriteLine("No Exists ? = " + !Directory.Exists(symbolFolderPath));
            if (!Directory.Exists(symbolFolderPath))
            {
                Directory.CreateDirectory(symbolFolderPath);

                Debug.WriteLine("Create = " + symbolFolderPath);
                MessageBox.Show("'Symbols' folder not found.\nA new folder 'Symbols' has been created on your Desktop in Symbol Maker folder.", "Error loading symbol list", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Return since there are no symbols to load yet
            }

            // Check if the folder contains any .sim files
            string[] symbolFiles = Directory.GetFiles(symbolFolderPath, "*.sim");
            if (symbolFiles.Length == 0)
            {
                MessageBox.Show("Documents \\ Symbol Maker \\ Symbols folder is empty.\nPlease add symbol files (*.sim) to the folder.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return; // Exit if no files are found
            }

            LoadAndDisplayThumbnails(symbolFolderPath);
        }

        public void LoadAndDisplayThumbnails(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                MessageBox.Show("The selected folder is invalid.");
                return;
            }

            List<ShapeBase> ldshapes = LoadSymbolsFromFolder(folderPath);
            flowLayoutPanel1.Controls.Clear();
            ToolTip toolTip = new ToolTip();

            foreach (var shape in ldshapes)
            {
                if (shape is SymbolShape symbol)
                {
                    loadedSymbols.Add(symbol);
                    Image thumbnail = GraphExtension.GenerateThumbnail(symbol, 64, 64);

                    // Store the SymbolID in the Tag (which is the unique identifier)
                    thumbnailPicBox = new PictureBox
                    {
                        Image = thumbnail,
                        SizeMode = PictureBoxSizeMode.CenterImage,
                        Width = 64,
                        Height = 64,
                        Tag = symbol.SymbolID, // Store the SymbolID in the Tag
                        BorderStyle = BorderStyle.None
                    };

                    toolTip.SetToolTip(thumbnailPicBox, symbol.SymbolName.StringText);
                    flowLayoutPanel1.Controls.Add(thumbnailPicBox);

                    // The default event is drawing -> put the symbol from flowLayoutPanel to drawing surface
                    thumbnailPicBox.MouseDoubleClick += ThumbnailPicBox_MouseDoubleClickForDrawing;

                    thumbnailPicBox.MouseEnter -= ThumbnailPicBox_MouseEnter;
                    thumbnailPicBox.MouseLeave -= ThumbnailPicBox_MouseLeave;
                    thumbnailPicBox.MouseEnter += ThumbnailPicBox_MouseEnter;
                    thumbnailPicBox.MouseLeave += ThumbnailPicBox_MouseLeave;
                }
            }
        }

        private void ThumbnailPicBox_MouseEnter(object sender, EventArgs e)
        {
            if (sender is PictureBox thumbnailPicBox)
            {
                thumbnailPicBox.Capture = true;
                // Loop through all thumbnails and reset their borders except the current one
                foreach (Control control in flowLayoutPanel1.Controls)
                {
                    if (control is PictureBox otherThumbnail && otherThumbnail != thumbnailPicBox)
                    {
                        otherThumbnail.BorderStyle = BorderStyle.None;
                    }

                }
                thumbnailPicBox.BorderStyle = BorderStyle.FixedSingle; // Highlight the currently entered thumbnail
            }
        }

        private void ThumbnailPicBox_MouseLeave(object sender, EventArgs e)
        {
            if (sender is PictureBox thumbnailPicBox)
            {
                thumbnailPicBox.Capture = false;
                thumbnailPicBox.BorderStyle = BorderStyle.None; // Revert to original border

                foreach (Control control in flowLayoutPanel1.Controls)
                {
                    if (control is PictureBox otherThumbnail && otherThumbnail != thumbnailPicBox)
                    {
                        otherThumbnail.BorderStyle = BorderStyle.None;
                    }

                }
            }
        }

        public List<ShapeBase> LoadSymbolsFromFolder(string folderPath)
        {
            List<ShapeBase> shapes = new List<ShapeBase>();

            string[] simFiles = Directory.GetFiles(folderPath, "*.sim");

            foreach (string file in simFiles)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<ShapeBase>), new Type[]
                {
                    typeof(SymbolShape)
                });

                using (StreamReader reader = new StreamReader(file))
                {
                    try
                    {
                        var loadedShapes = (List<ShapeBase>)serializer.Deserialize(reader);
                        shapes.AddRange(loadedShapes);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading file {file}: {ex.Message}", "Loading symbol error");
                    }
                }
            }

            return shapes;
        }

        public void SaveSymbolToFolderAndLoadedSymbolsAndAddToFlowLayout(SymbolShape symbol)
        {
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Define the path to the "Symbols" folder inside "Documents\Symbol Maker"
            string symbolFolderPath = Path.Combine(documentsFolder, "Symbol Maker", "Symbols");

            // Check if the folder exists; if not, create it
            if (!Directory.Exists(symbolFolderPath))
            {
                Directory.CreateDirectory(symbolFolderPath);
            }

            // Define the file path for the new symbol, using the SymbolName or ID as the file name
            string fileName = symbol.SymbolName.StringText + ".sim"; // Use symbol's name or unique ID
            string filePath = Path.Combine(symbolFolderPath, fileName);

            // Ensure the file name is unique by appending a number if the file already exists
            int counter = 1;
            while (File.Exists(filePath))
            {
                fileName = symbol.SymbolName.StringText + $"_{counter}.sim";
                filePath = Path.Combine(symbolFolderPath, fileName);
                counter++;
            }

            // Serialize the symbol as a list of ShapeBase, including derived types
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
                try
                {
                    // Serialize the new symbol as a single-item list and
                    // save the new symbol in Symbols folder
                    serializer.Serialize(writer, new List<ShapeBase> { symbol });

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving symbol: {ex.Message}");
                    return; // Stop if there's an error
                }
            }

            // After successfully saving the symbol, generate a thumbnail and add it to the FlowLayoutPanel
            Image thumbnail = GraphExtension.GenerateThumbnail(symbol, 65, 65); // Generate the thumbnail

            // Create PictureBox and set its properties
            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = thumbnail;
            pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox.Width = 65;  // Set PictureBox size to match thumbnail size
            pictureBox.Height = 65;
            pictureBox.Tag = symbol.SymbolID; // Assign the SymbolID as the Tag

            // Add to flowLayoutPanel1
            flowLayoutPanel1.Controls.Add(pictureBox);

            // Add to loadedSymbols list
            loadedSymbols.Add(symbol);
            MessageBox.Show($"Symbol '{symbol.SymbolName.StringText}' saved successfully.");
        }

        public void RemoveSymbolFromPanelLoadedSymbolsAndFolder(SymbolShape symbol)
        {
            if (symbol == null)
            {
                MessageBox.Show("Symbol cannot be null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Confirm deletion with the user
            DialogResult dialogResult = MessageBox.Show($"Are you sure you want to delete the symbol '{symbol.SymbolName.StringText}'?\nThis command cannot be undone.", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button2);
            if (dialogResult != DialogResult.Yes)
            {
                return;
            }

            loadedSymbols.Remove(symbol);// First, remove from loadedSymbols list

            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string symbolFolderPath = Path.Combine(documentsFolder, "Symbol Maker", "Symbols");

            // Get all .sim files in the Symbols folder
            string[] symbolFiles = Directory.GetFiles(symbolFolderPath, "*.sim");

            bool symbolFound = false;

            foreach (string file in symbolFiles)
            {
                try
                {
                    // Deserialize the content of each file and check for SymbolID
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

                    using (StreamReader reader = new StreamReader(file))
                    {
                        List<ShapeBase> shapes = (List<ShapeBase>)serializer.Deserialize(reader);
                        SymbolShape deserializedSymbol = shapes.OfType<SymbolShape>().FirstOrDefault();

                        if (deserializedSymbol != null && deserializedSymbol.SymbolID == symbol.SymbolID)
                        {
                            // Symbol found, proceed to delete the file
                            reader.Close(); // Close the reader to unlock the file
                            File.Delete(file); // Now delete the file safely from Symbols folder

                            symbolFound = true;

                            // Remove the symbol's PictureBox from the FlowLayoutPanel by SymbolID
                            var pictureBoxToRemove = flowLayoutPanel1.Controls
                                .OfType<PictureBox>()
                                .FirstOrDefault(pb => pb.Tag?.ToString() == symbol.SymbolID.ToString());

                            if (pictureBoxToRemove != null)
                            {
                                // Remove the symbol from the UI flowLayoutPanel
                                flowLayoutPanel1.Controls.Remove(pictureBoxToRemove);
                            }
                            else
                            {
                                MessageBox.Show("Unable to find the symbol in the UI for deletion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading symbol file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (!symbolFound)
            {
                MessageBox.Show($"Symbol with ID '{symbol.SymbolID}' could not be found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Iterate over all PictureBoxes in the FlowLayoutPanel and assign the chosen event
        private void AssignEventHandlersForAll(bool deleteMode)
        {
            foreach (Control control in flowLayoutPanel1.Controls)
            {
                if (control is PictureBox thumbnailPicBox && thumbnailPicBox.Tag is string symbolIDString)
                {
                    // Retrieve the corresponding symbol from the loaded symbols list
                    SymbolShape symbol = loadedSymbols.FirstOrDefault(s => s.SymbolID == symbolIDString);

                    if (symbol != null)
                    {
                        // Switch event handlers for this PictureBox based on the mode (delete or add)
                        SwitchMode(deleteMode, thumbnailPicBox, symbol);
                    }
                }
            }
        }

        private void ResetEventHandlersForAll()
        {
            foreach (Control control in flowLayoutPanel1.Controls)
            {
                if (control is PictureBox thumbnailPicBox)
                {
                    // Unsubscribe all events before switching modes
                    thumbnailPicBox.MouseDoubleClick -= ThumbnailPicBox_MouseDoubleClickForDrawing;
                    thumbnailPicBox.MouseDown -= ThumbnailPicBox_MouseDownForDelete;
                }
            }
        }

        // Switch between delete mode and drawing mode
        private void SwitchMode(bool deleteMode, PictureBox thumbnailPicBox, SymbolShape symbol)
        {
            //ResetEventHandlersForAll(); // Clear all existing handlers
            if (deleteMode)
            {
                // Enable deletion mode (MouseDown for deleting)
                // Unsubscribe from the specific event handlers before subscribing
                thumbnailPicBox.MouseDoubleClick -= ThumbnailPicBox_MouseDoubleClickForDrawing;
                thumbnailPicBox.MouseDown -= ThumbnailPicBox_MouseDownForDelete;
                thumbnailPicBox.MouseEnter -= ThumbnailPicBox_MouseEnter;
                thumbnailPicBox.MouseLeave -= ThumbnailPicBox_MouseLeave;

                // Subscribe to MouseDown for deletion
                thumbnailPicBox.MouseDown += ThumbnailPicBox_MouseDownForDelete;
                thumbnailPicBox.MouseEnter += ThumbnailPicBox_MouseEnter;
                thumbnailPicBox.MouseLeave += ThumbnailPicBox_MouseLeave;
            }
            else if (!deleteMode)
            {
                // Enable drawing mode (DoubleClick for adding)
                // Unsubscribe from the specific event handlers before subscribing
                thumbnailPicBox.MouseDown -= ThumbnailPicBox_MouseDownForDelete;
                thumbnailPicBox.MouseDoubleClick -= ThumbnailPicBox_MouseDoubleClickForDrawing;
                thumbnailPicBox.MouseEnter -= ThumbnailPicBox_MouseEnter;
                thumbnailPicBox.MouseLeave -= ThumbnailPicBox_MouseLeave;

                // Subscribe to MouseDoubleClick for adding to the drawing surface
                thumbnailPicBox.MouseDoubleClick += ThumbnailPicBox_MouseDoubleClickForDrawing;
                thumbnailPicBox.MouseEnter += ThumbnailPicBox_MouseEnter;
                thumbnailPicBox.MouseLeave += ThumbnailPicBox_MouseLeave;
            }
        }

        private void ThumbnailPicBox_MouseDownForDelete(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && sender is PictureBox thumbnailPicBox && thumbnailPicBox.Tag is string symbolIDString)
            {
                // Find the symbol by its ID
                SymbolShape symbol = loadedSymbols.FirstOrDefault(s => s.SymbolID == symbolIDString);
                if (symbol != null)
                {
                    // Call the method to delete the symbol
                    RemoveSymbolFromPanelLoadedSymbolsAndFolder(symbol);
                }
            }
        }

        private void ThumbnailPicBox_MouseDoubleClickForDrawing(object sender, EventArgs e)
        {
            if (sender is PictureBox thumbnailPicBox && thumbnailPicBox.Tag is string symbolIDString)
            {
                // Find the symbol by its ID
                SymbolShape symbol = loadedSymbols.FirstOrDefault(s => s.SymbolID == symbolIDString);
                if (symbol != null)
                {
                    // Call the method to add the symbol to the drawing surface
                    AddSymbolToWorkShapesList(symbol.SymbolID);
                }
            }
        }

        private void AddSymbolToWorkShapesList(string symbolID)
        {
            // Find the symbol in the loadedSymbols list
            SymbolShape symbol = loadedSymbols.FirstOrDefault(s => s.SymbolID.ToString() == symbolID);

            if (symbol != null)
            {
                var newSymbol = symbol.Clone();

                float shapeShiftInPixels = ConvertMmToPixels(8);
                float offset = mouseNumClick * shapeShiftInPixels;
                symbol.Move(mouseNumClick * offset, mouseNumClick * offset);
                string newId = Guid.NewGuid().ToString();//assign a new Guid to distinguish between others similar symbols

                if (newSymbol is SymbolShape sym)
                {
                    sym.SymbolID = newId;
                    var addShapeCommand = new AddShapeCommand(shapes, sym);
                    commandManager.ExecuteCommand(addShapeCommand);
                    UpdateUndoRedoButtons();
                }
            }
            pictureBox1.Invalidate();
        }

        private double selectedGrid = 4;
        private void gridSizeDropDownButton_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                if (Convert.ToInt32(menuItem.Tag) is int index)
                {
                    if (index >= 0 && index < gridSizes.Length)
                    {
                        gridSize = gridSizes[index];
                        selectedGrid = gridSizes[index];
                        UpdateCanvas();
                        gridSizeDropDownButton.Text = gridSize.ToString();
                    }
                }
            }
        }

        private void saveAsPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument;

            // Directly assign the "Microsoft Print to PDF" printer
            printDocument.PrinterSettings.PrinterName = "Microsoft Print to PDF";

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Set the full path where the PDF will be saved
            printDocument.PrinterSettings.PrintToFile = true;
            printDocument.PrinterSettings.PrintFileName = Path.Combine(desktopPath);
            printDocument.DocumentName = "Drawing";

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            foreach (var shape in shapes)
            {
                shape.Draw(e.Graphics);
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm hf = new HelpForm();
            hf.Show();
        }
    }
}
