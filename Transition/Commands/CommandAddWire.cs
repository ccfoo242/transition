﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor;
using Easycoustics.Transition.CircuitEditor.Serializable;

namespace Easycoustics.Transition.Commands
{
    public class CommandAddWire : ICircuitCommand
    {
        public string Title => "Add Wire Command: " + Wire.ToString();

        public SerializableWire Wire { get; set; }

        public override string ToString() => Title;

        public void execute()
        {
            CircuitEditor.CircuitEditor.currentInstance.CurrentDesign.Wires.Add(Wire);
        }

        public void unExecute()
        {
            CircuitEditor.CircuitEditor.currentInstance.CurrentDesign.Wires.Remove(Wire);
        }


    }
}
