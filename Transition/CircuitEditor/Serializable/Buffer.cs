using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.ParametersControls;

namespace Transition.CircuitEditor.Serializable
{
    public class Buffer : SerializableComponent
    {
        public override string ElementLetter => "B";
        public override string ElementType => "Buffer";

        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        private EngrNumber rIn;
        public EngrNumber RIn
        {
            get => rIn;
            set { SetProperty(ref rIn, value); }
        }

        private EngrNumber rOut;
        public EngrNumber ROut
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

        private EngrNumber gain; /* times */
        public EngrNumber Gain
        {
            get => gain;
            set
            {
                SetProperty(ref gain, value);
                raiseLayoutChanged();
            }
        }

        private EngrNumber delay; /* seconds */
        public EngrNumber Delay
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
            rIn = "1T";
            rOut = "1u";

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
                case "RIn": RIn = (EngrNumber)value; break;
                case "ROut": ROut = (EngrNumber)value; break;

                case "InverterInput": InverterInput = (bool)value; break;
                case "Gain": Gain = (EngrNumber)value; break;
                case "Delay": Delay = (EngrNumber)value; break;

            }
        }


    }
}
