using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor.ParametersControls;
using Easycoustics.Transition.Common;
using Easycoustics.Transition.Functions;

namespace Easycoustics.Transition.CircuitEditor.Serializable
{
    public class TransferFunctionComponent : SerializableComponent, IPassive, IIsolateSection
    {
        public override string ElementLetter => "H";
        public override string ElementType => "Transfer Function";

        public override byte QuantityOfTerminals { get => 4; set => throw new NotImplementedException(); }
        public ComplexDecimal GetImpedance(decimal frequency) => RIn;
        public Tuple<byte, byte> GetImpedanceTerminals => new Tuple<byte, byte>(0, 1);

        private decimal rIn;
        public decimal RIn
            { get => rIn;
              set { SetProperty(ref rIn, value); } }

        private decimal rOut;
        public decimal ROut
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


        public decimal Ao
        {
            get => standardTf.Ao; set
            {
                standardTf.Ao = value;
                OnPropertyChanged("Ao");
            }
        }

        public decimal Fp
        {
            get => standardTf.Fp; set
            {
                standardTf.Fp = value;
                OnPropertyChanged("Fp");
            }
        }

        public decimal Fz
        {
            get => standardTf.Fz; set
            {
                standardTf.Fz = value;
                OnPropertyChanged("Fz");
            }
        }

        public decimal Qp
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
            rIn = 1e12m;
            rOut = 1e-6m;

            Ao = 1m;
            Fp = 1e3m;
            Fz = 1e3m;
            Qp = 1m;

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
                case "RIn": RIn = (decimal)value; break;
                case "ROut": ROut = (decimal)value; break;

                case "Ao": Ao = (decimal)value; break;
                case "Fp": Fp = (decimal)value; break;
                case "Fz": Fz = (decimal)value; break;
                case "Qp": Qp = (decimal)value; break;

                case "Invert": Invert = (bool)value;break;
                case "Reverse": Reverse = (bool)value;break;

                case "FunctionType": FunctionType = (int)value;break;
                case "StandardFunction": StandardFunction = (string)value;break;
            }
        }

        public byte[] getOtherTerminalsIsolated(byte terminal)
        {
            if (terminal == 0) return new byte[] { 1 };
            else if (terminal == 1) return new byte[] { 0 };
            else if (terminal == 2) return new byte[] { 3 };
            else return new byte[] { 2 };

        }
    }
}
