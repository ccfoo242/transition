using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Transition.Functions;
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
    public sealed partial class VoltageSource : UserControl, IComponentParameter, INotifyPropertyChanged
    {
       
        public double SchematicWidth  { get { return 120; } }
        public double SchematicHeight { get { return 120; } }


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

        public string ComponentLetter { get { return "V"; } }

        public Canvas CnvLabels { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool flipX;
        private bool flipY;
        private double actualRotation;

        public Function OutputVoltageFrequencyFunction;
        public Function OutputImpedanceFrequencyFunction;

        private int selectedVoltageFunctionType { get; set; }
        private int selectedImpedanceFunctionType { get; set; }

        public TextBlock txtComponentName;
        public TextBlock txtVoltage;
        public TextBlock txtImpedance;

        public VoltageSource()
        {
            this.InitializeComponent();
            init();
        }

        public void init()
        {
            CnvLabels = new Canvas();

            OutputVoltageFrequencyFunction = new ConstantValueFunction(1);
            OutputImpedanceFrequencyFunction = new ConstantValueFunction(1E-6);

            boxConstVoltage.Value = new EngrNumber(1);
            boxConstImpedance.Value = new EngrNumber(1, "u");

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

            txtVoltage = new TextBlock()
            {

            };

            txtImpedance = new TextBlock()
            {

            };
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

        public void setRotation(double rotation)
        {
            actualRotation = rotation % 360;
            setPositionTextBoxes();
        }

        public void setPositionTextBoxes()
        {
            
        }
        
        private void voltageFunctionTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            pnlVoltageConstant.Visibility = Visibility.Collapsed;
            pnlVoltageLibraryCurve.Visibility = Visibility.Collapsed;

            if (selectedVoltageFunctionType == 0)
                pnlVoltageConstant.Visibility = Visibility.Visible;
            else
                pnlVoltageLibraryCurve.Visibility = Visibility.Visible;
        }

        private void impedanceFunctionTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            pnlImpedanceConstant.Visibility = Visibility.Collapsed;
            pnlImpedanceLibraryCurve.Visibility = Visibility.Collapsed;

            if (selectedImpedanceFunctionType == 0)
                pnlImpedanceConstant.Visibility = Visibility.Visible;
            else
                pnlImpedanceLibraryCurve.Visibility = Visibility.Visible;
        }

        private void changeConstVoltage(object sender, PropertyChangedEventArgs e)
        {
            OutputVoltageFrequencyFunction = new ConstantValueFunction(
                boxConstVoltage.Value.ValueDouble);
        }

        private void changeConstImpedance(object sender, PropertyChangedEventArgs e)
        {
            OutputImpedanceFrequencyFunction = new ConstantValueFunction(
                boxConstImpedance.Value.ValueDouble);
        }
    }
}
