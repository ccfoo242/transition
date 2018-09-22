using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Transition.CircuitEditor.OnScreenComponents
{
    public class WireScreen : Canvas
    {
        private Line line { get; }

        public WireScreen(Wire wire) : base()
        {
            line = new Line();
            line.DataContext = wire;
            line.StrokeThickness = 2;
            line.Stroke = new SolidColorBrush(Colors.Black);

            Binding bX1 = new Binding()
            {
                Path = new PropertyPath("X1"),
                Mode = BindingMode.OneWay
            };
            line.SetBinding(Line.X1Property, bX1);

            Binding bY1 = new Binding()
            {
                Path = new PropertyPath("Y1"),
                Mode = BindingMode.OneWay
            };
            line.SetBinding(Line.Y1Property, bY1);

            Binding bX2 = new Binding()
            {
                Path = new PropertyPath("X2"),
                Mode = BindingMode.OneWay
            };
            line.SetBinding(Line.X2Property, bX2);

            Binding bY2 = new Binding()
            {
                Path = new PropertyPath("Y2"),
                Mode = BindingMode.OneWay
            };
            line.SetBinding(Line.Y2Property, bY2);
            
            Children.Add(line);
        }

    }
}
