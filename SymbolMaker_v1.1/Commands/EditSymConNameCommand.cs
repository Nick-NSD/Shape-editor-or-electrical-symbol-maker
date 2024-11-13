using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserControls;

namespace SymbolMaker
{
    public class EditSymConNameCommand : ICommand
    {
        private readonly List<ShapeBase> _shapes;
        private ConnectionShape _connection;
        private SymbolShape _symbol;
        private List<ConnectionShape> _allSymbolConnections;
        private List<ConnectionShape> _allSymbolsConnections;

        // Old values for undo
        private string _oldText;
        private bool _oldVisible;
        private Font _oldFont;
        private Color _oldColor;
        private TextAlignment _oldAlign;
        private TextRotation _oldRotation;

        // New values for redo
        private string _newText;
        private bool _newVisible;
        private Font _newFont;
        private Color _newColor;
        private TextAlignment _newAlign;
        private TextRotation _newRotation;

        private bool _applyToSymbolConnections;
        private bool _applyToAllSymbolsConnections;

        public EditSymConNameCommand(
            List<ShapeBase> shapes,
            ConnectionShape connection,
            SymbolShape symbol,
            string newText,
            bool newVisible,
            Font newFont,
            Color newColor,
            TextAlignment newAlign,
            TextRotation newRotation,
            bool applyToSymbolConnections,
            bool applyToAllSymbolsConnections
        )
        {
            _shapes = shapes;
            _connection = connection;
            _symbol = symbol;

            // Store the old values
            _oldText = connection.ConnectionName.StringText;
            _oldVisible = connection.ConnectionNameVisible;
            _oldFont = connection.ConnectionName.TextFont;
            _oldColor = connection.ConnectionName.TextColor;
            _oldAlign = connection.ConnectionName.TextShapeAlign;
            _oldRotation = connection.ConnectionName.TextShapeRotation;

            // Store the new values
            _newText = newText;
            _newVisible = newVisible;
            _newFont = newFont;
            _newColor = newColor;
            _newAlign = newAlign;
            _newRotation = newRotation;

            _applyToSymbolConnections = applyToSymbolConnections;
            _applyToAllSymbolsConnections = applyToAllSymbolsConnections;

            // Get connections if needed
            if (_applyToSymbolConnections)
                _allSymbolConnections = symbol.GetAllConnections();
            if (_applyToAllSymbolsConnections)
                _allSymbolsConnections = GetAllConnectionsFromAllSymbols(); // Custom method to get all connections from all symbols
        }

        public void Execute()
        {
            ApplyChanges(_newText, _newVisible, _newFont, _newColor, _newAlign, _newRotation);

            if (_applyToSymbolConnections)
            {
                foreach (var conn in _allSymbolConnections)
                    ApplyChangesToConnection(conn, _newFont, _newColor, _newAlign, _newRotation);
            }

            if (_applyToAllSymbolsConnections)
            {
                foreach (var conn in _allSymbolsConnections)
                    ApplyChangesToConnection(conn, _newFont, _newColor, _newAlign, _newRotation);
            }
        }

        public void Unexecute()
        {
            ApplyChanges(_oldText, _oldVisible, _oldFont, _oldColor, _oldAlign, _oldRotation);

            if (_applyToSymbolConnections)
            {
                foreach (var conn in _allSymbolConnections)
                    ApplyChangesToConnection(conn, _oldFont, _oldColor, _oldAlign, _oldRotation);
            }

            if (_applyToAllSymbolsConnections)
            {
                foreach (var conn in _allSymbolsConnections)
                    ApplyChangesToConnection(conn, _oldFont, _oldColor, _oldAlign, _oldRotation);
            }
        }

        private void ApplyChanges(string text, bool visible, Font font, Color color, TextAlignment align, TextRotation rotation)
        {
            _connection.ConnectionName.StringText = text;
            _connection.ConnectionNameVisible = visible;
            _connection.ConnectionName.TextFont = font;
            _connection.ConnectionName.TextColor = color;
            _connection.ConnectionName.TextShapeAlign = align;
            _connection.ConnectionName.TextShapeRotation = rotation;
            _connection.ConnectionName.GetSingleShapeBounds();
        }

        private void ApplyChangesToConnection(ConnectionShape conn, Font font, Color color, TextAlignment align, TextRotation rotation)
        {
            conn.ConnectionName.TextFont = font;
            conn.ConnectionName.TextColor = color;
            conn.ConnectionName.TextShapeAlign = align;
            conn.ConnectionName.TextShapeRotation = rotation;
            conn.ConnectionName.GetSingleShapeBounds();
        }

        private List<ConnectionShape> GetAllConnectionsFromAllSymbols()
        {
            var allConnections = new List<ConnectionShape>();

            // Iterate over all shapes in the shapes list
            foreach (var shape in _shapes)
            {
                if (shape is SymbolShape symbol)
                {
                    // Get connections within the SymbolShape
                    var symbolConnections = symbol.GetAllConnections();
                    allConnections.AddRange(symbolConnections);
                }
            }
            return allConnections;
        }
    }

}