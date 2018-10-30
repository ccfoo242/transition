using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor;
using Transition.CircuitEditor.Serializable;

namespace Transition.Commands
{
    class CommandRemoveComponent : ICircuitCommand
    {
        public string Title => "Remove Component: " + Component.ToString();
        public override string ToString() => Title;

        public SerializableComponent Component { get; set; }
        
        public void execute()
        {
            CircuitEditor.CircuitEditor.currentInstance.currentDesign.removeComponent(Component);
        }

        public void unExecute()
        {
            throw new NotImplementedException();
        }
    }
}
