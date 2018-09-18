using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transition.CircuitEditor.Serializable
{
    public class Ground : SerializableComponent
    {
        public override string ElementLetter => "G";

        public override int QuantityOfTerminals { get => 1; set => throw new NotImplementedException(); }

    }
}
