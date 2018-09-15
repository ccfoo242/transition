using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Transition.CircuitEditor.SerializableModels;
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
    public sealed partial class InductorParametersControl : UserControl
    {
        public Inductor SerializableInductor { get; }
        private int SelectedInductorModel { get { return SerializableInductor.InductorModel; } }

        public InductorParametersControl()
        {
            this.InitializeComponent();
           
        }

        public InductorParametersControl(Inductor inductor)
        {
            SerializableInductor = inductor;
            DataContext = inductor;

            this.InitializeComponent();
            
           
        }
        
        private void modelInductorChanged(object sender, SelectionChangedEventArgs e)
        {
            pnlExponential.Visibility = Visibility.Collapsed;
            pnlParasitic.Visibility = Visibility.Collapsed;

            if (SelectedInductorModel == 1)
                pnlParasitic.Visibility = Visibility.Visible;

            if (SelectedInductorModel == 2)
                pnlExponential.Visibility = Visibility.Visible;
        }
        
    }
}
