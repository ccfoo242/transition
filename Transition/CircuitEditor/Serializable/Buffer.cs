using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor.ParametersControls;
using Easycoustics.Transition.Common;

namespace Easycoustics.Transition.CircuitEditor.Serializable
{
    public class Buffer : SerializableComponent, IPassive, IIsolateSection, IImplicitGroundedComponent
    {
        public override string ElementLetter => "B";
        public override string ElementType => "Buffer";

        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        public ComplexDecimal GetImpedance(decimal frequency) => RIn;
        public Tuple<byte, byte> GetImpedanceTerminals => new Tuple<byte, byte>(0, 255);

       

        private decimal rIn;
        public decimal RIn
        {
            get => rIn;
            set { SetProperty(ref rIn, value); }
        }

        private decimal rOut;
        public decimal ROut
        {
            get => rOut;
            set { SetProperty(ref rOut, value); }
        }

        private bool inverterInput;
        public bool InverterInput
        {
            get => inverterInput;
            set
            {
                SetProperty(ref inverterInput, value);
                raiseLayoutChanged();
            }
        }

        private decimal gain; /* times */
        public decimal Gain
        {
            get => gain;
            set
            {
                SetProperty(ref gain, value);
                raiseLayoutChanged();
            }
        }

        private decimal delay; /* seconds */
        public decimal Delay
        {
            get => delay;
            set
            {
                SetProperty(ref delay, value);
                raiseLayoutChanged();
            }
        }
        

        public Buffer() : base()
        {
            rIn = 1e12m;
            rOut = 12-6m;

            inverterInput = false;
            delay = 0;
            gain = 1;

            ParametersControl = new BufferParametersControl(this);
            OnScreenElement = new OnScreenComponents.BufferScreen(this);
        }

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "RIn": RIn = (decimal)value; break;
                case "ROut": ROut = (decimal)value; break;

                case "InverterInput": InverterInput = (bool)value; break;
                case "Gain": Gain = (decimal)value; break;
                case "Delay": Delay = (decimal)value; break;

            }
        }

        public byte[] getOtherTerminalsIsolated(byte terminal)
        {
            return new byte[] { };
        }
    }
}
