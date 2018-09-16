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
    public class PotentiometerScreen : ScreenComponentBase
    {
        public override double SchematicWidth => 120;
        public override double SchematicHeight => 160;

        public TextBlock txtCW;
        public TextBlock txtCCW;
        public TextBlock txtResistanceValue;
        public TextBlock txtPositionValue;
        public TextBlock txtComponentName;

        public ContentControl SymbolPotentiometer { get; }

        public PotentiometerScreen(Potentiometer pot) : base(pot)
        {
            SymbolPotentiometer = new ContentControl()
            {
                ContentTemplate = (DataTemplate)Application.Current.Resources["symbolPotentiometer"]
            };
            ComponentCanvas.Children.Add(SymbolPotentiometer);
            SetTop(SymbolPotentiometer, 20);
            SetLeft(SymbolPotentiometer, 20);

            txtComponentName = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform()
            };

            txtResistanceValue = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform()
            };

            txtPositionValue = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform()
            };

            txtCW = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform(),
                Text = "CW"
            };
            Children.Add(txtCW);

            txtCCW = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform(),
                Text = "CCW"
            };
            Children.Add(txtCCW);

            Binding b1 = new Binding()
            {
                Path = new PropertyPath("ComponentName"),
                Mode = BindingMode.OneWay
            };
            txtComponentName.SetBinding(TextBlock.TextProperty, b1);
            txtComponentName.SizeChanged += delegate { setPositionTextBoxes(); };
            Children.Add(txtComponentName);


            Binding b2 = new Binding()
            {
                Path = new PropertyPath("ResistanceValue"),
                Mode = BindingMode.OneWay,
                Converter = new EngrConverter() { AllowNegativeNumber = false }
            };
            txtResistanceValue.SetBinding(TextBlock.TextProperty, b2);
            txtResistanceValue.SizeChanged += delegate { setPositionTextBoxes(); };
            Children.Add(txtResistanceValue);


            Binding b3 = new Binding()
            {
                Path = new PropertyPath("PositionValue"),
                Mode = BindingMode.OneWay,
                Converter = new PotValueConverter() 
            };
            txtPositionValue.SetBinding(TextBlock.TextProperty, b3);
            txtPositionValue.SizeChanged += delegate { setPositionTextBoxes(); };
            Children.Add(txtPositionValue);

            postConstruct();
        }

        public override void setPositionTextBoxes()
        {

            double leftRV; double topRV;
            double leftCN; double topCN;
            double leftPV; double topPV;
            double leftCW; double topCW;
            double leftCCW; double topCCW;

            if (ActualRotation == 0)
            {
                if (!FlipX)
                {
                    topCN = 20;
                    leftCN = 60;
                    topRV = 40;
                    leftRV = 60;
                    topPV = 100;
                    leftPV = 60;


                    leftCW = 20 - txtCW.ActualWidth;
                    leftCCW = 20 - txtCCW.ActualWidth;
                    if (FlipY)
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

                    if (FlipY)
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
            else if (ActualRotation == 90)
            {
                if (!FlipX)
                {
                    topCN = 20;
                    leftCN = 60 - (txtComponentName.ActualWidth / 2);
                    topRV = 80;
                    leftRV = 80;
                    topPV = 80;
                    leftPV = 0;

                    topCW = 20;
                    topCCW = 20;

                    if (FlipY)
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

                    if (FlipY)
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
            else if (ActualRotation == 180)
            {
                if (!FlipX)
                {
                    topCN = 20;
                    leftCN = 60 - txtComponentName.ActualWidth;
                    topRV = 40;
                    leftRV = 60 - txtResistanceValue.ActualWidth;
                    topPV = 100;
                    leftPV = 60 - txtPositionValue.ActualWidth;

                    leftCW = 100;
                    leftCCW = 100;

                    if (FlipY)
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

                    if (FlipY)
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
                if (!FlipX)
                {
                    topCN = 110;
                    leftCN = 60 - (txtComponentName.ActualWidth / 2);
                    topRV = 60;
                    leftRV = 80;
                    topPV = 60;
                    leftPV = 0;

                    topCW = 110;
                    topCCW = 110;

                    if (FlipY)
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

                    if (FlipY)
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
    }

    public class PotValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //number to string
            if (value == null) return "";
            double dvalue = (double)value;
            decimal devalue = Decimal.Round((decimal)dvalue,2);
            
            return devalue.ToString() + " %";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
