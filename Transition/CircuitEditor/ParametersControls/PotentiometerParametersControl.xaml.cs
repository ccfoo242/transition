using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Transition.CircuitEditor.Serializable;
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
    public sealed partial class PotentiometerParametersControl : UserControl
    { 
        private Potentiometer SerializablePotentioMeter { get; }

        public PotentiometerParametersControl()
        {
            this.InitializeComponent();
           
        }

        public PotentiometerParametersControl(Potentiometer pot)
        {
            this.InitializeComponent();

            SerializablePotentioMeter = pot;
            DataContext = pot;
        }

        private void ClickCCW(object sender, RoutedEventArgs e)
        {
            sldPosition.Value = 0;
        }

        private void ClickCenter(object sender, RoutedEventArgs e)
        {
            sldPosition.Value = 50;
        }

        private void ClickCW(object sender, RoutedEventArgs e)
        {
            sldPosition.Value = 100;
        }
    }
}
