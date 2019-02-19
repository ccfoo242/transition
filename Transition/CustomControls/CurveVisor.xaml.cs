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

        private Dictionary<Function, LineSeries> dictFL = new Dictionary<Function, LineSeries>();

        private SeriesCollection lvcSeriesCollection;

        public FrequencyCurveVisor()
        {
            this.InitializeComponent();
            Curves.CollectionChanged += colFunctionsChanged;

            lvcSeriesCollection = new LiveCharts.SeriesCollection(Mappers.Xy<ObservablePoint>()
                .X(point => Math.Log10(point.X))
                .Y(point => point.Y));

            lvcControl.Series = lvcSeriesCollection;

            AxisX = new Axis()
            {
                Title = "Frequency (Hz)",
                MinValue = 1,
                MaxValue = 5,
                Separator = new Separator()
                {
                    Step = 1,
                    Stroke = new SolidColorBrush(Colors.Gray),
                    StrokeThickness = .5
                }
            };
            lvcControl.AxisX.Add(AxisX);

            AxisY = new Axis()
            {
                Title = "dB",
                MinValue = -80,
                MaxValue = 10,
                Separator = new Separator()
                {
                    Step = 10,
                    Stroke = new SolidColorBrush(Colors.Gray),
                    StrokeThickness = .5
                }
            };
            lvcControl.AxisY.Add(AxisY);

          
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
                            StrokeThickness = 2,
                            Stroke = new SolidColorBrush(Colors.Black),
                            LineSmoothness = 0,
                            PointGeometry = null,
                            PointGeometrySize = 0
                        };

                        dictFL.Add(func, l);

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

                case NotifyCollectionChangedAction.Remove:
                    foreach (var function in e.OldItems)
                    {
                        var func = (Function)function;
                        lvcSeriesCollection.Remove(dictFL[func]);
                        dictFL.Remove(func);
                        func.FunctionChanged -= functionChanged;
                    }
                    break;
                    
                case NotifyCollectionChangedAction.Reset:
                    lvcSeriesCollection.Clear();
                    foreach (var function in dictFL.Keys)
                        function.FunctionChanged -= functionChanged;
                    
                    dictFL.Clear();
                    break;
            }
        }

        private void functionChanged(Function obj, FunctionChangedEventArgs args)
        {
            var series = dictFL[obj];
            
            Func<decimal, ObservablePoint> GetObsPoint = (X) => {
                var x = Convert.ToDouble(X);
                foreach (var obs in series.Values.OfType<ObservablePoint>())
                    if (obs.X == x) return obs;

                return null;
                    };
            switch (args.Action)
            {
                case FunctionChangedEventArgs.FunctionChangeAction.PointAdded:
                    double mag = 20 * Math.Log10(Convert.ToDouble(args.Y.Magnitude));
                    series.Values.Add(new ObservablePoint(Convert.ToDouble(args.X), mag));
                    break;

                case FunctionChangedEventArgs.FunctionChangeAction.PointChanged:
                    var obsPoint = GetObsPoint(args.X);
                    if (obsPoint != null)
                        obsPoint.Y = 20 * Math.Log10(Convert.ToDouble(args.Y.Magnitude));
                  
                    break;

                case FunctionChangedEventArgs.FunctionChangeAction.PointRemoved:
                    var obsPoint2 = GetObsPoint(args.X);
                    if (obsPoint2 != null)
                        series.Values.Remove(obsPoint2);
                    break;

                case FunctionChangedEventArgs.FunctionChangeAction.Reset:
                    var ObsToDelete = new List<ObservablePoint>();
                    ObsToDelete.AddRange(series.Values.OfType<ObservablePoint>());
                    ObservablePoint obsPoint3;
                    double mag2;

                    foreach (var point in obj.Points)
                    {
                        obsPoint3 = GetObsPoint(point.Key);
                        mag2 = 20 * Math.Log10(Convert.ToDouble(point.Value.Magnitude));

                        if (obsPoint3 == null)
                        { series.Values.Add(new ObservablePoint(Convert.ToDouble(point.Key), mag2)); }
                        else
                        {
                            obsPoint3.Y = mag2;
                            ObsToDelete.Remove(obsPoint3);
                        }
                    }
                    foreach (var obsDel in ObsToDelete)
                        series.Values.Remove(obsDel);
                    break;
            }
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
