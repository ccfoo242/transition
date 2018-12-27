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
    public class BufferScreen : ScreenComponentBase
    {
        public override int[,] TerminalPositions
        {
            get => new int[,] { { 20, 60 }, { 140, 60 } };
        }

        public override double SchematicWidth => 160;
        public override double SchematicHeight => 120;

        private ContentControl SymbolBuffer;
        private TextBlock txtComponentName;
        private TextBlock txtPolarity;
        private TextBlock txtDelay;
        private TextBlock txtGain;


        public BufferScreen(Serializable.Buffer buf) : base(buf)
        {
            SymbolBuffer = new ContentControl()
            {
                ContentTemplate = (DataTemplate)Application.Current.Resources["symbolBuffer"]
            };

            ComponentCanvas.Children.Add(SymbolBuffer);
            Canvas.SetTop(SymbolBuffer, 19);
            Canvas.SetLeft(SymbolBuffer, 19);

            txtComponentName = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform()
            };

            Binding b1 = new Binding()
            {
                Path = new PropertyPath("ElementName"),
                Mode = BindingMode.OneWay,
                Source = buf
            };
            txtComponentName.SetBinding(TextBlock.TextProperty, b1);
            txtComponentName.SizeChanged += delegate { setPositionTextBoxes(SerializableComponent); };
            Children.Add(txtComponentName);

            txtPolarity = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform()
            };

            Binding b2 = new Binding()
            {
                Path = new PropertyPath("InverterInput"),
                Mode = BindingMode.OneWay,
                Source = buf,
                Converter = new PolarityStringValueConverter()
            };
            txtPolarity.SetBinding(TextBlock.TextProperty, b2);
            txtPolarity.SizeChanged += delegate { setPositionTextBoxes(SerializableComponent); };
            Children.Add(txtPolarity);

            txtGain = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform()
            };

            Binding b3 = new Binding()
            {
                Path = new PropertyPath("GainString"),
                Mode = BindingMode.OneWay,
                Source = buf
            };
            txtGain.SetBinding(TextBlock.TextProperty, b3);
            txtGain.SizeChanged += delegate { setPositionTextBoxes(SerializableComponent); };
            Children.Add(txtGain);


            txtDelay = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform()
            };

            Binding b4 = new Binding()
            {
                Path = new PropertyPath("DelayString"),
                Mode = BindingMode.OneWay,
                Source = buf
            };
            txtDelay.SetBinding(TextBlock.TextProperty, b4);
            txtDelay.SizeChanged += delegate { setPositionTextBoxes(SerializableComponent); };
            Children.Add(txtDelay);


            postConstruct();
        }

        public override void setPositionTextBoxes(SerializableElement element)
        {
          
        }

        public class PolarityStringValueConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, string language)
            {
                //bool to string
                if (value == null) return "";
                bool bvalue = (bool)value;

                return bvalue ? "-" : "+";
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
            {
                throw new NotImplementedException();
            }
        }
    }
}
