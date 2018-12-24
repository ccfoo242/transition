using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Transition.Functions
{
    public class StandardTransferFunction : Function
    {

        private EngrNumber _ao;
        public EngrNumber Ao { get => _ao; set { _ao = value; RecalculatePoints(); RaiseFunctionChanged(); } }

        private EngrNumber _fp;
        public EngrNumber Fp { get => _fp; set { _fp = value; RecalculatePoints(); RaiseFunctionChanged(); } }

        private EngrNumber _fz;
        public EngrNumber Fz { get => _fz; set { _fz = value; RecalculatePoints(); RaiseFunctionChanged(); } }

        private EngrNumber _qp;
        public EngrNumber Qp { get => _qp; set { _qp = value; RecalculatePoints(); RaiseFunctionChanged(); } }

        private bool _invert;
        public bool Invert { get => _invert; set { _invert = value; RecalculatePoints(); RaiseFunctionChanged(); } }

        private bool _reverse;
        public bool Reverse { get => _reverse; set { _reverse = value; RecalculatePoints(); RaiseFunctionChanged(); } }

        public double wp { get => 2 * Math.PI * fp; }
        public double wz { get => 2 * Math.PI * fz; }

        private double ao => Ao.ToDouble;
        private double fp => Fp.ToDouble;
        private double fz => Fz.ToDouble;
        private double qp => Qp.ToDouble;

        private List<EngrNumber> FrequencyPoints => CircuitEditor.CircuitEditor.StaticCurrentDesign.getFrequencyPoints();

        private string currentFunction;
        public string CurrentFunction
        {
            get => currentFunction; set
            {
                currentFunction = value;
                RecalculatePoints();
                RaiseFunctionChanged();
            }
        }
        private Dictionary<EngrNumber, Complex> points = new Dictionary<EngrNumber, Complex>();

        public StandardTransferFunction()
        {
            _ao = 1;
            _fp = 1000;
            _fz = 1000;
            _qp = 1;
            _invert = false;
            _reverse = false;
            currentFunction = "LP1";
        }

        private void RecalculatePoints()
        {
            points = new Dictionary<EngrNumber, Complex>();

            foreach (var freq in FrequencyPoints)
                points.Add(freq, Calculate(freq));
        }

        public override Dictionary<EngrNumber, Complex> Points => points;

        public override Complex Calculate(EngrNumber f)
        {
            /* s = jw */
            return Calculate(Complex.ImaginaryOne * f * 2 * Math.PI);
        }

        public Complex Calculate(Complex s)
        {
            /* here s is equal to jw */

            Complex output = 0;

            switch (CurrentFunction)
            {
                case "LP1":
                    output = ao / (1 + (s / wp));
                    break;
                case "HP1":
                    output = (ao * s / wp) / (1 + (s / wp));
                    break;
                case "AP1":
                    output = (ao * (1 - (s / wp))) / (1 + (s / wp));
                    break;
                case "LP2":
                    output = ao / (1 + (s / (qp * wp)) + (Complex.Pow(s, 2) / Complex.Pow(wp, 2)));
                    break;
                case "HP2":
                    output = (ao * Complex.Pow(s, 2) / Math.Pow(wp, 2)) / (1 + (s / (qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2)));
                    break;
                case "AP2":
                    output = (ao * (1 - (s / qp * wp) + (Complex.Pow(s, 2) / Math.Pow(wp, 2)))) / (1 + (s / (qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2)));
                    break;
                case "BP1":
                    output = (ao * (s / (qp * wp))) / (1 + (s / (qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2)));
                    break;
                case "BR1":
                    output = (ao * (1 + (Complex.Pow(s, 2) / Math.Pow(wz, 2)))) / (1 + (s / (qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2)));
                    break;
                case "LP12":
                    output = (ao / (Complex.Sqrt(1 + (s / wp))));
                    break;
                case "HP12":
                    output = ao * Complex.Sqrt((s / wp) / (1 + (s / wp)));
                    break;
                case "LEQ":
                    if (ao >= 1)
                        output = (ao + (s / wp)) / (1 + (s / wp));
                    else
                        output = (1 + (s / wp)) / ((1 / ao) + (s / wp));
                    break;
                case "HEQ":
                    if (ao >= 1)
                        output = (1 + (ao * (s / wp))) / (1 + (s / wp));
                    else
                        output = (1 + (s / wp)) / (1 + (s / (ao * wp)));
                    break;
                case "BEQ":
                    if (ao >= 1)
                        output = (1 + ((ao * s) / (qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2))) /
                               (1 + (s / (qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2)));
                    else
                        output = (1 + (s / (qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2))) /
                               (1 + (s / (ao * qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2)));
                    break;
                case "Sinc":
                    double w = (s / Complex.ImaginaryOne).Magnitude;
                    double fs = fp;
                    double lambda = w / (2 * fs);
                    output = ao * (Math.Sin(lambda) / lambda) * Complex.Exp((-1 * s) / (2 * fs));
                    break;
                 default: throw new NotSupportedException();
            }

            if (Invert) output = Complex.Reciprocal(output);
            if (Reverse) output = Complex.Negate(output);

            return output;
        }
        

    }
    
}
