using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.ParametersControls;
using Transition.Functions;

namespace Transition.CircuitEditor.Serializable
{
    public class TransferFunctionComponent : SerializableComponent
    {
        public override string ElementLetter => "H";
        public override string ElementType => "TransferFunction";

        public override byte QuantityOfTerminals { get => 4; set => throw new NotImplementedException(); }

        private EngrNumber rIn;
        public EngrNumber RIn  { get => rIn;  set { SetProperty(ref rIn, value); } }

        private EngrNumber rOut;
        public EngrNumber ROut { get => rOut; set { SetProperty(ref rOut, value); } }

        private Function tf;
        public Function Tf     { get => tf;   set { SetProperty(ref tf, value); } }

        public TransferFunctionComponent() : base()
        {
            rIn = "1T";
            rOut = "1u";

            ParametersControl = new TransferFunctionParametersControl(this);
            OnScreenElement = new OnScreenComponents.TransferFunctionScreen(this);
        }
    }
}
