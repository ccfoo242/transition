using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor.Components;
using Easycoustics.Transition.CircuitEditor.OnScreenComponents;
using Easycoustics.Transition.Common;
using Easycoustics.Transition.Functions;

namespace Easycoustics.Transition.CircuitEditor.Serializable
{
    public class Potentiometer : SerializableComponent
    {
        public override string ElementLetter => "P";
        public override string ElementType => "Potentiometer";

        private decimal resistanceValue;
        public decimal ResistanceValue
        {
            get { return resistanceValue; }
            set
            {
                SetProperty(ref resistanceValue, value);
                OnPropertyChanged("ResistanceString");
                raiseLayoutChanged();
            }
        }
        
        private double positionValue;
        public double PositionValue
        {
            get { return positionValue; }
            set { SetProperty(ref positionValue, value); raiseLayoutChanged(); }
        }

        private double tapAPositionValue;
        public double TapAPositionValue
        {
            get { return tapAPositionValue; }
            set { SetProperty(ref tapAPositionValue, value);
                if (TapAPositionValue > TapBPositionValue)
                    TapBPositionValue = TapAPositionValue;
                if (TapAPositionValue > TapCPositionValue)
                    TapCPositionValue = TapAPositionValue;
            }
        }

        private double tapBPositionValue;
        public double TapBPositionValue
        {
            get { return tapBPositionValue; }
            set { SetProperty(ref tapBPositionValue, value);
                if (TapBPositionValue < TapAPositionValue)
                    TapAPositionValue = TapBPositionValue;

                if (TapBPositionValue > TapCPositionValue)
                    TapCPositionValue = TapBPositionValue;
            }
        }

        private double tapCPositionValue;
        public double TapCPositionValue
        {
            get { return tapCPositionValue; }
            set { SetProperty(ref tapCPositionValue, value);
                if (TapCPositionValue < TapAPositionValue)
                    TapAPositionValue = TapCPositionValue;
                if (TapCPositionValue < TapBPositionValue)
                    TapBPositionValue = TapCPositionValue;
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

        private SampledFunction taperFunction;
        public SampledFunction TaperFunction
            { get { return taperFunction; }
              set { taperFunction = value;
                    TaperChanged?.Invoke();
            }
        }
        

        /* Potentiometer allows to change the quantity
         of terminals, because it can have taps at determined points
         of its path , taps can be specified in the actual element */

        private byte quantityOfTerminals;
        public override byte QuantityOfTerminals { get { return quantityOfTerminals; }
            set
            {
                byte oldValue = quantityOfTerminals;
                SetProperty(ref quantityOfTerminals, value);
                TerminalsChanged?.Invoke(oldValue, value);

                if (value < oldValue)
                    for (byte x = oldValue; x > value; x--)
                        raiseTerminalDeleted((byte)(x - 1));

                raiseLayoutChanged();
            } }

        public delegate void DelegateTerminalsChanged(byte oldQuantity, byte newQuantity);
        public event DelegateTerminalsChanged TerminalsChanged;

        public delegate void DelegateTaperChanged();
        public event DelegateTaperChanged TaperChanged;

        public Potentiometer() : base()
        {
            // potentiometer always start as the simplest form, 3 terminal
            // later can be changed by the user to 4, 5 or 6 terminal
            // ( added terminals are tap connections )

            QuantityOfTerminals = 3;

            ResistanceValue = 1;
            PositionValue = 50;

            TaperFunction = new SampledFunction();
            TaperFunction.addSample(0, 0);
            TaperFunction.addSample(100, 100);
            
            OnScreenElement = new PotentiometerScreen(this);
            ParametersControl = new PotentiometerParametersControl(this);
        }

        public string ResistanceString
        {
            get
            {
                // this one sets the String for the component in the schematic window
                string returnString;
                var conv = new DecimalEngrConverter() { ShortString = false };
                var convShort = new DecimalEngrConverter() { ShortString = true };

                if (AnyPrecisionSelected)
                    returnString = (string)conv.Convert(ResistanceValue, typeof(string), null, "");
                else
                    returnString = (string)convShort.Convert(ResistanceValue, typeof(string), null, "");

                return returnString + "Ω";
            }
        }

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "ResistanceValue"     : ResistanceValue = (decimal)value; break;
                case "PositionValue"       : PositionValue = (double)value; break;
                case "ComponentPrecision"  : ComponentPrecision = (Precision)value; break;
                case "TapAPositionValue"   : TapAPositionValue = (double)value; break;
                case "TapBPositionValue"   : TapBPositionValue = (double)value; break;
                case "TapCPositionValue"   : TapCPositionValue = (double)value; break;
                case "TaperFunction"       : TaperFunction = (SampledFunction)value; break;
             
            }
        }
    }
    
}
