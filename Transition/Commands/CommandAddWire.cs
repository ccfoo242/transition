using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor;
using Transition.CircuitEditor.Serializable;

namespace Transition.Commands
{
    public class CommandAddWire : ICircuitCommand
    {
        public string Title => "Add Wire Command: " + Wire.ToString();

        public SerializableWire Wire { get; set; }

        public override string ToString() => Title;

        public void execute()
        {
            CircuitEditor.CircuitEditor.currentInstance.currentDesign.addWire(Wire);
        }

        public void unExecute()
        {
            CircuitEditor.CircuitEditor.currentInstance.currentDesign.removeWire(Wire);
        }


    }
}
