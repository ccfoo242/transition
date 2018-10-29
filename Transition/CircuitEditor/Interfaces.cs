
using System;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Transition.CircuitEditor
{
    public interface ICircuitCommand
    {
        void execute();
        void unExecute();

        string Title { get; }
    }
}