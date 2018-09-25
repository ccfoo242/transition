using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Transition.CircuitEditor.OnScreenComponents
{
    public class WireTerminal : ScreenElementBase
    {
        public override double PositionX => TerminalNumber == 0 ? wireScreen.X1 : wireScreen.X2;
        public override double PositionY => TerminalNumber == 0 ? wireScreen.Y1 : wireScreen.Y2;

        public override double RadiusClick => 10;

        public byte TerminalNumber { get; }

        public override byte QuantityOfTerminals => 1;
        
        public Wire SerializableWire => wireScreen.wire;
        public override SerializableElement Serializable => SerializableWire;

        public WireScreen wireScreen;
        public Rectangle rectangle;

        public WireTerminal(WireScreen wireScreen, byte terminalNumber)
        {
            this.TerminalNumber = terminalNumber;
            this.wireScreen = wireScreen;
            /* this rectangle is for selection, it appears as long
             the extreme of the wire remains selected */
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
                Path = new PropertyPath("X" + (terminalNumber + 1).ToString()),
                Mode = BindingMode.OneWay
            };
            SetBinding(Canvas.LeftProperty, bPX1);

            Binding bPY1 = new Binding()
            {
                Path = new PropertyPath("Y" + (terminalNumber + 1).ToString()),
                Mode = BindingMode.OneWay
            };
            SetBinding(Canvas.TopProperty, bPY1);

            /* this other rectangle is for highlighting the extreme
              when the user makes a bind to other wire*/
            terminalsRectangles = new Dictionary<int, Rectangle>();

            Rectangle highlightRect;
          
            highlightRect = new Rectangle()
            {
                Width = 12,
                Height = 12,
                Fill = new SolidColorBrush(Colors.Transparent),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                Visibility = Visibility.Collapsed,
                Tag = 0,
                RenderTransform = new TranslateTransform() { X = -6, Y = -6 }
            };
            Children.Add(highlightRect);
            terminalsRectangles.Add(0, highlightRect);
        }
        
        public override double getDistance(double pointX, double pointY)
        {
            return Math.Sqrt(Math.Pow(PositionX - pointX, 2) + Math.Pow(PositionY - pointY, 2));
        }

        public override void moveRelative(double pointX, double pointY)
        {
            if (TerminalNumber == 0)
            {
                wireScreen.wire.X0 = originalPositionX - pointX;
                wireScreen.wire.Y0 = originalPositionY - pointY;
            }
            else
            {
                wireScreen.wire.X1 = originalPositionX - pointX;
                wireScreen.wire.Y1 = originalPositionY - pointY;
            }
        }

        public override void moveAbsolute(double pointX, double pointY)
        {
            if (TerminalNumber == 0)
            {
                wireScreen.wire.X0 = pointX;
                wireScreen.wire.Y0 = pointY;
            }
            else
            {
                wireScreen.wire.X1 = pointX;
                wireScreen.wire.Y1 = pointY;
            }
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

        public override Point getAbsoluteTerminalPosition(byte terminal)
        {
            return new Point(PositionX, PositionY) ;
        }

        public override double getDistance(byte terminal, double pointX, double pointY)
        {
            return getDistance(pointX, pointY);
        }

        public override void highlightTerminal(byte terminal)
        {
            terminalsRectangles[0].Visibility = Visibility.Visible;
        }

        public override void lowlightTerminal(byte terminal)
        {
            terminalsRectangles[0].Visibility = Visibility.Collapsed;
        }

        public override void lowlightAllTerminals()
        {
            terminalsRectangles[0].Visibility = Visibility.Collapsed;
        }
    }
    
}
