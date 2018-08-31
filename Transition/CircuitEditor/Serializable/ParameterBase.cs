using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.Common;

namespace Transition.CircuitEditor.Serializable
{
    public class ParameterBase : BindableBase
    {
        public int Rotation { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }

        public int PositionX { get; set; }
        public int PositionY { get; set; }
    }
}
