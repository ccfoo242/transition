﻿using Easycoustics.Transition.CircuitEditor.Serializable;
using Easycoustics.Transition.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Easycoustics.Transition.CircuitEditor.ParametersControls
{
    public sealed partial class VoltageOutputNodeParametersControl : UserControl
    {
        private VoltageOutputNode SerializableVoltageNode { get; }
        private string oldElementName;
        private string oldDescription;

        public VoltageOutputNodeParametersControl()
        {
            this.InitializeComponent();
        }

        public VoltageOutputNodeParametersControl(VoltageOutputNode node)
        {
            this.InitializeComponent();

            SerializableVoltageNode = node;
            SerializableVoltageNode.PropertyChanged += handleChangeOfControls;
        }

        private void ElementNameFocus(object sender, RoutedEventArgs e)
        {
            oldElementName = txtElementName.Text;
        }

        private void ElementNameChanged(object sender, TextChangedEventArgs e)
        {
            if (oldElementName == txtElementName.Text) return;

            var command = new CommandSetValue()
            {
                Component = SerializableVoltageNode,
                Property = "ElementName",
                OldValue = oldElementName,
                NewValue = txtElementName.Text
            };

            executeCommand(command);

            oldElementName = txtElementName.Text;
        }

        private void executeCommand(ICircuitCommand command)
        {
            CircuitEditor.currentInstance.executeCommand(command);
        }

        private void DescriptionFocus(object sender, RoutedEventArgs e)
        {
            oldDescription = txtDescription.Text;
        }

        private void DescriptionChanged(object sender, TextChangedEventArgs e)
        {
            if (oldElementName == txtDescription.Text) return;

            var command = new CommandSetValue()
            {
                Component = SerializableVoltageNode,
                Property = "Description",
                OldValue = oldDescription,
                NewValue = txtDescription.Text
            };

            executeCommand(command);

            oldElementName = txtDescription.Text;
        }


        private void handleChangeOfControls(object sender, PropertyChangedEventArgs args)
        {
            SerializableVoltageNode.PropertyChanged -= handleChangeOfControls;

            txtElementName.TextChanged -= ElementNameChanged;
            txtElementName.Text = SerializableVoltageNode.ElementName;
            txtElementName.TextChanged += ElementNameChanged;

            txtDescription.TextChanged -= DescriptionChanged;
            txtDescription.Text = SerializableVoltageNode.Description;
            txtDescription.TextChanged += DescriptionChanged;

            SerializableVoltageNode.PropertyChanged += handleChangeOfControls;
        }
    }
}