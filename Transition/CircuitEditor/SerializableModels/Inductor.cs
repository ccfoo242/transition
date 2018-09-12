using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;

namespace Transition.CircuitEditor.SerializableModels
{
    public class Inductor : SerializableComponent
    {
        public override string ComponentLetter => "L";
        
        private EngrNumber inductorValue;
        public EngrNumber InductorValue
        {
            get { return inductorValue; }
            set
            {
                inductorValue = value;
                SetProperty(ref inductorValue, value);
            }
        }

        private int inductorModel;
        public int InductorModel
        {
            get { return inductorModel; }
            set { SetProperty(ref inductorModel, value); }
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
                calculateRsCp();
            }
        }

        private EngrNumber q;
        public EngrNumber Q
        {
            get { return q; }
            set
            {
                SetProperty(ref q, value);
                calculateRsCp();
            }
        }


        private void calculateFoQ()
        {
            double dL = inductorValue.ValueDouble;
            double dRs = Rs.ValueDouble;
            double dCp = Cp.ValueDouble;

            double dWop = Math.Sqrt(1 / (dL * dCp));

            double dQ = (dWop * dL) / dRs;
            double dFo = dWop / (2 * Math.PI);

            Fo = new EngrNumber(dFo);
            Q = new EngrNumber(dQ);

        }

        private void calculateRsCp()
        {
            double dQ = Q.ValueDouble;
            double dFo = Fo.ValueDouble;
            double dL = inductorValue.ValueDouble;

            double dWo = 2 * Math.PI * dFo;

            double dRs = (dWo * dL) / dQ;
            double dCp = 1 / (dL * dWo * dWo);

            Rs = new EngrNumber(dRs);
            Cp = new EngrNumber(dCp);

        }

    }
}
