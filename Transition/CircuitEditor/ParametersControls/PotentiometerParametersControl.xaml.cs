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

            pot.TerminalsChanged += quantityOfTerminalsChanged;
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

        private void cmbSelectedQuantityOfTerminalsChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cmbQuantityOfTerminals.SelectedIndex)
            {
                case 0: SerializablePotentioMeter.QuantityOfTerminals = 3; break;
                case 1: SerializablePotentioMeter.QuantityOfTerminals = 4; break;
                case 2: SerializablePotentioMeter.QuantityOfTerminals = 5; break;
                case 3: SerializablePotentioMeter.QuantityOfTerminals = 6; break;
            }
        }

        private void quantityOfTerminalsChanged()
        {
            switch (SerializablePotentioMeter.QuantityOfTerminals)
            {
                case 3: cmbQuantityOfTerminals.SelectedIndex = 0; break;
                case 4: cmbQuantityOfTerminals.SelectedIndex = 1; break;
                case 5: cmbQuantityOfTerminals.SelectedIndex = 2; break;
                case 6: cmbQuantityOfTerminals.SelectedIndex = 3; break;

            }
        }
    }
}
