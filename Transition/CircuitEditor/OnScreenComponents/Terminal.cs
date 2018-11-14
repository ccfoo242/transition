using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.Common;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Transition.CircuitEditor.OnScreenComponents
{
    public class Terminal : Grid
    {
        public delegate void TerminalDelegate(Terminal t);
        public event TerminalDelegate TerminalPositionChanged;

        private Point2D terminalPosition;
        public Point2D TerminalPosition
        {
            get => terminalPosition;
            set
            {
                if (terminalPosition == value) return;

                terminalPosition = value;
                rectTransform.X = value.X - rectangleWidth / 2;
                rectTransform.Y = value.Y - rectangleWidth / 2;
            }
        }

        private double rectangleWidth => 8;

        private Rectangle rectangle;
        private TranslateTransform rectTransform;

        public Terminal()
        {
            rectTransform = new TranslateTransform();

            rectangle = new Rectangle()
            {
                Width = rectangleWidth,
                Height = rectangleWidth,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                Visibility = Visibility.Collapsed,
                RenderTransform = rectTransform
            };

            Children.Add(rectangle);
        }

        public void highlight()
        {
            rectangle.Visibility = Visibility.Visible;
        }

        public void lowlight()
        {
            rectangle.Visibility = Visibility.Collapsed;
        }

    }

    public class WireTerminal : Terminal
    {
       
    }

}
