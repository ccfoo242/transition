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
    class CommandRemoveComponent : ICircuitCommand
    {
        public string Title => "Remove Component: " + Component.ToString();
       

        public SerializableComponent Component { get; }
        public Dictionary<byte, List<SerializableWireTerminal>> Bindings { get; }

        public void execute()
        {
            CircuitEditor.CircuitEditor.currentInstance.currentDesign.removeComponent(Component);
        }
        
        public void unExecute()
        {
            CircuitEditor.CircuitEditor.currentInstance.currentDesign.addComponent(Component);

            for (byte i = 0; i < Component.QuantityOfTerminals; i++)
                foreach (SerializableWireTerminal wt in Bindings[i])
                    wt.Wire.bind(Component, i, wt.Terminal);
            
        }

        public CommandRemoveComponent(SerializableComponent comp)
        {
            Component = comp;
            Bindings = new Dictionary<byte, List<SerializableWireTerminal>>();

            for (byte i = 0; i < comp.QuantityOfTerminals; i++)
                Bindings[i] = CircuitEditor.CircuitEditor.currentInstance.currentDesign.getBoundedWires(comp, i);
            
        }

        public override string ToString()
        {
            string output = Title;
            output += Environment.NewLine;

            foreach (KeyValuePair<byte, List<SerializableWireTerminal>> kvp in Bindings)
            {
                output += "Component terminal " + kvp.Key.ToString() + Environment.NewLine;

                foreach (SerializableWireTerminal wt in kvp.Value)
                    output += "Wire " + wt.Wire.ToString() + " Terminal: " + wt.Terminal.ToString() + Environment.NewLine;                
            }

            return output;
        }

    }
}
