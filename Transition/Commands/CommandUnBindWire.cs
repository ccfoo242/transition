using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor;
using Transition.CircuitEditor.Serializable;

namespace Transition.Commands
{
    public class CommandUnBindWire : ICircuitCommand
    {
        public string Title => throw new NotImplementedException();

        public Wire Wire { get; set; }
        public byte Terminal { get; set; }

        public SerializableElement BoundedObject { get; set; }
        public byte ObjectTerminal { get; set; }

        public double newPositionX;
        public double newPositionY;

        public void execute()
        {
            Wire.unBind(Terminal);
            if (Terminal == 0)
                { Wire.X0 = newPositionX; Wire.Y0 = newPositionY;
                Wire.OnScreenWire.wt0.updateOriginalPosition();
            }
            else
                { Wire.X1 = newPositionX; Wire.Y1 = newPositionY;
                Wire.OnScreenWire.wt1.updateOriginalPosition();
            }
        }

        public void unExecute()
        {
            Wire.bind(BoundedObject, ObjectTerminal, Terminal);
        }
    }
}
