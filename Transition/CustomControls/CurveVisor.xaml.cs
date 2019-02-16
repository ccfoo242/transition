using Easycoustics.Transition.Design;
using Easycoustics.Transition.Functions;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Uwp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class FrequencyCurveVisor : UserControl
    {
        public ObservableCollection<Function> Curves { get; } = new ObservableCollection<Function>();
        
        public double MinFreq { get => AxisX.MinValue; set { AxisX.MinValue = value; } }
        public double MaxFreq { get => AxisY.MaxValue; set { AxisY.MaxValue = value; } } 

        private Axis AxisX;
        private Axis AxisY;

        private SeriesCollection lvcSeriesCollection;

        public FrequencyCurveVisor()
        {
            this.InitializeComponent();
            Curves.CollectionChanged += colFunctionsChanged;

            lvcSeriesCollection = new LiveCharts.SeriesCollection(Mappers.Xy<ObservablePoint>()
                .X(point => Math.Log10(point.X))
                .Y(point => point.Y));

            lvcControl.Series = lvcSeriesCollection;

            lvcControl.AxisX.Add(new Axis()
            {
                Title = "Frequency (Hz)",
                MinValue = 10,
                MaxValue = 40000,
                Separator = new Separator()
                {
                    Step = 25,
                    Stroke = new SolidColorBrush(Colors.Gray),
                    StrokeThickness = .5
                }
            });

            lvcControl.AxisY.Add(new Axis()
            {
                Title = "dB",
                MinValue = -60,
                MaxValue = 60,
                Separator = new Separator()
                {
                    Step = 25,
                    Stroke = new SolidColorBrush(Colors.Gray),
                    StrokeThickness = .5
                }
            });

          
        }

        private void colFunctionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    LineSeries l;
                    foreach (var function in e.NewItems)
                    {
                        var func = (Function)function;

                        l = new LineSeries()
                        {
                            Values = new ChartValues<ObservablePoint>(),
                            PointGeometrySize = 5,
                            StrokeThickness = 2,
                            Stroke = new SolidColorBrush(Colors.Black),
                            LineSmoothness = 0
                        };

                        func.FunctionChanged += functionChanged;
                        double mag;

                        foreach (var point in func.Points)
                        {
                            mag = 20 * Math.Log10(Convert.ToDouble(point.Value.Magnitude));
                            l.Values.Add(new ObservablePoint(Convert.ToDouble(point.Key), mag));
                        }
                        lvcSeriesCollection.Add(l);
                    }
                 
                    break;
                case NotifyCollectionChangedAction.Remove: break;
                case NotifyCollectionChangedAction.Reset: break;
            }
        }

        private void functionChanged(Function obj)
        {
           
        }

        private async void curvesTap(object sender, TappedRoutedEventArgs e)
        {
            var curveSelector = new CurveSelection();

            var dialog = new ContentDialog()
            {
                Title = "Select curves you want to display",
                Content = curveSelector,
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                foreach (var curve in curveSelector.selectedCurves())
                    if (!Curves.Contains(curve)) Curves.Add(curve);

                var toDelete = new List<Function>();

                foreach (var curve in Curves)
                    if (!curveSelector.selectedCurves().Contains(curve))
                        toDelete.Add(curve);

                foreach (var curve in toDelete)
                    Curves.Remove(curve);

            }
            
        }

        private void graphParamTap(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
