
using System;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Transition.CircuitEditor
{ 
    public interface IElectricElement
    {
        // an element can be a Wire or Component
        void selected();
        void deselected();

        void moveRelative(Point point);

        bool isInside(Rectangle rect);

        void updateOriginPoint();

    }
    

    public interface IComponentParameterControl
    {
        String ComponentName { get; set; }
        String ComponentLetter { get; }

        Canvas CnvLabels { get; set; }
        void setRotation(double rotation);
        void setFlipX(bool flip);
        void setFlipY(bool flip);

        event PropertyChangedEventHandler PropertyChanged;
    }
}