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

        public override double RadiusClick => 10;

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
                StrokeThickness = 1,
                Visibility = Visibility.Collapsed,
                RenderTransform = new TranslateTransform() { X = -4, Y = -4 }
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
            wireScreen.wire.X1 = originalPositionX - pointX;
            wireScreen.wire.Y1 = originalPositionY - pointY;
        }

        public override void moveAbsolute(double positionX, double positionY)
        {
            wireScreen.wire.X1 = positionX;
            wireScreen.wire.Y1 = positionY;
        }

        public override void selected()
        {
            updateOriginalPosition();
            rectangle.Visibility = Visibility.Visible;
        }

        public override void deselected()
        {
            rectangle.Visibility = Visibility.Collapsed;
        }
        public override bool isInside(Rectangle rect)
        {
            if (rect == null) return false;

            double x1 = Canvas.GetLeft(rect);
            double y1 = Canvas.GetTop(rect);
            double x2 = Canvas.GetLeft(rect) + rect.Width;
            double y2 = Canvas.GetTop(rect) + rect.Height;

            if ((x1 < PositionX) &&
                (x2 > PositionX) &&
                (y1 < PositionY) &&
                (y2 > PositionY))
                return true;
            else
                return false;
        }
    }




        public class WireTerminal2 : ScreenElementBase
        {
            public override double PositionX => wireScreen.X2;
            public override double PositionY => wireScreen.Y2;

            public WireScreen wireScreen;
            public Rectangle rectangle;

            public override double RadiusClick => 10;

            public WireTerminal2(WireScreen wireScreen)
            {
                this.wireScreen = wireScreen;
                rectangle = new Rectangle()
                {
                    Width = 8,
                    Height = 8,
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1,
                    Visibility = Visibility.Collapsed,
                    RenderTransform = new TranslateTransform() { X = -4, Y = -4 }
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
                wireScreen.wire.X2 = originalPositionX - pointX;
                wireScreen.wire.Y2 = originalPositionY - pointY;
            }

            public override void moveAbsolute(double positionX, double positionY)
            {
                wireScreen.wire.X2 = positionX;
                wireScreen.wire.Y2 = positionY;
            }

            public override void selected()
            {
            rectangle.Visibility = Visibility.Visible;
                updateOriginalPosition();
            }

            public override void deselected()
            {
            rectangle.Visibility = Visibility.Collapsed;    
            }
            public override bool isInside(Rectangle rect)
            {
                if (rect == null) return false;

                double x1 = Canvas.GetLeft(rect);
                double y1 = Canvas.GetTop(rect);
                double x2 = Canvas.GetLeft(rect) + rect.Width;
                double y2 = Canvas.GetTop(rect) + rect.Height;

                if ((x1 < PositionX) &&
                    (x2 > PositionX) &&
                    (y1 < PositionY) &&
                    (y2 > PositionY))
                    return true;
                else
                    return false;
            }
        }
}
