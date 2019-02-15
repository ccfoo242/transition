using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easycoustics.Transition.CircuitEditor.Serializable;
using Easycoustics.Transition.Common;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Easycoustics.Transition.CircuitEditor.OnScreenComponents
{
    public class VoltageOutputNodeScreen : ScreenComponentBase
    {
        public override int[,] TerminalPositions
        {
            get => new int[,] { { 20, 20 } };
        }

        public override double SchematicWidth => 40;
        public override double SchematicHeight => 40;

        private VoltageOutputNode Node;

        public TextBlock txtComponentName;

        public override void setPositionTextBoxes(SerializableElement element)
        {
           
        }

        public VoltageOutputNodeScreen(VoltageOutputNode node) : base(node)
        {
            Node = node;

            txtComponentName = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b1 = new Binding()
            {
                Path = new PropertyPath("ElementName"),
                Mode = BindingMode.OneWay,
                Source = Node
            };
            txtComponentName.SetBinding(TextBlock.TextProperty, b1);
            txtComponentName.RenderTransform = new TranslateTransform() { };
            txtComponentName.SizeChanged += delegate { setPositionTextBoxes(SerializableComponent); };

            ((TranslateTransform)txtComponentName.RenderTransform).X = 20;
            ((TranslateTransform)txtComponentName.RenderTransform).Y = 0;

            Children.Add(txtComponentName);

            var rect = new Rectangle()
            {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush(Colors.Black)
            };
            //rect.RenderTransform = new TranslateTransform() { X = 15, Y = 15 };
            Children.Add(rect);
            postConstruct();
        }
    }
}
