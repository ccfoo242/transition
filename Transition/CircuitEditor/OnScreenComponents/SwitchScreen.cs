using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Transition.CircuitEditor.OnScreenComponents
{
    public class SwitchScreen : ScreenComponentBase
    {
        public override int[,] TerminalPositions
        { get {
                var output = new int[QuantityOfTerminals, 2];
                output[0, 0] = 00;
                output[0, 1] = 00;
                for (byte x = 1; x < QuantityOfTerminals; x++)
                {
                    output[x, 0] = 80;
                    output[x, 1] = (x - 1) * 20;
                }
                return output;
            } }

        public override double SchematicWidth => 80;
        public override double SchematicHeight => 20 + (QuantityOfTerminals + 20);

        public ContentControl SymbolSwitch { get; set; }

        public SwitchScreen(Switch sw) : base(sw)
        {
            sw.TerminalsChanged += TerminalsChanged;

            TerminalsChanged(0, sw.QuantityOfTerminals);

            postConstruct();
        }

        private void TerminalsChanged(byte oldValue, byte newValue)
        {
            foreach(Ellipse e in Children.OfType<Ellipse>())
                Children.Remove(e);

            Children.Add(new Ellipse()
            {
                Width = 6,
                Height = 6,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(Colors.Black),
                RenderTransform = new TranslateTransform()
                {
                    X = -3,
                    Y = -3
                }
            });

            for (byte x = 1; x < QuantityOfTerminals; x++)
                Children.Add(new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    StrokeThickness = 1,
                    Fill = new SolidColorBrush(Colors.Black),
                    RenderTransform = new TranslateTransform()
                    {
                        X = 77,
                        Y = -3 + ((x - 1) * 20)
                    }
                });
            

            if (newValue < oldValue)
            {
                ElementTerminal t;

                for (byte x = oldValue; x > newValue; x--)
                {
                    t = Terminals[x - 1];
                    Terminals.Remove(t);
                    Children.Remove(t);
                }
            }

            if (newValue > oldValue)
            {
                ElementTerminal t;

                for (byte x = oldValue; x < newValue; x++)
                {
                    t = new ElementTerminal(x, this);
                    Terminals.Add(t); Children.Add(t);
                }
            }

        }

        public override void setPositionTextBoxes(SerializableElement element)
        {
         //   throw new NotImplementedException();
        }
    }
}
