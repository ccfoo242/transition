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
    public sealed partial class Resistor : UserControl, IComponentParameter, INotifyPropertyChanged
    {
        public double SchematicWidth { get { return 140; } }
        public double SchematicHeight { get { return 80; } }
        public Canvas CnvLabels { get; set; }

        public double actualRotation;
        private ElectricComponent ec;
        public string ComponentLetter { get { return "R"; } }

        public TextBlock txtComponentName;
        public TextBlock txtResistanceValue;

        public int selectedResistorModel = 0;

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
        
        private EngrNumber resistorValue;
        public EngrNumber ResistorValue
        {
            get { return resistorValue; }
            set
            {
                resistorValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResistorValue"));
            }
        }

        private EngrNumber fo;
        private EngrNumber ls;
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

        public EngrNumber Ls
        {
            get { return ls; }
            set
            {
                ls = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Ls"));
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

        public event PropertyChangedEventHandler PropertyChanged;

        public Resistor()
        {
            this.InitializeComponent();
            init();
        }

        public Resistor(ElectricComponent ec)
        {
            this.InitializeComponent();

            this.ec = ec;
            init();

        }

        private void init()
        {
            Ls = new EngrNumber(1, "p");
            Cp = new EngrNumber(1, "p");
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
                Path = new PropertyPath("ValueString"),
                Source = componentValueBox,
                Mode = BindingMode.OneWay
            };
            txtResistanceValue.SetBinding(TextBlock.TextProperty, b2);
            txtResistanceValue.RenderTransform = new TranslateTransform() { };
            txtResistanceValue.SizeChanged += delegate { setPositionTextBoxes(); };
            CnvLabels.Children.Add(txtResistanceValue);

            setPositionTextBoxes();
        }

        public void setPositionTextBoxes()
        {
            double leftRV; double topRV;
            double leftCN; double topCN;

            if (actualRotation == 0)
            {
                topCN = 0;
                leftCN = (SchematicWidth / 2) - (txtComponentName.ActualWidth / 2);
                topRV = 60;
                leftRV = (SchematicWidth / 2) - (txtResistanceValue.ActualWidth / 2);
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
                leftRV = -20 + (SchematicWidth / 2) - (txtResistanceValue.ActualWidth / 2);
            }
            else
            {
                topCN = 20;
                leftCN = 40 - (txtComponentName.ActualWidth);
                topRV = 20;
                leftRV = 80;
            }

            /* I use a Transform because it allows to position the visual objects
             outside de Canvas (or other layouts) without affecting the
             layout itself, that is useful if it is required to position
             a textbox outside the control itself. */

            ((TranslateTransform)txtComponentName.RenderTransform).X = leftCN;
            ((TranslateTransform)txtComponentName.RenderTransform).Y = topCN;

            ((TranslateTransform)txtResistanceValue.RenderTransform).X = leftRV;
            ((TranslateTransform)txtResistanceValue.RenderTransform).Y = topRV;
        }

        public void setPositionTextBoxes(double rotation)
        {
            rotation = rotation % 360;

            actualRotation = rotation;
            setPositionTextBoxes();
        }

        public void setFlipX(bool flip)
        {
        }

        public void setFlipY(bool flip)
        {
        }
        

        private void changeLs(object sender, PropertyChangedEventArgs e)
        {
            Ls = BoxLs.Value;
            calculateFoQ();
        }

        private void changeCp(object sender, PropertyChangedEventArgs e)
        {
            Cp = BoxCp.Value;
            calculateFoQ();
        }

        private void changeFo(object sender, PropertyChangedEventArgs e)
        {
            Fo = BoxFo.Value;
            calculateLsCp();
        }

        private void changeQ(object sender, PropertyChangedEventArgs e)
        {
            Q = BoxQ.Value;
            calculateLsCp();
        }

        private void calculateFoQ()
        {
            double dR = ResistorValue.ValueDouble;
            double dLs = Ls.ValueDouble;
            double dCp = Cp.ValueDouble;

            //  double dWo = Math.Sqrt(Math.Abs((1 / (dLs * dCp)) - Math.Pow(dR / dLs, 2)));
            double dWop = Math.Sqrt(1 / (dLs * dCp));


            double dQ = (dWop * dLs) / dR;
            double dFo = dWop / (2 * Math.PI);

            Fo = new EngrNumber(dFo);
            Q = new EngrNumber(dQ);

        }

        private void calculateLsCp()
        {
            double dQ = Q.ValueDouble;
            double dFo = Fo.ValueDouble;
            double dR = ResistorValue.ValueDouble;

            double dWo = 2 * Math.PI * dFo;

            //  double dLs = Math.Sqrt(Math.Abs( ((dQ * dQ * dR * dR) - (dR * dR)) / Math.Pow(dWo, 2) ));
            double dLs = dR * dQ / dWo;
            double dCp = dLs / (dQ * dQ * dR * dR);

            Ls = new EngrNumber(dLs);
            Cp = new EngrNumber(dCp);

        }

        private void changeR(object sender, PropertyChangedEventArgs e)
        {
            ResistorValue = componentValueBox.ComponentValue;
            calculateFoQ();
        }


        private void modelResistorChanged(object sender, SelectionChangedEventArgs e)
        {
            pnlExponential.Visibility = Visibility.Collapsed;
            pnlParasitic.Visibility = Visibility.Collapsed;

            if (selectedResistorModel == 1)
                pnlParasitic.Visibility = Visibility.Visible;

            if (selectedResistorModel == 2)
                pnlExponential.Visibility = Visibility.Visible;
        }

    }
}
