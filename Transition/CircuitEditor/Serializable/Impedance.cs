using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor.Components;
using Easycoustics.Transition.CircuitEditor.OnScreenComponents;
using Easycoustics.Transition.CircuitEditor.ParametersControls;
using Easycoustics.Transition.Common;
using Easycoustics.Transition.Functions;

namespace Easycoustics.Transition.CircuitEditor.Serializable
{
    public class Impedance : SerializableComponent, IPassive, IVoltageCurrentOutput
    {
        public override string ElementLetter => "Z";
        public override string ElementType => "Impedance";

        private bool outputVoltageAcross;
        private bool outputCurrentThrough;

        public bool OutputVoltageAcross
        {
            get => outputVoltageAcross;
            set { SetProperty(ref outputVoltageAcross, value); raiseLayoutChanged(); }
        }

        public bool OutputCurrentThrough
        {
            get => outputCurrentThrough;
            set { SetProperty(ref outputCurrentThrough, value); raiseLayoutChanged(); }
        }

        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        private string description;
        public string Description { get => description;
            set {
                SetProperty(ref description, value);
                raiseLayoutChanged();
            }
        }

        private Function functionImpedance;
        public Function FunctionImpedance { get => functionImpedance;
            set { SetProperty(ref functionImpedance, value); }
        }

        public SampledFunction resultVoltageCurve { get; set; } = new SampledFunction();
        public SampledFunction resultCurrentCurve { get; set; } = new SampledFunction();

        public Impedance() : base()
        {
            functionImpedance = new ConstantValueFunction(1);
            description = "Z";

            ParametersControl = new ImpedanceParametersControl(this);
            OnScreenElement = new ImpedanceScreen(this);
        }

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "Description": Description = (string)value; break;
                case "FunctionImpedance": FunctionImpedance = (Function)value; break;
                case "OutputVoltageAcross": OutputVoltageAcross = (bool)value; break;
                case "OutputCurrentThrough": OutputCurrentThrough = (bool)value; break;
            }
            
            resultVoltageCurve.Title = "Voltage Across Impedance " + ElementName;
            resultCurrentCurve.Title = "Current Through Impedance " + ElementName;
        }

        public ComplexDecimal getImpedance(decimal frequency)
        {
            throw new NotImplementedException();
        }

        List<Tuple<byte, byte, ComplexDecimal>> IPassive.getImpedance(decimal frequency)
        {
            var output = new List<Tuple<byte, byte, ComplexDecimal>>();
            output.Add(new Tuple<byte, byte, ComplexDecimal>(0, 1, getImpedance(frequency)));

            return output;
        }
    }
}
