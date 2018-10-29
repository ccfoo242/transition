using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public override string ElementType => "Potentiometer";

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
            set { SetProperty(ref positionValue, value); }
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

        private ObservableCollection<TaperPoint> taperFunction ;
        public ObservableCollection<TaperPoint> TaperFunction
            { get { return taperFunction; }
              set { taperFunction = value;
                    TaperChanged?.Invoke();
                    taperFunction.CollectionChanged += TaperFunction_CollectionChanged;
            }
        }

        private void TaperFunction_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            TaperChanged?.Invoke();
        }

        /* Potentiometer allows to change the quantity
         of terminals, because it can have taps at determined points
         of its path , taps can be specified in the actual element */

        private byte quantityOfTerminals;
        public override byte QuantityOfTerminals { get { return quantityOfTerminals; }
            set
            {
                unbindElement();
                SetProperty(ref quantityOfTerminals, value);
                TerminalsChanged?.Invoke();
            } }

        public delegate void DelegateTerminalsChanged();
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

            TaperFunction = new ObservableCollection<TaperPoint>();


            TaperFunction.Add(new TaperPoint()
            {
                PositionValuePercent = 0,
                ResistanceValuePercent = 0
            });

            TaperFunction.Add(new TaperPoint()
            {
                PositionValuePercent = 50,
                ResistanceValuePercent = 25
            });

            TaperFunction.Add(new TaperPoint()
            {
                PositionValuePercent = 100,
                ResistanceValuePercent = 100
            });


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

        public override void SetProperty(string property, object value)
        {
            base.SetProperty(property, value);

            switch (property)
            {
                case "ResistanceValue"   : ResistanceValue = (EngrNumber)value; break;
                case "PositionValue"     : PositionValue = (double)value; break;
                case "ComponentPrecision": ComponentPrecision = (Precision)value; break;
                case "TapAPositionValue" : TapAPositionValue = (double)value; break;
                case "TapBPositionValue" : TapBPositionValue = (double)value; break;
                case "TapCPositionValue" : TapCPositionValue = (double)value; break;
            }
        }
    }

    public class TaperPoint
    {
        public double PositionValuePercent { get; set; }
        public double ResistanceValuePercent { get; set; }
    }
}
