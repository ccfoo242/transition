using LiveCharts;
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

        private SeriesCollection sc = new SeriesCollection();

        public FrequencyCurveControl()
        {
            this.InitializeComponent();

            functions.CollectionChanged += functionsChanged;

            chartControl.Series = sc;
        }

        private void functionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    LineSeries sMag;
                    LineSeries sPhase;

                    foreach (Function function in e.NewItems.OfType<Function>())
                    {
                        function.FunctionChanged += FunctionChanged;
                        sMag = new LineSeries()
                        {
                            Values = new ChartValues<ObservablePoint>(),
                            PointGeometrySize = 5,
                            StrokeThickness = 2,
                            Stroke = new SolidColorBrush(Colors.Black),
                            LineSmoothness = 0
                        };

                        sPhase = new LineSeries()
                        {
                            Values = new ChartValues<ObservablePoint>(),
                            PointGeometrySize = 5,
                            StrokeThickness = 2,
                            Stroke = new SolidColorBrush(Colors.Gray),
                            LineSmoothness = 0
                        };

                        seriesMag.Add(function, sMag);
                        seriesPhase.Add(function, sPhase);
                        sc.Add(sMag);
                        sc.Add(sPhase);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:

                    LineSeries sMag;
                    LineSeries sPhase;

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
            foreach (KeyValuePair<EngrNumber, Complex> point in SerializablePotentiometer.TaperFunction.Data)
                l.Values.Add(new ObservablePoint(point.Key.ToDouble,
                     point.Value.Magnitude));

            seriesCollection.Add(l);
        }
    }
}
