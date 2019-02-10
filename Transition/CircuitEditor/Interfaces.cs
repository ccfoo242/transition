
using System;
using System.ComponentModel;
using Transition.Common;
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

    public interface ICircuitSelectable
    {
        void selected();
        void deselected(); 
        bool isInside(Rectangle rect);
        bool isClicked(Point2D point);
    }

    public interface ICircuitMovable
    {
        void moveRelative(Point2D vector);
        void moveRelativeCommand(Point2D vector);
    }

    public interface IPassive
    {
        ComplexDecimal getImpedance(decimal frequency); /* not angular frequency */
       
    }


}