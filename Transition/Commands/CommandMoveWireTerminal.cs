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
    public class CommandMoveWireTerminal : ICircuitCommand
    {
        public string Title => "Move WireTerminal " + Wire.ToString() + " terminal number " + WireTerminalNumber.ToString() + ", from " + OldPosition.ToString() + " to " + NewPosition.ToString();

        public SerializableWire Wire { get; set; }
        public byte WireTerminalNumber { get; set; }

        public Point2D OldPosition { get; set; }
        public Point2D NewPosition { get; set; }

        public void execute()
        {
            Wire.updatePosition(NewPosition, WireTerminalNumber);
        }

        public void unExecute()
        {
            Wire.updatePosition(OldPosition, WireTerminalNumber);
        }

        public override string ToString() => Title;
    }
}
