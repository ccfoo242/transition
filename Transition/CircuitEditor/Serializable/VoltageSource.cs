using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.Functions;

namespace Transition.CircuitEditor.Serializable
{
    public class VoltageSource : SerializableComponent
    {
        public override string ElementLetter => "V";
        public override string ElementType => "Voltage Source";

        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        private int outputVoltageFunctionType; // 0=constant 1=curve
        public int OutputVoltageFunctionType
        {
            get { return outputVoltageFunctionType; }
            set { SetProperty(ref outputVoltageFunctionType, value);
                OnPropertyChanged("VoltageString");
            }
        }

        private int outputImpedanceFunctionType; // 0=constant 1=curve
        public int OutputImpedanceFunctionType
        {
            get { return outputImpedanceFunctionType; }
            set { SetProperty(ref outputImpedanceFunctionType, value); }
        }

        private EngrNumber constantOutputVoltage;
        public EngrNumber ConstantOutputVoltage
        {
            get { return constantOutputVoltage; }
            set { SetProperty(ref constantOutputVoltage, value);
                OnPropertyChanged("VoltageString");
            }
        }

        private Function functionOutputVoltage;
        public Function FunctionOutputVoltage
        {
            get { return functionOutputVoltage; }
            set { SetProperty(ref functionOutputVoltage, value); }
        }

        private EngrNumber constantOutputImpedance;
        public EngrNumber ConstantOutputImpedance
        {
            get { return constantOutputImpedance; }
            set { SetProperty(ref constantOutputImpedance, value); }
        }

        private Function functionOutputImpedance;
        public Function FunctionOutputImpedance
        {
            get { return functionOutputImpedance; }
            set { SetProperty(ref functionOutputImpedance, value); }
        }

        public VoltageSource() : base()
        {
            ConstantOutputVoltage = 1;
            ConstantOutputImpedance = new EngrNumber(1M, "u");

            OnScreenElement = new VoltageSourceScreen(this);
            ParametersControl = new VoltageSourceComponentParameters(this);
            
        }

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "OutputVoltageFunctionType": OutputVoltageFunctionType = (int)value; break;
                case "OutputImpedanceFunctionType": OutputImpedanceFunctionType = (int)value; break;
                case "ConstantOutputVoltage": ConstantOutputVoltage = (EngrNumber)value; break;
                case "FunctionOutputVoltage": FunctionOutputVoltage = (Function)value; break;
                case "ConstantOutputImpedance": ConstantOutputImpedance= (EngrNumber)value; break;
                case "FunctionOutputImpedance": FunctionOutputImpedance = (Function)value; break;

            }
        }

        public string VoltageString
        {
            get
            {
                if (OutputVoltageFunctionType == 0)
                    return ConstantOutputVoltage.ToString() + " V";
                else
                    return "Curve";
            }
        }
    }
}
