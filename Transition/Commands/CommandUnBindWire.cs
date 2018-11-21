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
    public class CommandUnBindWire : ICircuitCommand
    {
        public string Title => "UnBindWire " + Wire.ToString() + " Terminal number" + WireTerminalNumber.ToString() + 
            " From " + BoundedObject.ToString() + " Terminal number " + ObjectTerminalNumber.ToString() ;

        public SerializableWire Wire { get; set; }
        public byte WireTerminalNumber { get; set; }

        public SerializableElement BoundedObject { get; set; }
        public byte ObjectTerminalNumber { get; set; }

        public Point2D newPosition;
        public override string ToString() => Title;

        public void execute()
        {
            Wire.unBind(WireTerminalNumber);
            Wire.updatePosition(newPosition, WireTerminalNumber);
        }

        public void unExecute()
        {
            Wire.doBind(WireTerminalNumber, BoundedObject, ObjectTerminalNumber);
        }

       
    }
}
