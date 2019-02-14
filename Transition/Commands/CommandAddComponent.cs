using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor;
using Easycoustics.Transition.CircuitEditor.Serializable;

namespace Easycoustics.Transition.Commands
{
    class CommandAddComponent : ICircuitCommand
    {
        public SerializableComponent Component { get; set; }

        public string Title => "Add Component Command: " + Component.ToString();
        public override string ToString() => Title;

        public void execute()
        {
            CircuitEditor.CircuitEditor.currentInstance.CurrentDesign.Components.Add(Component);
        }

        public void unExecute()
        {
            CircuitEditor.CircuitEditor.currentInstance.CurrentDesign.Components.Remove(Component);
        }

        
    }
}
