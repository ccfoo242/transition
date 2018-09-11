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
    public class CapacitorScreen : ScreenComponentBase
    {
        public TextBlock txtComponentName;
        public TextBlock txtCapacitorValue;

        public CapacitorScreen(Capacitor capacitor) : base(capacitor)
        {

            txtComponentName = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b1 = new Binding()
            {
                Path = new PropertyPath("ComponentName"),
                Mode = BindingMode.OneWay
            };
            txtComponentName.SetBinding(TextBlock.TextProperty, b1);
            txtComponentName.RenderTransform = new TranslateTransform() { };
            txtComponentName.SizeChanged += delegate { setPositionTextBoxes(); };
            ComponentCanvas.Children.Add(txtComponentName);


            txtCapacitorValue = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b2 = new Binding()
            {
                Path = new PropertyPath("CapacitorValue"),
                Mode = BindingMode.OneWay
            };
            txtCapacitorValue.SetBinding(TextBlock.TextProperty, b2);
            txtCapacitorValue.RenderTransform = new TranslateTransform() { };
            txtCapacitorValue.SizeChanged += delegate { setPositionTextBoxes(); };
            ComponentCanvas.Children.Add(txtCapacitorValue);

            setPositionTextBoxes();
        }


        private void setPositionTextBoxes()
        {
            double leftRV; double topRV;
            double leftCN; double topCN;

            if ((actualRotation == 0) || (actualRotation == 180))
            {
                topCN = 0;
                leftCN = (SchematicWidth / 2) - (txtComponentName.ActualWidth / 2);
                topRV = 60;
                leftRV = (SchematicWidth / 2) - (txtCapacitanceValue.ActualWidth / 2);
            }
            else
            {
                topCN = 40 - (txtComponentName.ActualHeight / 2);
                leftCN = 40 - (txtComponentName.ActualWidth);
                topRV = 40 - (txtCapacitanceValue.ActualHeight / 2);
                leftRV = 80;
            }

            ((TranslateTransform)txtComponentName.RenderTransform).X = leftCN;
            ((TranslateTransform)txtComponentName.RenderTransform).Y = topCN;

            ((TranslateTransform)txtCapacitanceValue.RenderTransform).X = leftRV;
            ((TranslateTransform)txtCapacitanceValue.RenderTransform).Y = topRV;
        }

    }
}
