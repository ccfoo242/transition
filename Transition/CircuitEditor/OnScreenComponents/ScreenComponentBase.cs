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
            //    BindingOperations.SetBinding(ComponentTransform, CompositeTransform.TranslateXProperty, bPX);
            SetBinding(Canvas.TopProperty, bPX);

            Binding bPY = new Binding()
            {
                Path = new PropertyPath("PositionY"),
                Mode = BindingMode.OneWay
            };
            //    BindingOperations.SetBinding(ComponentTransform, CompositeTransform.TranslateYProperty, bPY);
            SetBinding(Canvas.LeftProperty, bPY);

            Binding bRt = new Binding()
            {
                Path = new PropertyPath("ActualRotation"),
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(ComponentTransform, CompositeTransform.RotationProperty, bRt);

            Binding bFX = new Binding()
            {
                Path = new PropertyPath("FlipX"),
                Mode = BindingMode.OneWay,
                Converter = new BoolToIntConverter()
            };
            BindingOperations.SetBinding(ComponentTransform, CompositeTransform.ScaleXProperty, bFX);

            Binding bFY = new Binding()
            {
                Path = new PropertyPath("FlipY"),
                Mode = BindingMode.OneWay,
                Converter = new BoolToIntConverter()
            };
            BindingOperations.SetBinding(ComponentTransform, CompositeTransform.ScaleXProperty, bFY);

        

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


    public class BoolToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return 1;
            bool valueBool = (bool)value;

            return valueBool ? -1 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return false;
            int valueInt = (int)value;

            return (valueInt == 1) ? false : true;
        }
    }
}
