using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor.Components;
using Easycoustics.Transition.Common;
using static Easycoustics.Transition.CustomControls.ComponentValueBox;

namespace Easycoustics.Transition.CircuitEditor.Serializable
{
    public class Resistor : SerializableComponent, IPassive
    {
        public override string ElementLetter => "R";
        public override string ElementType => "Resistor";

        public bool OutputVoltageAcross { get; set; }
        public bool OutputCurrentThrough { get; set; }

        private decimal resistorValue;
        public decimal ResistorValue
        {
            get { return resistorValue; }
            set { SetProperty(ref resistorValue, value, "ResistorValue");
                  calculateFoQ();
                  OnPropertyChanged("ValueString");
                  raiseLayoutChanged();
            }
        }

        private int resistorModel;  //0=ideal 1=parasitic 2=exponential
        public int ResistorModel
        {
            get { return resistorModel; }
            set { SetProperty(ref resistorModel, value /*, "ResistorModel" */); }
        }

        private Precision componentPrecision;
        public Precision ComponentPrecision
        {
            get { return componentPrecision; }
            set { SetProperty(ref componentPrecision, value /*, "ComponentPrecision" */); }
        }

        private decimal ls;
        public decimal Ls
        {
            get { return ls; }
            set { SetProperty(ref ls, value,"Ls");
                  calculateFoQ(); }
        }

        private decimal cp;
        public decimal Cp
        {
            get { return cp; }
            set { SetProperty(ref cp, value, "Cp");
                  calculateFoQ(); }
        }

        private decimal ew;
        public decimal Ew
        {
            get { return ew; }
            set { SetProperty(ref ew, value, "Ew"); }
        }

        private decimal fo;
        public decimal Fo
        {
            get { return fo;}
            set { SetProperty(ref fo, value, "Fo");
                  calculateLsCp(); }
        }

        private decimal q;
        public decimal Q
        {
            get { return q; }
            set { SetProperty(ref q, value, "Q");
                  calculateLsCp(); }
        }

        public bool AnyPrecisionSelected { get { return (ComponentPrecision == Precision.Arbitrary); } }


        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        public Resistor() : base()
        {
            ResistorValue = 1m ;
            ResistorModel = 0;

            SetProperty(ref ls, 1e-12m, "Ls");
            SetProperty(ref cp, 1e-12m, "Cp");
            calculateFoQ();

            ParametersControl = new ResistorParametersControl(this);
            OnScreenElement = new OnScreenComponents.ResistorScreen(this);
        }

        private void calculateFoQ()
        {
            if (ResistorValue == 0m) return;
            if (ls == 0m) return;
            if (cp == 0m) return;
            
            var dWop = DecimalMath.Sqrt(1 / (Ls * Cp));
            
            var dQ = (dWop * Ls) / ResistorValue;
            var dFo = dWop / (2 * DecimalMath.Pi);

            SetProperty(ref fo, dFo, "Fo");
            SetProperty(ref q, dQ, "Q");
            SetProperty(ref ls, Ls, "Ls");
        }

        private void calculateLsCp()
        {
            decimal dWo = 2 * DecimalMath.Pi * Fo;

            //  double dLs = Math.Sqrt(Math.Abs( ((dQ * dQ * dR * dR) - (dR * dR)) / Math.Pow(dWo, 2) ));
            decimal dLs = ResistorValue * Q / dWo;
            decimal dCp = dLs / (Q * Q * ResistorValue * ResistorValue);

            SetProperty(ref ls, dLs, "Ls");
            SetProperty(ref cp, dCp, "Cp");

        }
        
        public string ValueString
        {
            get
            {
                // this one sets de String for the component in the schematic window
                string returnString;
                var conv = new DecimalEngrConverter() { ShortString = false };
                var convShort = new DecimalEngrConverter() { ShortString = true };

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
                case "ResistorValue":       ResistorValue = (decimal)value; break;
                case "ResistorModel":       ResistorModel = (int)value; break;
                case "ComponentPrecision":  ComponentPrecision = (Precision)value; break;
                case "Ls":                  Ls = (decimal)value; break;
                case "Cp":                  Cp = (decimal)value; break;
                case "Fo":                  Fo = (decimal)value; break;
                case "Q":                   Q = (decimal)value; break;
                case "Ew":                  Ew = (decimal)value; break;
            }
        }

        public ComplexDecimal getImpedance(decimal frequency)
        {
            var w = 2m * DecimalMath.Pi * frequency;

            var ZCp = -1 * ComplexDecimal.ImaginaryOne / (w * Cp);
            var ZLs = ComplexDecimal.ImaginaryOne * w * Ls;

            switch (ResistorModel)
            {
                case 0: return ResistorValue;
                case 1: return (ResistorValue + ZLs) | ZCp;
                case 2: return ResistorValue * DecimalMath.Power(w, Ew);
            }

            throw new NotImplementedException();
        }

        List<Tuple<byte, byte, ComplexDecimal>> IPassive.getImpedance(decimal frequency)
        {
            var output = new List<Tuple<byte, byte, ComplexDecimal>>();
            output.Add(new Tuple<byte, byte, ComplexDecimal>(0, 1, getImpedance(frequency)));

            return output;
        }
    }
}
