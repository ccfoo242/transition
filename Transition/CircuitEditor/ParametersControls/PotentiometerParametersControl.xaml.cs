using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Uwp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Transition.CircuitEditor.Serializable;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class PotentiometerParametersControl : UserControl
    { 
        private Potentiometer SerializablePotentiometer { get; }

        private SeriesCollection seriesCollection;

        public PotentiometerParametersControl()
        {
            this.InitializeComponent();
        }

        public PotentiometerParametersControl(Potentiometer pot)
        {
            this.InitializeComponent();

            SerializablePotentiometer = pot;
            DataContext = pot;
            
            seriesCollection = new SeriesCollection();
            lvcTaperCurve.Series = seriesCollection;

            lvcTaperCurve.AxisX.Add(new Axis()
            {
                Title = "Rotation (%)",
                MinValue = 0,
                MaxValue = 100,
                Separator = new Separator()
                {
                    Step = 25,
                    Stroke = new SolidColorBrush(Colors.Gray),
                    StrokeThickness = .5
                }
            });

            lvcTaperCurve.AxisY.Add(new Axis()
            {
                Title = "Resistance (%)",
                MinValue = 0,
                MaxValue = 100,
                Separator = new Separator()
                {
                    Step = 25,
                    Stroke = new SolidColorBrush(Colors.LightGray),
                    StrokeThickness = .5
                }
            });
            
            pot.TerminalsChanged += quantityOfTerminalsChanged;
            pot.TaperChanged += Pot_TaperChanged;

            Pot_TaperChanged();

            quantityOfTerminalsChanged();
        }

        private void Pot_TaperChanged()
        {
            seriesCollection.Clear();

            LineSeries l = new LineSeries()
            {
                Values = new ChartValues<ObservablePoint>(),
                PointGeometrySize = 5,
                StrokeThickness = 2,
                Stroke = new SolidColorBrush(Colors.Black),
                LineSmoothness = 0
            };
         
            foreach (TaperPoint point in SerializablePotentiometer.TaperFunction)
                  l.Values.Add(new ObservablePoint(point.PositionValuePercent,
                       point.ResistanceValuePercent));

            seriesCollection.Add(l);
             
        }

        private void ClickCCW(object sender, RoutedEventArgs e)
        {
            sldPosition.Value = 0;
        }

        private void ClickCenter(object sender, RoutedEventArgs e)
        {
            sldPosition.Value = 50;
        }

        private void ClickCW(object sender, RoutedEventArgs e)
        {
            sldPosition.Value = 100;
        }

        private void cmbSelectedQuantityOfTerminalsChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cmbQuantityOfTerminals.SelectedIndex)
            {
                case 0: SerializablePotentiometer.QuantityOfTerminals = 3; break;
                case 1: SerializablePotentiometer.QuantityOfTerminals = 4; break;
                case 2: SerializablePotentiometer.QuantityOfTerminals = 5; break;
                case 3: SerializablePotentiometer.QuantityOfTerminals = 6; break;
            }
        }

        private void quantityOfTerminalsChanged()
        {
            switch (SerializablePotentiometer.QuantityOfTerminals)
            {
                case 3:
                    cmbQuantityOfTerminals.SelectedIndex = 0;
                    sldTapAPosition.IsEnabled = false;
                    sldTapBPosition.IsEnabled = false;
                    sldTapCPosition.IsEnabled = false;
                    break;
                case 4:
                    cmbQuantityOfTerminals.SelectedIndex = 1;
                    sldTapAPosition.IsEnabled = false;
                    sldTapBPosition.IsEnabled = true;
                    sldTapCPosition.IsEnabled = false;
                    break;
                case 5:
                    cmbQuantityOfTerminals.SelectedIndex = 2;
                    sldTapAPosition.IsEnabled = true;
                    sldTapBPosition.IsEnabled = false;
                    sldTapCPosition.IsEnabled = true;
                    break;
                case 6:
                    cmbQuantityOfTerminals.SelectedIndex = 3;
                    sldTapAPosition.IsEnabled = true;
                    sldTapBPosition.IsEnabled = true;
                    sldTapCPosition.IsEnabled = true;
                    break;
            }


        }
    }
}
