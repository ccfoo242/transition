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
    public sealed partial class Potentiometer : UserControl, IComponentParameter, INotifyPropertyChanged
    {
        public double SchematicWidth { get { return 120; } }
        public double SchematicHeight { get { return 160; } }

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

        public string ComponentLetter { get { return "P"; } }

        public Canvas CnvLabels { get; set; }

        public TextBlock txtComponentName;
        public TextBlock txtResistanceValue;
        public TextBlock txtPositionValue;

        public TextBlock txtCW;
        public TextBlock txtCCW;

        private double actualRotation = 0;
        private bool flipX;
        private bool flipY;

        private EngrNumber potentiometerValue;
        public EngrNumber PotentiometerValue
        {
            get { return potentiometerValue; }
            set
            {
                potentiometerValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PotentiometerValue"));
            }
        }


        private String positionValue;

        public String PositionValue
        {
            get { return positionValue; }
            set
            {
                positionValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PositionValue"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public Potentiometer()
        {
            this.InitializeComponent();
            init();
        }

        public void init()
        {
            componentValueBox.ComponentValue = EngrNumber.one();

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

            txtResistanceValue = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b2 = new Binding()
            {
                Path = new PropertyPath("PotentiometerValue"),
                Source = this,
                Mode = BindingMode.OneWay,
                Converter = new EngrConverter() { ShortString = true }
            };
            txtResistanceValue.SetBinding(TextBlock.TextProperty, b2);
            txtResistanceValue.RenderTransform = new TranslateTransform() { };
            txtResistanceValue.SizeChanged += delegate { setPositionTextBoxes(); };
            CnvLabels.Children.Add(txtResistanceValue);

            Binding b3 = new Binding()
            {
                Path = new PropertyPath("PositionValue"),
                Source = this,
                Mode = BindingMode.OneWay
            };

            txtPositionValue = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform() { }
            };

            txtPositionValue.SetBinding(TextBlock.TextProperty, b3);

            txtPositionValue.SizeChanged += delegate { setPositionTextBoxes(); };
            CnvLabels.Children.Add(txtPositionValue);

            txtCW = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                Text = "CW",
                RenderTransform = new TranslateTransform() { }
            };

            txtCCW = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                Text = "CCW",
                RenderTransform = new TranslateTransform() { }
            };

            CnvLabels.Children.Add(txtCW);
            CnvLabels.Children.Add(txtCCW);

            PositionValue = "0 %";

            setPositionTextBoxes();
        }

        public void setPositionTextBoxes()
        {
            double leftRV; double topRV;
            double leftCN; double topCN;
            double leftPV; double topPV;
            double leftCW; double topCW;
            double leftCCW; double topCCW;

            if (actualRotation == 0)
            {
                if (!flipX)
                {
                    topCN = 20;
                    leftCN = 60;
                    topRV = 40;
                    leftRV = 60;
                    topPV = 100;
                    leftPV = 60;


                    leftCW = 20 - txtCW.ActualWidth;
                    leftCCW = 20 - txtCCW.ActualWidth;
                    if (flipY)
                    {
                        topCCW = 40;
                        topCW = 100;
                    }
                    else
                    {
                        topCW = 40;
                        topCCW = 100;
                    }
                }
                else
                {
                    topCN = 20;
                    leftCN = 60 - txtComponentName.ActualWidth;
                    topRV = 40;
                    leftRV = 60 - txtResistanceValue.ActualWidth;
                    topPV = 100;
                    leftPV = 60 - txtPositionValue.ActualWidth;

                    leftCW = 100;
                    leftCCW = 100;

                    if (flipY)
                    {
                        topCCW = 40;
                        topCW = 100;
                    }
                    else
                    {
                        topCW = 40;
                        topCCW = 100;
                    }
                }

            }
            else if (actualRotation == 90)
            {
                if (!flipX)
                {
                    topCN = 20;
                    leftCN = 60 - (txtComponentName.ActualWidth / 2);
                    topRV = 80;
                    leftRV = 80;
                    topPV = 80;
                    leftPV = 0;

                    topCW = 20;
                    topCCW = 20;

                    if (flipY)
                    {
                        leftCW = 0;
                        leftCCW = 100;
                    }
                    else
                    {
                        leftCW = 100;
                        leftCCW = 0;
                    }
                }
                else
                {
                    topCN = 120;
                    leftCN = 60 - (txtComponentName.ActualWidth / 2);
                    topRV = 60;
                    leftRV = 80;
                    topPV = 60;
                    leftPV = 0;

                    topCW = 120;
                    topCCW = 120;

                    if (flipY)
                    {
                        leftCW = 0;
                        leftCCW = 100;
                    }
                    else
                    {
                        leftCW = 100;
                        leftCCW = 0;
                    }
                }
            }
            else if (actualRotation == 180)
            {
                if (!flipX)
                {
                    topCN = 20;
                    leftCN = 60 - txtComponentName.ActualWidth;
                    topRV = 40;
                    leftRV = 60 - txtResistanceValue.ActualWidth;
                    topPV = 100;
                    leftPV = 60 - txtPositionValue.ActualWidth;

                    leftCW = 100;
                    leftCCW = 100;

                    if (flipY)
                    {
                        topCCW = 100;
                        topCW = 40;
                    }
                    else
                    {
                        topCW = 100;
                        topCCW = 40;
                    }
                }
                else
                {
                    topCN = 20;
                    leftCN = 60;
                    topRV = 40;
                    leftRV = 60;
                    topPV = 100;
                    leftPV = 60;

                    leftCW = 20 - txtCW.ActualWidth;
                    leftCCW = 20 - txtCCW.ActualWidth;

                    if (flipY)
                    {
                        topCCW = 100;
                        topCW = 40;
                    }
                    else
                    {
                        topCW = 100;
                        topCCW = 40;
                    }
                }
            }
            else
            {
                if (!flipX)
                {
                    topCN = 110;
                    leftCN = 60 - (txtComponentName.ActualWidth / 2);
                    topRV = 60;
                    leftRV = 80;
                    topPV = 60;
                    leftPV = 0;

                    topCW = 110;
                    topCCW = 110;

                    if (flipY)
                    {
                        leftCW = 80;
                        leftCCW = 0;
                    }
                    else
                    {
                        leftCW = 0;
                        leftCCW = 80;
                    }
                }
                else
                {
                    topCN = 20;
                    leftCN = 60 - (txtComponentName.ActualWidth / 2);
                    topRV = 80;
                    leftRV = 80;
                    topPV = 80;
                    leftPV = 0;

                    topCW = 20;
                    topCCW = 20;

                    if (flipY)
                    {
                        leftCW = 80;
                        leftCCW = 0;
                    }
                    else
                    {
                        leftCW = 0;
                        leftCCW = 80;
                    }
                }
            }

            ((TranslateTransform)txtComponentName.RenderTransform).X = leftCN;
            ((TranslateTransform)txtComponentName.RenderTransform).Y = topCN;
            ((TranslateTransform)txtPositionValue.RenderTransform).X = leftPV;
            ((TranslateTransform)txtPositionValue.RenderTransform).Y = topPV;
            ((TranslateTransform)txtResistanceValue.RenderTransform).X = leftRV;
            ((TranslateTransform)txtResistanceValue.RenderTransform).Y = topRV;

            ((TranslateTransform)txtCW.RenderTransform).X = leftCW;
            ((TranslateTransform)txtCW.RenderTransform).Y = topCW;
            ((TranslateTransform)txtCCW.RenderTransform).X = leftCCW;
            ((TranslateTransform)txtCCW.RenderTransform).Y = topCCW;

        }

        public void setPositionTextBoxes(double rotation)
        {
            rotation = rotation % 360;
            actualRotation = rotation;

            setPositionTextBoxes();
        }


        private void changeR(object sender, PropertyChangedEventArgs e)
        {
            PotentiometerValue = componentValueBox.ComponentValue;
        }

        private void ClickCCW(object sender, RoutedEventArgs e)
        {
            sldPosition.Value = 0;
        }

        private void positionChange(object sender, RangeBaseValueChangedEventArgs e)
        {
            PositionValue = sldPosition.Value.ToString() + " %";
        }

        private void ClickCenter(object sender, RoutedEventArgs e)
        {
            sldPosition.Value = 50;
        }

        private void ClickCW(object sender, RoutedEventArgs e)
        {
            sldPosition.Value = 100;
        }

        public void setFlipX(bool flip)
        {
            flipX = flip;
            setPositionTextBoxes();
        }

        public void setFlipY(bool flip)
        {
            flipY = flip;
            setPositionTextBoxes();
        }
    }
}
