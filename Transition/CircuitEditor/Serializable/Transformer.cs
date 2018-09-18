using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transition.CircuitEditor.Serializable
{
    public class Transformer : SerializableComponent
    {
        public override string ElementLetter => "T";


        private EngrNumber turnsRatio;
        public EngrNumber TurnsRatio
        {
            get { return turnsRatio; }
            set { SetProperty(ref turnsRatio, value);
                changeTR();
            }
        }
     
        private decimal kCouplingCoef;
        public decimal KCouplingCoef
        {
            get { return kCouplingCoef; }
            set { SetProperty(ref kCouplingCoef, value);
                updateM();
            }
        }

        private EngrNumber lpri;
        public EngrNumber Lpri
        {
            get { return lpri; }
            set
            {
                SetProperty(ref lpri, value);
                changeLpri();
            }
        }

        private EngrNumber lsec;
        public EngrNumber Lsec
        {
            get { return lsec; }
            set
            {
                SetProperty(ref lsec, value);
                changeLsec();
            }
        }

        private EngrNumber mutualL;
        public EngrNumber MutualL { get { return mutualL; }}

        private EngrNumber lpLeak;
        public EngrNumber LpLeak { get { return lpLeak; }}

        private EngrNumber lsLeak;
        public EngrNumber LsLeak { get { return lsLeak; }}

        public override int QuantityOfTerminals { get => 4; set => throw new NotImplementedException(); }


        private void changeTR()
        {
            double tr = TurnsRatio.ValueDouble;
            double lp = Lpri.ValueDouble;

            double ls = tr * tr * lp;
            SetProperty(ref lsec, ls);
            updateM();
        }

        private void changeLpri()
        {
            double lp = Lpri.ValueDouble;
            double tr = TurnsRatio.ValueDouble;

            double ls = tr * tr * lp;
            SetProperty(ref lsec, ls);
            updateM();
        }
        
        private void changeLsec()
        {
            double ls = Lsec.ValueDouble;
            double lp = Lpri.ValueDouble;

            double tr = Math.Sqrt(ls / lp);
            SetProperty(ref turnsRatio, tr);
            updateM();
        }

        private void updateM()
        {
            double lp = Lpri.ValueDouble;
            double ls = Lsec.ValueDouble;
            double k = (double)KCouplingCoef;

            SetProperty(ref mutualL, new EngrNumber(k * Math.Sqrt(lp * ls)));
            SetProperty(ref lpLeak, new EngrNumber(lp * (1 - k)));
            SetProperty(ref lsLeak, new EngrNumber(ls * (1 - k)));
        }
        
    }
}
