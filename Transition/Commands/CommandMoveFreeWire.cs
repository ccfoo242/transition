using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor;
using Transition.CircuitEditor.Serializable;
using Transition.Common;

namespace Transition.Commands
{
    public class CommandMoveFreeWire : ICircuitCommand
    {
        public string Title => "Command Move Free Wire ";

        public SerializableWire Wire { get; set; }

        public Point2D OldPositionTerminal0 { get; set; }
        public Point2D OldPositionTerminal1 { get; set; }
        public Point2D NewPositionTerminal0 { get; set; }
        public Point2D NewPositionTerminal1 { get; set; }

        public void execute()
        {
            Wire.PositionTerminal0 = NewPositionTerminal0;
            Wire.PositionTerminal1 = NewPositionTerminal1;
        }

        public void unExecute()
        {
            Wire.PositionTerminal0 = OldPositionTerminal0;
            Wire.PositionTerminal1 = OldPositionTerminal1;
        }
    }
}
