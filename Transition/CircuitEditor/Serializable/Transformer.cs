using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;

namespace Transition.CircuitEditor.Serializable
{
    public class Transformer : SerializableComponent
    {
        public override string ElementLetter => "T";
        public override string ElementType => "Transformer";

        private EngrNumber turnsRatio;
        public EngrNumber TurnsRatio
        {
            get { return turnsRatio; }
            set { SetProperty(ref turnsRatio, value);
                changeTR();
            }
        }
     
        private double kCouplingCoef;
        public double KCouplingCoef
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

        public override byte QuantityOfTerminals { get => 4; set => throw new NotImplementedException(); }

        public Transformer() : base()
        {
         
            SetProperty(ref turnsRatio, EngrNumber.One, "TurnsRatio");
            SetProperty(ref lpri, EngrNumber.One, "Lpri");
            SetProperty(ref lsec, EngrNumber.One, "Lsec");
            SetProperty(ref kCouplingCoef, 0.99, "KCouplingCoef");
            updateM();

            ParametersControl = new TransformerParametersControl(this);
            OnScreenComponent = new OnScreenComponents.TransformerScreen(this);
        }

        private void changeTR()
        {
            double tr = TurnsRatio.ValueDouble;
            double lp = Lpri.ValueDouble;

            double ls = tr * tr * lp;
            SetProperty(ref lsec, ls, "Lsec");
            updateM();
        }

        private void changeLpri()
        {
            double lp = Lpri.ValueDouble;
            double tr = TurnsRatio.ValueDouble;

            double ls = tr * tr * lp;
            SetProperty(ref lsec, ls, "Lsec");
            updateM();
        }
        
        private void changeLsec()
        {
            double ls = Lsec.ValueDouble;
            double lp = Lpri.ValueDouble;

            double tr = Math.Sqrt(ls / lp);
            SetProperty(ref turnsRatio, tr, "TurnsRatio");
            updateM();
        }

        private void updateM()
        {
            double lp = Lpri.ValueDouble;
            double ls = Lsec.ValueDouble;
            double k = (double)KCouplingCoef;

            SetProperty(ref mutualL, new EngrNumber(k * Math.Sqrt(lp * ls)), "MutualL");
            SetProperty(ref lpLeak, new EngrNumber(lp * (1 - k)), "LpLeak");
            SetProperty(ref lsLeak, new EngrNumber(ls * (1 - k)), "LsLeak");
        }

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "TurnsRatio": TurnsRatio = (EngrNumber)value; break;
                case "KCouplingCoef": KCouplingCoef = (double)value; break;
                case "Lpri": Lpri = (EngrNumber)value; break;
                case "Lsec": Lsec = (EngrNumber)value; break;
            }
        }
    }
}
