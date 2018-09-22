using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;
using Transition.CircuitEditor.OnScreenComponents;

namespace Transition.CircuitEditor.Serializable
{
    public class Potentiometer : SerializableComponent
    {
        public override string ElementLetter => "P";
        
        private EngrNumber resistanceValue;
        public EngrNumber ResistanceValue
        {
            get { return resistanceValue; }
            set
            {
                SetProperty(ref resistanceValue, value);
                OnPropertyChanged("ResistanceString");
            }
        }
        
        private double positionValue;
        public double PositionValue
        {
            get { return positionValue; }
            set
            {
                SetProperty(ref positionValue, value);
            }
        }

        private Precision componentPrecision;
        public Precision ComponentPrecision
        {
            get { return componentPrecision; }
            set { SetProperty(ref componentPrecision, value);
                OnPropertyChanged("ResistanceString"); }
        }

        public bool AnyPrecisionSelected { get { return (ComponentPrecision == Precision.Arbitrary); } }

        public override int QuantityOfTerminals { get; set; }

        public Potentiometer() : base()
        {
            // potentiometer always start as the simplest form, 3 terminal
            // later can be changed by the user to 4, 5 or 6 terminal (
            // added terminals are mid-point connections

            QuantityOfTerminals = 3;

            ResistanceValue = 1;
            PositionValue = 50;

            OnScreenComponent = new PotentiometerScreen(this);
            ParametersControl = new PotentiometerParametersControl(this);
            
        }

        public string ResistanceString
        {
            get
            {
                // this one sets de String for the component in the schematic window
                string returnString;
                EngrConverter conv = new EngrConverter() { ShortString = false };
                EngrConverter convShort = new EngrConverter() { ShortString = true };

                if (AnyPrecisionSelected)
                    returnString = (string)conv.Convert(ResistanceValue, typeof(string), null, "");
                else
                    returnString = (string)convShort.Convert(ResistanceValue, typeof(string), null, "");

                return returnString + "Ω";
            }
        }


    }
}
