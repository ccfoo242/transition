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
    class ResistorScreen : ScreenComponentBase
    {
        public TextBlock txtComponentName;
        public TextBlock txtResistanceValue;
        
        public ResistorScreen(SerializableComponent component) : base(component)
        {
            SchematicWidth = 120;
            SchematicHeight = 80;
            
            ContentControl symbolResistor = new ContentControl()
            {
                ContentTemplate = (DataTemplate)Application.Current.Resources["symbolResistor"]
            };

            ComponentCanvas.Children.Add(symbolResistor);
            Canvas.SetTop(symbolResistor, 20);
            Canvas.SetLeft(symbolResistor, 20);

            Binding b1 = new Binding()
            {
                Path = new PropertyPath("ComponentName"),
                Mode = BindingMode.OneWay
            };
            txtComponentName.SetBinding(TextBlock.TextProperty, b1);
            txtComponentName.SizeChanged += delegate { setPositionTextBoxes(); };
            ComponentCanvas.Children.Add(txtComponentName);


            txtResistanceValue = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform()
            };

            Binding b2 = new Binding()
            {
                Path = new PropertyPath("ResistanceValue"),
                Mode = BindingMode.OneWay,
                Converter = new EngrConverter()
            };
            txtResistanceValue.SetBinding(TextBlock.TextProperty, b2);
            txtResistanceValue.SizeChanged += delegate { setPositionTextBoxes(); };
            ComponentCanvas.Children.Add(txtResistanceValue);
            
            postConstruct();

        }


        public void setPositionTextBoxes()
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
