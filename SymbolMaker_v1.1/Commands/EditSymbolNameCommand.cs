using System.Collections.Generic;
using System.Drawing;
using UserControls;

namespace SymbolMaker
{
    public class EditSymbolNameCommand : ICommand
    {
        private readonly List<ShapeBase> _shapes;
        private readonly SymbolShape _symbol;
        private readonly string _oldName;
        private readonly string _newName;
        private readonly bool _oldVisibility;
        private readonly bool _newVisibility;
        private readonly Font _oldFont;
        private readonly Color _oldColor;
        private readonly TextAlignment _oldAlignment;
        private readonly TextRotation _oldRotation;
        private readonly Font _newFont;
        private readonly Color _newColor;
        private readonly TextAlignment _newAlignment;
        private readonly TextRotation _newRotation;
        public bool _setAllSymbolNames;

        public EditSymbolNameCommand(SymbolShape symbol, string newName, bool newVisibility, Font newFont, Color newColor, TextAlignment newAlignment, TextRotation newRotation, bool setAllSymbolNames, List<ShapeBase>shapes)
        {
            _symbol = symbol;
            _oldName = symbol.SymbolName.StringText;
            _newName = newName;
            _oldVisibility = symbol.SymbolNameVisible;
            _newVisibility = newVisibility;
            _oldFont = symbol.SymbolName.TextFont;
            _oldColor = symbol.SymbolName.TextColor;
            _oldAlignment = symbol.SymbolName.TextShapeAlign;
            _oldRotation = symbol.SymbolName.TextShapeRotation;
            _newFont = newFont;
            _newColor = newColor;
            _newAlignment = newAlignment;
            _newRotation = newRotation;
            _setAllSymbolNames = setAllSymbolNames;
            _shapes = shapes;
        }

        public void Execute()
        {
            _symbol.SymbolName.StringText = _newName;
            _symbol.SymbolNameVisible = _newVisibility;
            _symbol.SymbolName.TextFont = _newFont;
            _symbol.SymbolName.TextColor = _newColor;
            _symbol.SymbolName.TextShapeAlign = _newAlignment;
            _symbol.SymbolName.TextShapeRotation = _newRotation;
            _symbol.SymbolName.GetSingleShapeBounds();

            if (_setAllSymbolNames)
            {
                foreach (var shape in _shapes)
                {
                    if (shape is SymbolShape sym)
                    {
                        foreach (var inshape in sym.InternalShapes)
                        {
                            if (inshape is TextShape ts && ts.TextType == 2)
                            {
                                ts.TextFont = _newFont;
                                ts.TextColor = _newColor;
                                ts.TextShapeAlign = _newAlignment;
                                ts.TextShapeRotation = _newRotation;
                                ts.GetSingleShapeBounds();
                            }
                        }
                    }
                }
            }
        }

        public void Unexecute()
        {
            _symbol.SymbolName.StringText = _oldName;
            _symbol.SymbolNameVisible = _oldVisibility;
            _symbol.SymbolName.TextFont = _oldFont;
            _symbol.SymbolName.TextColor = _oldColor;
            _symbol.SymbolName.TextShapeAlign = _oldAlignment;
            _symbol.SymbolName.TextShapeRotation = _oldRotation;
            _symbol.SymbolName.GetSingleShapeBounds();

            if (_setAllSymbolNames)
            {
                foreach (var shape in _shapes)
                {
                    if (shape is SymbolShape sym)
                    {
                        foreach (var inshape in sym.InternalShapes)
                        {
                            if (inshape is TextShape ts && ts.TextType == 2)
                            {
                                ts.TextFont = _oldFont;
                                ts.TextColor = _oldColor;
                                ts.TextShapeAlign = _oldAlignment;
                                ts.TextShapeRotation = _oldRotation;
                                ts.GetSingleShapeBounds();
                            }
                        }
                    }
                }
            }
        }
    }
}
