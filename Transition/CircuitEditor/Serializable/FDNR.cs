using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transition.CircuitEditor.Serializable
{
    public class FDNR : SerializableComponent
    {
        public override string ElementLetter => "D";
        
        private EngrNumber fdnrValue;
        public EngrNumber FdnrValue
        {
            get { return fdnrValue; }
            set { SetProperty(ref fdnrValue, value); }
        }

        public override int QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

    }
}
