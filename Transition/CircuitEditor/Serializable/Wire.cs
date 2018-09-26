﻿using System;
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

        public override string ElementLetter => "W";

        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

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
        }
        
        public bool IsBounded0 => BoundedObject0 != null;
        public bool IsBounded1 => BoundedObject1 != null;

        public bool IsWireBounded0 => BoundedObject0 is Wire;
        public bool IsWireBounded1 => BoundedObject1 is Wire;

        public delegate void WireLayoutChangedHandler();
        public event WireLayoutChangedHandler WireLayoutChanged;

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
            if (thisWireTerminal == 0) bind0(component, componentTerminal);
            if (thisWireTerminal == 1) bind1(component, componentTerminal);
        }

        public void bind(Wire otherWire, byte otherWireTerminal, byte thisWireTerminal)
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
            BoundedTerminal0 = componentTerminal;
        }
        
        public void bind1(SerializableComponent component, byte componentTerminal)
        {
            BoundedObject1 = component;
            component.ComponentLayoutChanged += RaiseLayoutChanged;
            component.ComponentPositionChanged += RaiseLayoutChanged;
            component.ElementDeleted += deleted1;
            BoundedTerminal1 = componentTerminal;
        }
        
        public void bind0(Wire otherWire, byte otherWireTerminal)
        {
            BoundedObject0 = otherWire;
            BoundedTerminal0 = otherWireTerminal;

            otherWire.ElementDeleted += deleted0;
            otherWire.WireLayoutChanged += RaiseLayoutChanged;
        }
        
        public void bind1(Wire otherWire, byte otherWireTerminal)
        {
            BoundedObject1 = otherWire;
            BoundedTerminal1 = otherWireTerminal;

            otherWire.ElementDeleted += deleted1;
            otherWire.WireLayoutChanged += RaiseLayoutChanged;
        }

        public void RaiseLayoutChanged()
        {
            ComponentChanged?.Invoke();
        }

        public void deleted0()
        {
            BoundedObject0 = null;
        }

        public void deleted1()
        {
            BoundedObject1 = null;
        }

        public override string ToString()
        {
            return "Wire " + ElementName;
        }
    }
}