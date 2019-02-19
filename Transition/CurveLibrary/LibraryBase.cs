using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easycoustics.Transition.CurveLibrary
{
    public abstract class LibraryBase
    {
        public abstract string Title { get; }
        public abstract ObservableCollection<LibraryBase> Children { get; }

        public abstract void submitCurvesChange();
    }
}
