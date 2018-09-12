using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;

namespace Transition.CircuitEditor.Serializable
{
    public class Resistor : SerializableComponent
    {
        public override string ComponentLetter => "R";

        private EngrNumber resistorValue;
        public EngrNumber ResistorValue
        {
            get { return resistorValue; }
            set { SetProperty(ref resistorValue, value); }
        }

        private int resistorModel;
        public int ResistorModel
        {
            get { return resistorModel; }
            set { SetProperty(ref resistorModel, value); }
        }

        private EngrNumber ls;
        public EngrNumber Ls
        {
            get { return ls; }
            set { SetProperty(ref ls, value);
                  calculateFoQ(); }
        }

        private EngrNumber cp;
        public EngrNumber Cp
        {
            get { return cp; }
            set { SetProperty(ref cp, value);
                  calculateFoQ(); }
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
            get { return fo;}
            set { SetProperty(ref fo, value);
                  calculateLsCp(); }
        }

        private EngrNumber q;
        public EngrNumber Q
        {
            get { return q; }
            set { SetProperty(ref q, value);
                  calculateLsCp(); }
        }


        public Resistor() : base()
        {
            ResistorValue = EngrNumber.One;
            
            ResistorModel = 0;
            Ls = new EngrNumber(1, "p");
            Cp = new EngrNumber(1, "p");

            ParametersControl = new ResistorParametersControl();
            OnScreenComponent = new OnScreenComponents.ResistorScreen(this);
        }

        private void calculateFoQ()
        {
            double dR = ResistorValue.ValueDouble;
            double dLs = Ls.ValueDouble;
            double dCp = Cp.ValueDouble;
            
            double dWop = Math.Sqrt(1 / (dLs * dCp));
            
            double dQ = (dWop * dLs) / dR;
            double dFo = dWop / (2 * Math.PI);

            SetProperty(ref fo, new EngrNumber(dFo));
            SetProperty(ref q, new EngrNumber(dQ));

        }

        private void calculateLsCp()
        {
            double dQ = Q.ValueDouble;
            double dFo = Fo.ValueDouble;
            double dR = ResistorValue.ValueDouble;

            double dWo = 2 * Math.PI * dFo;

            //  double dLs = Math.Sqrt(Math.Abs( ((dQ * dQ * dR * dR) - (dR * dR)) / Math.Pow(dWo, 2) ));
            double dLs = dR * dQ / dWo;
            double dCp = dLs / (dQ * dQ * dR * dR);

            SetProperty(ref ls, new EngrNumber(dLs));
            SetProperty(ref cp, new EngrNumber(dCp));

        }

    }
}
