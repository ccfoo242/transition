using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor;
using Transition.CircuitEditor.Serializable;

namespace Transition.Commands
{
    public class CommandRemoveWire : ICircuitCommand
    {
        public string Title => "Remove Wire Command: " + Wire.ToString();
        public override string ToString() => Title;

        public Wire Wire { get; set; }

        public void execute()
        {
            CircuitEditor.CircuitEditor.currentInstance.currentDesign.removeWire(Wire);
        }

        public void unExecute()
        {
            throw new NotImplementedException();
        }
    }
}
