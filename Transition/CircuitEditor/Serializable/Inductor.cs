using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;
using Transition.CircuitEditor.OnScreenComponents;

namespace Transition.CircuitEditor.Serializable
{
    public class Inductor : SerializableComponent
    {
        public override string ElementLetter => "L";
        public override string ElementType => "Inductor";

        private EngrNumber inductorValue;
        public EngrNumber InductorValue
        {
            get { return inductorValue; }
            set { SetProperty(ref inductorValue, value);
                  calculateFoQ();
                  OnPropertyChanged("ValueString");
            }
        }

        private int inductorModel;
        public int InductorModel
        {
            get { return inductorModel; }
            set { SetProperty(ref inductorModel, value); }
        }

        private Precision componentPrecision;
        public Precision ComponentPrecision
        {
            get { return componentPrecision; }
            set { SetProperty(ref componentPrecision, value); }
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

        public bool AnyPrecisionSelected { get { return (ComponentPrecision == Precision.Arbitrary); } }

        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        public Inductor()
        {
            InductorValue = EngrNumber.One;
            InductorModel = 0;
            
            SetProperty(ref rs, new EngrNumber(1, "p"), "Rs");
            SetProperty(ref cp, new EngrNumber(1, "p"), "Cp");
            calculateFoQ();

            ParametersControl = new InductorParametersControl(this);
            OnScreenElement = new InductorScreen(this);

        }

        private void calculateFoQ()
        {
            if (Rs.ValueDouble == 0) return;
            if (Cp.ValueDouble == 0) return;

            double dL = inductorValue.ValueDouble;
            double dRs = Rs.ValueDouble;
            double dCp = Cp.ValueDouble;

            double dWop = Math.Sqrt(1 / (dL * dCp));

            double dQ = (dWop * dL) / dRs;
            double dFo = dWop / (2 * Math.PI);

            SetProperty(ref fo, dFo, "Fo");
            SetProperty(ref q, dQ, "Q");
            
        }

        private void calculateRsCp()
        {
            double dQ = Q.ValueDouble;
            double dFo = Fo.ValueDouble;
            double dL = inductorValue.ValueDouble;

            double dWo = 2 * Math.PI * dFo;

            double dRs = (dWo * dL) / dQ;
            double dCp = 1 / (dL * dWo * dWo);

            SetProperty(ref rs, dRs, "Rs");
            SetProperty(ref cp, dCp, "Cp");

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
                case "InductorValue": InductorValue = (EngrNumber)value; break;
                case "InductorModel": InductorModel = (int)value; break;
                case "ComponentPrecision": ComponentPrecision = (Precision)value; break;
                case "Rs": Rs = (EngrNumber)value; break;
                case "Cp": Cp = (EngrNumber)value; break;
                case "Fo": Fo = (EngrNumber)value; break;
                case "Q": Q = (EngrNumber)value; break;
                case "Ew": Ew = (EngrNumber)value; break;
            }
        }
    }
}
