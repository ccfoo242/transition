using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor.Components;
using Easycoustics.Transition.CircuitEditor.OnScreenComponents;
using Easycoustics.Transition.Common;
using Easycoustics.Transition.Functions;

namespace Easycoustics.Transition.CircuitEditor.Serializable
{
    public class Inductor : SerializableComponent, IPassive, IVoltageCurrentOutput
    {
        public override string ElementLetter => "L";
        public override string ElementType => "Inductor";

        private bool outputVoltageAcross;
        private bool outputCurrentThrough;

        public bool OutputVoltageAcross
        {
            get => outputVoltageAcross;
            set { SetProperty(ref outputVoltageAcross, value); raiseLayoutChanged(); }
        }

        public bool OutputCurrentThrough
        {
            get => outputCurrentThrough;
            set { SetProperty(ref outputCurrentThrough, value); raiseLayoutChanged(); }
        }

        public SampledFunction resultVoltageCurve { get; set; } = new SampledFunction() { FunctionQuantity = "Voltage", FunctionUnit = "Volt" };
        public SampledFunction resultCurrentCurve { get; set; } = new SampledFunction() { FunctionQuantity = "Current", FunctionUnit = "Amper" };

        private decimal inductorValue;
        public decimal InductorValue
        {
            get => inductorValue; 
            set { SetProperty(ref inductorValue, value); calculateFoQ(); OnPropertyChanged("ValueString"); }
        }

        private int inductorModel;
        public int InductorModel
        {
            get => inductorModel; 
            set { SetProperty(ref inductorModel, value); }
        }

        private Precision componentPrecision;
        public Precision ComponentPrecision
        {
            get => componentPrecision; 
            set { SetProperty(ref componentPrecision, value); }
        }

        private decimal rs;
        public decimal Rs
        {
            get => rs; 
            set { SetProperty(ref rs, value); calculateFoQ(); }
        }

        private decimal cp;
        public decimal Cp
        {
            get => cp; 
            set { SetProperty(ref cp, value); calculateFoQ(); }
        }

        private decimal ew;
        public decimal Ew
        {
            get => ew; 
            set { SetProperty(ref ew, value); }
        }

        private decimal fo;
        public decimal Fo
        {
            get => fo; 
            set { SetProperty(ref fo, value); calculateRsCp(); }
        }

        private decimal q;
        public decimal Q
        {
            get => q; 
            set { SetProperty(ref q, value); calculateRsCp(); }
        }

        public bool AnyPrecisionSelected { get { return (ComponentPrecision == Precision.Arbitrary); } }

        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        public Inductor()
        {
            InductorValue = 1m;
            InductorModel = 0;
            
            SetProperty(ref rs, 1e-12m, "Rs");
            SetProperty(ref cp, 1e-12m, "Cp");
            calculateFoQ();

            ParametersControl = new InductorParametersControl(this);
            OnScreenElement = new InductorScreen(this);

        }

        private void calculateFoQ()
        {
            if (Rs == 0m) return;
            if (Cp == 0m) return;

            decimal dWop = DecimalMath.Sqrt(1m / (InductorValue * Cp));

            decimal dQ = (dWop * InductorValue) / Rs;
            decimal dFo = dWop / (2 * DecimalMath.Pi);

            SetProperty(ref fo, dFo, "Fo");
            SetProperty(ref q, dQ, "Q");
            
        }

        private void calculateRsCp()
        {
            decimal dWo = 2m * DecimalMath.Pi * Fo;

            decimal dRs = (dWo * inductorValue) / Q;
            decimal dCp = 1 / (inductorValue * dWo * dWo);

            SetProperty(ref rs, dRs, "Rs");
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
                    returnString = (string)conv.Convert(InductorValue, typeof(string), null, "");
                else
                    returnString = (string)convShort.Convert(InductorValue, typeof(string), null, "");

                return returnString + "H";
            }
        }

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "InductorValue": InductorValue = (decimal)value; break;
                case "InductorModel": InductorModel = (int)value; break;
                case "ComponentPrecision": ComponentPrecision = (Precision)value; break;
                case "Rs": Rs = (decimal)value; break;
                case "Cp": Cp = (decimal)value; break;
                case "Fo": Fo = (decimal)value; break;
                case "Q": Q = (decimal)value; break;
                case "Ew": Ew = (decimal)value; break;
                case "OutputVoltageAcross": OutputVoltageAcross = (bool)value; break;
                case "OutputCurrentThrough": OutputCurrentThrough = (bool)value; break;
            }
            
            resultVoltageCurve.Title = "Voltage Across Inductor " + ElementName;
            resultCurrentCurve.Title = "Current Through Inductor " + ElementName;
        }

        public ComplexDecimal getImpedance(decimal frequency)
        {
            var w = 2m * DecimalMath.Pi * frequency;

            var ZCp = -1 * ComplexDecimal.ImaginaryOne / (w * Cp);
            var ZL = ComplexDecimal.ImaginaryOne * w * InductorValue;

            switch (InductorModel)
            {
                case 0: return ZL;
                case 1: return (Rs + ZL) | ZCp;
                case 2: return (InductorValue * DecimalMath.Power(w, Ew)) * ComplexDecimal.ImaginaryOne * w;
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
