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

        private SampledFunction taperFunction; /* this function MUST be monothonic */
        public SampledFunction TaperFunction
        {
            get { return taperFunction; }
            set { taperFunction = value;
                  TaperChanged?.Invoke();
            }
        }
        
        private List<Tuple<byte, decimal>> OrderedResistances { get; set; }


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

            ResistanceValue = 1e3m;
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
                case "ResistanceValue"    : ResistanceValue    = (decimal)value;         break;
                case "PositionValue"      : PositionValue      = (double)value;          break;
                case "ComponentPrecision" : ComponentPrecision = (Precision)value;       break;
                case "TapAPositionValue"  : TapAPositionValue  = (double)value;          break;
                case "TapBPositionValue"  : TapBPositionValue  = (double)value;          break;
                case "TapCPositionValue"  : TapCPositionValue  = (double)value;          break;
                case "TaperFunction"      : TaperFunction      = (SampledFunction)value; break;
            }

            updateResistances();
            
        }

        private void updateResistances()
        {
          
            Func<double, decimal> getResistance = (position) => {

                var ResistancePercent = TaperFunction.Calculate((decimal)position);
                return ResistancePercent.RealPart / 100 * ResistanceValue;
            };

            var list = new List<Tuple<byte,decimal>>();

            list.Add(new Tuple<byte, decimal>(0, 0));
            list.Add(new Tuple<byte, decimal>(1, getResistance(100d)));
            list.Add(new Tuple<byte, decimal>(2, getResistance(PositionValue)));

            if (QuantityOfTerminals == 3) { }
            else if (QuantityOfTerminals == 4)
            {
                list.Add(new Tuple<byte, decimal>(3, getResistance(TapBPositionValue)));
            }
            else if (QuantityOfTerminals == 5)
            {
                list.Add(new Tuple<byte, decimal>(3, getResistance(TapAPositionValue)));
                list.Add(new Tuple<byte, decimal>(4, getResistance(TapCPositionValue)));
            }
            else
            { /* QuantityOfTerminals == 6 */
                list.Add(new Tuple<byte, decimal>(3, getResistance(TapAPositionValue)));
                list.Add(new Tuple<byte, decimal>(4, getResistance(TapBPositionValue)));
                list.Add(new Tuple<byte, decimal>(5, getResistance(TapCPositionValue)));
            }

            OrderedResistances = list.OrderBy(t => t.Item2).ToList();
           
        }

        public override ComplexDecimal[] GetAdmittancesForTerminal(byte terminal, decimal frequency)
        {
            var output = new ComplexDecimal[QuantityOfTerminals];

            if (terminal == 0)
            {
                var res = OrderedResistances[1];
                output[0] = 1 / res.Item2; // admittance
                output[res.Item1] = -1 / res.Item2;
            }
            else if (terminal == 1)
            {
                var resAbove = OrderedResistances[QuantityOfTerminals - 2];
                var resBelow = OrderedResistances[QuantityOfTerminals - 1];
                var resistance = resBelow.Item2 - resAbove.Item2;

                output[terminal] = (1 / resistance);
                output[resAbove.Item1] = -1 / resistance;

            } else
            {
                /* selected terminal has two resistances attached, one above and one below */

                var res = OrderedResistances.Find(tup => (tup.Item1 == terminal));
                var index = OrderedResistances.IndexOf(res);

                var resAbove = OrderedResistances[index - 1];
                var resBelow = OrderedResistances[index + 1];

                var resistance1 = res.Item2 - resAbove.Item2;
                var resistance2 = resBelow.Item2 - res.Item2;

                output[terminal] = (1 / resistance1) + (1 / resistance2);
                output[resAbove.Item1] = -1 * (1 / resistance1);
                output[resBelow.Item1] = -1 * (1 / resistance2);
            }
            
            
            return output;
        }
    }
    
}
