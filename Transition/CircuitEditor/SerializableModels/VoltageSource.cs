using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;

namespace Transition.CircuitEditor.SerializableModels
{
    public class VoltageSource : SerializableComponent
    {
        public override string ElementLetter => "V";

        public override int QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

    }
}
