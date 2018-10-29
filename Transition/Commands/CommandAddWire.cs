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
        public string Title => throw new NotImplementedException();

        public Wire Wire { get; set; }

        public void execute()
        {
            CircuitEditor.CircuitEditor.currentInstance.currentDesign.addWire(Wire);
        }

        public void unExecute()
        {
            throw new NotImplementedException();
        }
    }
}
