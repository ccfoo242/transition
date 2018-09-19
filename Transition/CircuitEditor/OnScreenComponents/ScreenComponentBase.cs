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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Transition.CircuitEditor.OnScreenComponents
{
    /* i had to use Grid class because it has a border
     as for Border class, it is sealed...*/

    public abstract class ScreenComponentBase : Grid
    {
        public Canvas ComponentCanvas { get; }
        public CompositeTransform ComponentTransform { get; }

        public SerializableComponent SerializableComponent { get; }

        public abstract double SchematicWidth { get; }
        public abstract double SchematicHeight { get; }
       
        public abstract void setPositionTextBoxes();
        public abstract int[,] TerminalPositions { get; }
        
        public double PositionX
        {
            get { return (double)GetValue(PositionXProperty); }
            set { SetValue(PositionXProperty, value);
                  updateComponentPosition();
            }
        }

        // Using a DependencyProperty as the backing store for PositionX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionXProperty =
            DependencyProperty.Register("PositionX", typeof(double), typeof(ScreenComponentBase), new PropertyMetadata(0));
        
        public double PositionY
        {
            get { return (double)GetValue(PositionYProperty); }
            set { SetValue(PositionYProperty, value);
                  updateComponentPosition();
            }
        }

        // Using a DependencyProperty as the backing store for PositionY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionYProperty =
            DependencyProperty.Register("PositionY", typeof(double), typeof(ScreenComponentBase), new PropertyMetadata(0));
        

        public double ActualRotation
        {
            get { return (double)GetValue(ActualRotationProperty); }
            set { SetValue(ActualRotationProperty, value);
                ComponentTransform.Rotation = value;
            }
        }

        // Using a DependencyProperty as the backing store for ActualRotation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualRotationProperty =
            DependencyProperty.Register("ActualRotation", typeof(double), typeof(ScreenComponentBase), new PropertyMetadata(0));
        

        public bool FlipX
        {
            get { return (bool)GetValue(FlipXProperty); }
            set { SetValue(FlipXProperty, value);
                ComponentTransform.ScaleX = value ? -1 : 1;
            }
        }

        // Using a DependencyProperty as the backing store for FlipX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlipXProperty =
            DependencyProperty.Register("FlipX", typeof(bool), typeof(ScreenComponentBase), new PropertyMetadata(0));
        
        public bool FlipY
        {
            get { return (bool)GetValue(FlipYProperty); }
            set { SetValue(FlipYProperty, value);
                ComponentTransform.ScaleY = value ? -1 : 1;
            }
        }

        // Using a DependencyProperty as the backing store for FlipY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlipYProperty =
            DependencyProperty.Register("FlipY", typeof(bool), typeof(ScreenComponentBase), new PropertyMetadata(0));





        public Point PressedPoint;

        public ScreenComponentBase(SerializableComponent component) : base()
        {
            SerializableComponent = component;

            ComponentTransform = new CompositeTransform();
            ComponentTransform.CenterX = SchematicWidth / 2;
            ComponentTransform.CenterY = SchematicHeight / 2;
            
            ComponentCanvas = new Canvas()
            {
                IsTapEnabled = true,
                Width = SchematicWidth,
                Height = SchematicHeight,
                RenderTransform = ComponentTransform
            };
            
            Children.Add(ComponentCanvas);
         
            ComponentCanvas.PointerPressed += Element_PointerPressed;
            DataContext = SerializableComponent;

            Binding bPX = new Binding()
            {
                Path = new PropertyPath("PositionX"),
                Mode = BindingMode.OneWay
            };
            SetBinding(PositionXProperty, bPX);

            Binding bPY = new Binding()
            {
                Path = new PropertyPath("PositionY"),
                Mode = BindingMode.OneWay
            };
            SetBinding(PositionYProperty, bPY);

            Binding bRotation = new Binding()
            {
                Path = new PropertyPath("ActualRotation"),
                Mode = BindingMode.OneWay
            };
            SetBinding(ActualRotationProperty, bRotation);

            Binding bFX = new Binding()
            {
                Path = new PropertyPath("FlipX"),
                Mode = BindingMode.OneWay
            };
            SetBinding(FlipXProperty, bFX);

            Binding bFY = new Binding()
            {
                Path = new PropertyPath("FlipY"),
                Mode = BindingMode.OneWay
            };
            SetBinding(FlipYProperty, bFY);

        }

        private void updateComponentPosition()
        {
            try
            {
                Canvas.SetLeft(this, PositionX);
                Canvas.SetTop(this, PositionY);
            }
            catch { }
            
        }

        protected void postConstruct()
        {
            ComponentCanvas.Width = SchematicWidth;
            ComponentCanvas.Height = SchematicHeight;
        }

        private void Element_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                var prop = e.GetCurrentPoint(ComponentCanvas).Properties;
                if (prop.IsRightButtonPressed) return;
                if (prop.IsMiddleButtonPressed) return;
            }
            CircuitEditor.currentInstance.clickElement(this.SerializableComponent);
        }


        public void selected()
        {
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new Thickness(1);
            updateOriginPoint();
        }
        

        public void deselected()
        {
             BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        public void updateOriginPoint()
        {
             PressedPoint = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
        }
    }
}
