using Easycoustics.Transition.Common;
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
    public sealed partial class GraphScaleSettings : UserControl
    {
        WindowCurveVisor WCV;
        ScaleParameters scaleParams;

        public GraphScaleSettings()
        {
            this.InitializeComponent();
        }

        public GraphScaleSettings(WindowCurveVisor wcv, ScaleParameters scale, string physicalQuantity)
        {
            this.InitializeComponent();

            WCV = wcv;
            DataContext = scale;
            scaleParams = scale;
            scale.PropertyChanged += checkVisuals;

            if (physicalQuantity == "Voltage")
            {
                cmbItemRef1.IsEnabled = true; cmbItemRef1.Content = "1 (dBV)";
                cmbItemRef077.IsEnabled = true; cmbItemRef077.Content = "774.6m (dBm)";
                cmbItemRef20u.IsEnabled = false; cmbItemRef20u.Content = "20u";
            }
            else
            if (physicalQuantity == "Pressure")
            {
                cmbItemRef1.IsEnabled = true; cmbItemRef1.Content = "1 (dBPa)";
                cmbItemRef077.IsEnabled = false; cmbItemRef077.Content = "774.6m";
                cmbItemRef20u.IsEnabled = true; cmbItemRef20u.Content = "20u (dBSPL)";
            }
            else
            if (physicalQuantity == "Impedance")
            {
                cmbItemRef1.IsEnabled = true; cmbItemRef1.Content = "1 (dBOhm)";
                cmbItemRef077.IsEnabled = false; cmbItemRef077.Content = "774.6m";
                cmbItemRef20u.IsEnabled = false; cmbItemRef20u.Content = "20u";
            }
            else
            if (physicalQuantity == "Time")
            {
                cmbItemRef1.IsEnabled = true; cmbItemRef1.Content = "1 (dBSec)";
                cmbItemRef077.IsEnabled = false; cmbItemRef077.Content = "774.6m";
                cmbItemRef20u.IsEnabled = false; cmbItemRef20u.Content = "20u";
            }else
            if (physicalQuantity == "Current")
            {
                cmbItemRef1.IsEnabled = true; cmbItemRef1.Content = "1 (dBA)";
                cmbItemRef077.IsEnabled = false; cmbItemRef077.Content = "774.6m";
                cmbItemRef20u.IsEnabled = false; cmbItemRef20u.Content = "20u";
            }
            else
            if (physicalQuantity == "Ratio")
            {
                cmbItemRef1.IsEnabled = true; cmbItemRef1.Content = "1 (dBR)";
                cmbItemRef077.IsEnabled = false; cmbItemRef077.Content = "774.6m";
                cmbItemRef20u.IsEnabled = false; cmbItemRef20u.Content = "20u";
            }
            dcmBoxVertMagMaximum.MinimumNumberAllowed = 1e-25m;

            checkVisuals(null, null);

        }

        private void checkVisuals(object sender, PropertyChangedEventArgs e)
        {
            dcmBoxVertMagMaximum.Visibility = Visibility.Collapsed;
            dcmBoxVertMagZeroMaximum.Visibility = Visibility.Collapsed;
            dcmBoxVertMagMaximumNegated.Visibility = Visibility.Collapsed;
            dcmBoxVertMagMinimum.Visibility = Visibility.Collapsed;
            dcmBoxVertMagZeroMinimum.Visibility = Visibility.Collapsed;

            if (scaleParams.VerticalScale == AxisScale.dB)
            {
                grpBoxVerticalLinearScalePolarity.IsEnabled = false;
                txtBoxVertMagMajorDivs.IsEnabled = true;
                grpBoxVerticaldB.IsEnabled = true;
                dcmBoxVertMagMaximum.IsEnabled = true;
                dcmBoxVertMagMinimum.IsEnabled = false;
                cmbVerticalMagUnits.IsEnabled = false;

                dcmBoxVertMagMaximum.Visibility = Visibility.Visible;
                dcmBoxVertMagMinimum.Visibility = Visibility.Visible;
            }
            else if (scaleParams.VerticalScale == AxisScale.Logarithmic)
            {
                grpBoxVerticalLinearScalePolarity.IsEnabled = false;
                txtBoxVertMagMajorDivs.IsEnabled = false;
                grpBoxVerticaldB.IsEnabled = false;
                dcmBoxVertMagMaximum.IsEnabled = true;
                dcmBoxVertMagMinimum.IsEnabled = true;
                cmbVerticalMagUnits.IsEnabled = true;

                dcmBoxVertMagMinimum.AllowPositiveNumber = true;
                dcmBoxVertMagMinimum.AllowNegativeNumber = false;
                if (scaleParams.MinimumMag <= 0m) scaleParams.MinimumMag = scaleParams.MaximumMag / 2;

                dcmBoxVertMagMinimum.MinimumNumberAllowed = 1E-25m;

                dcmBoxVertMagMaximum.Visibility = Visibility.Visible;
                dcmBoxVertMagMinimum.Visibility = Visibility.Visible;
            } else  /* linear */
            {
                grpBoxVerticalLinearScalePolarity.IsEnabled = true;
                txtBoxVertMagMajorDivs.IsEnabled = true;
                grpBoxVerticaldB.IsEnabled = false;
                dcmBoxVertMagMinimum.IsEnabled = true;
                cmbVerticalMagUnits.IsEnabled = true;

                dcmBoxVertMagMinimum.MinimumNumberAllowed = -1E25m;

                dcmBoxVertMagMinimum.AllowNegativeNumber = true;
                dcmBoxVertMagMinimum.AllowPositiveNumber = false;

                if (scaleParams.MagnitudePolarity == Polarity.Bipolar)
                {
                    dcmBoxVertMagMaximum.Visibility = Visibility.Visible;
                    dcmBoxVertMagMaximumNegated.Visibility = Visibility.Visible;

                }
                else if (scaleParams.MagnitudePolarity == Polarity.Positive)
                {
                    dcmBoxVertMagMaximum.Visibility = Visibility.Visible;
                    dcmBoxVertMagZeroMinimum.Visibility = Visibility.Visible;
                }
                else
                {
                    if (scaleParams.MinimumMag > 0m) scaleParams.MinimumMag *= -1;
                    dcmBoxVertMagZeroMaximum.Visibility = Visibility.Visible;
                    dcmBoxVertMagMinimum.Visibility = Visibility.Visible;
                }
            }

            dcmBoxVertPhaseMaximum.Visibility = Visibility.Collapsed;
            dcmBoxVertPhaseMaximumNegated.Visibility = Visibility.Collapsed;
            dcmBoxVertPhaseMaximumZero.Visibility = Visibility.Collapsed;
            dcmBoxVertPhaseMinimum.Visibility = Visibility.Collapsed;
            dcmBoxVertPhaseMinimumZero.Visibility = Visibility.Collapsed;

            if (scaleParams.PhasePolarity == Polarity.Bipolar)
            {
                dcmBoxVertPhaseMaximum.Visibility = Visibility.Visible;
                dcmBoxVertPhaseMaximumNegated.Visibility = Visibility.Visible;
            }
            else
            if (scaleParams.PhasePolarity == Polarity.Positive)
            {
                dcmBoxVertPhaseMaximum.Visibility = Visibility.Visible;
                dcmBoxVertPhaseMinimumZero.Visibility = Visibility.Visible;
            }
            else
            {
                /* phase polarity negative */
                dcmBoxVertPhaseMaximumZero.Visibility = Visibility.Visible;
                dcmBoxVertPhaseMinimum.Visibility = Visibility.Visible;

            }


        }

        private void tapOK(object sender, TappedRoutedEventArgs e)
        {

        }

        private void tapCancel(object sender, TappedRoutedEventArgs e)
        {

        }
    }

    public class EnumBooleanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        { /* enum to bool*/
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            object parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {/*bool to enum*/
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            switch (parameterString)
            {
                case "Bipolar": return Polarity.Bipolar;
                case "Positive": return Polarity.Positive;
                case "Negative": return Polarity.Negative;

                case "Linear": return AxisScale.Linear;
                case "Logarithmic": return AxisScale.Logarithmic;
                case "dB": return AxisScale.dB;
            }

            return Enum.Parse(targetType, parameterString);
        }
    }


    public class CombodBZeroRefConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //enum to int
            dBReference enumValue = (dBReference)value;
            switch (enumValue)
            {
                case dBReference.dBV: return 0;
                case dBReference.dBm: return 1;
                case dBReference.dBSPL: return 2;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            int iValue = (int)value;
            switch (iValue)
            {
                case 0: return dBReference.dBV;
                case 1: return dBReference.dBm;
                case 2: return dBReference.dBSPL;
            }

            return dBReference.dBV;
        }
    }

    public class ComboUnitPrefixConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //int to intIndex
            int iValue = (int)value;

            switch (iValue)
            {
                case 24:  return 0;
                case 21:  return 1;
                case 18:  return 2;
                case 15:  return 3;
                case 12:  return 4;
                case 9:   return 5;
                case 6:   return 6;
                case 3:   return 7;
                case 0:   return 8;
                case -3:  return 9;
                case -6:  return 10;
                case -9:  return 11;
                case -12: return 12;
                case -15: return 13;
                case -18: return 14;
                case -21: return 15;
                case -24: return 16;
            }
            return 8;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            int index = (int)value;

            switch (index)
            {   case 0: return 24;
                case 1: return 21;
                case 2: return 18;
                case 3: return 15;
                case 4: return 12;
                case 5: return 9;
                case 6: return 6;
                case 7: return 3;
                case 8: return 0;
                case 9: return -3;
                case 10: return -6;
                case 11: return -9;
                case 12: return -12;
                case 13: return -15;
                case 14: return -18;
                case 15: return -21;
                case 16: return -24;
            }

            return 0;
        }
    }

    public class ComboDbPerDivConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            //decimal to int index
            decimal dvalue = (decimal)value;

            switch (dvalue)
            {
                case 50m: return 0;
                case 30m: return 1;
                case 20m: return 2;
                case 10m: return 3;
                case 5m: return 4;
                case 3m: return 5;
                case 2m: return 6;
                case 1m: return 7;
                case 0.5m: return 8;
                case 0.3m: return 9;
                case 0.2m: return 10;
                case 0.1m: return 11;
                case 0.05m: return 12;
                case 0.03m: return 13;
                case 0.02m: return 14;
                case 0.01m: return 15;
                case 0.005m: return 16;
                case 0.003m: return 17;
                case 0.002m: return 18;
                case 0.001m: return 19;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            int ivalue = (int)value;
            switch (ivalue)
            {
                case 0: return 50m;
                case 1: return 30m;
                case 2: return 20m;
                case 3: return 10m;
                case 4: return 5m;
                case 5: return 3m;
                case 6: return 2m;
                case 7: return 1m;
                case 8: return 0.5m;
                case 9: return 0.3m;
                case 10: return 0.2m;
                case 11: return 0.1m;
                case 12: return 0.05m;
                case 13: return 0.03m;
                case 14: return 0.02m;
                case 15: return 0.01m;
                case 16: return 0.005m;
                case 17: return 0.003m;
                case 18: return 0.002m;
                case 19: return 0.001m;
            }
            return 50m;
        }
    }
}
