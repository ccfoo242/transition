using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.CircuitEditor.Serializable;

namespace Transition.Commands
{
    public class CommandBindComponent : ICircuitCommand
    {
        public string Title => "Command Bind Component";

        public SerializableComponent Component { get; set; }

        private List<SerializableWire> wiresForComponents = new List<SerializableWire>();
        private Dictionary<byte, ElementTerminal> binds;

        public void execute()
        {
            foreach (SerializableWire wire in wiresForComponents)
                CircuitEditor.CircuitEditor.currentInstance.currentDesign.Wires.Add(wire);
          
        }

        public void unExecute()
        {
            foreach (SerializableWire wire in wiresForComponents)
                CircuitEditor.CircuitEditor.currentInstance.currentDesign.Wires.Remove(wire);
          
        }

        public CommandBindComponent(Dictionary<byte, ElementTerminal> binds, SerializableComponent comp)
        {
            Component = comp;
            this.binds = binds;
            
            SerializableWire wire;

            foreach (KeyValuePair<byte, ElementTerminal> bind in binds)
            { 
                wire = CircuitEditor.CircuitEditor.currentInstance.currentDesign.bindComponentTerminal(comp, bind.Key, bind.Value.ScreenElement.Serializable, bind.Value.TerminalNumber);
                if (wire != null) wiresForComponents.Add(wire);
            }
        }
    }
}
