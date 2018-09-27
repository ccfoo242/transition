using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.CircuitEditor.Serializable;
using Windows.UI.Xaml;

namespace Transition.Design
{
    public class UserDesign
    {
        public ObservableCollection<SerializableComponent> components { get; }
        public ObservableCollection<Wire> wires { get; }
        
        public delegate void ElementDelegate(UserDesign sender, SerializableElement element);
        public event ElementDelegate ElementAdded;
        public event ElementDelegate ElementRemoved;

        public bool SnapToGrid { get; set; } = true;

        public UserDesign()
        {
            components = new ObservableCollection<SerializableComponent>();
            wires = new ObservableCollection<Wire>();
        }

        public void addComponent(SerializableComponent component)
        {
            components.Add(component);
            ElementAdded?.Invoke(this, component);
        }

        public void removeElement(SerializableComponent component)
        {
            if (components.Contains(component))
            {
                components.Remove(component);
                ElementRemoved?.Invoke(this, component);
            }
        }

        public void addWire(Wire wire)
        {
            wires.Add(wire);
            ElementAdded?.Invoke(this, wire);
        }

        public void removeWire(Wire wire)
        {
            if (wires.Contains(wire))
            {
                wires.Remove(wire);
                ElementRemoved?.Invoke(this, wire);
            }
        }

        public Wire bindTwoComponentsTerminals(SerializableComponent comp1, byte terminal1, SerializableComponent comp2, byte terminal2)
        {
            if (areTwoComponentsTerminalsBounded(comp1, terminal1, comp2, terminal2))
                return null;

            if ((comp1 == comp2) && (terminal1 == terminal2)) return null;

            Wire wire = new Wire();
            
            wire.bind0(comp1, terminal1);
            wire.bind1(comp2, terminal2);
            wire.ElementName = "W" + (getMaximumNumberWire() + 1).ToString();

            addWire(wire);

            return wire;
        }

        public bool areTwoComponentsTerminalsBounded(SerializableComponent comp1, byte terminal1, SerializableComponent comp2, byte terminal2)
        {
            foreach (Wire wire in wires)
            {
                if ((wire.BoundedObject0 == comp1) && (wire.BoundedTerminal0 == terminal1)
                   && (wire.BoundedObject1 == comp2) && (wire.BoundedTerminal1 == terminal2) ||
                      (wire.BoundedObject0 == comp2) && (wire.BoundedTerminal0 == terminal2)
                   && (wire.BoundedObject1 == comp1) && (wire.BoundedTerminal1 == terminal1))
                { return true; }
            }
            return false;
        }


        public int getMaximumNumberElement(string ElementLetter)
        {
            if (ElementLetter == null) return 0;
            if (ElementLetter == "") return 0;

            int maximum = 0;
            int result;

            foreach (SerializableElement element in components)
                if (element.ElementName != null)
                    if (element.ElementName.Substring(0, ElementLetter.Length) == ElementLetter)
                        if (int.TryParse(element.ElementName.Substring(ElementLetter.Length, element.ElementName.Length - ElementLetter.Length), out result))
                            if (result > maximum) maximum = result;

            return maximum;
        }

        public int getMaximumNumberWire()
        {
            int result;
            int maximum = 0;

            foreach (Wire wire in wires)
                if (wire.ElementName != null)
                    if (wire.ElementName.Substring(0, 1) == "W")
                        if (int.TryParse(wire.ElementName.Substring(1, wire.ElementName.Length - 1), out result))
                            if (result > maximum) maximum = result;

            return maximum;
        }

        public int getNextNumberLetter(string ElementLetter)
        {
            return getMaximumNumberElement(ElementLetter) + 1;
        }
    }
}
