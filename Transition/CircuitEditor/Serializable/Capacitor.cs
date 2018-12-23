using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;
using Transition.CircuitEditor.OnScreenComponents;

namespace Transition.CircuitEditor.Serializable
{
    public class Capacitor : SerializableComponent
    {
        public override string ElementLetter => "C";
        public override string ElementType => "Capacitor";

        private EngrNumber capacitorValue;
        public EngrNumber CapacitorValue
        {
            get { return capacitorValue; }
            set { SetProperty(ref capacitorValue, value);
                  calculateFoQ();
                  OnPropertyChanged("ValueString");  
            }
        }

        private int capacitorModel;
        public int CapacitorModel
        {
            get { return capacitorModel; }
            set { SetProperty(ref capacitorModel, value); }
        }

        private Precision componentPrecision;
        public Precision ComponentPrecision
        {
            get { return componentPrecision; }
            set { SetProperty(ref componentPrecision, value);
                OnPropertyChanged("ValueString");
            }
        }

        private EngrNumber ls;
        public EngrNumber Ls
        {
            get { return ls; }
            set
            {
                SetProperty(ref ls, value);
                calculateFoQ();
            }
        }

        private EngrNumber rs;
        public EngrNumber Rs
        {
            get { return rs; }
            set
            {
                SetProperty(ref rs, value);
                calculateFoQ();
            }
        }


        private EngrNumber rp;
        public EngrNumber Rp
        {
            get { return rp; }
            set
            {
                SetProperty(ref rp, value);
                calculateFoQ();
            }
        }

        private EngrNumber ew;
        public EngrNumber Ew
        {
            get { return ew; }
            set { SetProperty(ref ew, value); }
        }

        private EngrNumber fo;
        public EngrNumber Fo
        {
            get { return fo; }
            set
            {
                SetProperty(ref fo, value);
                calculateRsLs();
            }
        }

        private EngrNumber q;
        public EngrNumber Q
        {
            get { return q; }
            set
            {
                SetProperty(ref q, value);
                calculateRsLs();
            }
        }

        public bool AnyPrecisionSelected { get { return (ComponentPrecision == Precision.Arbitrary); } }

        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        public Capacitor()
        {
            CapacitorValue = EngrNumber.One;
            CapacitorModel = 0;
            
            SetProperty(ref ls, new EngrNumber(1, "p"), "Ls");
            SetProperty(ref rs, new EngrNumber(1, "p"), "Rs");
            SetProperty(ref rp, new EngrNumber(1, "T"), "Rp");
            calculateFoQ();

            ParametersControl = new CapacitorParametersControl(this);
            OnScreenElement = new CapacitorScreen(this);
        }


        private void calculateFoQ()
        {
            if (CapacitorValue.ToDouble == 0) return;
            if (Rs.ToDouble == 0) return;
            if (Ls.ToDouble == 0) return;

            double dC = capacitorValue.ToDouble;
            double dLs = Ls.ToDouble;
            double dRs = Rs.ToDouble;

            double dWo = Math.Sqrt(1 / (dLs * dC));

            double dQ = (dWo * dLs) / dRs;
            double dFo = dWo / (2 * Math.PI);

            SetProperty(ref fo, dFo, "Fo");
            SetProperty(ref q, dQ, "Q");
        }

        private void calculateRsLs()
        {
            double dQ = Q.ToDouble;
            double dFo = Fo.ToDouble;
            double dC = capacitorValue.ToDouble;

            double dWo = 2 * Math.PI * dFo;

            //  double dLs = Math.Sqrt(Math.Abs( ((dQ * dQ * dR * dR) - (dR * dR)) / Math.Pow(dWo, 2) ));
            double dRs = 1 / (dQ * dWo * dC);
            double dLs = 1 / (dC * dWo * dWo);

            SetProperty(ref ls, dLs, "Ls");
            SetProperty(ref rs, dRs, "Rs");
            
        }

        public string ValueString
        {
            get
            {
                // this one sets de String for the component in the schematic window
                string returnString;
                EngrConverter conv = new EngrConverter() { ShortString = false };
                EngrConverter convShort = new EngrConverter() { ShortString = true };

                if (AnyPrecisionSelected)
                    returnString = (string)conv.Convert(CapacitorValue, typeof(string), null, "");
                else
                    returnString = (string)convShort.Convert(CapacitorValue, typeof(string), null, "");

                return returnString + "F";
            }
        }

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "CapacitorValue": CapacitorValue = (EngrNumber)value; break;
                case "CapacitorModel": CapacitorModel = (int)value; break;
                case "ComponentPrecision": ComponentPrecision = (Precision)value; break;
                case "Ls": Ls = (EngrNumber)value; break;
                case "Rp": Rp = (EngrNumber)value; break;
                case "Rs": Rs = (EngrNumber)value; break;
                case "Fo": Fo = (EngrNumber)value; break;
                case "Q":  Q = (EngrNumber)value; break;
                case "Ew": Ew = (EngrNumber)value; break;
            }
        }

    }
}
