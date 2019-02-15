using Easycoustics.Transition.CircuitEditor.OnScreenComponents;
using Easycoustics.Transition.CircuitEditor.ParametersControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easycoustics.Transition.CircuitEditor.Serializable
{
    public class VoltageOutputNode : SerializableComponent
    {
        public override string ElementLetter => "N";
        public override string ElementType => "Voltage Curve Output Node";

        public override byte QuantityOfTerminals { get => 1; set => throw new NotImplementedException(); }

        public VoltageOutputNode() : base()
        {
            OnScreenElement = new VoltageOutputNodeScreen(this);
            ParametersControl = new VoltageOutputNodeParametersControl(this);
        }
    }
}
