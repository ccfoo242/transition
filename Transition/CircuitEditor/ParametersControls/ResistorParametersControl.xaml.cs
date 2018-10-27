using System;
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
            DataContext = resistor;
        }
        
        private void modelResistorChanged(object sender, SelectionChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                property = "ResistorModel",
                oldValue = (int)(e.RemovedItems[0] as ComboBoxItem).Tag,
                newValue = cmbResistorModel.SelectedIndex
            };

            CircuitEditor.currentInstance.executeCommand(command);

            pnlExponential.Visibility = Visibility.Collapsed;
            pnlParasitic.Visibility = Visibility.Collapsed;

            if (cmbResistorModel.SelectedIndex == 1)
                pnlParasitic.Visibility = Visibility.Visible;

            if (cmbResistorModel.SelectedIndex == 2)
                pnlExponential.Visibility = Visibility.Visible;
        }

        private void changeLs(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                property = "Ls",
                oldValue = e.oldValue,
                newValue = e.newValue
            };

            CircuitEditor.currentInstance.executeCommand(command);

        }

        private void changeCp(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                property = "Cp",
                oldValue = e.oldValue,
                newValue = e.newValue
            };

            CircuitEditor.currentInstance.executeCommand(command);
        }

        private void changeFo(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                property = "Fo",
                oldValue = e.oldValue,
                newValue = e.newValue
            };

            CircuitEditor.currentInstance.executeCommand(command);
        }

        private void changeQ(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                property = "Q",
                oldValue = e.oldValue,
                newValue = e.newValue
            };

            CircuitEditor.currentInstance.executeCommand(command);
        }

        private void ChangeEw(object sender, ValueChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                property = "Ew",
                oldValue = e.oldValue,
                newValue = e.newValue
            };

            CircuitEditor.currentInstance.executeCommand(command);
        }

        private void elementNameChanged(object sender, TextChangedEventArgs e)
        {
            var command = new CommandSetValue()
            {
                property = "ElementName",
                oldValue = oldElementName,
                newValue = txtElementName.Text
            };

            CircuitEditor.currentInstance.executeCommand(command);

            oldElementName = txtElementName.Text;
        }

        private void elementNameFocus(object sender, RoutedEventArgs e)
        {
            oldElementName = SerializableResistor.ElementName;
        }

        private void ResistorValueChanged(object sender, PropertyChangedEventArgs e)
        {

        }
    }
}
