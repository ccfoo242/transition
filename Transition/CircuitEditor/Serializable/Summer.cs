using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.ParametersControls;

namespace Transition.CircuitEditor.Serializable
{
    public class Summer : SerializableComponent
    {
        public override string ElementLetter =>"E";
        public override string ElementType => "Summer";

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

        private bool inAInverterInput;
        public bool InAInverterInput
        {
            get => inAInverterInput;
            set { SetProperty(ref inAInverterInput, value);
                raiseLayoutChanged();
            }
        }

        private bool inBInverterInput;
        public bool InBInverterInput
        {
            get => inBInverterInput;
            set
            {
                SetProperty(ref inBInverterInput, value);
                raiseLayoutChanged();
            }
        }

        private bool inCInverterInput;
        public bool InCInverterInput
        {
            get => inCInverterInput;
            set
            {
                SetProperty(ref inCInverterInput, value);
                raiseLayoutChanged();
            }
        }

        public delegate void DelegateTerminalsChanged(byte oldQuantity, byte newQuantity);
        public event DelegateTerminalsChanged TerminalsChanged;
        
        public Summer() : base()
        {
            rIn = "1T";
            rOut = "1u";
            QuantityOfTerminals = 3;

            ParametersControl = new SummerParametersControl(this);
            OnScreenElement = new OnScreenComponents.SummerScreen(this);
        }

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "RIn": RIn = (EngrNumber)value; break;
                case "ROut": ROut = (EngrNumber)value; break;
                    
                case "InAInverterInput": InAInverterInput = (bool)value; break;
                case "InBInverterInput": InBInverterInput = (bool)value; break;
                case "InCInverterInput": InCInverterInput = (bool)value; break;

            }
        }

    }
}

