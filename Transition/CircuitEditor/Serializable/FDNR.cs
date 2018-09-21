using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;
using Transition.CircuitEditor.OnScreenComponents;

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

        public FDNR() : base()
        {
            FdnrValue = EngrNumber.One;
            
            ParametersControl = new FDNRParametersControl(this);
            OnScreenComponent = new FDNRScreen(this);
        }
    }
}
