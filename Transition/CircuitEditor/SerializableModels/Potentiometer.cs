using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;

namespace Transition.CircuitEditor.SerializableModels
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
            }
        }
        
        private double positionValue;
        public double PositionValue
        {
            get { return positionValue; }
            set
            {
                positionValue = value;
                SetProperty(ref positionValue, value);
            }
        }

        public override int QuantityOfTerminals { get; set; }

        public Potentiometer() : base()
        {
            // potentiometer always start as the simplest form, 3 terminal
            // later can be changed by the user to 4, 5 or 6 terminal (
            // added terminals are mid-point connections

            QuantityOfTerminals = 3;

        }
    }
}
