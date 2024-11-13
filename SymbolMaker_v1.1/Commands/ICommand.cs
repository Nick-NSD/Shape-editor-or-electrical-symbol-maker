using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymbolMaker
{
    public interface ICommand
    {
        void Execute();
        void Unexecute();
    }
}
