using System;
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
    public class WireScreen : Grid, INotifyPropertyChanged
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
                if (wire.IsBounded0)
                {
                    if (!wire.IsWireBounded0)
                        return wire.BoundedObject0.OnScreenComponent.getAbsoluteTerminalPosition(wire.BoundedTerminal0).X;
                    else
                        return ((Wire)wire.BoundedObject0).OnScreenWire.X1;
                }
                else
                { return wire.X0; }
            }
        }

        public double Y1
        {
            get
            {
                if (wire.IsBounded0)
                {
                    if (!wire.IsWireBounded0)
                        return wire.BoundedObject0.OnScreenComponent.getAbsoluteTerminalPosition(wire.BoundedTerminal0).Y;
                     else
                        return ((Wire)wire.BoundedObject0).OnScreenWire.Y1;
                }
                else
                { return wire.Y0; }
            }
        }

        public double X2
        {
            get
            {
                if (wire.IsBounded1)
                {
                    if (!wire.IsWireBounded1)
                        return wire.BoundedObject1.OnScreenComponent.getAbsoluteTerminalPosition(wire.BoundedTerminal1).X;
                    else
                        return ((Wire)wire.BoundedObject1).OnScreenWire.X2;
                }
                else
                { return wire.X1; }
            }
        }

        public double Y2
        {
            get
            {
                if (wire.IsBounded1)
                {
                    if (!wire.IsWireBounded1)
                        return wire.BoundedObject1.OnScreenComponent.getAbsoluteTerminalPosition(wire.BoundedTerminal1).Y;
                    else
                        return ((Wire)wire.BoundedObject1).OnScreenWire.Y2;
                }
                else
                { return wire.Y1; }
            }
        }

        public Wire wire;
        public WireTerminal wt1;
        public WireTerminal wt2;

        public event PropertyChangedEventHandler PropertyChanged;

        public WireScreen(Wire wire) : base()
        {
            line = new Line()
            {
                StrokeThickness = 4,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round
            };
           
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

            wt1 = new WireTerminal(this, 0);
            wt2 = new WireTerminal(this, 1);
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
