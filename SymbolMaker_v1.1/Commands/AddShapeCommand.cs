using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolMaker
{
    public class AddShapeCommand : ICommand
    {
        private readonly List<ShapeBase> ShapeList;
        private readonly ShapeBase Shape;

        public AddShapeCommand(List<ShapeBase> shapeList, ShapeBase shape)
        {
            ShapeList = shapeList;
            Shape = shape;
        }

        public void Execute()
        {
            ShapeList.Add(Shape);
        }

        public void Unexecute()
        {
            ShapeList.Remove(Shape);
        }
    }
}
