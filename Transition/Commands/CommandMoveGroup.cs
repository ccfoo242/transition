using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor;
using Transition.Common;

namespace Transition.Commands
{
    public class CommandMoveGroup : ICircuitCommand
    {
        public string Title => "Move Group of selectable elements";

        public List<ICircuitSelectable> Elements { get; set; } = new List<ICircuitSelectable>();
        public Point2D DistanceVector { get; set; }

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
