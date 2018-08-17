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
    public sealed partial class Transformer : UserControl, IComponentParameter, INotifyPropertyChanged
    {

        public double SchematicWidth { get { return 120; } }
        public double SchematicHeight { get { return 140; } }

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

    
        private double actualRotation = 0;
        private bool flipX;

        public string ComponentLetter { get { return "T"; } }
        public Canvas CnvLabels { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public TextBlock txtPri;
        public TextBlock txtSec;
        public TextBlock txtComponentName;
        public TextBlock txtTurnsRatio;

        public EngrNumber TurnsRatio
        {
            get { return turnsRatio; }
            set
            {
                turnsRatio = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TurnsRatio"));
               
            }
        }
        private EngrNumber turnsRatio;

        public double KCouplingCoef
        {
            get { return (double)GetValue(KCouplingCoefProperty); }
            set { SetValue(KCouplingCoefProperty, value);
               
            }
        }

        public static readonly DependencyProperty KCouplingCoefProperty =
        DependencyProperty.Register("KCouplingCoef",
            typeof(double), typeof(Transformer), new PropertyMetadata(0));



        public EngrNumber Lpri
        {
            get { return lpri; }
            set
            {
                lpri = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Lpri"));
                updateM();
            }
        }
        private EngrNumber lpri;

        public EngrNumber Lsec
        {
            get { return lsec; }
            set
            {
                lsec = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Lsec"));
                updateM();
            }
        }
        private EngrNumber lsec;


        public EngrNumber MutualL
        {
            get { return mutualL; }
            set
            {
                mutualL = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MutualL"));
            }
        }
        private EngrNumber mutualL;


        public EngrNumber LpLeak
        {
            get { return lpLeak; }
            set
            {
                lpLeak = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LpLeak"));
            }
        }
        private EngrNumber lpLeak;

        public EngrNumber LsLeak
        {
            get { return lsLeak; }
            set
            {
                lsLeak = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LsLeak"));
            }
        }
        private EngrNumber lsLeak;


        public Transformer()
        {
            this.InitializeComponent();
            init();
        }

        private void init()
        {
            Binding b0 = new Binding()
            {
                Path = new PropertyPath("Value"),
                Source = sldKCouplingCoef,
                Mode = BindingMode.TwoWay
            };
            this.SetBinding(KCouplingCoefProperty, b0);

            KCouplingCoef = 1;
            CnvLabels = new Canvas();

            Lpri = EngrNumber.one();
            Lsec = EngrNumber.one();
            
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

            txtTurnsRatio = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                RenderTransform = new TranslateTransform()
            };

            Binding b2 = new Binding()
            {
                Path = new PropertyPath("TurnsRatio"),
                Source = this,
                Converter = new EngrConverter() { ShortString = true, AllowNegativeNumber = false },
                Mode = BindingMode.OneWay
            };
            txtTurnsRatio.SetBinding(TextBlock.TextProperty, b2);
            txtTurnsRatio.SizeChanged += delegate { setPositionTextBoxes(); };
            CnvLabels.Children.Add(txtTurnsRatio);

            txtPri = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                Text = "Pri",
                RenderTransform = new TranslateTransform()
            };
            CnvLabels.Children.Add(txtPri);

            txtSec = new TextBlock()
            {
                FontWeight = FontWeights.ExtraBold,
                Text = "Sec",
                RenderTransform = new TranslateTransform()
            };
            CnvLabels.Children.Add(txtSec);


            setPositionTextBoxes();
        }

        public void setFlipX(bool flip)
        {
            flipX = flip;
            setPositionTextBoxes();
    }

        public void setFlipY(bool flip)
        {
        }

        public void setPositionTextBoxes(double rotation)
        {
            actualRotation = rotation % 360;
            setPositionTextBoxes();
        }

        public void setPositionTextBoxes()
        {
            double leftTR; double topTR;
            double leftCN; double topCN;
            double leftPri; double topPri;
            double leftSec; double topSec;

            if (actualRotation == 0)
            {
                leftCN = (SchematicWidth / 2) - (txtComponentName.ActualWidth / 2);
                topCN = -20;
                leftTR = (SchematicWidth / 2) - (txtTurnsRatio.ActualWidth / 2);
                topTR = 0;

                topPri = (SchematicHeight / 2) - (txtPri.ActualHeight / 2);
                topSec = (SchematicHeight / 2) - (txtSec.ActualHeight / 2);

                if (!flipX)
                {
                    leftPri = 0;
                    leftSec = 100;
                }
                else
                {
                    leftPri = 100;
                    leftSec = 0;
                }
            }
            else if (actualRotation == 90)
            {
                leftCN = -1 * txtComponentName.ActualWidth;
                topCN = (SchematicHeight / 2) - (txtComponentName.ActualHeight / 2) - 10;
                leftTR = 100;
                topTR = (SchematicHeight / 2) - (txtTurnsRatio.ActualHeight / 2) - 10;

                leftPri = (SchematicWidth / 2) - (txtPri.ActualWidth / 2) - 10;
                leftSec = (SchematicWidth / 2) - (txtSec.ActualWidth / 2) - 10;

                if (!flipX)
                {
                    topPri = 0;
                    topSec = 100;
                }
                else
                {
                    topPri = 100;
                    topSec = 0;
                }
            }
            else if (actualRotation == 180)
            {
                leftCN = (SchematicWidth / 2) - (txtComponentName.ActualWidth / 2);
                topCN = -40;
                leftTR = (SchematicWidth / 2) - (txtTurnsRatio.ActualWidth / 2);
                topTR = -20;

                topPri = (SchematicHeight / 2) - (txtPri.ActualHeight / 2) - 20;
                topSec = (SchematicHeight / 2) - (txtSec.ActualHeight / 2) - 20;

                if (!flipX)
                {
                    leftPri = 100;
                    leftSec = 0;
                }
                else
                {
                    leftPri = 0;
                    leftSec = 100;
                }

            }
            else
            {
                leftCN = (-1 * txtComponentName.ActualWidth) + 20;
                topCN = (SchematicHeight / 2) - (txtComponentName.ActualHeight / 2) - 10;
                leftTR = 120;
                topTR = (SchematicHeight / 2) - (txtTurnsRatio.ActualHeight / 2) - 10;

                leftPri = (SchematicWidth / 2) - (txtPri.ActualWidth / 2) + 10;
                leftSec = (SchematicWidth / 2) - (txtSec.ActualWidth / 2) + 10;

                if (!flipX)
                {
                    topPri = 100;
                    topSec = 0;
                }
                else
                {
                    topPri = 0;
                    topSec = 100;
                }
            }

            ((TranslateTransform)txtComponentName.RenderTransform).X = leftCN;
            ((TranslateTransform)txtComponentName.RenderTransform).Y = topCN;

            ((TranslateTransform)txtTurnsRatio.RenderTransform).X = leftTR;
            ((TranslateTransform)txtTurnsRatio.RenderTransform).Y = topTR;

            ((TranslateTransform)txtPri.RenderTransform).X = leftPri;
            ((TranslateTransform)txtPri.RenderTransform).Y = topPri;

            ((TranslateTransform)txtSec.RenderTransform).X = leftSec;
            ((TranslateTransform)txtSec.RenderTransform).Y = topSec;

        }

        private void changeTR(object sender, PropertyChangedEventArgs e)
        {
            TurnsRatio = boxTR.Value;

            double tr = TurnsRatio.ValueDouble;
            double lp = Lpri.ValueDouble;

            double ls = tr * tr * lp;
            Lsec = new EngrNumber(ls);
        }

        private void changeLpri(object sender, PropertyChangedEventArgs e)
        {
            Lpri = BoxLpri.Value;

            double lp = Lpri.ValueDouble;
            double tr = TurnsRatio.ValueDouble;

            double ls = tr * tr * lp;
            Lsec = new EngrNumber(ls);
        }


        private void changeLsec(object sender, PropertyChangedEventArgs e)
        {
            Lsec = BoxLsec.Value;

            double ls = Lsec.ValueDouble;
            double lp = Lpri.ValueDouble;

            double tr = Math.Sqrt(ls / lp);
            TurnsRatio = new EngrNumber(tr);
        }

        private void updateM()
        {
            double lp = Lpri.ValueDouble;
            double ls = Lsec.ValueDouble;
            double k = KCouplingCoef;

            MutualL = new EngrNumber(k * Math.Sqrt(lp * ls));
        }

        private void changeK(object sender, RangeBaseValueChangedEventArgs e)
        {
            updateM();
        }
    }
}
