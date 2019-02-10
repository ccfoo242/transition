﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.ParametersControls;
using Transition.Functions;

namespace Transition.CircuitEditor.Serializable
{
    public class Transducer : SerializableComponent
    {
        public override string ElementLetter => "K";
        public override string ElementType => "Transducer";

        private string description;
        public string Description
        {
            get => description;
            set
            {
                SetProperty(ref description, value);
                raiseLayoutChanged();
            }
        }

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

        private Function functionSPL;
        public Function FunctionSPL
        {
            get => functionSPL;
            set { SetProperty(ref functionSPL, value); }
        }

        private Function functionImpedance;
        public Function FunctionImpedance
        {
            get => functionImpedance;
            set { SetProperty(ref functionImpedance, value); }
        }

        private decimal inputVoltage;
        public decimal InputVoltage
        {
            get => inputVoltage;
            set { SetProperty(ref inputVoltage, value); }
        }

        private decimal micDistance;
        public decimal MicDistance
        {
            get => micDistance;
            set { SetProperty(ref micDistance, value); }
        }

        private decimal refDistanceX;
        public decimal RefDistanceX
        {
            get => refDistanceX;
            set { SetProperty(ref refDistanceX, value); }
        }

        private decimal refDistanceY;
        public decimal RefDistanceY
        {
            get => refDistanceY;
            set { SetProperty(ref refDistanceY, value); }
        }

        private decimal refDistanceZ;
        public decimal RefDistanceZ
        {
            get => refDistanceZ;
            set { SetProperty(ref refDistanceZ, value); }
        }

        private bool polarityReverse;
        public bool PolarityReverse
        {
            get => polarityReverse;
            set { SetProperty(ref polarityReverse, value);
                TransducerReversed?.Invoke();
            }
        }

        public delegate void DelegateTerminalsChanged(byte oldQuantity, byte newQuantity);
        public event DelegateTerminalsChanged TerminalsChanged;

        public event Action TransducerReversed;

        public Transducer() : base()
        {
            inputVoltage = 2.83m;
            micDistance = 1m;

            quantityOfTerminals = 2;
            description = "Spkr";

            refDistanceX = 0m;
            refDistanceY = 0m;
            refDistanceZ = 0m;

            polarityReverse = false;
            
            ParametersControl = new TransducerParametersControl(this);
            OnScreenElement = new OnScreenComponents.TransducerScreen(this);
        }


        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "Description": Description = (string)value; break;
                case "InputVoltage": InputVoltage = (decimal)value; break;
                case "MicDistance": MicDistance = (decimal)value; break;

                case "PolarityReverse": PolarityReverse = (bool)value; break;

                case "RefDistanceX": RefDistanceX = (decimal)value; break;
                case "RefDistanceY": RefDistanceY = (decimal)value; break;
                case "RefDistanceZ": RefDistanceZ = (decimal)value; break;

            }
        }
    }
}
