﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Transition.CircuitEditor.OnScreenComponents
{
    public class WireScreen : Canvas, INotifyPropertyChanged
    {
        private Line line { get; }

        Binding bX1;
        Binding bY1;
        Binding bX2;
        Binding bY2;
        
        public double X1
        {
            get
            {
                if (wire.IsBounded1)
                { return wire.BoundedObject1.OnScreenComponent.getAbsoluteTerminalPosition((byte)(wire.BoundedTerminal1 - 1)).X; }
                else
                { return wire.X1; }
            }
        }

        public double Y1
        {
            get
            {
                if (wire.IsBounded1)
                { return wire.BoundedObject1.OnScreenComponent.getAbsoluteTerminalPosition((byte)(wire.BoundedTerminal1 - 1)).Y; }
                else
                { return wire.Y1; }
            }
        }
        public double X2
        {
            get
            {
                if (wire.IsBounded2)
                { return wire.BoundedObject2.OnScreenComponent.getAbsoluteTerminalPosition((byte)(wire.BoundedTerminal2 - 1)).X; }
                else
                { return wire.X2; }
            }
        }
        public double Y2
        {
            get
            {
                if (wire.IsBounded2)
                { return wire.BoundedObject2.OnScreenComponent.getAbsoluteTerminalPosition((byte)(wire.BoundedTerminal2 - 1)).Y; }
                else
                { return wire.Y2; }
            }
        }

        public Wire wire;

        public event PropertyChangedEventHandler PropertyChanged;

        public WireScreen(Wire wire) : base()
        {
            line = new Line();
            line.StrokeThickness = 4;
            line.Stroke = new SolidColorBrush(Colors.Black);
            this.wire = wire;

            wire.PropertyChanged += checkBounds;
            wire.ComponentChanged += checkBounds2;

            bX1 = new Binding()
            {
                Path = new PropertyPath("X1"),
                Mode = BindingMode.OneWay,
                Source = this
            };
            line.SetBinding(Line.X1Property, bX1);

            bY1 = new Binding()
            {
                Path = new PropertyPath("Y1"),
                Mode = BindingMode.OneWay,
                Source = this
            };
            line.SetBinding(Line.Y1Property, bY1);

            bX2 = new Binding()
            {
                Path = new PropertyPath("X2"),
                Mode = BindingMode.OneWay,
                Source = this
            };
            line.SetBinding(Line.X2Property, bX2);

            bY2 = new Binding()
            {
                Path = new PropertyPath("Y2"),
                Mode = BindingMode.OneWay,
                Source = this
            };
            line.SetBinding(Line.Y2Property, bY2);

            Children.Add(line);
        }
        
        public void checkBounds(object sender, PropertyChangedEventArgs e)
        {
            checkBounds2();
        }

        public void checkBounds2()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X1"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X2"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y1"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y2"));
        }
    }
}
