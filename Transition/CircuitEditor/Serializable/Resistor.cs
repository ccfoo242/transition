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
        public override string ElementType => "Resistor";

        private EngrNumber resistorValue;
        public EngrNumber ResistorValue
        {
            get { return resistorValue; }
            set { SetProperty(ref resistorValue, value, "ResistorValue");
                  calculateFoQ();
                  OnPropertyChanged("ValueString");
            }
        }

        private int resistorModel;  //0=ideal 1=parasitic 2=exponential
        public int ResistorModel
        {
            get { return resistorModel; }
            set { SetProperty(ref resistorModel, value, "ResistorModel"); }
        }

        private Precision componentPrecision;
        public Precision ComponentPrecision
        {
            get { return componentPrecision; }
            set { SetProperty(ref componentPrecision, value, "ComponentPrecision"); }
        }

        private EngrNumber ls;
        public EngrNumber Ls
        {
            get { return ls; }
            set { SetProperty(ref ls, value,"Ls");
                  calculateFoQ(); }
        }

        private EngrNumber cp;
        public EngrNumber Cp
        {
            get { return cp; }
            set { SetProperty(ref cp, value, "Cp");
                  calculateFoQ(); }
        }

        private EngrNumber ew;
        public EngrNumber Ew
        {
            get { return ew; }
            set { SetProperty(ref ew, value, "Ew"); }
        }

        private EngrNumber fo;
        public EngrNumber Fo
        {
            get { return fo;}
            set { SetProperty(ref fo, value, "Fo");
                  calculateLsCp(); }
        }

        private EngrNumber q;
        public EngrNumber Q
        {
            get { return q; }
            set { SetProperty(ref q, value, "Q");
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
            OnScreenElement = new OnScreenComponents.ResistorScreen(this);
        }

        private void calculateFoQ()
        {
            if (ResistorValue.ValueDouble == 0) return;
            if (ls.ValueDouble == 0) return;
            if (cp.ValueDouble == 0) return;

            double dR = ResistorValue.ValueDouble;
            double dLs = Ls.ValueDouble;
            double dCp = Cp.ValueDouble;
            
            double dWop = Math.Sqrt(1 / (dLs * dCp));
            
            double dQ = (dWop * dLs) / dR;
            double dFo = dWop / (2 * Math.PI);

            SetProperty(ref fo, new EngrNumber(dFo), "Fo");
            SetProperty(ref q, new EngrNumber(dQ), "Q");
            SetProperty(ref ls, Ls, "Ls");
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

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "ResistorValue":       ResistorValue = (EngrNumber)value; break;
                case "ResistorModel":       ResistorModel = (int)value; break;
                case "ComponentPrecision":  ComponentPrecision = (Precision)value; break;
                case "Ls":                  Ls = (EngrNumber)value; break;
                case "Cp":                  Cp = (EngrNumber)value; break;
                case "Fo":                  Fo = (EngrNumber)value; break;
                case "Q":                   Q = (EngrNumber)value; break;
                case "Ew":                  Ew = (EngrNumber)value; break;
            }
        }
    }
}
