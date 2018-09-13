﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;

namespace Transition.CircuitEditor.SerializableModels
{
    public class FDNR : SerializableComponent
    {
        public override string ComponentLetter => "D";
        
        private EngrNumber fdnrValue;
        public EngrNumber FdnrValue
        {
            get { return fdnrValue; }
            set { SetProperty(ref fdnrValue, value); }
        }
    }
}
