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

namespace Easycoustics.Transition.CustomControls
{
    public sealed partial class EngrDecimalBox : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
                typeof(decimal), typeof(EngrDecimalBox), new PropertyMetadata(1m));

        public bool AllowNegativeNumber { get; set; }

        public EngrDecimalBox()
        {
            this.InitializeComponent();
        }
    }
}
