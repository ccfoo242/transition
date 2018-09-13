using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;

namespace Transition.CircuitEditor.SerializableModels
{
    public class Capacitor : SerializableComponent
    {
        public override string ComponentLetter => "C";


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

        public Capacitor()
        {
            CapacitorValue = EngrNumber.One;

            CapacitorModel = 0;

        }


        private void calculateFoQ()
        {
            double dC = capacitorValue.ValueDouble;
            double dLs = Ls.ValueDouble;
            double dRs = Rs.ValueDouble;

            double dWo = Math.Sqrt(1 / (dLs * dC));

            double dQ = (dWo * dLs) / dRs;
            double dFo = dWo / (2 * Math.PI);

            SetProperty(ref fo, dFo);
            SetProperty(ref q, dQ);
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

            Ls = new EngrNumber(dLs);
            Rs = new EngrNumber(dRs);
        }


    }
}
