using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.Common;

namespace Transition.CircuitEditor.Serializable
{
    public class SerializableWire : SerializableElement
    {
        private Point2D positionTerminal0;
        public Point2D PositionTerminal0
        {
            get { return positionTerminal0; }
            set { SetProperty(ref positionTerminal0, value);
                  raiseLayoutChanged();
            }
        }

        private Point2D positionTerminal1;
        public Point2D PositionTerminal1
        {
            get { return positionTerminal1; }
            set { SetProperty(ref positionTerminal1, value);
                  raiseLayoutChanged();
            }
        }
        
        /*
        private double x0;
        public double X0 {
            get { return x0; }
            set { SetProperty(ref x0, value);
                WireLayoutChanged?.Invoke();
            } }

        private double y0;
        public double Y0 {
            get { return y0; }
            set { SetProperty(ref y0, value);
                WireLayoutChanged?.Invoke(); } }

        private double x1;
        public double X1 {
            get { return x1; }
            set { SetProperty(ref x1, value);
                WireLayoutChanged?.Invoke(); } }

        private double y1;
        public double Y1 {
            get { return y1; }
            set { SetProperty(ref y1, value);
                WireLayoutChanged?.Invoke(); } }
                */

        public override string ElementLetter => "W";
        public override string ElementType => "Wire";

        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }
        /*
        private SerializableElement boundedObject0;
        public SerializableElement BoundedObject0 {
                    get { return boundedObject0; }
                    set { SetProperty(ref boundedObject0, value); } }

                private byte boundedTerminal0;
                public byte BoundedTerminal0 {
                    get { return boundedTerminal0; }
                    set { SetProperty(ref boundedTerminal0, value); } }


                private SerializableElement boundedObject1;
                public SerializableElement BoundedObject1
                {
                    get { return boundedObject1; }
                    set { SetProperty(ref boundedObject1, value); }
                }

                private byte boundedTerminal1;
                public byte BoundedTerminal1
                {
                    get { return boundedTerminal1; }
                    set { SetProperty(ref boundedTerminal1, value); }
        }*/

        private Tuple<SerializableElement, byte> bind0;
        public Tuple<SerializableElement, byte> Bind0
        {
            get { return bind0; }
            set { Tuple<SerializableElement, byte> previousValue = bind0;
                  SetProperty(ref bind0, value);
                  ChangeBinding(0, previousValue, value);
                  raiseLayoutChanged();
            }
        }

        private Tuple<SerializableElement, byte> bind1;
        public Tuple<SerializableElement, byte> Bind1
        {
            get { return bind1; }
            set { Tuple<SerializableElement, byte> previousValue = bind1;
                  SetProperty(ref bind1, value);
                  ChangeBinding(1, previousValue, value);
                  raiseLayoutChanged();
            }
        }
        
        public bool IsTerminal0Bounded => Bind0 != null;
        public bool IsTerminal1Bounded => Bind1 != null;

        public bool IsWireBounded0 => Bind0.Item1 is SerializableWire;
        public bool IsWireBounded1 => Bind1.Item1 is SerializableWire;
        
    
        public delegate void WireBindDelegate(SerializableWire wire, byte terminal, Tuple<SerializableElement, byte> previousValue,
                                                 Tuple<SerializableElement, byte> newValue);
        public event WireBindDelegate WireBindingChanged;

        public WireScreen OnScreenWire { get; }

       /* public event ComponentLayoutChanged ComponentChanged; */
        public delegate void ComponentLayoutChanged();

        public SerializableWire()
        {
            OnScreenWire = new WireScreen(this);
        }

        public Tuple<SerializableElement,byte> bnd(byte terminal)
        {
            return (terminal == 0) ? bind0 : bind1;
        }

        public void ChangeBinding(byte terminal, Tuple<SerializableElement, byte> previousValue,
                                                 Tuple<SerializableElement, byte> newValue)
        {
            if (previousValue != null)
            {
                previousValue.Item1.LayoutChanged -= BindedElementLayoutChanged;
                previousValue.Item1.ElementDeleted -= BindedElementDeleted;
            }

            if (newValue != null)
            {
                newValue.Item1.LayoutChanged += BindedElementLayoutChanged;
                newValue.Item1.ElementDeleted += BindedElementDeleted;
            }

            WireBindingChanged?.Invoke(this, terminal, previousValue, newValue);
        }

