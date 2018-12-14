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
        public override string ElementType => "Transfer Function";

        public override byte QuantityOfTerminals { get => 4; set => throw new NotImplementedException(); }

        private EngrNumber rIn;
        public EngrNumber RIn
            { get => rIn;
              set { SetProperty(ref rIn, value); } }

        private EngrNumber rOut;
        public EngrNumber ROut
            { get => rOut; set { SetProperty(ref rOut, value); } }

        private Function tf;
        public Function Tf
            { get => tf;
              set { SetProperty(ref tf, value);
                raiseLayoutChanged(); } }

        private int functionType;   /* 0=standard, 1=custom curve, 2=laplace expression */
        public int FunctionType { get => functionType;
            set { SetProperty(ref functionType, value);
                raiseLayoutChanged(); }
        }

        private string standardFunction;
        public string StandardFunction { get => standardFunction;
            set { SetProperty(ref standardFunction, value);
                raiseLayoutChanged();
            }
        }

        private EngrNumber ao;
        public EngrNumber Ao { get => ao; set { SetProperty(ref ao, value); } }

        private EngrNumber fp;
        public EngrNumber Fp { get => fp; set { SetProperty(ref fp, value); } }

        private EngrNumber fz;
        public EngrNumber Fz { get => fz; set { SetProperty(ref fz, value); } }

        private EngrNumber qp;
        public EngrNumber Qp { get => qp; set { SetProperty(ref qp, value); } }

        private EngrNumber qz;
        public EngrNumber Qz { get => qz; set { SetProperty(ref qz, value); } }

        private bool invert;
        public bool Invert { get => invert; set { SetProperty(ref invert, value); } }

        private bool reverse;
        public bool Reverse { get => reverse; set { SetProperty(ref reverse, value); } }

        public TransferFunctionComponent() : base()
        {
            rIn = "1T";
            rOut = "1u";

            ao = 0;
            fp = "1K";
            fz = "1K";
            qp = 1;
            qz = 1;

            functionType = 0;
            standardFunction = "LP1";

            invert = false;
            reverse = false;

            ParametersControl = new TransferFunctionParametersControl(this);
            OnScreenElement = new OnScreenComponents.TransferFunctionScreen(this);
        }


        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "RIn": RIn = (EngrNumber)value; break;
                case "ROut": ROut = (EngrNumber)value; break;

                case "Ao": Ao = (EngrNumber)value; break;
                case "Fp": Fp = (EngrNumber)value; break;
                case "Fz": Fz = (EngrNumber)value; break;
                case "Qp": Qp = (EngrNumber)value; break;
                case "Qz": Qz = (EngrNumber)value; break;

                case "Invert": Invert = (bool)value;break;
                case "Reverse": Reverse = (bool)value;break;

                case "FunctionType": FunctionType = (int)value;break;
                case "StandardFunction": StandardFunction = (string)value;break;
            }
        }
    }
}
