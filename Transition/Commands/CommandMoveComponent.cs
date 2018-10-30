using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.CircuitEditor.Serializable;

namespace Transition.Commands
{
    public class CommandMoveElement : ICircuitCommand
    {
        public string Title => "Move component " + Element.ToString() + " from Position " + 
              OldPositionX.ToString() + " , " + OldPositionY.ToString() + " to position " +
              NewPositionX.ToString() + " , " + NewPositionY.ToString() ;

        public override string ToString() => Title;

        public double OldPositionX { get; set; }
        public double OldPositionY { get; set; }
        
        public double NewPositionX { get; set; }
        public double NewPositionY { get; set; }

        public ScreenElementBase Element { get; set; }

        public void execute()
        {
            Element.moveAbsolute(NewPositionX, NewPositionY);
            Element.updateOriginalPosition();
        }

        public void unExecute()
        {
            Element.moveAbsolute(OldPositionX, OldPositionX);
            Element.updateOriginalPosition();
        }

    }
}
