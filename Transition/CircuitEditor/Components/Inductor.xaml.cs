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
    public sealed partial class Inductor : UserControl, IComponentParameter, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public double SchematicWidth { get { return 140; } }
        public double SchematicHeight { get { return 80; } }
        public String ComponentLetter { get { return "L"; } }

        public ElectricComponent ec;

        public TextBlock txtComponentName;
        public TextBlock txtInductanceValue;

        private double actualRotation = 0;

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

        private EngrNumber inductorValue;
        public EngrNumber InductorValue
        {
            get { return inductorValue; }
            set
            {
                inductorValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InductorValue"));
            }
        }
        // 0 = ideal, 1 = parasitic, 2 = exponent
        public int selectedInductorModel = 0;

        public Canvas CnvLabels { get; set; }

        private EngrNumber fo;
        private EngrNumber rs;
        private EngrNumber cp;
        private EngrNumber q;

        public EngrNumber Fo
        {
            get { return fo; }
            set
            {
                fo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Fo"));
            }
        }

        public EngrNumber Q
        {
            get { return q; }
            set
            {
                q = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Q"));
            }
        }

        public EngrNumber Rs
        {
            get { return rs; }
            set
            {
                rs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rs"));
            }
        }

        public EngrNumber Cp
        {
            get { return cp; }
            set
            {
                cp = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cp"));
            }
        }

        public Inductor()
        {
            this.InitializeComponent();
            init();

        }

        public Inductor(ElectricComponent ec)
        {
            this.InitializeComponent();

            this.ec = ec;
            init();
        }

        private void init()
        {
            Rs = new EngrNumber(1, "p");
            Cp = new EngrNumber(1, "p");
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


            txtInductanceValue = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b2 = new Binding()
            {
                Path = new PropertyPath("InductorValue"),
                Source = this,
                Converter = new EngrShortConverter(),
                Mode = BindingMode.OneWay
            };
            txtInductanceValue.SetBinding(TextBlock.TextProperty, b2);
            txtInductanceValue.RenderTransform = new TranslateTransform() { };
            txtInductanceValue.SizeChanged += delegate { setPositionTextBoxes(); };
            CnvLabels.Children.Add(txtInductanceValue);

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
                leftRV = (SchematicWidth / 2) - (txtInductanceValue.ActualWidth / 2);
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
                leftRV = -20 + (SchematicWidth / 2) - (txtInductanceValue.ActualWidth / 2);
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

            ((TranslateTransform)txtInductanceValue.RenderTransform).X = leftRV;
            ((TranslateTransform)txtInductanceValue.RenderTransform).Y = topRV;
        }

        public void setPositionTextBoxes(double rotation)
        {
            rotation = rotation % 360;
            actualRotation = rotation;
            setPositionTextBoxes();
        }

        private void changeRs(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Rs = BoxRs.Value;
            calculateFoQ();
        }

        private void changeCp(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Cp = BoxCp.Value;
            calculateFoQ();
        }

        private void changeFo(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Fo = BoxFo.Value;
            calculateRsCp();
        }

        private void changeQ(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Q = BoxQ.Value;
            calculateRsCp();
        }

        private void calculateFoQ()
        {
            double dL = inductorValue.ValueDouble;
            double dRs = Rs.ValueDouble;
            double dCp = Cp.ValueDouble;

            double dWop = Math.Sqrt(1 / (dL * dCp));

            double dQ = (dWop * dL) / dRs;
            double dFo = dWop / (2 * Math.PI);

            Fo = new EngrNumber(dFo);
            Q = new EngrNumber(dQ);

        }

        private void calculateRsCp()
        {
            double dQ = Q.ValueDouble;
            double dFo = Fo.ValueDouble;
            double dL = inductorValue.ValueDouble;

            double dWo = 2 * Math.PI * dFo;

            double dRs = (dWo * dL) / dQ;
            double dCp = 1 / (dL * dWo * dWo);

            Rs = new EngrNumber(dRs);
            Cp = new EngrNumber(dCp);

        }

        private void changeL(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InductorValue = ComponentValueBox.ComponentValue;
            calculateFoQ();
        }

        private void modelInductorChanged(object sender, SelectionChangedEventArgs e)
        {
            pnlExponential.Visibility = Visibility.Collapsed;
            pnlParasitic.Visibility = Visibility.Collapsed;

            if (selectedInductorModel == 1)
                pnlParasitic.Visibility = Visibility.Visible;

            if (selectedInductorModel == 2)
                pnlExponential.Visibility = Visibility.Visible;
        }

        public void setFlipX(bool flip)
        {
        }

        public void setFlipY(bool flip)
        {
        }
    }
}
