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
    public sealed partial class Capacitor : UserControl, IComponentParameter, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public double SchematicWidth { get { return 120; } }
        public double SchematicHeight { get { return 80; } }
        public String ComponentLetter { get { return "C"; } }

        public ElectricComponent ec;

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

        private EngrNumber capacitorValue;
        public EngrNumber CapacitorValue
        {
            get { return capacitorValue; }
            set
            {
                capacitorValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CapacitorValue"));
            }
        }

        // 0 = ideal, 1 = parasitic, 2 = exponent
        public int selectedCapacitorModel = 0;

        public TextBlock txtComponentName;
        public TextBlock txtCapacitanceValue;

        private double actualRotation = 0;

        private EngrNumber ls;
        private EngrNumber rp;
        private EngrNumber rs;
        private EngrNumber q;
        private EngrNumber fo;

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

        public EngrNumber Ls
        {
            get { return ls; }
            set
            {
                ls = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Ls"));
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

        public EngrNumber Rp
        {
            get { return rp; }
            set
            {
                rp = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rp"));
            }
        }

        public Canvas CnvLabels { get; set; }

        public Capacitor()
        {
            this.InitializeComponent();
            init();
        }

        public Capacitor(ElectricComponent ec)
        {
            this.InitializeComponent();

            this.ec = ec;
            init();
        }

        private void init()
        {

            Ls = new EngrNumber(1, "p");
            Rp = new EngrNumber(1, "T");
            Rs = new EngrNumber(1, "u");
            componentValueBox.ComponentValue = EngrNumber.One;


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


            txtCapacitanceValue = new TextBlock() { FontWeight = FontWeights.ExtraBold };
            Binding b2 = new Binding()
            {
                Path = new PropertyPath("ValueString"),
                Source = componentValueBox,
                Mode = BindingMode.OneWay
            };
            txtCapacitanceValue.SetBinding(TextBlock.TextProperty, b2);
            txtCapacitanceValue.RenderTransform = new TranslateTransform() { };
            txtCapacitanceValue.SizeChanged += delegate { setPositionTextBoxes(); };
            CnvLabels.Children.Add(txtCapacitanceValue);

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

        public void setRotation(double rotation)
        {
            rotation = rotation % 360;

            actualRotation = rotation;
            setPositionTextBoxes();
        }

        private void changeLs(object sender, PropertyChangedEventArgs e)
        {
            Ls = BoxLs.Value;
            calculateFoQ();
        }

        private void changeRp(object sender, PropertyChangedEventArgs e)
        {
            Rp = BoxRp.Value;
        }

        private void changeRs(object sender, PropertyChangedEventArgs e)
        {
            Rs = BoxRs.Value;
            calculateFoQ();
        }

        private void changeFo(object sender, PropertyChangedEventArgs e)
        {
            Fo = BoxFo.Value;
            calculateRsLs();
        }

        private void changeQ(object sender, PropertyChangedEventArgs e)
        {
            Q = BoxQ.Value;
            calculateRsLs();
        }

        private void calculateFoQ()
        {
            double dC = capacitorValue.ValueDouble;
            double dLs = Ls.ValueDouble;
            double dRs = Rs.ValueDouble;

            double dWo = Math.Sqrt(1 / (dLs * dC));

            double dQ = (dWo * dLs) / dRs;
            double dFo = dWo / (2 * Math.PI);

            Fo = new EngrNumber(dFo);
            Q = new EngrNumber(dQ);
        }

        private void calculateRsLs()
        {
            double dQ = Q.ValueDouble;
            double dFo = Fo.ValueDouble;
            double dC = capacitorValue.ValueDouble;

            double dWo = 2 * Math.PI * dFo;

            //  double dLs = Math.Sqrt(Math.Abs( ((dQ * dQ * dR * dR) - (dR * dR)) / Math.Pow(dWo, 2) ));
            double dRs = 1 / (dQ * dWo * dC);
            double dLs = 1 / (dC * dWo * dWo);

            Ls = new EngrNumber(dLs);
            Rs = new EngrNumber(dRs);
        }

        private void changeC(object sender, PropertyChangedEventArgs e)
        {
            CapacitorValue = componentValueBox.ComponentValue;
            calculateFoQ();
        }

        private void modelCapacitorChanged(object sender, SelectionChangedEventArgs e)
        {
            pnlExponential.Visibility = Visibility.Collapsed;
            pnlParasitic.Visibility = Visibility.Collapsed;

            if (selectedCapacitorModel == 1)
                pnlParasitic.Visibility = Visibility.Visible;

            if (selectedCapacitorModel == 2)
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
