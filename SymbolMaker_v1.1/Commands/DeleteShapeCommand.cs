using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolMaker
{
    public class DeleteShapeCommand : ICommand
    {
        private readonly List<ShapeBase> ShapeList;
        private readonly ShapeBase Shape;

        public DeleteShapeCommand(List<ShapeBase> shapeList, ShapeBase shape)
        {
            ShapeList = shapeList;
            Shape = shape;
        }

        public void Execute()
        {
            ShapeList.Remove(Shape);
        }

        public void Unexecute()
        {
            ShapeList.Add(Shape);
        }
    }
}
