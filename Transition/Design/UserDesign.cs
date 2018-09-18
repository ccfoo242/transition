using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;

namespace Transition.Design
{
    public class UserDesign
    {
        public ObservableCollection<SerializableElement> elements;

        private UserDesign()
        {
            elements = new ObservableCollection<SerializableElement>();
        }

        public void addComponent(SerializableComponent component)
        {

        }
        
    }
}
