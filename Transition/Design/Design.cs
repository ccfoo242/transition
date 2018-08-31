using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;

namespace Transition.Design
{
    public class Design
    {
        public static Design currentInstance;

        public ObservableCollection<SerializableComponent> elements;

        private Design()
        {
            elements = new ObservableCollection<SerializableComponent>();
        }
        
    }
}
