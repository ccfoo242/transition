﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;

namespace Transition.CircuitEditor.OnScreenComponents
{
    public class GroundScreen : ScreenComponentBase
    {
        public override double SchematicWidth => 80;
        public override double SchematicHeight => 80;

        public override int[,] TerminalPositions
        {
            get => new int[,] { { 40, 20 } };
        }

        public GroundScreen(Ground ground) : base(ground)
        {

        }

        public override void setPositionTextBoxes()
        {
          
        }
    }
}
