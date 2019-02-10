using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.CircuitEditor.ParametersControls;
using Transition.Common;
using Transition.Functions;

namespace Transition.CircuitEditor.Serializable
{
    public class Impedance : SerializableComponent, IPassive
    {
        public override string ElementLetter => "Z";
        public override string ElementType => "Impedance";

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
            }
        }

        public ComplexDecimal getImpedance(decimal frequency)
        {
            throw new NotImplementedException();
        }
    }
}
