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

        public override int QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        public VoltageSource() : base()
        {
            OnScreenComponent = new VoltageSourceScreen(this);
            ParametersControl = new VoltageSourceComponentParameters(this);
        }

    }
}
