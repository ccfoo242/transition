using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor;
using Transition.CircuitEditor.Serializable;

namespace Transition.Commands
{
    public class CommandSetValue : ICircuitCommand
    {
        private SerializableComponent component;

        public object oldValue;
        public object newValue;

        public string property;

        public void execute()
        {
            //this method is extremely complex
            component.SetProperty(property, newValue);
        }

        public void unExecute()
        {
            component.SetProperty(property, oldValue);
        }
        
    }
}
