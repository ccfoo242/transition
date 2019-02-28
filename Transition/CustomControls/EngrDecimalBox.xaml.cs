using Easycoustics.Transition.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace Easycoustics.Transition.CustomControls
{
    public sealed partial class EngrDecimalBox : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public static DecimalEngrConverter Converter { get; } = new DecimalEngrConverter();

        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value",
                typeof(decimal), typeof(EngrDecimalBox), new PropertyMetadata(1m, PropertyChangedCallback));

        public static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                string newString = (string)Converter.Convert((decimal)e.NewValue, null, null, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                (d as EngrDecimalBox).txtBox.Text = newString;
                (d as EngrDecimalBox).txtChanged = false;
            }

        }

        public bool AllowNegativeNumber { get; set; }
        private bool txtChanged = false;

        public EngrDecimalBox()
        {
            this.InitializeComponent();
        }
        
        private void txtLostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtChanged) return;

            string txt = txtBox.Text;
            bool didConvert = true;
            decimal ConvertedValue = 0m;
            try
            {
                ConvertedValue = (decimal)Converter.ConvertBack(txt, null, null, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            }
            catch { didConvert = false; }

            if (didConvert)
            {
                if (!AllowNegativeNumber && (ConvertedValue < 0)) ConvertedValue = 0;
                Value = ConvertedValue;
            }
            else
            {
                string newString = (string)Converter.Convert(Value, null, null, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                txtBox.Text = newString;
            }
            
        }

        private void textChanged(object sender, TextChangedEventArgs e)
        {
            txtChanged = true;
        }
    }
}
