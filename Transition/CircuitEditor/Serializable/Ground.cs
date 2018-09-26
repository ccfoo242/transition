using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;
using Transition.CircuitEditor.OnScreenComponents;

namespace Transition.CircuitEditor.Serializable
{
    public class Ground : SerializableComponent
    {
        public override string ElementLetter => "G";

        public override byte QuantityOfTerminals { get => 1; set => throw new NotImplementedException(); }

        public Ground()
        {
            OnScreenComponent = new GroundScreen(this);
            ParametersControl = new GroundParameters(this);

        }
    }
}
