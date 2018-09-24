using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;
using static Transition.CustomControls.ComponentValueBox;

namespace Transition.CircuitEditor.Serializable
{
    public class Resistor : SerializableComponent
    {
        public override string ElementLetter => "R";

        private EngrNumber resistorValue;
        public EngrNumber ResistorValue
        {
            get { return resistorValue; }
            set { SetProperty(ref resistorValue, value);
                OnPropertyChanged("ValueString");
            }
        }

        private int resistorModel;
        public int ResistorModel
        {
            get { return resistorModel; }
            set { SetProperty(ref resistorModel, value); }
        }

        private Precision componentPrecision;
        public Precision ComponentPrecision
        {
            get { return componentPrecision; }
            set { SetProperty(ref componentPrecision, value); }
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

        public bool AnyPrecisionSelected { get { return (ComponentPrecision == Precision.Arbitrary); } }


        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        public Resistor() : base()
        {
            ResistorValue = EngrNumber.One ;
            ResistorModel = 0;

            SetProperty(ref ls, new EngrNumber(1, "p"), "Ls");
            SetProperty(ref cp, new EngrNumber(1, "p"), "Cp");
            calculateFoQ();

            ParametersControl = new ResistorParametersControl(this);
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

            SetProperty(ref fo, new EngrNumber(dFo), "Fo");
            SetProperty(ref q, new EngrNumber(dQ), "Q");

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

            SetProperty(ref ls, new EngrNumber(dLs), "Ls");
            SetProperty(ref cp, new EngrNumber(dCp), "Cp");

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
                    returnString = (string)conv.Convert(ResistorValue, typeof(string), null, "");
                else
                    returnString = (string)convShort.Convert(ResistorValue, typeof(string), null, "");

                return returnString + "Ω";
            }
        }


    }
}
