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

        public Point2D Position(byte terminal) => (terminal == 0) ? PositionTerminal0 : PositionTerminal1;
        
        public override string ElementLetter => "W";
        public override string ElementType => "Wire";

        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }
       

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

        public void doBind(byte thisWireTerminal, SerializableElement element, byte elementTerminal)
        {
            if (thisWireTerminal == 0)
                Bind0 = new Tuple<SerializableElement, byte>(element, elementTerminal);
            else
                Bind1 = new Tuple<SerializableElement, byte>(element, elementTerminal);
        }

        public void unBind(byte thisWireTerminal)
        {
            if (thisWireTerminal == 0)
                Bind0 = null;
            else
                Bind1 = null;
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
