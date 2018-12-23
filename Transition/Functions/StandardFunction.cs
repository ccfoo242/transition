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
        public EngrNumber Ao { get => _ao; set { _ao = value; RaiseFunctionChanged(); RecalculatePoints()} }

        private EngrNumber _fp;
        public EngrNumber Fp { get => _fp; set { _fp = value; RaiseFunctionChanged(); RecalculatePoints() } }

        private EngrNumber _fz;
        public EngrNumber Fz { get => _fz; set { _fz = value; RaiseFunctionChanged(); RecalculatePoints() } }

        private EngrNumber _qp;
        public EngrNumber Qp { get => _qp; set { _qp = value; RaiseFunctionChanged(); RecalculatePoints() } }

        private EngrNumber _qz;
        public EngrNumber Qz { get => _qz; set { _qz = value; RaiseFunctionChanged(); RecalculatePoints()} }

        private bool _invert;
        public bool invert { get => _invert; set { _invert = value; RaiseFunctionChanged(); RecalculatePoints() } }

        private bool _reverse;
        public bool reverse { get => _reverse; set { _reverse = value; RaiseFunctionChanged(); RecalculatePoints() } }

        public double wp { get => 2 * Math.PI * fp; }
        public double wz { get => 2 * Math.PI * fz; }

        private double ao => Ao.ToDouble;
        private double fp => Fp.ToDouble;
        private double fz => Fz.ToDouble;
        private double qp => Qp.ToDouble;
        private double qz => Qz.ToDouble;

        private List<EngrNumber> FrequencyPoints => CircuitEditor.CircuitEditor.StaticCurrentDesign.getFrequencyPoints();

        public enum StandardFunction { LP1, LP2,  HP1,  HP2, AP1, AP2, BP1,
                                       BR1, LP12, HP12, LEQ, BEQ, HEQ, Sinc};

        public StandardFunction CurrentFunction { get; set; }
        private Dictionary<EngrNumber, Complex> points = new Dictionary<EngrNumber, Complex>();

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
            switch (CurrentFunction)
            {
                case StandardFunction.LP1:
                    return ao / (1 + (s / wp));

                case StandardFunction.HP1:
                    return (ao * s / wp) / (1 + (s / wp));

                case StandardFunction.AP1:
                    return (ao * (1 - (s / wp))) / (1 + (s / wp));

                case StandardFunction.LP2:
                    return ao / (1 + (s / (qp * wp)) + (Complex.Pow(s, 2) / Complex.Pow(wp, 2)));

                case StandardFunction.HP2:
                    return (ao * Complex.Pow(s, 2) / Math.Pow(wp, 2)) / (1 + (s / (qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2)));

                case StandardFunction.AP2:
                    return (ao * (1 - (s / qp * wp) + (Complex.Pow(s, 2) / Math.Pow(wp, 2)))) / (1 + (s / (qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2)));

                case StandardFunction.BP1:
                    return (ao * (s / (qp * wp))) / (1 + (s / (qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2)));

                case StandardFunction.BR1:
                    return (ao * (1 + (Complex.Pow(s, 2) / Math.Pow(wz, 2)))) / (1 + (s / (qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2)));

                case StandardFunction.LP12:
                    return (ao / (Complex.Sqrt(1 + (s / wp))));

                case StandardFunction.HP12:
                    return ao * Complex.Sqrt((s / wp) / (1 + (s / wp)));

                case StandardFunction.LEQ:
                    if (ao >= 1)
                        return (ao + (s / wp)) / (1 + (s / wp));
                    else
                        return (1 + (s / wp)) / ((1 / ao) + (s / wp));

                case StandardFunction.HEQ:
                    if (ao >= 1)
                        return (1 + (ao * (s / wp))) / (1 + (s / wp));
                    else
                        return (1 + (s / wp)) / (1 + (s / (ao * wp)));

                case StandardFunction.BEQ:
                    if (ao >= 1)
                        return (1 + ((ao * s) / (qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2))) /
                               (1 + (s / (qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2)));
                    else
                        return (1 + (s / (qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2))) /
                               (1 + (s / (ao * qp * wp)) + (Complex.Pow(s, 2) / Math.Pow(wp, 2)));

                case StandardFunction.Sinc:
                    double w = (s / Complex.ImaginaryOne).Magnitude;
                    double fs = fp;
                    double lambda = w / (2 * fs);
                    return ao * (Math.Sin(lambda) / lambda) * Complex.Exp((-1 * s) / (2 * fs));
            }
            throw new NotSupportedException();
        }
        

    }
    
}