        public void BindedElementLayoutChanged(SerializableElement el)
        {
            raiseLayoutChanged();
        }


        /*
        public void bind(SerializableElement element, byte elementTerminal, byte thisWireTerminal)
        {
            if (element is SerializableComponent)
                bind((SerializableComponent)element, elementTerminal, thisWireTerminal);
            if (element is SerializableWire)
                bind((SerializableWire)element, elementTerminal, thisWireTerminal);
        }

        public void bind(SerializableComponent component, byte componentTerminal, byte thisWireTerminal)
        {
            if (thisWireTerminal == 0) bind0(component, componentTerminal);
            if (thisWireTerminal == 1) bind1(component, componentTerminal);
        }

        public void unBind(byte thisWireTerminal)
        {

            if (thisWireTerminal == 0)
            {
                if (!IsBounded0) return;
                BoundedObject0 = null;
                BoundedTerminal0 = 0;
            }
            else
            {
                if (!IsBounded1) return;
                BoundedObject1 = null;
                BoundedTerminal1 = 0;
            }
        }

        public void bind(SerializableWire otherWire, byte otherWireTerminal, byte thisWireTerminal)
        {
            //we check to not bound to a wire, that is already bounded to this one

            if (otherWire == this) return;

            if (otherWire.IsWireBounded0 && otherWireTerminal == 0)
                if (otherWire.BoundedObject0 == this)
                    return;

            if (otherWire.IsWireBounded1 && otherWireTerminal == 1)
                if (otherWire.BoundedObject1 == this)
                    return;

            if (thisWireTerminal == 0) bind0(otherWire, otherWireTerminal);
            if (thisWireTerminal == 1) bind1(otherWire, otherWireTerminal);
        }

        public void bind0(SerializableComponent component, byte componentTerminal)
        {
            BoundedObject0 = component;
            component.ComponentLayoutChanged += RaiseLayoutChanged;
            component.ComponentPositionChanged += RaiseLayoutChanged;
            component.ElementDeleted += deleted0;
            component.UnBindElement += deleted0;
            BoundedTerminal0 = componentTerminal;
            WireBeingBinded?.Invoke(0);
        }
        
        public void bind1(SerializableComponent component, byte componentTerminal)
        {
            BoundedObject1 = component;
            component.ComponentLayoutChanged += RaiseLayoutChanged;
            component.ComponentPositionChanged += RaiseLayoutChanged;
            component.ElementDeleted += deleted1;
            component.UnBindElement += deleted1;
            BoundedTerminal1 = componentTerminal;
            WireBeingBinded?.Invoke(1);
        }
        
        public void bind0(SerializableWire otherWire, byte otherWireTerminal)
        {
            BoundedObject0 = otherWire;
            BoundedTerminal0 = otherWireTerminal;

            otherWire.ElementDeleted += deleted0;
            otherWire.WireLayoutChanged += RaiseLayoutChanged;
            WireBeingBinded?.Invoke(0);
        }
        
        public void bind1(SerializableWire otherWire, byte otherWireTerminal)
        {
            BoundedObject1 = otherWire;
            BoundedTerminal1 = otherWireTerminal;

            otherWire.ElementDeleted += deleted1;
            otherWire.WireLayoutChanged += RaiseLayoutChanged;
            WireBeingBinded?.Invoke(1);
        }
        */

        public void updatePosition(Point2D newPosition, byte terminal)
        {
            if (terminal == 0)
            { PositionTerminal0 = newPosition; }
            else
            { PositionTerminal1 = newPosition; }
        }

        public void BindedElementDeleted(SerializableElement el)
        {
            if (Bind0.Item1 == el) Bind0 = null;
            if (Bind1.Item1 == el) Bind1 = null;
        }

        

        public override string ToString()
        {
            return "Wire " + ElementName;
        }

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);
        }

       
    }
}
