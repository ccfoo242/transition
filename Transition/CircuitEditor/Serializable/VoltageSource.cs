using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;
using Transition.CircuitEditor.OnScreenComponents;

namespace Transition.CircuitEditor.Serializable
{
    public class VoltageSource : SerializableComponent
    {
        public override string ElementLetter => "V";
        public override string ElementType => "Voltage Source";

        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        public VoltageSource() : base()
        {
            OnScreenElement = new VoltageSourceScreen(this);
            ParametersControl = new VoltageSourceComponentParameters(this);
        }

    }
}
