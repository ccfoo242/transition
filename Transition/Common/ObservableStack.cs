using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transition.Common
{
    public class ObservableStack<T> : ObservableCollection<T>
    {
        public bool IsEmpty => (Count == 0);

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

    }
}
