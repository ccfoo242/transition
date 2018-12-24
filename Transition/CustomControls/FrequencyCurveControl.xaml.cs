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
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Transition.Functions;
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

namespace Transition.CustomControls
{
    public sealed partial class FrequencyCurveControl : UserControl
    {
        public enum AxisScale { Logarithmic, Linear };
        public AxisScale FrequencyScale { get; set; }

        public ObservableCollection<Function> functions = new ObservableCollection<Function>();
        private Dictionary<Function, LineSeries> seriesMag = new Dictionary<Function, LineSeries>();
        private Dictionary<Function, LineSeries> seriesPhase = new Dictionary<Function, LineSeries>();

        private SeriesCollection sc ;
        public Func<double, string> Formatter { get; set; }
        
        public FrequencyCurveControl()
        {
            this.InitializeComponent();

            functions.CollectionChanged += functionsChanged;

           
            double Base = 10;

            var mapper = Mappers.Xy<ObservablePoint>()
                .X(point => Math.Log(point.X, Base)) //a 10 base log scale in the X axis
                .Y(point => point.Y);
            sc = new SeriesCollection(mapper);
            chartControl.Series = sc;

            Formatter = value => Math.Pow(Base, value).ToString("N");

            chartControl.AxisX.Add(new Axis()
            {
                MinValue = 10,
                MaxValue = 40000
            });

            chartControl.AxisY.Add(new Axis()
            {
                MinValue = 0,
                MaxValue = 2
            }
            );

          //  chartControl.AxisX[0].LabelFormatter = Formatter;


        }

        private void functionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            LineSeries sMag;
            LineSeries sPhase;
            

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    
                    foreach (Function function in e.NewItems.OfType<Function>())
                    {
                        function.FunctionChanged += FunctionChanged;
                        sMag = new LineSeries()
                        {
                            Values = new ChartValues<ObservablePoint>(),
                            StrokeThickness = 2,
                            Stroke = new SolidColorBrush(Colors.Black),
                            LineSmoothness = 0
                        };

                        sPhase = new LineSeries()
                        {
                            Values = new ChartValues<ObservablePoint>(),
                            StrokeThickness = 2,
                            Stroke = new SolidColorBrush(Colors.Gray),
                            LineSmoothness = 0
                        };

                        seriesMag.Add(function, sMag);
                        seriesPhase.Add(function, sPhase);
                        sc.Add(sMag);
                        sc.Add(sPhase);

                        foreach (var pair in function.Points)
                        {
                            sMag.Values.Add(new ObservablePoint(pair.Key.ToDouble, pair.Value.Magnitude));
                            sMag.Values.Add(new ObservablePoint(pair.Key.ToDouble, pair.Value.Phase * 180 / Math.PI));
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    
                    foreach (Function function in e.OldItems.OfType<Function>())
                    {
                        function.FunctionChanged -= FunctionChanged;
                        sMag = seriesMag[function];
                        sPhase = seriesPhase[function];
                        seriesMag.Remove(function);
                        seriesPhase.Remove(function);
                        sc.Remove(sMag);
                        sc.Remove(sPhase);
                    }
                    break;
                
            }

        }

        private void FunctionChanged(Function func)
        {
            foreach (KeyValuePair<EngrNumber, Complex> point in func.Points)
            {
                alterOrAddPoint(point.Key.ToDouble, point.Value.Magnitude, seriesMag[func]);
                alterOrAddPoint(point.Key.ToDouble, point.Value.Phase * 180 / Math.PI, seriesPhase[func]);
            }

            bool exists;

            foreach (ObservablePoint point in seriesMag[func].Values.OfType<ObservablePoint>())
            {
                exists = false;
                foreach (var kvp in func.Points)
                {
                    if (kvp.Key.ToDouble == point.X)
                        exists = true;
                }
                if (!exists)
                {
                    seriesMag[func].Values.Remove(point);
                    seriesPhase[func].Values.Remove(point);
                }
            }
        }

        private ObservablePoint findPoint(double Frequency, LineSeries series)
        {
            foreach (var p in series.Values.OfType<ObservablePoint>())
                if (p.X == Frequency) return p;
            
            return null;
        }

        private void alterOrAddPoint(double Frequency, double newValue, LineSeries series)
        {
            var point = findPoint(Frequency, series);
            if (point != null) point.Y = newValue;
            else
                series.Values.Add(new ObservablePoint(Frequency, newValue));
        }
    }
}
