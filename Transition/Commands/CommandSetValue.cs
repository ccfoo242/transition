using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor;
using Easycoustics.Transition.CircuitEditor.Serializable;

namespace Easycoustics.Transition.Commands
{
    public class CommandSetValue : ICircuitCommand
    {
        public SerializableComponent Component { get; set; }

        public object OldValue { get; set; }
        public object NewValue { get; set; }

        public string Property { get; set; }


        public string Title => "Change " + Property + " from " + OldValue.ToString() + " to " + NewValue.ToString();


        public void execute()
        {
            //this method is extremely complex
            Component.SetProperty(Property, NewValue);
            System.Diagnostics.Debug.WriteLine("EXECUTE COMMAND!");
            System.Diagnostics.Debug.WriteLine(ToString());
        }

        public void unExecute()
        {
            Component.SetProperty(Property, OldValue);
            System.Diagnostics.Debug.WriteLine("UN-EXECUTE COMMAND!");
            System.Diagnostics.Debug.WriteLine(ToString());
        }

        public override string ToString()
        {
            string output = "";
            output += "Command Set Value: " + Environment.NewLine;
            output += "Component: " + Component.ToString() + Environment.NewLine;
            output += "Property: " + Property + Environment.NewLine;
            output += "Old Value: " + OldValue.ToString() + Environment.NewLine;
            output += "New Value: " + NewValue.ToString() + Environment.NewLine;

            return output;
        }

    }
}
