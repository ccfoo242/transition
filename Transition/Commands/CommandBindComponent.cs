using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor;
using Transition.CircuitEditor.Serializable;

namespace Transition.Commands
{
    public class CommandBindComponent : ICircuitCommand
    {
        public string Title => "Command Bind Component";

        public SerializableComponent Component { get; set; }
        private List<Wire> wiresForComponents;
        private List<Tuple<SerializableElement, byte, byte>> binds;
        public void execute()
        {
            foreach (Wire wire in wiresForComponents)
                CircuitEditor.CircuitEditor.currentInstance.currentDesign.addWire(wire);

            foreach (Tuple<SerializableElement, byte, byte> tuple in binds)
                if (tuple.Item1 is Wire)
                    ((Wire)tuple.Item1).bind(Component, tuple.Item3, tuple.Item2);
                
        }

        public void unExecute()
        {
            foreach (Wire wire in wiresForComponents)
                CircuitEditor.CircuitEditor.currentInstance.currentDesign.removeWire(wire);

            foreach (Tuple<SerializableElement, byte, byte> tuple in binds)
                if (tuple.Item1 is Wire)
                { ((Wire)tuple.Item1).unBind(tuple.Item2);
                  ((Wire)tuple.Item1).OnScreenWire.getWireTerminal(tuple.Item2).updateOriginalPosition();
                }

        }

        public CommandBindComponent(List<Tuple<SerializableElement,byte,byte>> binds, SerializableComponent comp)
        {
            Component = comp;
            this.binds = binds;

            wiresForComponents = new List<Wire>();
            Wire wire;

            foreach (Tuple<SerializableElement,byte,byte> tuple in binds)
                if (tuple.Item1 is SerializableComponent)
                {
                    wire = CircuitEditor.CircuitEditor.currentInstance.currentDesign.bindTwoComponentsTerminals(Component, tuple.Item3, (SerializableComponent)tuple.Item1, tuple.Item2);
                    if (wire!=null) wiresForComponents.Add(wire);
                }
        }
    }
}
