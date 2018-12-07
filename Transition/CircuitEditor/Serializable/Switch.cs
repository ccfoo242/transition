﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transition.CircuitEditor.Serializable
{
    public class Switch : SerializableComponent
    {
        public override string ElementLetter => "S";
        public override string ElementType => "Switch";


        private byte quantityOfTerminals;
        public override byte QuantityOfTerminals
        {
            get { return quantityOfTerminals; }
            set
            {
                byte oldValue = quantityOfTerminals;
                SetProperty(ref quantityOfTerminals, value);
                TerminalsChanged?.Invoke(oldValue, value);

                if (value < oldValue)
                    for (byte x = oldValue; x > value; x--)
                        raiseTerminalDeleted((byte)(x - 1));

                raiseLayoutChanged();
            }
        }

        public delegate void DelegateTerminalsChanged(byte oldQuantity, byte newQuantity);
        public event DelegateTerminalsChanged TerminalsChanged;

        private byte currentPosition;
        public byte CurrentPosition { get => currentPosition;
            set { SetProperty(ref currentPosition, value); }
        }

        private EngrNumber rClosed;
        public EngrNumber RClosed { get => rClosed; set {
                SetProperty(ref rClosed, value); } }

        private EngrNumber cOpen;
        public EngrNumber COpen { get => cOpen; set {
                SetProperty(ref cOpen, value); } }

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "CurrentPosition": CurrentPosition = (byte)value; break;
                case "QuantityOfTerminals": QuantityOfTerminals = (byte)value; break;
                case "RClosed": RClosed = (EngrNumber)value; break;
                case "COpen": COpen = (EngrNumber)value; break;
            }
        }
    }
}
