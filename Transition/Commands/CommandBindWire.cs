using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor;
using Transition.CircuitEditor.Serializable;
using Transition.Design;

namespace Transition.Commands
{
    public class CommandBindWire : ICircuitCommand
    {
        public string Title => "Bind " + Wt.ToString() + " Element: " + boundedObject.ToString() + " Element Terminal: " + boundedTerminal.ToString();

        public SerializableWireTerminal Wt { get; set; }

        public SerializableElement boundedObject { get; set; }
        public byte boundedTerminal { get; set; }

        public bool previousStateBounded { get; set; }
        public SerializableElement previousBoundedObject { get; set; }
        public byte previuosBoundedTerminal { get; set; }

        public void execute()
        {
            Wt.Wire.bind(boundedObject, boundedTerminal, Wt.Terminal);
        }

        public void unExecute()
        {
            if (previousStateBounded)
                Wt.Wire.bind(previousBoundedObject, previuosBoundedTerminal, Wt.Terminal);
            else
            {
                Wt.Wire.unBind(Wt.Terminal);
                Wt.Wire.OnScreenWire.getWireTerminal(Wt.Terminal).updateOriginalPosition();
            };
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
