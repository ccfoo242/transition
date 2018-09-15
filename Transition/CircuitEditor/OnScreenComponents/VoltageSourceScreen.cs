using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transition.CircuitEditor.SerializableModels;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Transition.CircuitEditor.OnScreenComponents
{
    public class VoltageSourceScreen : ScreenComponentBase
    {
        public override double SchematicWidth => 120;
        public override double SchematicHeight => 120;

        public TextBlock txtComponentName;
        public TextBlock txtVoltage;
        public TextBlock txtImpedance;

        public ContentControl SymbolGenerator;

        public VoltageSourceScreen(VoltageSource vs) : base(vs)
        {
            SymbolGenerator = new ContentControl()
            {
                ContentTemplate = (DataTemplate)Application.Current.Resources["symbolGenerator"]
            };
            ComponentCanvas.Children.Add(SymbolGenerator);
            Canvas.SetTop(SymbolGenerator, 20);
            Canvas.SetLeft(SymbolGenerator, 20);

            txtComponentName = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b1 = new Binding()
            {
                Path = new PropertyPath("ComponentName"),
                Mode = BindingMode.OneWay
            };
            txtComponentName.SetBinding(TextBlock.TextProperty, b1);
            txtComponentName.RenderTransform = new TranslateTransform() { };
            txtComponentName.SizeChanged += delegate { setPositionTextBoxes(); };
            TextCanvas.Children.Add(txtComponentName);

            txtVoltage = new TextBlock()
            {

            };
            TextCanvas.Children.Add(txtVoltage);

            txtImpedance = new TextBlock()
            {

            };
            TextCanvas.Children.Add(txtImpedance);
        }

        public override void setPositionTextBoxes()
        {
            
        }


    }
}
