using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor;
using Transition.CircuitEditor.Serializable;

namespace Transition.Commands
{
    class CommandAddComponent : ICircuitCommand
    {
        public SerializableComponent Component { get; set; }

        public string Title => "Add Component Command: " + Component.ToString();
        
        public void execute()
        {
            CircuitEditor.CircuitEditor.currentInstance.currentDesign.addComponent(Component);
        }

        public void unExecute()
        {
            CircuitEditor.CircuitEditor.currentInstance.currentDesign.removeComponent(Component);
        }
    }
}
