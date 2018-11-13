using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.Common;

namespace Transition.CircuitEditor.OnScreenComponents
{
    public class Terminal : Tuple<ScreenElementBase, byte>
    {
        public Point2D Position { get; set; }

        public ScreenElementBase Element => Item1;
        public byte TerminalNumber => Item2;

        public Terminal(ScreenElementBase el, byte terminal) : base(el, terminal)
        {

        }
        
    }

    public class WireTerminal : Terminal
    {
        public WireTerminal(ScreenElementBase el, byte terminal) : base(el, terminal)
        {

        }
    }

}
