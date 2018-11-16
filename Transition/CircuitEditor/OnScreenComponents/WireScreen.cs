﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;
using Transition.Common;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Transition.CircuitEditor.OnScreenComponents
{
    public class WireScreen : ScreenElementBase, INotifyPropertyChanged
    {
        private Line line { get; }

        public SerializableWire serializableWire;

        Binding bX0;
        Binding bY0;
        Binding bX1;
        Binding bY1;

        private Point2D position0;
        public Point2D PositionTerminal0
        {
            get { return position0; }
            set
            {
                position0 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position0"));
                RaiseScreenLayoutChanged();
            }
        }
        
        private Point2D position1;
        public Point2D PositionTerminal1
        {
            get { return position1; }
            set
            {
                position1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position1"));
                RaiseScreenLayoutChanged();
            }
        }


        public Tuple<ScreenElementBase, byte> bind0
        {
            get
            {
                if (serializableWire.Bind0 != null)
                    return new Tuple<ScreenElementBase, byte>
                        (serializableWire.Bind0.Item1.OnScreenComponent,
                         serializableWire.Bind0.Item2);
                else return null;
            }
        }

        public Tuple<ScreenElementBase, byte> bind1
        {
            get
            {
                if (serializableWire.Bind1 != null)
                    return new Tuple<ScreenElementBase, byte>
                        (serializableWire.Bind1.Item1.OnScreenComponent,
                         serializableWire.Bind1.Item2);
                else return null;
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsTerminal0Bounded => serializableWire.IsTerminal0Bounded;
        public bool IsTerminal1Bounded => serializableWire.IsTerminal1Bounded;

        public override SerializableElement Serializable => serializableWire;

        public override double RadiusClick => 15;

        public override byte QuantityOfTerminals => 2;

        public WireScreen(SerializableWire wire) : base(wire)
        {
            this.serializableWire = wire;

            line = new Line()
            {
                StrokeThickness = 4,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round
            };
           
            bX0 = new Binding()
            {
                Path = new PropertyPath("Position0.X"),
                Mode = BindingMode.OneWay,
                Source = this
            };
            line.SetBinding(Line.X1Property, bX0);

            bY0 = new Binding()
            {
                Path = new PropertyPath("Position0.Y"),
                Mode = BindingMode.OneWay,
                Source = this
            };
            line.SetBinding(Line.Y1Property, bY0);

            bX1 = new Binding()
            {
                Path = new PropertyPath("Position1.X"),
                Mode = BindingMode.OneWay,
                Source = this
            };
            line.SetBinding(Line.X2Property, bX1);

            bY1 = new Binding()
            {
                Path = new PropertyPath("Position1.Y"),
                Mode = BindingMode.OneWay,
                Source = this
            };
            line.SetBinding(Line.Y2Property, bY1);
            
            wire.WireBindingChanged += updateBinding;
            Children.Add(line);
            
            Terminals.Add(new WireTerminal()
                { TerminalPosition = serializableWire.PositionTerminal0 });

            Terminals.Add(new WireTerminal()
                { TerminalPosition = serializableWire.PositionTerminal1 });

        }

        private void updateBinding(SerializableWire wire, byte terminal, Tuple<SerializableElement, byte> previousValue, Tuple<SerializableElement, byte> newValue)
        {
            if (previousValue != null)
                previousValue.Item1.OnScreenComponent.ScreenLayoutChanged -= updateScreenLayoutForBinded;
            
            if (newValue != null)
                newValue.Item1.OnScreenComponent.ScreenLayoutChanged += updateScreenLayoutForBinded;
        }

        private void updateScreenLayoutForBinded(ScreenElementBase el)
        {
            if (IsTerminal0Bounded)
                PositionTerminal0 = el.getAbsoluteTerminalPosition(bind0.Item2);

            if (IsTerminal1Bounded)
                PositionTerminal1 = el.getAbsoluteTerminalPosition(bind1.Item2);
        }
        

        public override void selected()
        {
            highlightTerminal(0);
            highlightTerminal(1);
        }

        public override void deselected()
        {
            lowlightTerminal(0);
            lowlightTerminal(1);
        }

        public override void highlightTerminal(byte terminal)
        {
            Terminals[terminal].highlight();
        }

        public override void lowlightTerminal(byte terminal)
        {
            Terminals[terminal].lowlight();
        }

        public override void lowlightAllTerminals()
        {
            lowlightTerminal(0);
            lowlightTerminal(1);
        }

        public override bool isInside(Rectangle rect)
        {
            if (rect == null) return false;

            double x1 = Canvas.GetLeft(rect);
            double y1 = Canvas.GetTop(rect);
            double x2 = Canvas.GetLeft(rect) + rect.Width;
            double y2 = Canvas.GetTop(rect) + rect.Height;

            double leftExtreme =   (PositionTerminal0.X < PositionTerminal1.X) ? PositionTerminal0.X : PositionTerminal1.X;
            double rightExtreme =  (PositionTerminal0.X > PositionTerminal1.X) ? PositionTerminal0.X : PositionTerminal1.X;
            double topExtreme =    (PositionTerminal0.Y < PositionTerminal1.Y) ? PositionTerminal0.Y : PositionTerminal1.Y;
            double bottomExtreme = (PositionTerminal0.Y > PositionTerminal1.Y) ? PositionTerminal0.Y : PositionTerminal1.Y;

            return (x1 < leftExtreme) && (x2 > rightExtreme) &&
                   (y1 < topExtreme) && (y2 > bottomExtreme);
        }

        public override Point2D getAbsoluteTerminalPosition(byte terminal)
        {
            return (terminal == 0) ? PositionTerminal0 : PositionTerminal1;
        }

        public override void SerializableLayoutChanged(SerializableElement el)
        {
            if (!IsTerminal0Bounded)
                PositionTerminal0 = serializableWire.PositionTerminal0;

            if (!IsTerminal1Bounded)
                PositionTerminal1 = serializableWire.PositionTerminal1;
        }

        public override void moveRelative(Point2D vector)
        {
            Point2D originalPositionTerminal0 = serializableWire.PositionTerminal0;
            Point2D originalPositionTerminal1 = serializableWire.PositionTerminal1;

            PositionTerminal0 = originalPositionTerminal0 + vector;
            PositionTerminal1 = originalPositionTerminal1 + vector;
        }
    }
}
