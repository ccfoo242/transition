using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor;
using Transition.CircuitEditor.Serializable;

namespace Transition.Commands
{
    class CommandMoveComponent : ICircuitCommand
    {
        public string Title => "Move component " + Component.ToString() + " from Position " + 
              OldPositionX.ToString() + " , " + OldPositionY.ToString() + " to position " +
              NewPositionX.ToString() + " , " + NewPositionY.ToString() ;

        public override string ToString() => Title;

        public double OldPositionX { get; set; }
        public double OldPositionY { get; set; }
        
        public double NewPositionX { get; set; }
        public double NewPositionY { get; set; }

        public SerializableComponent Component { get; set; }

        public void execute()
        {
            throw new NotImplementedException();
        }

        public void unExecute()
        {
            throw new NotImplementedException();
        }

    }
}
