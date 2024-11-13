using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserControls;

namespace SymbolMaker
{
    public class EditTextCommand : ICommand
    {
        private TextShape _textShape;
        private string _oldStringText, _newStringText;
        private Font _oldTextFont, _newTextFont;
        private Color _oldTextColor, _newTextColor;
        private TextAlignment _oldTextShapeAlign, _newTextShapeAlign;
        private TextRotation _oldTextShapeRotation, _newTextShapeRotation;

        public EditTextCommand(
            TextShape textShape,
            string newStringText,
            Font newTextFont,
            Color newTextColor,
            TextAlignment newTextShapeAlign,
            TextRotation newTextShapeRotation
        )
        {
            _textShape = textShape;

            // Store original values
            _oldStringText = textShape.StringText;
            _oldTextFont = textShape.TextFont;
            _oldTextColor = textShape.TextColor;
            _oldTextShapeAlign = textShape.TextShapeAlign;
            _oldTextShapeRotation = textShape.TextShapeRotation;

            // Store new values
            _newStringText = newStringText;
            _newTextFont = newTextFont;
            _newTextColor = newTextColor;
            _newTextShapeAlign = newTextShapeAlign;
            _newTextShapeRotation = newTextShapeRotation;
        }

        public void Execute()
        {
            // Apply new values
            _textShape.StringText = _newStringText;
            _textShape.TextFont = _newTextFont;
            _textShape.TextColor = _newTextColor;
            _textShape.TextShapeAlign = _newTextShapeAlign;
            _textShape.TextShapeRotation = _newTextShapeRotation;
        }

        public void Unexecute()
        {
            // Revert to original values
            _textShape.StringText = _oldStringText;
            _textShape.TextFont = _oldTextFont;
            _textShape.TextColor = _oldTextColor;
            _textShape.TextShapeAlign = _oldTextShapeAlign;
            _textShape.TextShapeRotation = _oldTextShapeRotation;
        }
    }

}
