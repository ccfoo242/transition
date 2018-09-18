using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.OnScreenComponents;
using Transition.CircuitEditor.Serializable;
using Windows.UI.Xaml;

namespace Transition.Design
{
    public class UserDesign
    {
        public ObservableCollection<SerializableElement> elements { get; }
        public ObservableCollection<ScreenComponentBase> visibleElements { get; }

        private UserDesign()
        {
            elements = new ObservableCollection<SerializableElement>();
            elements.CollectionChanged += elementCollectionChanged;

            visibleElements = new ObservableCollection<ScreenComponentBase>();
        }

        public void elementCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            visibleElements.Clear();
            foreach (SerializableElement element in elements)
                visibleElements.Add(element.OnScreenComponent);
        }

        public void addComponent(SerializableElement element)
        {

        }
    }
}
