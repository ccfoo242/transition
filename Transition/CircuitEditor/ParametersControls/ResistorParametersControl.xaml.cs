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
    public sealed partial class ResistorParametersControl : UserControl
    {
        private Resistor SerializableResistor { get; }
       
        private int SelectedResistorModel { get { return SerializableResistor.ResistorModel; } }

        private string oldElementName;

        public ResistorParametersControl()
        {
            InitializeComponent();
        }

        public ResistorParametersControl(Resistor resistor)
        {
            InitializeComponent();

            SerializableResistor = resistor;
     
            resistor.PropertyChanged += handleChangeOfControls;

            handleChangeOfControls(null, null);
        }
        
        private void modelResistorChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!cmbResistorModel.IsDropDownOpen) return;

            var command = new CommandSetValue()
            {
                Component = SerializableResistor,
                Property = "ResistorModel",
                OldValue = int.Parse((string)(e.RemovedItems[0] as ComboBoxItem).Tag),
                NewValue = cmbResistorModel.SelectedIndex
            };

            if (command.OldValue == command.NewValue) return;

            executeCommand(command);
            
        }

        private void handleChangeOfControls(object sender, PropertyChangedEventArgs args)
        {
            SerializableResistor.PropertyChanged -= handleChangeOfControls;

            BoxLs.Value = SerializableResistor.Ls;
            BoxCp.Value = SerializableResistor.Cp;
            BoxFo.Value = SerializableResistor.Fo;
            BoxQ.Value = SerializableResistor.Q;
            BoxEw.Value = SerializableResistor.Ew;

            cmbResistorModel.SelectedIndex = SerializableResistor.ResistorModel;

            componentValueBox.ComponentValue = SerializableResistor.ResistorValue;
            componentValueBox.ComponentPrecision = SerializableResistor.ComponentPrecision;

            txtElementName.Text = SerializableResistor.ElementName;

            pnlExponential.Visibility = Visibility.Collapsed;
            pnlParasitic.Visibility = Visibility.Collapsed;

            if (SerializableResistor.ResistorModel == 1)
                pnlParasitic.Visibility = Visibility.Visible;

            if (SerializableResistor.ResistorModel == 2)
                pnlExponential.Visibility = Visibility.Visible;

            SerializableResistor.PropertyChanged += handleChangeOfControls;
        }

        private void changeLs(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableResistor,
                Property = "Ls",
                OldValue = e.oldValue,
                NewValue = e.newValue
            };

            executeCommand(command);

        }

        private void changeCp(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableResistor,
                Property = "Cp",
                OldValue = e.oldValue,
                NewValue = e.newValue
            };

            executeCommand(command);
        }

        private void changeFo(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableResistor,
                Property = "Fo",
                OldValue = e.oldValue,
                NewValue = e.newValue
            };

            executeCommand(command);
        }

        private void changeQ(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableResistor,
                Property = "Q",
                OldValue = e.oldValue,
                NewValue = e.newValue
            };

            executeCommand(command);
        }

        private void ChangeEw(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableResistor,
                Property = "Ew",
                OldValue = e.oldValue,
                NewValue = e.newValue
            };

            executeCommand(command);
        }

        private void elementNameChanged(object sender, TextChangedEventArgs e)
        {
            if (oldElementName == txtElementName.Text) return;

            var command = new CommandSetValue()
            {
                Component = SerializableResistor,
                Property = "ElementName",
                OldValue = oldElementName,
                NewValue = txtElementName.Text
            };

            executeCommand(command);

            oldElementName = txtElementName.Text;
        }

        private void elementNameFocus(object sender, RoutedEventArgs e)
        {
            oldElementName = SerializableResistor.ElementName;
        }
        
        private void ResistorValueChanged(object sender, ValueChangedEventArgs args)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableResistor,
                Property = "ResistorValue",
                OldValue = args.oldValue,
                NewValue = args.newValue
            };

            executeCommand(command);
        }

        private void ResistorPrecisionChanged(object sender, ValueChangedEventArgs args)
        {
            var command = new CommandSetValue()
            {
                Component = SerializableResistor,
                Property = "ComponentPrecision",
                OldValue = args.oldValue,
                NewValue = args.newValue
            };

            executeCommand(command);
           
        }

        private void executeCommand(ICircuitCommand command)
        {
            SerializableResistor.PropertyChanged -= handleChangeOfControls;

            CircuitEditor.currentInstance.executeCommand(command);

            SerializableResistor.PropertyChanged += handleChangeOfControls;

        }
    }
}
