﻿using System.Collections.Generic;

namespace SymbolMaker
{
    public class SendToBackCommand : ICommand
    {
        private readonly List<ShapeBase> shapes;
        private readonly ShapeBase shape;
        private int originalIndex;

        public SendToBackCommand(List<ShapeBase> shapes, ShapeBase shape)
        {
            this.shapes = shapes;
            this.shape = shape;
        }

        public void Execute()
        {
            if (shapes.Contains(shape))
            {
                // Save the original index before moving the shape
                originalIndex = shapes.IndexOf(shape);
                shapes.Remove(shape);
                shapes.Insert(0, shape); // Move to the back
            }
        }

        public void Unexecute()
        {
            // Restore the shape to its original position
            if (shapes.Contains(shape))
            {
                shapes.Remove(shape);
                shapes.Insert(originalIndex, shape);
            }
        }
    }

}
