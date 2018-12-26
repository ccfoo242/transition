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
                raiseLayoutChanged();
                OnPropertyChanged("TFString");
            }
        }

        public StandardTransferFunction standardTf = new StandardTransferFunction();

        public string TFString { get {
                if (FunctionType == 0) return StandardFunction;
                else
                if (FunctionType == 1) return "Custom";
                else
                    return "Laplace";
            } }
        
        public string StandardFunction
        {
            get => standardTf.CurrentFunction;
            set
            {
                standardTf.CurrentFunction = value;
                OnPropertyChanged("StandardFunction");
                OnPropertyChanged("TFString");
                raiseLayoutChanged();
            }
        }


        public EngrNumber Ao
        {
            get => standardTf.Ao; set
            {
                standardTf.Ao = value;
                OnPropertyChanged("Ao");
            }
        }

        public EngrNumber Fp
        {
            get => standardTf.Fp; set
            {
                standardTf.Fp = value;
                OnPropertyChanged("Fp");
            }
        }

        public EngrNumber Fz
        {
            get => standardTf.Fz; set
            {
                standardTf.Fz = value;
                OnPropertyChanged("Fz");
            }
        }

        public EngrNumber Qp
        {
            get => standardTf.Qp; set
            {
                standardTf.Qp = value;
                OnPropertyChanged("Qp");
            }
        }
        
        public bool Invert
        {
            get => standardTf.Invert; set
            {
                standardTf.Invert = value;
                OnPropertyChanged("Invert");
            }
        }

        public bool Reverse
        {
            get => standardTf.Reverse; set
            {
                standardTf.Reverse = value;
                OnPropertyChanged("Reverse");
            }
        }

        public LaplaceFunction laplaceTf;
        public Function customCurve;

        public TransferFunctionComponent() : base()
        {
            rIn = "1T";
            rOut = "1u";

            Ao = 1;
            Fp = "1K";
            Fz = "1K";
            Qp = 1;

            StandardFunction = "LP1";

            Invert = false;
            Reverse = false;

            functionType = 0;

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

                case "Invert": Invert = (bool)value;break;
                case "Reverse": Reverse = (bool)value;break;

                case "FunctionType": FunctionType = (int)value;break;
                case "StandardFunction": StandardFunction = (string)value;break;
            }
        }
    }
}
