﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;
using Transition.Common;
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

    public abstract class ScreenElementBase : Grid , ICircuitSelectable
    {
        //public Point2D originalPosition;

      //  public abstract Point2D Position { get; set; }

     //   public abstract void moveRelative(Point2D destination);
      //  public abstract void moveAbsolute(Point2D destination);
      //  public abstract void moveAbsoluteCommand(Point2D destination);
     
        public abstract void selected();
        public abstract void deselected();

        public abstract void highlightTerminal(byte terminal);
        public abstract void lowlightTerminal(byte terminal);
        public abstract void lowlightAllTerminals();

        public abstract SerializableElement Serializable { get; }

        public abstract double RadiusClick { get; }
        public double RadiusNear => 15;
        public abstract bool isInside(Rectangle rect);
        public abstract Point2D getAbsoluteTerminalPosition(byte terminal);

        public abstract byte QuantityOfTerminals { get; }

        public abstract void SerializableLayoutChanged(SerializableElement el);

        public List<Terminal> Terminals { get; } = new List<Terminal>();

        // public Dictionary<int, Rectangle> terminalsRectangles;
        

        /* public bool isClicked(Point2D point)
        {
            return getDistance(pointX, pointY) < RadiusClick;
        } */

        public bool isPointNearTerminal(byte terminal, Point2D point)
        {
            return point.getDistance(getAbsoluteTerminalPosition(terminal)) < RadiusNear;
        }
        
        public ScreenElementBase(SerializableElement element)
        {
            element.LayoutChanged += SerializableLayoutChanged;
        }
    }

    public abstract class ScreenComponentBase : ScreenElementBase, INotifyPropertyChanged
    {
        public Canvas ComponentCanvas { get; }
        public CompositeTransform ComponentTransform { get; }

        public SerializableComponent SerializableComponent { get; }

        public override SerializableElement Serializable => SerializableComponent;

        public abstract int[,] TerminalPositions { get; }

        public abstract double SchematicWidth { get; }
        public abstract double SchematicHeight { get; }

        public abstract void setPositionTextBoxes(SerializableElement element);
   
        public double ActualRotation => SerializableComponent.Rotation;
        public bool FlipX => SerializableComponent.FlipX;
        public bool FlipY => SerializableComponent.FlipY;

        public Point2D componentPosition;
        public Point2D ComponentPosition
            { get => componentPosition;
              set  { componentPosition = value;
                     setPositionTerminals(Serializable);
                     setPositionTextBoxes(Serializable);
            } }

        public override byte QuantityOfTerminals => SerializableComponent.QuantityOfTerminals;
        public override double RadiusClick => 30;

        public event PropertyChangedEventHandler PropertyChanged;
       
        public override void SerializableLayoutChanged(SerializableElement el)
        {
            setPositionTerminals(Serializable);
            setPositionTextBoxes(Serializable);
        }

        public ScreenComponentBase(SerializableComponent component) : base(component)
        {
            SerializableComponent = component;

            component.LayoutChanged += setPositionTextBoxes;
            component.LayoutChanged += setPositionTerminals;

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
            
            //DataContext = SerializableComponent;

            Binding bPX = new Binding()
            {
                Path = new PropertyPath("ComponentPosition.X"),
                Mode = BindingMode.OneWay
            };
            SetBinding(Canvas.LeftProperty, bPX);

            Binding bPY = new Binding()
            {
                Path = new PropertyPath("ComponentPosition.Y"),
                Mode = BindingMode.OneWay
            };
            SetBinding(Canvas.TopProperty, bPY);

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
            BindingOperations.SetBinding(ComponentTransform, CompositeTransform.ScaleYProperty, bFY);

            Terminal t;

            for (byte x = 0; x < QuantityOfTerminals; x++)
            {
                t = new Terminal();
                Terminals.Add(t);
                Children.Add(t);
            }
            
        }
        

        protected void postConstruct()
        {
            Width = SchematicWidth;
            Height = SchematicHeight;
        }
        
        
        public override bool isInside(Rectangle rect)
        {
            if (rect == null) return false;

            double x1 = Canvas.GetLeft(rect);
            double y1 = Canvas.GetTop(rect);
            double x2 = Canvas.GetLeft(rect) + rect.Width;
            double y2 = Canvas.GetTop(rect) + rect.Height;

            if ((x1 < (ComponentPosition.X + (SchematicWidth / 2))) &&
                (x2 > (ComponentPosition.X + (SchematicWidth / 2))) &&
                (y1 < (ComponentPosition.Y + (SchematicHeight / 2))) &&
                (y2 > (ComponentPosition.Y + (SchematicHeight / 2))))
                return true;
            else
                return false;
        }

        public bool isPointInsideComponent(Point2D point)
        {
            return (point.X > ComponentPosition.X) && (point.X < (ComponentPosition.X + SchematicWidth)) &&
                   (point.Y > ComponentPosition.Y) && (point.Y < (ComponentPosition.Y + SchematicHeight));
        }
        

        public override void selected()
        {
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new Thickness(1);
        }
        
        public override void deselected()
        {
             BorderBrush = new SolidColorBrush(Colors.Transparent);
        }
        
        public void moveRelative(Point2D displacement)
        {
            Point2D originalPosition = SerializableComponent.ComponentPosition;

            ComponentPosition = originalPosition + displacement;
        }

        public void moveAbsolute(Point2D newPosition)
        {
            ComponentPosition = newPosition;
        }
        
        public Point2D getRelativeTerminalPosition(byte terminal)
        {
          
            double X =  ((FlipX ? -1 : 1) *
                           Math.Cos(Math.PI - (ActualRotation * Math.PI / 180)) * 
                           ((SchematicWidth / 2) - TerminalPositions[terminal, 0])) +
                           ((FlipY ? -1 : 1) *
                           Math.Sin(Math.PI - (ActualRotation * Math.PI / 180)) *
                           ((SchematicHeight / 2) - TerminalPositions[terminal, 1]));

            double Y =  ((FlipX ? -1 : 1) *
                           Math.Sin(Math.PI - (ActualRotation * Math.PI / 180)) *
                           ((SchematicWidth / 2) - TerminalPositions[terminal, 0])) +
                           ((FlipY ? -1 : 1) *
                           Math.Cos(Math.PI - (ActualRotation * Math.PI / 180)) *
                           ((SchematicHeight / 2) - TerminalPositions[terminal, 1]));
            
            return new Point2D(X, Y);
        }

        public void setPositionTerminals(SerializableElement element)
        {
            for (byte x = 0; x < QuantityOfTerminals; x++)
                Terminals[x].TerminalPosition = getRelativeTerminalPosition(x);
            
        }


        public override Point2D getAbsoluteTerminalPosition(byte terminal)
        {
            Point2D relative = getRelativeTerminalPosition(terminal);

            double X = (SchematicWidth / 2) + relative.X + ComponentPosition.X;
            double Y = (SchematicHeight / 2) + relative.Y + ComponentPosition.Y;
            
            return new Point2D(X, Y);
        }


        public override void highlightTerminal(byte terminal)
        {
            Terminals[terminal].highlight();
        }

        public override void lowlightAllTerminals()
        {
            for (byte i = 0; i < QuantityOfTerminals; i++)
                lowlightTerminal(i);
        }

        public override void lowlightTerminal(byte terminal)
        {
            Terminals[terminal].lowlight();
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
