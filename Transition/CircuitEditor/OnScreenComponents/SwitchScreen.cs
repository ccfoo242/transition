using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;
using Windows.UI.Xaml.Controls;

namespace Transition.CircuitEditor.OnScreenComponents
{
    public class SwitchScreen : ScreenComponentBase
    {
        public override int[,] TerminalPositions => throw new NotImplementedException();

        public override double SchematicWidth => throw new NotImplementedException();
        public override double SchematicHeight => throw new NotImplementedException();

        public ContentControl SymbolSwitch { get; set; }

        public SwitchScreen(Switch sw) : base(sw)
        {
            sw.TerminalsChanged += TerminalsChanged;
        }

        private void TerminalsChanged(byte oldQuantity, byte newQuantity)
        {
          
        }

        public override void setPositionTextBoxes(SerializableElement element)
        {
            throw new NotImplementedException();
        }
    }
}
