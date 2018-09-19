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

        public delegate void ElementDelegate(UserDesign sender, SerializableElement element);
        public event ElementDelegate ElementAdded;
        public event ElementDelegate ElementRemoved;


        public bool SnapToGrid { get; set; }

        public UserDesign()
        {
            elements = new ObservableCollection<SerializableElement>();
            elements.CollectionChanged += elementCollectionChanged;

            visibleElements = new ObservableCollection<ScreenComponentBase>();
        }

        public void elementCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
           // visibleElements.Clear();
           
            foreach (SerializableElement element in elements)
                if (!visibleElements.Contains(element.OnScreenComponent))
                    visibleElements.Add(element.OnScreenComponent);

            List<ScreenComponentBase> toDelete = new List<ScreenComponentBase>();
            foreach (ScreenComponentBase element in visibleElements)
                if (!elements.Contains(element.SerializableComponent))
                    toDelete.Add(element);

            foreach (ScreenComponentBase delete in toDelete)
                visibleElements.Remove(delete);
            
        }

        public void addElement(SerializableElement element)
        {
            elements.Add(element);
            ElementAdded?.Invoke(this, element);
        }

        public void removeElement(SerializableElement element)
        {
            if (elements.Contains(element))
            {
                elements.Remove(element);
                ElementRemoved?.Invoke(this, element);
            }
        }
    }
}
