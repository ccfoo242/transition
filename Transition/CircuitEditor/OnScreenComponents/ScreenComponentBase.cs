﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public abstract class ScreenElementBase : Grid
    {
        public double originalPositionX;
        public double originalPositionY;

        public abstract double PositionX { get; }
        public abstract double PositionY { get; }

        public abstract void  moveRelative(double pointX, double pointY);
        public abstract double getDistance(double pointX, double pointY);

        public abstract void selected();
        public abstract void deselected();

        public void updateOriginalPosition()
        {
            originalPositionX = PositionX;
            originalPositionY = PositionY;
        }
    }


    public abstract class ScreenComponentBase : ScreenElementBase, INotifyPropertyChanged
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

        public override double PositionX => SerializableComponent.PositionX;
        public override double PositionY => SerializableComponent.PositionY;

    
        public event PropertyChangedEventHandler PropertyChanged;

        public double T1X => getAbsoluteTerminalPosition(0).X;
        public double T1Y => getAbsoluteTerminalPosition(0).Y;
        public double T2X => getAbsoluteTerminalPosition(1).X;
        public double T2Y => getAbsoluteTerminalPosition(1).Y;
        public double T3X => getAbsoluteTerminalPosition(2).X;
        public double T3Y => getAbsoluteTerminalPosition(2).Y;
        public double T4X => getAbsoluteTerminalPosition(3).X;
        public double T4Y => getAbsoluteTerminalPosition(3).Y;
        public double T5X => getAbsoluteTerminalPosition(4).X;
        public double T5Y => getAbsoluteTerminalPosition(4).Y;
        public double T6X => getAbsoluteTerminalPosition(5).X;
        public double T6Y => getAbsoluteTerminalPosition(5).Y;



        public ScreenComponentBase(SerializableComponent component) : base()
        {
            SerializableComponent = component;

            component.ComponentLayoutChanged += setPositionTextBoxes;
            component.ComponentLayoutChanged += TerminalsPositionsChanged;
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
        
        public void TerminalsPositionsChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("T1X"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("T1Y"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("T2X"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("T2Y"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("T3X"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("T3Y"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("T4X"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("T4Y"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("T5X"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("T5Y"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("T6X"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("T6Y"));
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

        public override double getDistance(double pointX, double pointY)
        {
            double midPositionX = PositionX + (SchematicWidth / 2);
            double midPositionY = PositionY + (SchematicHeight / 2);

            return Math.Sqrt(Math.Pow(midPositionX - pointX, 2) +
                             Math.Pow(midPositionY - pointY, 2));

        }

        public override void selected()
        {
            BorderBrush = new SolidColorBrush(Colors.Black);
            BorderThickness = new Thickness(1);
            updateOriginalPosition();
        }
        
        public override void deselected()
        {
             BorderBrush = new SolidColorBrush(Colors.Transparent);
        }


        public override void moveRelative(double positionX, double positionY)
        {
            SerializableComponent.PositionX = originalPositionX - positionX;
            SerializableComponent.PositionY = originalPositionY - positionY;
        }

        public Point getRelativeTerminalPosition(byte terminal)
        {
            Point output = new Point();

            output.X =  (SchematicWidth / 2) + 
                           ((FlipX ? -1 : 1) *
                           Math.Cos(Math.PI - ActualRotation) * 
                           ((SchematicWidth / 2) - TerminalPositions[terminal, 0])) +
                           ((FlipY ? -1 : 1) *
                           Math.Sin(Math.PI - ActualRotation) *
                           ((SchematicHeight / 2) - TerminalPositions[terminal, 1]));

            output.Y =  (SchematicHeight / 2) +
                           ((FlipX ? -1 : 1) *
                           Math.Sin(Math.PI - ActualRotation) *
                           ((SchematicWidth / 2) - TerminalPositions[terminal, 0])) +
                           ((FlipY ? -1 : 1) *
                           Math.Cos(Math.PI - ActualRotation) *
                           ((SchematicHeight / 2) - TerminalPositions[terminal, 1]));
            
            return output;
        }

        

        public Point getAbsoluteTerminalPosition(byte terminal)
        {
            Point relative = getRelativeTerminalPosition(terminal);

            return new Point(relative.X + PositionX, relative.Y + PositionY);
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
