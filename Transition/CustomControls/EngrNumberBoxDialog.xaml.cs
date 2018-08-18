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
        public event PropertyChangedEventHandler ValueManuallyChanged;

        public String VariableName
        {
            get { return (String)GetValue(VariableNameProperty); }
            set { SetValue(VariableNameProperty, value); }
        }

        public static readonly DependencyProperty VariableNameProperty =
        DependencyProperty.Register("VariableName",
            typeof(String), typeof(EngrNumberBoxDialog), new PropertyMetadata(""));

        public String Unit
        {
            get { return (String)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit",
                typeof(String), typeof(EngrNumberBoxDialog), new PropertyMetadata(""));


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
                typeof(EngrNumber), typeof(EngrNumberBoxDialog), new PropertyMetadata(EngrNumber.one()));

        public bool AllowNegativeNumber { get; set; }


        public EngrNumberBoxDialog()
        {
            this.InitializeComponent();
        }


        private async void changeValue(object sender, RoutedEventArgs e)
        {
            StackPanel stk = new StackPanel()
                { Orientation = Orientation.Horizontal };

            stk.Children.Add(new TextBlock()
                { Text = "New Value:", Margin = new Thickness(4) });

            EngrNumberBox box = new EngrNumberBox()
            { Value = this.Value, Margin = new Thickness(4),
                AllowNegativeNumber = AllowNegativeNumber };

            stk.Children.Add(box);


            ContentDialog dialog = new ContentDialog()
            {
                Title = "Enter new Value" + Environment.NewLine +
                VariableName,
                Content = stk,
                CloseButtonText = "Cancel",
                PrimaryButtonText = "OK"
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Value = box.Value;
                ValueManuallyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }


        }
    }
}
