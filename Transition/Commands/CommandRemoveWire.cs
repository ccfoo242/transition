using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor;
using Easycoustics.Transition.CircuitEditor.Serializable;

namespace Easycoustics.Transition.Commands
{
    public class CommandRemoveWire : ICircuitCommand
    {
        public string Title => "Remove Wire Command: " + Wire.ToString();
        public override string ToString() => Title;

        public SerializableWire Wire { get; set; }

        public void execute()
        {
            CircuitEditor.CircuitEditor.currentInstance.CurrentDesign.Wires.Remove(Wire);
        }

        public void unExecute()
        {
            CircuitEditor.CircuitEditor.currentInstance.CurrentDesign.Wires.Add(Wire);
        }
    }
}
