using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.Common;

namespace Transition.CircuitEditor.Serializable
{
    public class FDNR : SerializableComponent, IPassive
    {
        public override string ElementLetter => "D";
        public override string ElementType => "Frequency Dependent Negative Resistor";

        private decimal fdnrValue;
        public decimal FdnrValue
        {
            get { return fdnrValue; }
            set { SetProperty(ref fdnrValue, value);
                OnPropertyChanged("ValueString");
            }
        }

        private Precision componentPrecision;
        public Precision ComponentPrecision
        {
            get { return componentPrecision; }
            set
            {
                SetProperty(ref componentPrecision, value);
                OnPropertyChanged("ValueString");
            }
        }

        public bool AnyPrecisionSelected { get { return (ComponentPrecision == Precision.Arbitrary); } }

        public override byte QuantityOfTerminals { get => 2; set => throw new NotImplementedException(); }

        public FDNR() : base()
        {
            FdnrValue = 1m;
            
            ParametersControl = new FDNRParametersControl(this);
            OnScreenElement = new FDNRScreen(this);
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
                    returnString = (string)conv.Convert(FdnrValue, typeof(string), null, "");
                else
                    returnString = (string)convShort.Convert(FdnrValue, typeof(string), null, "");

                return returnString + "F²";
            }
        }

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "FdnrValue": FdnrValue = (decimal)value; break;
                case "ComponentPrecision": ComponentPrecision = (Precision)value; break;
           
            }
        }

        public ComplexDecimal getImpedance(decimal frequency)
        {
            decimal w = 2 * DecimalMath.Pi * frequency;
            return -1 / (w * w * FdnrValue);
        }
    }
}
