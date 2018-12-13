using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.ParametersControls;

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

        private byte state;
        public byte State { get => state;
            set { SetProperty(ref state, value);
                raiseLayoutChanged();
            }
        }

        private EngrNumber rClosed;
        public EngrNumber RClosed { get => rClosed; set {
                SetProperty(ref rClosed, value); } }

        private EngrNumber cOpen;
        public EngrNumber COpen { get => cOpen; set {
                SetProperty(ref cOpen, value); } }

        public Switch() : base()
        {
            RClosed = "10m";
            COpen = "1p";
            QuantityOfTerminals = 3;
            State = 1;

            ParametersControl = new SwitchParametersControl(this);
            OnScreenElement = new OnScreenComponents.SwitchScreen(this);
        }
        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "State": State = (byte)value; break;
                case "QuantityOfTerminals": QuantityOfTerminals = (byte)value; break;
                case "RClosed": RClosed = (EngrNumber)value; break;
                case "COpen": COpen = (EngrNumber)value; break;
            }
        }
    }
}
