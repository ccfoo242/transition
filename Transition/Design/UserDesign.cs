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
    }
}
