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
    public sealed partial class ResistorParametersControl : UserControl, INotifyPropertyChanged
    {
        
        private Resistor SerializableResistor { get; }
       
        private int SelectedResistorModel { get { return SerializableResistor.ResistorModel; } }
        public event PropertyChangedEventHandler PropertyChanged;

        public ResistorParametersControl()
        {
            InitializeComponent();
        }

        public ResistorParametersControl(Resistor resistor)
        {
            InitializeComponent();

            SerializableResistor = resistor;
            DataContext = resistor;
        }
        

        private void modelResistorChanged(object sender, SelectionChangedEventArgs e)
        {
            pnlExponential.Visibility = Visibility.Collapsed;
            pnlParasitic.Visibility = Visibility.Collapsed;

            if (SelectedResistorModel == 1)
                pnlParasitic.Visibility = Visibility.Visible;

            if (SelectedResistorModel == 2)
                pnlExponential.Visibility = Visibility.Visible;
        }
        
    }
}
