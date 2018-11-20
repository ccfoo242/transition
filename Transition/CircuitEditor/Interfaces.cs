﻿
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
        void deselected(); /* deselect? ask Oxford American Dictionary guys */
        bool isInside(Rectangle rect);
        bool isClicked(Point2D point);
    }

    public interface ICircuitMovable
    {
        void moveRelative(Point2D vector);

    }

}