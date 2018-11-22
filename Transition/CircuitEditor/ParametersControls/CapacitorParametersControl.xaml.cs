﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Transition.CircuitEditor.Serializable;
using Transition.Commands;
using Transition.CustomControls;
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

        private string oldElementName;

        public CapacitorParametersControl()
        {
            this.InitializeComponent();
           
        }

        public CapacitorParametersControl(Capacitor cap)
        {
            this.InitializeComponent();

            SerializableCapacitor = cap;

            cap.PropertyChanged += handleChangeOfControls;

            handleChangeOfControls(null, null);
        }
        
        

        private void modelCapacitorChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!cmbCapacitorModel.IsDropDownOpen) return;

           

            var command = new CommandSetValue()
            {
                Component = SerializableCapacitor,
                Property = "CapacitorModel",
                OldValue = int.Parse((string)(e.RemovedItems[0] as ComboBoxItem).Tag),
                NewValue = cmbCapacitorModel.SelectedIndex
            };

            if (command.OldValue == command.NewValue) return;
            
            executeCommand(command);

            handleCapacitorModel();
        }
        

        private void CapacitorPrecisionChanged(object sender, ValueChangedEventArgs args)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableCapacitor,
                Property = "ComponentPrecision",
                OldValue = args.oldValue,
                NewValue = args.newValue
            };

            executeCommand(command);
        }

        private void CapacitorValueChanged(object sender, ValueChangedEventArgs args)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableCapacitor,
                Property = "CapacitorValue",
                OldValue = args.oldValue,
                NewValue = args.newValue
            };

            executeCommand(command);

        }

        private void executeCommand(ICircuitCommand command)
        {
      //      SerializableCapacitor.PropertyChanged -= handleChangeOfControls;

            CircuitEditor.currentInstance.executeCommand(command);

          //  SerializableCapacitor.PropertyChanged += handleChangeOfControls;

        }


        private void handleChangeOfControls(object sender, PropertyChangedEventArgs args)
        {
            SerializableCapacitor.PropertyChanged -= handleChangeOfControls;

            BoxLs.Value = SerializableCapacitor.Ls;
            BoxRp.Value = SerializableCapacitor.Rp;
            BoxRs.Value = SerializableCapacitor.Rs;
            BoxFo.Value = SerializableCapacitor.Fo;
            BoxQ.Value = SerializableCapacitor.Q;
            BoxEw.Value = SerializableCapacitor.Ew;

            cmbCapacitorModel.SelectedIndex = SerializableCapacitor.CapacitorModel;

            componentValueBox.ComponentValue = SerializableCapacitor.CapacitorValue;
            componentValueBox.ComponentPrecision = SerializableCapacitor.ComponentPrecision;

            txtElementName.Text = SerializableCapacitor.ElementName;

            handleCapacitorModel();


            SerializableCapacitor.PropertyChanged += handleChangeOfControls;
        }

        private void handleCapacitorModel()
        {

            pnlExponential.Visibility = Visibility.Collapsed;
            pnlParasitic.Visibility = Visibility.Collapsed;

            if (SerializableCapacitor.CapacitorModel == 1)
                pnlParasitic.Visibility = Visibility.Visible;

            if (SerializableCapacitor.CapacitorModel == 2)
                pnlExponential.Visibility = Visibility.Visible;
        }

        private void elementNameChanged(object sender, TextChangedEventArgs e)
        {
            if (oldElementName == txtElementName.Text) return;

            var command = new CommandSetValue()
            {
                Component = SerializableCapacitor,
                Property = "ElementName",
                OldValue = oldElementName,
                NewValue = txtElementName.Text
            };

            executeCommand(command);

            oldElementName = txtElementName.Text;
        }

        private void elementNameFocus(object sender, RoutedEventArgs e)
        {
            oldElementName = SerializableCapacitor.ElementName;
        }

        private void ChangeEw(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableCapacitor,
                Property = "Ew",
                OldValue = e.oldValue,
                NewValue = e.newValue
            };

            executeCommand(command);
        }

        private void ChangeLs(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableCapacitor,
                Property = "Ls",
                OldValue = e.oldValue,
                NewValue = e.newValue
            };

            executeCommand(command);
        }

        private void ChangeRp(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableCapacitor,
                Property = "Rp",
                OldValue = e.oldValue,
                NewValue = e.newValue
            };

            executeCommand(command);
        }

        private void ChangeRs(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableCapacitor,
                Property = "Rs",
                OldValue = e.oldValue,
                NewValue = e.newValue
            };

            executeCommand(command);
        }

        private void ChangeFo(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableCapacitor,
                Property = "Fo",
                OldValue = e.oldValue,
                NewValue = e.newValue
            };

            executeCommand(command);
        }

        private void ChangeQ(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableCapacitor,
                Property = "Q",
                OldValue = e.oldValue,
                NewValue = e.newValue
            };

            executeCommand(command);
        }
    }
}
