using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SymbolMaker.ShapeBase;

namespace SymbolMaker
{
    public class EditShapeCommand : ICommand
    {
        private ShapeBase _shape;
        private Color _originalPenColor, _newPenColor;
        private float _originalPenThickness, _newPenThickness;
        private Color _originalFillColor, _newFillColor;
        private LineStyle _originalLineStyle, _newLineStyle;
        private CustomHatchStyle _originalCustomHatch, _newCustomHatch;
        private float _newStartAngle, _oldStartAngle;
        private float _newSweepAngle, _oldSweepAngle;
        private float _newDotRadius, _oldDotRadius;
        private Color _newDotColor, _oldDotColor;

        public EditShapeCommand(
            ShapeBase shape, Color newPenColor, float newPenThickness, Color newFillColor, 
            LineStyle newLineStyle, CustomHatchStyle newCustomHatch,
            float? startAngle = null, float? sweepAngle = null,
            float? dotRadius = null, Color? dotColor = null)
        {
            _shape = shape;
            _originalPenColor = shape.BorderPenColor;
            _originalPenThickness = shape.PenThickness;
            _originalFillColor = shape.FillBrushColor;
            _originalLineStyle = _shape.LinStyle;
            _originalCustomHatch = _shape.HatchStyl;

            _newPenColor = newPenColor;
            _newPenThickness = newPenThickness;
            _newFillColor = newFillColor;
            _newLineStyle = newLineStyle;
            _newCustomHatch = newCustomHatch;

            // Handle ArcShape properties
            if (shape is ArcShape arc)
            {
                _oldStartAngle = arc.StartAngle;
                _oldSweepAngle = arc.SweepAngle;
                _newStartAngle = startAngle ?? _oldStartAngle;
                _newSweepAngle = sweepAngle ?? _oldSweepAngle;
            }

            // Handle DotShape properties
            if (shape is DotShape dot)
            {
                _oldDotRadius = dot.DotRadius;
                _oldDotColor = dot.DotColor;
                _newDotRadius = dotRadius ?? _oldDotRadius;
                _newDotColor = dotColor ?? _oldDotColor;
            }
        }

        public void Execute()
        {
            // Apply new values
            _shape.BorderPenColor = _newPenColor;
            _shape.PenThickness = _newPenThickness;
            _shape.FillBrushColor = _newFillColor;
            _shape.LinStyle = _newLineStyle;
            _shape.HatchStyl = _newCustomHatch;
            if (_shape is ArcShape arc)
            {
                arc.StartAngle = _newStartAngle;
                arc.SweepAngle = _newSweepAngle;
            }

            if (_shape is DotShape dot)
            {
                dot.DotRadius = _newDotRadius;
                dot.DotColor = _newDotColor;
            }
        }

        public void Unexecute()
        {
            // Revert to original values
            _shape.BorderPenColor = _originalPenColor;
            _shape.PenThickness = _originalPenThickness;
            _shape.FillBrushColor = _originalFillColor;
            _shape.LinStyle = _originalLineStyle;
            _shape.HatchStyl = _originalCustomHatch;
            if (_shape is ArcShape arc)
            {
                arc.StartAngle = _oldStartAngle;
                arc.SweepAngle = _oldSweepAngle;
            }

            if (_shape is DotShape dot)
            {
                dot.DotRadius = _oldDotRadius;
                dot.DotColor = _oldDotColor;
            }
        }
    }
}
