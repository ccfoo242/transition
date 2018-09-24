using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.OnScreenComponents;

namespace Transition.CircuitEditor.Serializable
{
    public class Wire : SerializableElement
    {
        private double x1;
        public double X1 {
            get { return x1; }
            set { SetProperty(ref x1, value); } }

        private double y1;
        public double Y1 {
            get { return y1; }
            set { SetProperty(ref y1, value); } }

        private double x2;
        public double X2 {
            get { return x2; }
            set { SetProperty(ref x2, value); } }

        private double y2;
        public double Y2 {
            get { return y2; }
            set { SetProperty(ref y2, value); } }

        public override string ElementLetter => "W";

        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        private SerializableElement boundedObject1;
        public SerializableElement BoundedObject1 {
            get { return boundedObject1; }
            set { SetProperty(ref boundedObject1, value); } }

        private byte boundedTerminal1;
        public byte BoundedTerminal1 {
            get { return boundedTerminal1; }
            set { SetProperty(ref boundedTerminal1, value); } }


        private SerializableElement boundedObject2;
        public SerializableElement BoundedObject2
        {
            get { return boundedObject2; }
            set { SetProperty(ref boundedObject2, value); }
        }

        private byte boundedTerminal2;
        public byte BoundedTerminal2
        {
            get { return boundedTerminal2; }
            set { SetProperty(ref boundedTerminal2, value); }
        }
        
        public bool IsBounded1 => BoundedObject1 != null;
        public bool IsBounded2 => BoundedObject2 != null;

        public bool IsWireBounded1 => BoundedObject1 is Wire;
        public bool IsWireBounded2 => BoundedObject2 is Wire;

        public WireScreen OnScreenWire { get; }

        public event ComponentLayoutChanged ComponentChanged;
        public delegate void ComponentLayoutChanged();

        public Wire()
        {
            OnScreenWire = new WireScreen(this);

        }


        public void bind(SerializableElement element, byte elementTerminal, byte thisWireTerminal)
        {
            if (element is SerializableComponent)
                bind((SerializableComponent)element, elementTerminal, thisWireTerminal);
            if (element is Wire)
                bind((Wire)element, elementTerminal, thisWireTerminal);
            
        }

        public void bind(SerializableComponent component, byte componentTerminal, byte thisWireTerminal)
        {
            if (thisWireTerminal == 1) bind1(component, componentTerminal);
            if (thisWireTerminal == 2) bind2(component, componentTerminal);
        }

        public void bind(Wire wire, byte otherWireTerminal, byte thisWireTerminal)
        {
            if (thisWireTerminal == 1) bind1(wire, otherWireTerminal);
            if (thisWireTerminal == 2) bind2(wire, otherWireTerminal);
        }

        public void bind1(SerializableComponent component, byte componentTerminal)
        {
            BoundedObject1 = component;
            component.ComponentLayoutChanged += RaiseLayoutChanged;
            BoundedTerminal1 = componentTerminal;
        }
        
        public void bind2(SerializableComponent component, byte componentTerminal)
        {
            BoundedObject2 = component;
            component.ComponentLayoutChanged += RaiseLayoutChanged;
            BoundedTerminal2 = componentTerminal;
        }


        public void bind1(Wire wire, byte otherWireTerminal)
        {
            BoundedObject1 = wire;
            BoundedTerminal1 = otherWireTerminal;

            if (otherWireTerminal == 1)
            {
                wire.BoundedObject1 = this;
                wire.BoundedTerminal1 = 1;
            }
            else
            {
                wire.BoundedObject2 = this;
                wire.BoundedTerminal2 = 1;
            }
        }


        public void bind2(Wire wire, byte otherWireTerminal)
        {
            BoundedObject2 = wire;
            BoundedTerminal2 = otherWireTerminal;
            
            if (otherWireTerminal == 1)
            {
                wire.BoundedObject1 = this;
                wire.BoundedTerminal1 = 2;
            }
            else
            {
                wire.BoundedObject2 = this;
                wire.BoundedTerminal2 = 2;
            }
        }

        public void RaiseLayoutChanged()
        {
            ComponentChanged?.Invoke();
        }
    }
}
