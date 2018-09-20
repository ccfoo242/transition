using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Transition.CircuitEditor.Serializable;
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
    public sealed partial class VoltageSourceComponentParameters : UserControl
    {

        public VoltageSource SerializableVoltageSource { get; }
        
        private int selectedVoltageFunctionType { get; set; }
        private int selectedImpedanceFunctionType { get; set; }
        

        public VoltageSourceComponentParameters()
        {
            this.InitializeComponent();
        }

        public VoltageSourceComponentParameters(VoltageSource vs)
        {
            SerializableVoltageSource = vs;
            DataContext = vs;
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
        
    }
}
