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
    public class FDNRScreen : ScreenComponentBase
    {
        public override double SchematicWidth => 120;
        public override double SchematicHeight => 80;
        
        public TextBlock txtComponentName;
        public TextBlock txtFdnrValue;

        public ContentControl SymbolFdnr { get; }

        public FDNRScreen(FDNR fdnr) : base(fdnr)
        {
            SymbolFdnr = new ContentControl()
            {
                ContentTemplate = (DataTemplate)Application.Current.Resources["symbolFdnr"]
            };

            ComponentCanvas.Children.Add(SymbolFdnr);
            Canvas.SetTop(SymbolFdnr, 20);
            Canvas.SetLeft(SymbolFdnr, 20);

            txtComponentName = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b1 = new Binding()
            {
                Path = new PropertyPath("ComponentName"),
                Source = this,
                Mode = BindingMode.OneWay
            };
            txtComponentName.SetBinding(TextBlock.TextProperty, b1);
            txtComponentName.RenderTransform = new TranslateTransform() { };
            txtComponentName.SizeChanged += delegate { setPositionTextBoxes(); };
            TextCanvas.Children.Add(txtComponentName);


            txtFdnrValue = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b2 = new Binding()
            {
                Path = new PropertyPath("ValueString"),
                Mode = BindingMode.OneWay
            };
            txtFdnrValue.SetBinding(TextBlock.TextProperty, b2);
            txtFdnrValue.RenderTransform = new TranslateTransform() { };
            txtFdnrValue.SizeChanged += delegate { setPositionTextBoxes(); };
            TextCanvas.Children.Add(txtFdnrValue);

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
                leftRV = (SchematicWidth / 2) - (txtFdnrValue.ActualWidth / 2);
            }
            else
            {
                topCN = (SchematicHeight / 2) - (txtComponentName.ActualHeight / 2);
                leftCN = 40 - (txtComponentName.ActualWidth);
                topRV = (SchematicHeight / 2) - (txtFdnrValue.ActualHeight / 2);
                leftRV = 80;
            }

            ((TranslateTransform)txtComponentName.RenderTransform).X = leftCN;
            ((TranslateTransform)txtComponentName.RenderTransform).Y = topCN;

            ((TranslateTransform)txtFdnrValue.RenderTransform).X = leftRV;
            ((TranslateTransform)txtFdnrValue.RenderTransform).Y = topRV;
        }
    }
}
