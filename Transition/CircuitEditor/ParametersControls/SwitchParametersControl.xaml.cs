using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Transition.CircuitEditor.Serializable;
using Transition.Commands;
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

namespace Transition.CircuitEditor.ParametersControls
{
    public sealed partial class SwitchParametersControl : UserControl
    {
        private Switch SerializableSwitch { get; }
        private string oldElementName;

        public SwitchParametersControl()
        {
            this.InitializeComponent();
        }

        public SwitchParametersControl(Switch sw)
        {
            this.InitializeComponent();

            SerializableSwitch = sw;

            sw.PropertyChanged += handleChangeOfControls;
            handleChangeOfControls(null, null);

        }
        
        private void handleChangeOfControls(object sender, PropertyChangedEventArgs args)
        {
            SerializableSwitch.PropertyChanged -= handleChangeOfControls;



            SerializableSwitch.PropertyChanged += handleChangeOfControls;
        }


        private void ElementNameFocus(object sender, RoutedEventArgs e)
        {
            oldElementName = SerializableSwitch.ElementName;
        }

        private void ElementNameChanged(object sender, TextChangedEventArgs e)
        {
            if (oldElementName == txtElementName.Text) return;

            var command = new CommandSetValue()
            {
                Component = SerializableSwitch,
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
    }
}
