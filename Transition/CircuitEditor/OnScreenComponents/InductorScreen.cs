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
    public class InductorScreen : ScreenComponentBase
    {
        
        public override double SchematicWidth => 120;
        public override double SchematicHeight => 80;

        public override int[,] TerminalPositions
        {
            get => new int[,] { { 20, 40 }, { 100, 40 } };
        }

        public TextBlock txtComponentName;
        public TextBlock txtInductorValue;

        public InductorScreen(Inductor inductor) : base(inductor)
        {
            ContentControl symbolInductor = new ContentControl()
            {
                ContentTemplate = (DataTemplate)Application.Current.Resources["symbolInductor"]
            };

            ComponentCanvas.Children.Add(symbolInductor);
            Canvas.SetTop(symbolInductor, 20);
            Canvas.SetLeft(symbolInductor, 20);
            
            txtComponentName = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b1 = new Binding()
            {
                Path = new PropertyPath("ElementName"),
                Source = this,
                Mode = BindingMode.OneWay
            };
            txtComponentName.SetBinding(TextBlock.TextProperty, b1);
            txtComponentName.RenderTransform = new TranslateTransform() { };
            txtComponentName.SizeChanged += delegate { setPositionTextBoxes(); };
            Children.Add(txtComponentName);
            
            txtInductorValue = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b2 = new Binding()
            {
                Path = new PropertyPath("InductorValue"),
                Mode = BindingMode.OneWay,
                Converter = new EngrConverter()
                { AllowNegativeNumber = false, ShortString = true }
            };
            txtInductorValue.SetBinding(TextBlock.TextProperty, b2);
            txtInductorValue.RenderTransform = new TranslateTransform() { };
            txtInductorValue.SizeChanged += delegate { setPositionTextBoxes(); };
            Children.Add(txtInductorValue);

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
                leftRV = (SchematicWidth / 2) - (txtInductorValue.ActualWidth / 2);
            }
            else
            {
                topCN = 40 - (txtComponentName.ActualHeight / 2);
                leftCN = 40 - (txtComponentName.ActualWidth);
                topRV = 40 - (txtInductorValue.ActualHeight / 2);
                leftRV = 80;
            }

            ((TranslateTransform)txtComponentName.RenderTransform).X = leftCN;
            ((TranslateTransform)txtComponentName.RenderTransform).Y = topCN;

            ((TranslateTransform)txtInductorValue.RenderTransform).X = leftRV;
            ((TranslateTransform)txtInductorValue.RenderTransform).Y = topRV;


        }
    }
}
