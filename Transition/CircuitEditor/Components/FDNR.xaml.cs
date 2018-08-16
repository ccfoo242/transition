using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Transition.CircuitEditor.Components
{
    public sealed partial class FDNR : UserControl, IComponentParameter, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public double SchematicWidth { get { return 140; } }
        public double SchematicHeight { get { return 80; } }
        public String ComponentLetter { get { return "D"; } }

        private String componentName;
        public String ComponentName
        {
            get { return componentName; }
            set
            {
                componentName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ComponentName"));
            }
        }

        private EngrNumber fdnrValue;
        public EngrNumber FdnrValue
        {
            get { return fdnrValue; }
            set
            {
                fdnrValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FdnrValue"));
            }
        }

        public TextBlock txtComponentName;
        public TextBlock txtFdnrValue;

        public Canvas CnvLabels { get; set; }

        private double actualRotation;

        public FDNR()
        {
            this.InitializeComponent();
            init();
        }

        private void init()
        {
            ComponentValueBox.ComponentValue = EngrNumber.one();

            CnvLabels = new Canvas();

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
            CnvLabels.Children.Add(txtComponentName);


            txtFdnrValue = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b2 = new Binding()
            {
                Path = new PropertyPath("FdnrValue"),
                Source = this,
                Converter = new EngrConverter() { ShortString = true },
                Mode = BindingMode.OneWay
            };
            txtFdnrValue.SetBinding(TextBlock.TextProperty, b2);
            txtFdnrValue.RenderTransform = new TranslateTransform() { };
            txtFdnrValue.SizeChanged += delegate { setPositionTextBoxes(); };
            CnvLabels.Children.Add(txtFdnrValue);

            setPositionTextBoxes();
        }

        private void setPositionTextBoxes()
        {
            double leftRV; double topRV;
            double leftCN; double topCN;

            if (actualRotation == 0)
            {
                topCN = 0;
                leftCN = (SchematicWidth / 2) - (txtComponentName.ActualWidth / 2);
                topRV = 60;
                leftRV = (SchematicWidth / 2) - (txtFdnrValue.ActualWidth / 2);
            }
            else if (actualRotation == 90)
            {
                topCN = 40;
                leftCN = 40 - (txtComponentName.ActualWidth);
                topRV = 40;
                leftRV = 80;
            }
            else if (actualRotation == 180)
            {
                topCN = 0;
                leftCN = -20 + (SchematicWidth / 2) - (txtComponentName.ActualWidth / 2);
                topRV = 60;
                leftRV = -20 + (SchematicWidth / 2) - (txtFdnrValue.ActualWidth / 2);
            }
            else
            {
                topCN = 20;
                leftCN = 40 - (txtComponentName.ActualWidth);
                topRV = 20;
                leftRV = 80;
            }

            ((TranslateTransform)txtComponentName.RenderTransform).X = leftCN;
            ((TranslateTransform)txtComponentName.RenderTransform).Y = topCN;

            ((TranslateTransform)txtFdnrValue.RenderTransform).X = leftRV;
            ((TranslateTransform)txtFdnrValue.RenderTransform).Y = topRV;
        }

        public void setPositionTextBoxes(double rotation)
        {
            rotation = rotation % 360;
          
            actualRotation = rotation;
            setPositionTextBoxes();
        }

        private void changeD(object sender, PropertyChangedEventArgs e)
        {
            FdnrValue = ComponentValueBox.ComponentValue;
        }


        public void setFlipX(bool flip)
        {
        }

        public void setFlipY(bool flip)
        {
        }
    }
}
