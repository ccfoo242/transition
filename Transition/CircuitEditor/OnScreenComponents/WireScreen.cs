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

        Binding bX0;
        Binding bY0;
        Binding bX1;
        Binding bY1;

        private double x0;
        public double X0 { get { return x0; }
            set { x0 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X0"));
            } }

        private double y0;
        public double Y0 { get { return y0; }
            set { y0 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y0"));
            } }

        private double x1;
        public double X1 { get { return x1; }
            set { x1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X1"));
            } }

        private double y1;
        public double Y1 { get { return y1; }
            set { y1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y1"));
            } }

        public SerializableWire wire;

        public List<WireTerminal> terminals = new List<WireTerminal>();

        public event PropertyChangedEventHandler PropertyChanged;

        public WireScreen(SerializableWire wire) : base()
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
            
            bX0 = new Binding()
            {
                Path = new PropertyPath("X0"),
                Mode = BindingMode.OneWay,
                Source = this
            };
            line.SetBinding(Line.X1Property, bX0);

            bY0 = new Binding()
            {
                Path = new PropertyPath("Y0"),
                Mode = BindingMode.OneWay,
                Source = this
            };
            line.SetBinding(Line.Y1Property, bY0);

            bX1 = new Binding()
            {
                Path = new PropertyPath("X1"),
                Mode = BindingMode.OneWay,
                Source = this
            };
            line.SetBinding(Line.X2Property, bX1);

            bY1 = new Binding()
            {
                Path = new PropertyPath("Y1"),
                Mode = BindingMode.OneWay,
                Source = this
            };
            line.SetBinding(Line.Y2Property, bY1);

            Children.Add(line);

            //wt0 = new WireTerminal(this, 0);
            //wt1 = new WireTerminal(this, 1);

            terminals.Add(new WireTerminal(this, 0));
            terminals.Add(new WireTerminal(this, 1));
        }

        
        public void checkBounds(object sender, PropertyChangedEventArgs e)
        {
            checkBounds2();
        }

        public void checkBounds2()
        {
            if (wire.IsBounded0)
            {
                if (!wire.IsWireBounded0)
                {
                    X0 = wire.BoundedObject0.OnScreenComponent.getAbsoluteTerminalPosition(wire.BoundedTerminal0).X;
                    Y0 = wire.BoundedObject0.OnScreenComponent.getAbsoluteTerminalPosition(wire.BoundedTerminal0).Y;
                }
                else
                {
                    if (wire.BoundedTerminal0 == 0)
                    {
                        X0 = ((SerializableWire)wire.BoundedObject0).OnScreenWire.X0;
                        Y0 = ((SerializableWire)wire.BoundedObject0).OnScreenWire.Y0;
                    }
                    else
                    {
                        X0 = ((SerializableWire)wire.BoundedObject0).OnScreenWire.X1;
                        Y0 = ((SerializableWire)wire.BoundedObject0).OnScreenWire.Y1;
                    }
                }
            }
            else
            {
                X0 = wire.X0;
                Y0 = wire.Y0;
            }


            if (wire.IsBounded1)
            {
                if (!wire.IsWireBounded1)
                {
                    X1 = wire.BoundedObject1.OnScreenComponent.getAbsoluteTerminalPosition(wire.BoundedTerminal1).X;
                    Y1 = wire.BoundedObject1.OnScreenComponent.getAbsoluteTerminalPosition(wire.BoundedTerminal1).Y;
                }
                else
                {
                    if (wire.BoundedTerminal1 == 0)
                    {
                        X1 = ((SerializableWire)wire.BoundedObject1).OnScreenWire.X0;
                        Y1 = ((SerializableWire)wire.BoundedObject1).OnScreenWire.Y0;
                    }
                    else
                    {
                        X1 = ((SerializableWire)wire.BoundedObject1).OnScreenWire.X1;
                        Y1 = ((SerializableWire)wire.BoundedObject1).OnScreenWire.Y1;
                    }
                }
            }
            else
            {
                X1 = wire.X1;
                Y1 = wire.Y1;
            }
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X0"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y0"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("X1"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Y1"));
        }
    }
}
