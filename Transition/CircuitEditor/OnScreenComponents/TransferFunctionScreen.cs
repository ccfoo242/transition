using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.Serializable;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Transition.CircuitEditor.OnScreenComponents
{
    public class TransferFunctionScreen : ScreenComponentBase
    {
        public override int[,] TerminalPositions
        {
            get => new int[,] { { 20, 40 }, { 20, 120 }, { 140, 40 }, { 140, 120} };
        }

        public override double SchematicWidth => 160;
        public override double SchematicHeight => 160;

        private TextBlock txtComponentName { get; }
        private TextBlock txtTF { get; }
        
        public ContentControl SymbolTF { get; }
        
        public TransferFunctionScreen(TransferFunctionComponent tf) : base(tf)
        {
            SymbolTF = new ContentControl()
            {
                ContentTemplate = (DataTemplate)Application.Current.Resources["symbolTF"]
            };

            ComponentCanvas.Children.Add(SymbolTF);
            Canvas.SetTop(SymbolTF, 19);
            Canvas.SetLeft(SymbolTF, 19);

            Binding b1 = new Binding()
            {
                Path = new PropertyPath("ElementName"),
                Mode = BindingMode.OneWay,
                Source = tf
            };

            txtComponentName = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform()
            };
            txtComponentName.SetBinding(TextBlock.TextProperty, b1);
            txtComponentName.SizeChanged += delegate { setPositionTextBoxes(SerializableComponent); };
            Children.Add(txtComponentName);

            txtTF = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform()
            };

            Binding b2 = new Binding()
            {
                Path = new PropertyPath("TFString"),
                Mode = BindingMode.OneWay,
                Source = tf
            };
            txtTF.SetBinding(TextBlock.TextProperty, b2);
            txtTF.SizeChanged += delegate { setPositionTextBoxes(SerializableComponent); };
            Children.Add(txtTF);

            postConstruct();
        }

        public override void setPositionTextBoxes(SerializableElement element)
        {
           
        }
    }
}
