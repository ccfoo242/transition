using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Components;

namespace Transition.CircuitEditor.Serializable
{
    public class ResistorParameters : ParameterBase
    {
        public EngrNumber ResistanceValue { get; set; }
        public int ResistorModel { get; set; }
        public EngrNumber Ls { get; set; }
        public EngrNumber Cp { get; set; }
        public EngrNumber Ew { get; set; }
        

    }
}
