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
    public sealed partial class InductorParametersControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Inductor SerializableInductor { get; }
        
        public InductorParametersControl()
        {
            this.InitializeComponent();
           
        }

        public InductorParametersControl(Inductor inductor)
        {
            SerializableInductor = inductor;
            this.InitializeComponent();
            
           
        }

        private void init()
        {

            setPositionTextBoxes();
        }

        private void setPositionTextBoxes()
        {
            
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
        
    }
}
