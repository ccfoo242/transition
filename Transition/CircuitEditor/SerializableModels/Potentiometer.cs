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
        public override string ComponentLetter => "P";


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
    }
}
