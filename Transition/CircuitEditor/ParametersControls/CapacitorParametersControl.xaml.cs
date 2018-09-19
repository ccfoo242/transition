﻿using System;
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
    public sealed partial class CapacitorParametersControl : UserControl
    {
        private Capacitor SerializableCapacitor { get; }

        private int SelectedCapacitorModel { get { return SerializableCapacitor.CapacitorModel; } }

        public CapacitorParametersControl()
        {
            this.InitializeComponent();
           
        }

        public CapacitorParametersControl(Capacitor cap)
        {
            this.InitializeComponent();

            SerializableCapacitor = cap;
            DataContext = cap;

        }
        
        

        private void modelCapacitorChanged(object sender, SelectionChangedEventArgs e)
        {
            pnlExponential.Visibility = Visibility.Collapsed;
            pnlParasitic.Visibility = Visibility.Collapsed;

            if (SelectedCapacitorModel == 1)
                pnlParasitic.Visibility = Visibility.Visible;

            if (SelectedCapacitorModel == 2)
                pnlExponential.Visibility = Visibility.Visible;
        }

       
    }
}
