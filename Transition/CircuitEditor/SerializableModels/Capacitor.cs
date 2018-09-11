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

        private EngrNumber cp;
        public EngrNumber Cp
        {
            get { return cp; }
            set
            {
                SetProperty(ref cp, value);
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
                calculateLsCp();
            }
        }

        private EngrNumber q;
        public EngrNumber Q
        {
            get { return q; }
            set
            {
                SetProperty(ref q, value);
                calculateLsCp();
            }
        }

        public Capacitor()
        { }


        private void calculateFoQ()
        {
            double dC = capacitorValue.ValueDouble;
            double dLs = Ls.ValueDouble;
            double dRs = Rs.ValueDouble;

            double dWo = Math.Sqrt(1 / (dLs * dC));

            double dQ = (dWo * dLs) / dRs;
            double dFo = dWo / (2 * Math.PI);

            Fo = new EngrNumber(dFo);
            Q = new EngrNumber(dQ);
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
