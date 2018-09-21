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
        
        private EngrNumber capacitorValue;
        public EngrNumber CapacitorValue
        {
            get { return capacitorValue; }
            set { SetProperty(ref capacitorValue, value); }
        }

        private int capacitorModel;
        public int CapacitorModel
        {
            get { return capacitorModel; }
            set { SetProperty(ref capacitorModel, value); }
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
        public EngrNumber RP
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
        
        public override int QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        public Capacitor()
        {
            CapacitorValue = EngrNumber.One;
            CapacitorModel = 0;
            
            SetProperty(ref ls, new EngrNumber(1, "p"), "Ls");
            SetProperty(ref rs, new EngrNumber(1, "p"), "Rs");
            SetProperty(ref rp, new EngrNumber(1, "T"), "Rp");
            calculateFoQ();

            ParametersControl = new CapacitorParametersControl(this);
            OnScreenComponent = new CapacitorScreen(this);
        }


        private void calculateFoQ()
        {
            double dC = capacitorValue.ValueDouble;
            double dLs = Ls.ValueDouble;
            double dRs = Rs.ValueDouble;

            double dWo = Math.Sqrt(1 / (dLs * dC));

            double dQ = (dWo * dLs) / dRs;
            double dFo = dWo / (2 * Math.PI);

            SetProperty(ref fo, dFo, "Fo");
            SetProperty(ref q, dQ, "Q");
        }

        private void calculateRsLs()
        {
            double dQ = Q.ValueDouble;
            double dFo = Fo.ValueDouble;
            double dC = capacitorValue.ValueDouble;

            double dWo = 2 * Math.PI * dFo;

            //  double dLs = Math.Sqrt(Math.Abs( ((dQ * dQ * dR * dR) - (dR * dR)) / Math.Pow(dWo, 2) ));
            double dRs = 1 / (dQ * dWo * dC);
            double dLs = 1 / (dC * dWo * dWo);

            SetProperty(ref ls, dLs, "Ls");
            SetProperty(ref rs, dRs, "Rs");
            
        }


    }
}
