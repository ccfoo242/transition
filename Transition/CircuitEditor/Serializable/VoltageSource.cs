using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transition.CircuitEditor.Serializable
{
    public class VoltageSource : SerializableComponent
    {
        public override string ElementLetter => "V";

        public override int QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

    }
}
