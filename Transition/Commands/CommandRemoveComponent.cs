using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor;
using Easycoustics.Transition.CircuitEditor.Serializable;
using Easycoustics.Transition.Design;

namespace Easycoustics.Transition.Commands
{
    class CommandRemoveComponent : ICircuitCommand
    {
        public string Title => "Remove Component: " + Component.ToString();
       
        public SerializableComponent Component { get; }
        public List<SerializableWire> BindedWires { get; } = new List<SerializableWire>();

        public void execute()
        {
            CircuitEditor.CircuitEditor.currentInstance.CurrentDesign.Components.Remove(Component);
        }
        
        public void unExecute()
        {
            CircuitEditor.CircuitEditor.currentInstance.CurrentDesign.Components.Add(Component);

            foreach (SerializableWire wire in BindedWires)
                CircuitEditor.CircuitEditor.currentInstance.CurrentDesign.Wires.Add(wire);

           /* for (byte i = 0; i < Component.QuantityOfTerminals; i++)
                foreach (Tuple<SerializableWire,byte> wt in Bindings[i])
                    wt.Wire.bind(Component, i, wt.Terminal);
            */
        }

        public CommandRemoveComponent(SerializableComponent comp)
        {
            Component = comp;

            for (byte i = 0; i < comp.QuantityOfTerminals; i++)
                BindedWires.AddRange(CircuitEditor.CircuitEditor.currentInstance.CurrentDesign.getBoundedWires(comp, i));
                 
        }

        public override string ToString()
        {
            string output = Title;
            output += Environment.NewLine;

            foreach (SerializableWire wire in BindedWires)
                output += "Wire " + wire.ToString() + Environment.NewLine;
            
            return output;
        }

    }
}
