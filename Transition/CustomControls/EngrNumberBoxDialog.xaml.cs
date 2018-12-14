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

namespace Transition.CustomControls
{
    public sealed partial class EngrNumberBoxDialog : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event ValueChangedEventHandler ValueManuallyChanged;

        public string VariableName { get; set; }
        public string Unit { get; set; }
        public string UnitShort { get; set; } = "";

        public delegate void ValueChangedEventHandler (object sender, ValueChangedEventArgs args);

        public EngrNumber Value
        {
            get { return (EngrNumber)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value",
                typeof(EngrNumber), typeof(EngrNumberBoxDialog), new PropertyMetadata(EngrNumber.One));

        public bool AllowNegativeNumber { get; set; }
        public bool AllowZero { get; set; }

        public EngrNumberBoxDialog()
        {
            this.InitializeComponent();
        }


        private async void changeValue(object sender, RoutedEventArgs e)
        {
            StackPanel stk2 = new StackPanel() { Orientation = Orientation.Vertical };
            StackPanel stk = new StackPanel()
                { Orientation = Orientation.Horizontal };

            stk.Children.Add(new TextBlock()
            {
                Text = "New Value:",
                Margin = new Thickness(4),
                VerticalAlignment = VerticalAlignment.Center
            });

            EngrNumberBox box = new EngrNumberBox()
            {
                Value = this.Value,
                Margin = new Thickness(4),
                AllowNegativeNumber = AllowNegativeNumber,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            stk.Children.Add(box);

            stk.Children.Add(new TextBlock()
            {
                Text = UnitShort,
                Margin = new Thickness(4),
                VerticalAlignment = VerticalAlignment.Center
            });

            stk2.Children.Add(stk);
            if (!AllowNegativeNumber) stk2.Children.Add(new TextBlock()
                { Text = "Only Positive number allowed", FontStyle = Windows.UI.Text.FontStyle.Italic });
            if (!AllowZero) stk2.Children.Add(new TextBlock()
                { Text = "Zero value is not allowed", FontStyle = Windows.UI.Text.FontStyle.Italic });


            ContentDialog dialog = new ContentDialog()
            {
                Title = "Enter new Value" + Environment.NewLine +
                VariableName,
                Content = stk2,
                CloseButtonText = "Cancel",
                PrimaryButtonText = "OK"
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (box.Value == 0 && !AllowZero) return;
                var args = new ValueChangedEventArgs
                {
                    oldValue = Value,
                    newValue = box.Value,
                    PropertyName = "Value"
                };

                Value = box.Value;
                ValueManuallyChanged?.Invoke(this, args);
            }
        }
    }

    public class ValueChangedEventArgs : EventArgs
    {
        public string PropertyName { get; set; }
        public object oldValue { get; set; }
        public object newValue { get; set; }

    }
}
