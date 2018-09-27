using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.CircuitEditor.Serializable;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Transition.Design
{
    public class UserDesign
    {
        /* the UserDesign class stores all circuit information, components, wires
         * and the bounds between them. Also it handles the Canvas where the Design is
         drawn, and has the responsability for drawing the circuit on this canvas.
         This allows the UserDesign class to show a circuit on screen. This is useful
         for showing circuit examples on different UI dialogs (and not only the 
         circuit is being edited by the user in CircuitEditor)
         On the other hand the CircuitEditor has all operations for manipulating
         the design, to add components, wires, move and link them.
         That means there is a strong coupling between UserDesign and CircuitEditor*/

        public ObservableCollection<SerializableComponent> components { get; }
        public ObservableCollection<Wire> wires { get; }
        
        public delegate void ElementDelegate(UserDesign sender, SerializableElement element);
        public event ElementDelegate ElementAdded;
        public event ElementDelegate ElementRemoved;

        public Canvas canvasCircuit { get; set; }

        public bool SnapToGrid { get; set; } = true;

        public UserDesign()
        {
            canvasCircuit = new Canvas()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 1080,
                Height = 720,
                AllowDrop = true
            };
            
            components = new ObservableCollection<SerializableComponent>();
            wires = new ObservableCollection<Wire>();

            components.CollectionChanged += Components_CollectionChanged;
            wires.CollectionChanged += Wires_CollectionChanged;
            
        }
        
        private void Components_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (SerializableComponent comp in components)
                if (!isComponentOnCanvas(comp))
                    canvasCircuit.Children.Add(comp.OnScreenComponent);
        }

        private void Wires_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (Wire wire in wires)
                if (!isWireOnCanvas(wire))
                {
                    canvasCircuit.Children.Add(wire.OnScreenWire);
                    canvasCircuit.Children.Add(wire.OnScreenWire.wt0);
                    canvasCircuit.Children.Add(wire.OnScreenWire.wt1);
                }
        }

        public bool isComponentOnCanvas(SerializableComponent comp)
        {
            return canvasCircuit.Children.Contains(comp.OnScreenComponent);
        }

        public bool isWireOnCanvas(Wire wire)
        {
            return canvasCircuit.Children.Contains(wire.OnScreenWire);
        }

        public void addComponent(SerializableComponent component)
        {
            components.Add(component);
            ElementAdded?.Invoke(this, component);
        }

        public void removeElement(SerializableElement element)
        {
            if (element is SerializableComponent) removeComponent((SerializableComponent)element);
            if (element is Wire) removeWire((Wire)element);
        }

        public void removeComponent(SerializableComponent component)
        {
            if (components.Contains(component))
            {
                component.deletedElement();
                components.Remove(component);
                canvasCircuit.Children.Remove(component.OnScreenComponent);
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
                wire.deletedElement();
                wires.Remove(wire);
                canvasCircuit.Children.Remove(wire.OnScreenWire);
                canvasCircuit.Children.Remove(wire.OnScreenWire.wt0);
                canvasCircuit.Children.Remove(wire.OnScreenWire.wt1);

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
