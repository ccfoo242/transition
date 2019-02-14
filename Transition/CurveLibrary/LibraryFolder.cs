using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easycoustics.Transition.CurveLibrary
{
    public class LibraryFolder : LibraryBase
    {
        public ObservableCollection<LibraryBase> items { get; set; }

        public LibraryFolder(string name)
        {
            this.Name = name;
            items = new ObservableCollection<LibraryBase>();
        }


    }
}
