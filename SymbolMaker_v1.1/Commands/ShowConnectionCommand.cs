using System.Collections.Generic;

namespace SymbolMaker
{
    public class ShowConnectionCommand : ICommand
    {
        private readonly List<ShapeBase> shapes;
        private Dictionary<SymbolShape, bool> previousVisibility;

        public ShowConnectionCommand(List<ShapeBase> shapes)
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
                    symbol.SymbolConnectionVisible = true;
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
