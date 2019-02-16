using Easycoustics.Transition.Functions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easycoustics.Transition.CurveLibrary
{
    public class LibraryFolder : LibraryBase
    {
        public ObservableCollection<LibraryBase> Items { get; set; } = new ObservableCollection<LibraryBase>();

        public LibraryFolder(string name)
        {
            this.Name = name;
            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add: break;
                case NotifyCollectionChangedAction.Remove: break;
                case NotifyCollectionChangedAction.Reset: break;
            }
        }

        public void AddIfNotAdded(Function func)
        {
            if (IsAdded(func)) return;
            
            Items.Add(new LibraryItem() { Curve = func });
        }

        private bool IsAdded(Function func)
        {
            foreach (var item in Items.OfType<LibraryItem>())
                if (func == item.Curve) return true;

            return false;
        }

    }
}
