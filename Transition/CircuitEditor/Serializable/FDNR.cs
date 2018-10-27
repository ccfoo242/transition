using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;
using Transition.CircuitEditor.OnScreenComponents;

namespace Transition.CircuitEditor.Serializable
{
    public class FDNR : SerializableComponent
    {
        public override string ElementLetter => "D";
        
        private EngrNumber fdnrValue;
        public EngrNumber FdnrValue
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
            FdnrValue = EngrNumber.One;
            
            ParametersControl = new FDNRParametersControl(this);
            OnScreenComponent = new FDNRScreen(this);
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
                case "FdnrValue": FdnrValue = (EngrNumber)value; break;
                case "ComponentPrecision": ComponentPrecision = (Precision)value; break;
           
            }
        }
    }
}
