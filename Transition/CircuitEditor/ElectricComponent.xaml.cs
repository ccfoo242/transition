using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class ElectricComponent : UserControl, IElectricElement
    {
        public Point PressedPoint;

        public CircuitEditor ce;
        public IComponentParameter parameters { get; set; }

        public String elementName
        {
            get
            {
                if (parameters != null)
                    return parameters.ComponentName;
                else return "";
            }
        }
        
        public ElectricComponent()
        {
            this.InitializeComponent();
        }

        public ElectricComponent(String tipoElto, CircuitEditor ce)
        {
            InitializeComponent();

            this.ce = ce;
            grd.RenderTransform = new CompositeTransform();

            parameters = new Components.Resistor();

            switch (tipoElto)
            {
                case "resistor":
                    parameters = new Components.Resistor();
                    break;

                case "capacitor":
                    parameters = new Components.Capacitor();
                    break;

                case "inductor":
                    parameters = new Components.Inductor();
                    break;

                case "ground":
                    parameters = new Components.Ground();
                    break;

                case "fdnr":
                    parameters = new Components.FDNR();
                    break;

                case "potentiometer":
                    parameters = new Components.Potentiometer();
                    break;

                case "transformer":
                    parameters = new Components.Transformer();
                    break;

                case "generator":
                    parameters = new Components.VoltageSource();
                    break;

                case "impedance":
                    grd.Width = 140;
                    grd.Height = 80;
                    break;

                case "scn":
                    grd.Width = 140;
                    grd.Height = 80;
                    break;

                case "opamp":
                    grd.Width = 180;
                    grd.Height = 200;
                    break;

                case "transferfunction":
                    grd.Width = 180;
                    grd.Height = 200;
                    break;

                case "summer":
                    grd.Width = 160;
                    grd.Height = 160;
                    break;

                case "buffer":
                    grd.Width = 140;
                    grd.Height = 120;
                    break;

                case "speaker":
                    grd.Width = 120;
                    grd.Height = 140;
                    break;

                default:
                    break;
            }

            cont.ContentTemplate = (DataTemplate)((UserControl)parameters).Resources["SchematicComponentLayout"];
            cont.DataContext = parameters;

            this.Height = parameters.SchematicHeight;
            this.Width = parameters.SchematicWidth;
            grd.Height = this.Height;
            grd.Width = this.Width;

            CompositeTransform t = (CompositeTransform)grd.RenderTransform;
            t.CenterX = Statics.round20(this.Width / 2);
            t.CenterY = Statics.round20(this.Height / 2);

            parameters.ComponentName = parameters.ComponentLetter + ce.getNextNumberLetter(parameters.ComponentLetter);
            labelsgrd.Children.Add(parameters.CnvLabels);
        }

        public bool isInside(Rectangle rect)
        {
            if (rect == null) return false;

            double x1 = Canvas.GetLeft(rect);
            double y1 = Canvas.GetTop(rect);
            double x2 = Canvas.GetLeft(rect) + rect.Width;
            double y2 = Canvas.GetTop(rect) + rect.Height;

            if ((x1 < Canvas.GetLeft(this)) &&
                (x2 > Canvas.GetLeft(this)) &&
                (y1 < Canvas.GetTop(this)) &&
                (y2 > Canvas.GetTop(this)))
                return true;
            else
                return false;
        }

        public void deselected()
        {
            grd.BorderBrush = new SolidColorBrush(Colors.Transparent);
            //  grd.BorderThickness = new Thickness(2);
        }


        public void moveRelative(Point point)
        {
            Canvas.SetLeft(this, Statics.round20(PressedPoint.X - point.X));
            Canvas.SetTop(this, Statics.round20(PressedPoint.Y - point.Y));
        }


        public void selected()
        {
            grd.BorderBrush = new SolidColorBrush(Colors.Black);
            grd.BorderThickness = new Thickness(1);
            updateOriginPoint();
        }
        

        private void Element_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                var prop = e.GetCurrentPoint(this).Properties;
                if (prop.IsRightButtonPressed) return;
                if (prop.IsMiddleButtonPressed) return;
            }

            ce.clickElement = this;
        }

        public void updateOriginPoint()
        {
            PressedPoint = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
        }

        private void rightTap(object sender, RightTappedRoutedEventArgs e)
        {
            ce.showParameters(this);
        }

        public void Rotate()
        {
            CompositeTransform t = (CompositeTransform)grd.RenderTransform;
            t.Rotation += 90;
            parameters.setPositionTextBoxes(t.Rotation);
        }

        public void FlipX()
        {
            CompositeTransform t = (CompositeTransform)grd.RenderTransform;
            t.ScaleX *= -1;
            parameters.setFlipX(t.ScaleX == -1);
        }

        public void FlipY()
        {
            CompositeTransform t = (CompositeTransform)grd.RenderTransform;
            t.ScaleY *= -1;
            parameters.setFlipY(t.ScaleY == -1);
        }

    }
}
