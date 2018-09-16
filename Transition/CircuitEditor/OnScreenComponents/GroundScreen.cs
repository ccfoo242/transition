using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.SerializableModels;

namespace Transition.CircuitEditor.OnScreenComponents
{
    public class GroundScreen : ScreenComponentBase
    {
        public override double SchematicWidth => 80;
        public override double SchematicHeight => 80;

        public GroundScreen(Ground ground) : base(ground)
        {

        }

        public override void setPositionTextBoxes()
        {
          
        }
    }
}
