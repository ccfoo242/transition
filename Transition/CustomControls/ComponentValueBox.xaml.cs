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
    public sealed partial class ComponentValueBox : UserControl, INotifyPropertyChanged
    {
        public enum ComponentPrecision { Arbitrary, p05, p1, p2, p5, p10, p20, p50 }

        private String unit;
        public String Unit { get { return unit; }
            set { unit = value;
                Header.Text = "Value (" + value + "): ";
            } }

        public String UnitShort { get;set; }

        public EngrNumber ComponentValue
        {
            get { return (EngrNumber)GetValue(ComponentValueProperty); }
            set
            {
                SetValue(ComponentValueProperty, value);
                ValueChanged?.Invoke(this, new PropertyChangedEventArgs("ComponentValue"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ComponentValue"));
            }
        }

        public static readonly DependencyProperty ComponentValueProperty =
          DependencyProperty.Register("ComponentValue",
              typeof(EngrNumber), typeof(ComponentValueBox), new PropertyMetadata(EngrNumber.One));


        public ComponentPrecision selectedComponentPrecision { get; set; }

        private bool anyPrecisionSelected;
        public bool AnyPrecisionSelected
        {
            get { return anyPrecisionSelected; }
            set { anyPrecisionSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AnyPrecisionSelected"));
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler ValueChanged;

        private String valueString;
        public String ValueString { get { return valueString; }
            set { valueString = value;
                  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ValueString"));
            }
        }
        

        public ComponentValueBox()
        {
            this.InitializeComponent();
            ValueChanged += updateValueString;

        }

        private void increaseClick(object sender, RoutedEventArgs e)
        {
            ComponentValue = new EngrNumber(getNextValue(), ComponentValue.Prefix);
        }

        private void decreaseClick(object sender, RoutedEventArgs e)
        {
            ComponentValue = new EngrNumber(getPreviousValue(), ComponentValue.Prefix);
        }

        private void changedPrecision(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPrecision.SelectedIndex == (int)ComponentPrecision.Arbitrary)
            {
                hideUpDownButtons();
                AnyPrecisionSelected = true;
            }
            else
            {
                showUpDownButtons();
                AnyPrecisionSelected = false;
            }

            selectedComponentPrecision = (ComponentPrecision)cmbPrecision.SelectedIndex;

            if (!AnyPrecisionSelected) ComponentValue = new EngrNumber(getNextOrEqualValue(), ComponentValue.Prefix);

        }

        private void showUpDownButtons()
        {
            stkButtonsIncDec.Visibility = Visibility.Visible;
        }

        private void hideUpDownButtons()
        {
            stkButtonsIncDec.Visibility = Visibility.Collapsed;
        }

        public decimal getNextOrEqualValue()
        {
            decimal currentOneDigitMantissa = ComponentValue.getOneDigitMantissa();
            decimal multiplier = ComponentValue.Mantissa / ComponentValue.getOneDigitMantissa();

            foreach (decimal number in arraySelectedPrecision())
            {
                if (number >= currentOneDigitMantissa)
                    return (number * multiplier);
            }

            return 10M * arraySelectedPrecision()[0] * multiplier;
        }

        public decimal getNextValue()
        {
            decimal currentOneDigitMantissa = ComponentValue.getOneDigitMantissa();
            decimal multiplier = ComponentValue.Mantissa / ComponentValue.getOneDigitMantissa();

            foreach (decimal number in arraySelectedPrecision())
            {
                if (number > currentOneDigitMantissa)
                    return (number * multiplier);
            }

            return 10M * arraySelectedPrecision()[0] * multiplier;
        }

        public decimal getPreviousValue()
        {
            decimal currentOneDigitMantissa = ComponentValue.getOneDigitMantissa();
            decimal multiplier = ComponentValue.Mantissa / ComponentValue.getOneDigitMantissa();

            decimal[] reversed = arraySelectedPrecision().Reverse().ToArray();

            foreach (decimal number in reversed)
            {
                if (number < currentOneDigitMantissa)
                    return (number * multiplier);
            }

            return (reversed[0] / 10) * multiplier;
        }

        private void updateValueString(object sender, PropertyChangedEventArgs e)
        {
            EngrConverter conv = new EngrConverter() { ShortString = false };
            EngrConverter convShort = new EngrConverter() { ShortString = true };

            if (AnyPrecisionSelected)
                ValueString = (String)conv.Convert(ComponentValue, typeof(String), null, "");
            else
                ValueString = (String)convShort.Convert(ComponentValue, typeof(String), null, "");

            if (UnitShort != null) ValueString += " " + UnitShort;
        }

        public decimal[] arraySelectedPrecision()
        {
            switch (selectedComponentPrecision)
            {
                case ComponentPrecision.p05: return e192;
                case ComponentPrecision.p1: return e96;
                case ComponentPrecision.p2: return e48;
                case ComponentPrecision.p5: return e24;
                case ComponentPrecision.p10: return e12;
                case ComponentPrecision.p20: return e6;
                case ComponentPrecision.p50: return e3;
                default: return new decimal[] { };
            }
        }

        //standard values por every precision
        //the M suffix is for decimal number type

        public static decimal[] e3 = new decimal[]
            { 1.0M , 2.2M , 4.7M };

        public static decimal[] e6 = new decimal[]
            { 1.0M , 1.5M , 2.2M , 3.3M , 4.7M , 6.8M };

        public static decimal[] e12 = new decimal[]
            { 1.0M , 1.2M , 1.5M , 1.8M , 2.2M , 2.7M,
              3.3M , 3.9M , 4.7M , 5.6M , 6.8M , 8.2M };

        public static decimal[] e24 = new decimal[]
            { 1.0M , 1.1M , 1.2M , 1.3M , 1.5M , 1.6M,
              1.8M , 2.0M , 2.2M , 2.4M , 2.7M , 3.0M,
              3.3M , 3.6M , 3.9M , 4.3M , 4.7M , 5.1M,
              5.6M , 6.2M , 6.8M , 7.5M , 8.2M , 9.1M };

        public static decimal[] e48 = new decimal[]
            { 1.00M, 1.05M, 1.10M, 1.15M, 1.21M, 1.27M,
              1.33M, 1.40M, 1.47M, 1.54M, 1.62M, 1.69M,
              1.78M, 1.87M, 1.96M, 2.05M, 2.15M, 2.26M,
              2.37M, 2.49M, 2.61M, 2.74M, 2.87M, 3.01M,
              3.16M, 3.32M, 3.48M, 3.65M, 3.83M, 4.02M,
              4.22M, 4.42M, 4.64M, 4.87M, 5.11M, 5.36M,
              5.62M, 5.90M, 6.19M, 6.49M, 6.81M, 7.15M,
              7.50M, 7.87M, 8.25M, 8.66M, 9.09M, 9.53M};

        public static decimal[] e96 = new decimal[]
            {
              1.00M, 1.02M, 1.05M, 1.07M, 1.10M, 1.13M,
              1.15M, 1.18M, 1.21M, 1.24M, 1.27M, 1.30M,
              1.33M, 1.37M, 1.40M, 1.43M, 1.47M, 1.50M,
              1.54M, 1.58M, 1.62M, 1.65M, 1.69M, 1.74M,
              1.78M, 1.82M, 1.87M, 1.91M, 1.96M, 2.00M,
              2.05M, 2.10M, 2.15M, 2.21M, 2.26M, 2.32M,
              2.37M, 2.43M, 2.49M, 2.55M, 2.61M, 2.67M,
              2.74M, 2.80M, 2.87M, 2.94M, 3.01M, 3.09M,
              3.16M, 3.24M, 3.32M, 3.40M, 3.48M, 3.57M,
              3.65M, 3.74M, 3.83M, 3.92M, 4.02M, 4.12M,
              4.22M, 4.32M, 4.42M, 4.53M, 4.64M, 4.75M,
              4.87M, 4.99M, 5.11M, 5.23M, 5.36M, 5.49M,
              5.62M, 5.76M, 5.90M, 6.04M, 6.19M, 6.34M,
              6.49M, 6.65M, 6.81M, 6.98M, 7.15M, 7.32M,
              7.50M, 7.68M, 7.87M, 8.06M, 8.25M, 8.45M,
              8.66M, 8.87M, 9.09M, 9.31M, 9.53M, 9.76M};

        public static decimal[] e192 = new decimal[]
            {
              1.00M, 1.01M, 1.02M, 1.04M, 1.05M, 1.06M,
              1.07M, 1.09M, 1.10M, 1.11M, 1.13M, 1.14M,
              1.15M, 1.17M, 1.18M, 1.20M, 1.21M, 1.23M,
              1.24M, 1.26M, 1.27M, 1.29M, 1.30M, 1.32M,
              1.33M, 1.35M, 1.37M, 1.38M, 1.40M, 1.42M,
              1.43M, 1.45M, 1.47M, 1.49M, 1.50M, 1.52M,
              1.54M, 1.56M, 1.58M, 1.60M, 1.62M, 1.64M,
              1.65M, 1.67M, 1.69M, 1.72M, 1.74M, 1.76M,
              1.78M, 1.80M, 1.82M, 1.84M, 1.87M, 1.89M,
              1.91M, 1.93M, 1.96M, 1.98M, 2.00M, 2.03M,
              2.05M, 2.08M, 2.10M, 2.13M, 2.15M, 2.18M,
              2.21M, 2.23M, 2.26M, 2.29M, 2.32M, 2.34M,
              2.37M, 2.40M, 2.43M, 2.46M, 2.49M, 2.52M,
              2.55M, 2.58M, 2.61M, 2.64M, 2.67M, 2.71M,
              2.74M, 2.77M, 2.80M, 2.84M, 2.87M, 2.91M,
              2.94M, 2.98M, 3.01M, 3.05M, 3.09M, 3.12M,
              3.16M, 3.20M, 3.24M, 3.28M, 3.32M, 3.36M,
              3.40M, 3.44M, 3.48M, 3.52M, 3.57M, 3.61M,
              3.65M, 3.70M, 3.74M, 3.79M, 3.83M, 3.88M,
              3.92M, 3.97M, 4.02M, 4.07M, 4.12M, 4.17M,
              4.22M, 4.27M, 4.32M, 4.37M, 4.42M, 4.48M,
              4.53M, 4.59M, 4.64M, 4.70M, 4.75M, 4.81M,
              4.87M, 4.93M, 4.99M, 5.05M, 5.11M, 5.17M,
              5.23M, 5.30M, 5.36M, 5.42M, 5.49M, 5.56M,
              5.62M, 5.69M, 5.76M, 5.83M, 5.90M, 5.97M,
              6.04M, 6.12M, 6.19M, 6.26M, 6.34M, 6.42M,
              6.49M, 6.57M, 6.65M, 6.73M, 6.81M, 6.90M,
              6.98M, 7.06M, 7.15M, 7.23M, 7.32M, 7.41M,
              7.50M, 7.59M, 7.68M, 7.77M, 7.87M, 7.96M,
              8.06M, 8.16M, 8.25M, 8.35M, 8.45M, 8.56M,
              8.66M, 8.76M, 8.87M, 8.98M, 9.09M, 9.20M,
              9.31M, 9.42M, 9.53M, 9.65M, 9.76M, 9.88M
            };
    }
}
