using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easycoustics.Transition.Common
{
    public class ObservableStack<T> : ObservableCollection<T>
    {
        public bool IsEmpty => (Count == 0);
        public event NotifyCollectionChangedEventHandler StackChanged;

        public T Peek()
        {
            return this.Last();
        }

        public T Pop()
        {
            var removedItem = Peek();

            Remove(removedItem);

            return removedItem;
        }

        public void Push(T obj)
        {
            Add(obj);
        }

       public ObservableStack() : base()
       {
            CollectionChanged += handleCollectionChanged;
       }

       private void handleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
       {
            StackChanged?.Invoke(this, e);
       }
    }
}
