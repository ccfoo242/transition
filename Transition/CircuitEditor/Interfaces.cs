
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Easycoustics.Transition.Common;
using Easycoustics.Transition.Functions;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Easycoustics.Transition.CircuitEditor
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
        /* some elements can exhibit multiple passive impedances on different terminal pairs
         for example potentiometer and switch */

        List<Tuple<byte, byte, ComplexDecimal>> getImpedance(decimal frequency); /* not angular frequency */    }

    public interface IVoltageCurrentOutput
    {
        SampledFunction resultVoltageCurve { get; set; } 
        SampledFunction resultCurrentCurve { get; set; }

        bool OutputVoltageAcross { get; set; }
        bool OutputCurrentThrough { get; set; }

        ComplexDecimal getImpedance(decimal frequency);
    }

    public interface IVoltageSource
    {
        byte PositiveTerminal { get; }
        byte NegativeTerminal { get; }
    }

    public interface IDependentVoltageSource
    {
        ComplexDecimal getTransferFunction(decimal frequency);
        byte PositiveReferenceVoltageTerminal { get; }
        byte NegativeReferenceVoltageTerminal { get; }

    }

}