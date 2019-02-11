using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.ParametersControls;
using Transition.Common;

namespace Transition.CircuitEditor.Serializable
{
    public class Switch : SerializableComponent, IPassive
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

        private decimal rClosed;
        public decimal RClosed { get => rClosed; set {
                SetProperty(ref rClosed, value); } }

        private decimal cOpen;
        public decimal COpen { get => cOpen; set {
                SetProperty(ref cOpen, value); } }

        public Switch() : base()
        {
            RClosed = 10e-3m;
            COpen = 1e-12m;
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
                case "RClosed": RClosed = (decimal)value; break;
                case "COpen": COpen = (decimal)value; break;
            }
        }

        public List<Tuple<byte, byte, ComplexDecimal>> getImpedance(decimal frequency)
        {
            var output = new List<Tuple<byte, byte, ComplexDecimal>>();

            var w = 2m * DecimalMath.Pi * frequency;
            var ZCOpen = 1 / (ComplexDecimal.ImaginaryOne * w * COpen);

            for (byte x = 1; x <= (QuantityOfTerminals - 1); x++)
            {
                if (x == State)
                    output.Add(new Tuple<byte, byte, ComplexDecimal>(0, x, RClosed));
                else
                    output.Add(new Tuple<byte, byte, ComplexDecimal>(0, x, ZCOpen));
            }
            return output;
        }
    }
}
