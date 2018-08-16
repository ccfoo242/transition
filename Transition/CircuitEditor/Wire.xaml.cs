using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Transition.CircuitEditor
{
    public sealed partial class Wire : UserControl, IElectricElement
    {
        public double distXPoint1;
        public double distYPoint1;
        public double distXPoint2;
        public double distYPoint2;

        public double origPointX1;
        public double origPointY1;
        public double origPointX2;
        public double origPointY2;
        
        public CircuitEditor ce;

        public Wire()
        {
            this.InitializeComponent();
        }

        public Wire(CircuitEditor ce)
        {
            this.InitializeComponent();
            this.ce = ce;
        }

        private void tapWire(object sender, TappedRoutedEventArgs e)
        {
            ce.clickElement = this;
        }

        private void line_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                var prop = e.GetCurrentPoint(this).Properties;
                if (prop.IsRightButtonPressed) return;
                if (prop.IsMiddleButtonPressed) return;
            }

            ce.clickElement = this;
        }

        private void rect1Pressed(object sender, PointerRoutedEventArgs e)
        {
            ce.manipulatingPnt1 = this;
        }
        
        private void rect2Pressed(object sender, PointerRoutedEventArgs e)
        {
            ce.manipulatingPnt2 = this;
        }

        public Point getPnt1()
        {
            return new Point(line.X1, line.Y1);
        }

        public Point getPnt2()
        {
            return new Point(line.X2, line.Y2);
        }

        public void setPnt(Point pnt1, Point pnt2)
        {
            setPnt1(pnt1);
            setPnt2(pnt2);
        }

        public void setPnt1(Point pnt1)
        {
            line.X1 = pnt1.X;
            line.Y1 = pnt1.Y;

            Canvas.SetLeft(rect1, pnt1.X - 5);
            Canvas.SetTop(rect1, pnt1.Y - 5);
        }

        public void setPnt2(Point pnt2)
        {
            line.X2 = pnt2.X;
            line.Y2 = pnt2.Y;

            Canvas.SetLeft(rect2, pnt2.X - 5);
            Canvas.SetTop(rect2, pnt2.Y - 5);
        }

        public bool isInside(Rectangle rect)
        {
            if (rect == null) return false;

            double x1 = Canvas.GetLeft(rect);
            double y1 = Canvas.GetTop(rect);
            double x2 = Canvas.GetLeft(rect) + rect.Width;
            double y2 = Canvas.GetTop(rect) + rect.Height;

            return (x1 < line.X1) && (x1 < line.X2) &&
                   (x2 > line.X2) && (x2 > line.X1) &&
                   (y1 < line.Y1) && (y1 < line.Y2) &&
                   (y2 > line.Y2) && (y2 > line.Y1);

        }

        public void selected()
        {
            rect1.Visibility = Visibility.Visible;
            rect2.Visibility = Visibility.Visible;
            updateOriginPoint();
        }

        public void deselected()
        {
            rect1.Visibility = Visibility.Collapsed;
            rect2.Visibility = Visibility.Collapsed;
        }

        public void moveRelative(Point point)
        {
            line.X1 = Statics.round20(origPointX1 - point.X);
            line.Y1 = Statics.round20(origPointY1 - point.Y);
            line.X2 = Statics.round20(origPointX2 - point.X);
            line.Y2 = Statics.round20(origPointY2 - point.Y);

            Canvas.SetLeft(rect1, line.X1 - 5);
            Canvas.SetTop(rect1, line.Y1 - 5);

            Canvas.SetLeft(rect2, line.X2 - 5);
            Canvas.SetTop(rect2, line.Y2 - 5);

        }

        public void updateOriginPoint()
        {
            origPointX1 = line.X1;
            origPointY1 = line.Y1;
            origPointX2 = line.X2;
            origPointY2 = line.Y2;
        }
    }
}
