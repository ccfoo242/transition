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
                output[0, 0] = 20;
                output[0, 1] = 20;
                for (byte x = 1; x < QuantityOfTerminals; x++)
                {
                    output[x, 0] = 100;
                    output[x, 1] = 20 + ((x - 1) * 20);
                }
                return output;
            } }

        public override double SchematicWidth => 120;
        public override double SchematicHeight {
            get {
                int output = 20 + ((QuantityOfTerminals - 1) * 20);
                if ((output % 40) != 0)
                    output += 20;
                return output;
            } }

        private List<TextBlock> letters = new List<TextBlock>();
        
        public SwitchScreen(Switch sw) : base(sw)
        {
            sw.TerminalsChanged += TerminalsChanged;
            
            TerminalsChanged(sw.QuantityOfTerminals, sw.QuantityOfTerminals);

            postConstruct();
        }

        private void TerminalsChanged(byte oldValue, byte newValue)
        {
            ComponentCanvas.Children.Clear();

            Ellipse el1 = new Ellipse()
            {
                Width = 6,
                Height = 6,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(Colors.Black)
            };
            
            ComponentCanvas.Children.Add(el1);
            Canvas.SetLeft(el1, 17);
            Canvas.SetTop(el1, 17);
            
            Ellipse el2;

            double yy;

            for (byte x = 1; x < QuantityOfTerminals; x++)
            {
                el2 = new Ellipse()
                {
                    Width = 6,
                    Height = 6,
                    StrokeThickness = 1,
                    Fill = new SolidColorBrush(Colors.Black)
                };
                ComponentCanvas.Children.Add(el2);
                Canvas.SetLeft(el2, 97);
                yy = 17 + ((x - 1) * 20);
                Canvas.SetTop(el2, yy);
            }

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

            postConstruct();
            setPositionTextBoxes(SerializableComponent);
        }

        public override void setPositionTextBoxes(SerializableElement element)
        {
         

        }
    }
}
