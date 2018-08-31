using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;

namespace Transition.CircuitEditor.Serializable
{
    public class ResistorParameters : SerializableComponent
    {

        private EngrNumber resistanceValue;
        public EngrNumber ResistanceValue
        {
            get { return resistanceValue; }
            set { SetProperty(ref resistanceValue, value); }
        }

        private int resistorModel;
        public int ResistorModel
        {
            get { return resistorModel; }
            set { SetProperty(ref resistorModel, value); }
        }

        private EngrNumber ls;
        public EngrNumber Ls
        {
            get { return ls; }
            set { SetProperty(ref ls, value); }
        }

        private EngrNumber cp;
        public EngrNumber Cp
        {
            get { return cp; }
            set { SetProperty(ref cp,  value); }
        }

        private EngrNumber ew;
        public EngrNumber Ew
        {
            get { return ew; }
            set { SetProperty(ref ew, value); }
        }

      
    }
}
