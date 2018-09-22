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
    public class ResistorScreen : ScreenComponentBase
    {
        public TextBlock txtComponentName;
        public TextBlock txtResistanceValue;

        public override double SchematicWidth => 120;
        public override double SchematicHeight => 80;

        public ContentControl SymbolResistor { get; }

        public override int[,] TerminalPositions
        {
            get => new int[,] { { 20, 40 }, { 100, 40 } };
        }

        public ResistorScreen(Resistor resistor) : base(resistor)
        {
            
            SymbolResistor = new ContentControl()
            {
                ContentTemplate = (DataTemplate)Application.Current.Resources["symbolResistor"]
            };

            ComponentCanvas.Children.Add(SymbolResistor);
            Canvas.SetTop(SymbolResistor, 20);
            Canvas.SetLeft(SymbolResistor, 20);

            Binding b1 = new Binding()
            {
                Path = new PropertyPath("ElementName"),
                Mode = BindingMode.OneWay
            };

            txtComponentName = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform()
            };
            txtComponentName.SetBinding(TextBlock.TextProperty, b1);
            txtComponentName.SizeChanged += delegate { setPositionTextBoxes(); };
            Children.Add(txtComponentName);


            txtResistanceValue = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform()
            };

            Binding b2 = new Binding()
            {
                Path = new PropertyPath("ValueString"),
                Mode = BindingMode.OneWay
             /*   Converter = new EngrConverter()
                    { AllowNegativeNumber = false, ShortString = true }*/
            };
            txtResistanceValue.SetBinding(TextBlock.TextProperty, b2);
            txtResistanceValue.SizeChanged += delegate { setPositionTextBoxes(); };
            Children.Add(txtResistanceValue);
            
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
                leftRV = (SchematicWidth / 2) - (txtResistanceValue.ActualWidth / 2);
            }
            else
            {
                topCN = 40 - (txtComponentName.ActualHeight / 2);
                leftCN = 40 - (txtComponentName.ActualWidth);
                topRV = 40 - (txtResistanceValue.ActualHeight / 2);
                leftRV = SchematicHeight;
            }

            ((TranslateTransform)txtComponentName.RenderTransform).X = leftCN;
            ((TranslateTransform)txtComponentName.RenderTransform).Y = topCN;

            ((TranslateTransform)txtResistanceValue.RenderTransform).X = leftRV;
            ((TranslateTransform)txtResistanceValue.RenderTransform).Y = topRV;


        }

    }
}
