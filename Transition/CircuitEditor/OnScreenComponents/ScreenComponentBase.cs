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
using Windows.UI.Xaml.Shapes;

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

        public double ActualRotation => SerializableComponent.Rotation;
        public bool FlipX => SerializableComponent.FlipX;
        public bool FlipY => SerializableComponent.FlipY;

        public double PositionX => SerializableComponent.PositionX;
        public double PositionY => SerializableComponent.PositionY;

        public double originalPositionX;
        public double originalPositionY;
        
        public ScreenComponentBase(SerializableComponent component) : base()
        {
            SerializableComponent = component;

            component.ComponentLayoutChanged += setPositionTextBoxes;
            
            ComponentTransform = new CompositeTransform();
            ComponentTransform.CenterX = SchematicWidth / 2;
            ComponentTransform.CenterY = SchematicHeight / 2;

            ComponentCanvas = new Canvas()
            {
                IsTapEnabled = true,
                RenderTransform = ComponentTransform
            };

            Background = new SolidColorBrush(Colors.Transparent);
            BorderThickness = new Thickness(1);
            Children.Add(ComponentCanvas);
          
         //   ComponentCanvas.PointerPressed += Element_PointerPressed;
            DataContext = SerializableComponent;

            Binding bPX = new Binding()
            {
                Path = new PropertyPath("PositionX"),
                Mode = BindingMode.OneWay
            };
            SetBinding(Canvas.LeftProperty, bPX);

            Binding bPY = new Binding()
            {
                Path = new PropertyPath("PositionY"),
                Mode = BindingMode.OneWay
            };
            SetBinding(Canvas.TopProperty, bPY);

            Binding bRt = new Binding()
            {
                Path = new PropertyPath("Rotation"),
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
            BindingOperations.SetBinding(ComponentTransform, CompositeTransform.ScaleYProperty, bFY);
            
        }

        protected void postConstruct()
        {
              Width = SchematicWidth ;
              Height = SchematicHeight;
        }
        
        
        public bool isInside(Rectangle rect)
        {
            if (rect == null) return false;

            double x1 = Canvas.GetLeft(rect);
            double y1 = Canvas.GetTop(rect);
            double x2 = Canvas.GetLeft(rect) + rect.Width;
            double y2 = Canvas.GetTop(rect) + rect.Height;

            if ((x1 < (PositionX + (SchematicWidth / 2))) &&
                (x2 > (PositionX + (SchematicWidth / 2))) &&
                (y1 < (PositionY + (SchematicHeight / 2))) &&
                (y2 > (PositionY + (SchematicHeight / 2))))
                return true;
            else
                return false;
        }

        public bool isInside(double pointX, double pointY)
        {
            return (pointX > PositionX) && (pointX < (PositionX + SchematicWidth)) &&
                   (pointY > PositionY) && (pointY < (PositionY + SchematicHeight));
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
            originalPositionX = Canvas.GetLeft(this);
            originalPositionY = Canvas.GetTop(this);
        }

        public void moveRelative(double positionX, double positionY)
        {
            SerializableComponent.PositionX = originalPositionX - positionX;
            SerializableComponent.PositionY = originalPositionY - positionY;
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

            return !(valueInt == 1);
        }
    }
}
