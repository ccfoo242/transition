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
    public class WireTerminal1 : ScreenElementBase
    {
        public override double PositionX => wireScreen.X1;
        public override double PositionY => wireScreen.Y1;

        public WireScreen wireScreen;
        public Rectangle rectangle;

        public WireTerminal1(WireScreen wireScreen)
        {
            this.wireScreen = wireScreen;
            rectangle = new Rectangle()
            {
                Width = 8,
                Height = 8,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };

            DataContext = wireScreen;
            Children.Add(rectangle);

            Binding bPX1 = new Binding()
            {
                Path = new PropertyPath("X1"),
                Mode = BindingMode.OneWay
            };
            SetBinding(Canvas.LeftProperty, bPX1);

            Binding bPY1 = new Binding()
            {
                Path = new PropertyPath("Y1"),
                Mode = BindingMode.OneWay
            };
            SetBinding(Canvas.TopProperty, bPY1);
        }

        public override double getDistance(double pointX, double pointY)
        {
            return Math.Sqrt(Math.Pow(PositionX - pointX, 2) + Math.Pow(PositionY - pointY, 2));
        }

        public override void moveRelative(double pointX, double pointY)
        {
            wireScreen.wire.X1 = pointX;
            wireScreen.wire.Y1 = pointY;
        }
        
        public override void selected()
        {
            rectangle.Stroke = new SolidColorBrush(Colors.Black);
        }

        public override void deselected()
        {
            rectangle.Stroke = new SolidColorBrush(Colors.Transparent);
        }
    }


    public class WireTerminal2 : ScreenElementBase
    {
        public override double PositionX => wireScreen.X2;
        public override double PositionY => wireScreen.Y2;

        public WireScreen wireScreen;
        public Rectangle rectangle;

        public WireTerminal2(WireScreen wireScreen)
        {
            this.wireScreen = wireScreen;
            rectangle = new Rectangle()
            {
                Width = 8,
                Height = 8,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };

            DataContext = wireScreen;
            Children.Add(rectangle);

            Binding bPX2 = new Binding()
            {
                Path = new PropertyPath("X2"),
                Mode = BindingMode.OneWay
            };
            SetBinding(Canvas.LeftProperty, bPX2);

            Binding bPY2 = new Binding()
            {
                Path = new PropertyPath("Y2"),
                Mode = BindingMode.OneWay
            };
            SetBinding(Canvas.TopProperty, bPY2);
        }

        public override double getDistance(double pointX, double pointY)
        {
            return Math.Sqrt(Math.Pow(PositionX - pointX, 2) + Math.Pow(PositionY - pointY, 2));
        }

        public override void moveRelative(double pointX, double pointY)
        {
            wireScreen.wire.X2 = pointX;
            wireScreen.wire.Y2 = pointY;
        }

        public override void selected()
        {
            rectangle.Stroke = new SolidColorBrush(Colors.Black);
        }

        public override void deselected()
        {
            rectangle.Stroke = new SolidColorBrush(Colors.Transparent);
        }
    }
}
