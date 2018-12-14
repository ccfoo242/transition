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
    public sealed partial class TransferFunctionParametersControl : UserControl
    {
        private TransferFunctionComponent SerializableTF { get; }
        private string oldElementName;

        public TransferFunctionParametersControl()
        {
            this.InitializeComponent();
        }

        public TransferFunctionParametersControl(TransferFunctionComponent tf)
        {
            this.InitializeComponent();

            SerializableTF = tf;

            tf.PropertyChanged += handleChangeOfControls;
            handleChangeOfControls(null, null);
        }

        private void handleChangeOfControls(object sender, PropertyChangedEventArgs args)
        {
            SerializableTF.PropertyChanged -= handleChangeOfControls;
            
            txtElementName.TextChanged -= ElementNameChanged;
            txtElementName.Text = SerializableTF.ElementName;
            txtElementName.TextChanged += ElementNameChanged;

            boxRIn.Value = SerializableTF.RIn;
            boxROut.Value = SerializableTF.ROut;

            SerializableTF.PropertyChanged += handleChangeOfControls;
        }

        private void ElementNameChanged(object sender, TextChangedEventArgs e)
        {
            if (oldElementName == txtElementName.Text) return;

            var command = new CommandSetValue()
            {
                Component = SerializableTF,
                Property = "ElementName",
                OldValue = oldElementName,
                NewValue = txtElementName.Text
            };

            executeCommand(command);

            oldElementName = txtElementName.Text;
        }
        

        private void RInChanged(object sender, ValueChangedEventArgs args)
        {

        }

        private void ROutChanged(object sender, ValueChangedEventArgs args)
        {

        }

        private void ElementNameFocus(object sender, RoutedEventArgs e)
        {
            oldElementName = SerializableTF.ElementName;
        }

        private void executeCommand(ICircuitCommand command)
        {
            CircuitEditor.currentInstance.executeCommand(command);
        }

    }
}
