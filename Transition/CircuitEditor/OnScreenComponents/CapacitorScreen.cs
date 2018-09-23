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
    public class CapacitorScreen : ScreenComponentBase
    {
        public TextBlock txtComponentName;
        public TextBlock txtCapacitorValue;

        public override double SchematicWidth => 120;
        public override double SchematicHeight => 80;
        public override int[,] TerminalPositions
        {
            get => new int[,] { { 20, 40 }, { 100, 40 } };
        }


        public CapacitorScreen(Capacitor capacitor) : base(capacitor)
        {
            ContentControl symbolCapacitor = new ContentControl()
            {
                ContentTemplate = (DataTemplate)Application.Current.Resources["symbolCapacitor"]
            };

            ComponentCanvas.Children.Add(symbolCapacitor);
            Canvas.SetTop(symbolCapacitor, 19);
            Canvas.SetLeft(symbolCapacitor, 19);

            txtComponentName = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b1 = new Binding()
            {
                Path = new PropertyPath("ElementName"),
                Mode = BindingMode.OneWay
            };
            txtComponentName.SetBinding(TextBlock.TextProperty, b1);
            txtComponentName.RenderTransform = new TranslateTransform() { };
            txtComponentName.SizeChanged += delegate { setPositionTextBoxes(); };
            Children.Add(txtComponentName);


            txtCapacitorValue = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b2 = new Binding()
            {
                Path = new PropertyPath("ValueString"),
                Mode = BindingMode.OneWay
            };
            txtCapacitorValue.SetBinding(TextBlock.TextProperty, b2);
            txtCapacitorValue.RenderTransform = new TranslateTransform() { };
            txtCapacitorValue.SizeChanged += delegate { setPositionTextBoxes(); };
            Children.Add(txtCapacitorValue);

            setPositionTextBoxes();

            postConstruct();
        }


        public override void setPositionTextBoxes()
        {
            double leftRV; double topRV;
            double leftCN; double topCN;

            if ((ActualRotation == 0) || (ActualRotation == 180))
            {
                topCN = 0;
                leftCN = (SchematicWidth / 2) - (txtComponentName.ActualWidth / 2);
                topRV = 60;
                leftRV = (SchematicWidth / 2) - (txtCapacitorValue.ActualWidth / 2);
            }
            else
            {
                topCN = 40 - (txtComponentName.ActualHeight / 2);
                leftCN = 40 - (txtComponentName.ActualWidth);
                topRV = 40 - (txtCapacitorValue.ActualHeight / 2);
                leftRV = 80;
            }

            ((TranslateTransform)txtComponentName.RenderTransform).X = leftCN;
            ((TranslateTransform)txtComponentName.RenderTransform).Y = topCN;

            ((TranslateTransform)txtCapacitorValue.RenderTransform).X = leftRV;
            ((TranslateTransform)txtCapacitorValue.RenderTransform).Y = topRV;
        }

    }
}
