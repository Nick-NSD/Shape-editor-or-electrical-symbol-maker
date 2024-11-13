using System.Collections.Generic;

namespace SymbolMaker
{
    public class HideConnectionCommand : ICommand
    {
        private readonly List<ShapeBase> shapes;
        private Dictionary<SymbolShape, bool> previousVisibility;

        public HideConnectionCommand(List<ShapeBase> shapes)
        {
            this.shapes = shapes;
            this.previousVisibility = new Dictionary<SymbolShape, bool>();
        }

        public void Execute()
        {
            foreach (var shape in shapes)
            {
                if (shape is SymbolShape symbol)
                {
                    // Store previous visibility state
                    previousVisibility[symbol] = symbol.SymbolConnectionVisible;
                    symbol.SymbolConnectionVisible = false;
                }
            }
        }

        public void Unexecute()
        {
            foreach (var kvp in previousVisibility)
            {
                // Restore the previous visibility state
                kvp.Key.SymbolConnectionVisible = kvp.Value;
            }
        }
    }

}
