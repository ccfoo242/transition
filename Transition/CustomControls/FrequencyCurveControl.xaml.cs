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
using Easycoustics.Transition.Functions;
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
using Easycoustics.Transition.Common;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Easycoustics.Transition.CustomControls
{
    public sealed partial class FrequencyCurveControl : UserControl
    {
        public enum AxisScale { Logarithmic, Linear, dB };

        private AxisScale frequencyScale;
        public AxisScale FrequencyScale
        {
            get => frequencyScale; set
            {
                frequencyScale = value;
                setScale();
            }
        }

        private AxisScale magnitudScale;
        public AxisScale MagnitudeScale
        {
            get => magnitudScale; set
            {
                magnitudScale = value;
                setScale();
            }
        }

        public ObservableCollection<Function> functions = new ObservableCollection<Function>();
        private Dictionary<Function, LineSeries> seriesMag = new Dictionary<Function, LineSeries>();
        private Dictionary<Function, LineSeries> seriesPhase = new Dictionary<Function, LineSeries>();

        private SeriesCollection sc ;
        /* public Func<double, string> Formatter { get; set; } */
        
        private Axis magAxis;
        private Axis phaseAxis;

        private CartesianMapper<ObservablePoint> mpPhaseLinFreq = Mappers.Xy<ObservablePoint>()
                .X(point => point.X)
                .Y(point => point.Y);

        private CartesianMapper<ObservablePoint> mpPhaseLogFreq = Mappers.Xy<ObservablePoint>()
                .X(point => Math.Log(point.X, 10))
                .Y(point => point.Y);

        private CartesianMapper<ObservablePoint> mpdBMagLogFreq = Mappers.Xy<ObservablePoint>()
                .X(point => Math.Log(point.X, 10))
                .Y(point => 20 * Math.Log10(point.Y));

        private CartesianMapper<ObservablePoint> mpLogMagLogFreq = Mappers.Xy<ObservablePoint>()
                .X(point => Math.Log(point.X, 10))
                .Y(point => Math.Log10(point.Y));

        private CartesianMapper<ObservablePoint> mpLinMagLogFreq = Mappers.Xy<ObservablePoint>()
                .X(point => Math.Log(point.X, 10))
                .Y(point => point.Y);

        private CartesianMapper<ObservablePoint> mpdBMagLinFreq = Mappers.Xy<ObservablePoint>()
                .X(point => point.X)
                .Y(point => 20 * Math.Log10(point.Y));

        private CartesianMapper<ObservablePoint> mpLogMagLinFreq = Mappers.Xy<ObservablePoint>()
                .X(point => point.X)
                .Y(point => Math.Log10(point.Y));

        private CartesianMapper<ObservablePoint> mpLinMagLinFreq = Mappers.Xy<ObservablePoint>()
                .X(point => point.X)
                .Y(point => point.Y);

        public FrequencyCurveControl()
        {
            this.InitializeComponent();

            double Base = 10;
            FrequencyScale = AxisScale.Logarithmic;
            MagnitudeScale = AxisScale.dB;

            chartControl.AxisX.Add(new Axis()
            {
                Title = "Frequency",
                LabelFormatter = value => Math.Pow(Base, value).ToString("N")
            });

            magAxis = new Axis()
            {
                Title = "Gain",
                Position=AxisPosition.LeftBottom
            };

            phaseAxis = new Axis()
            {
                Title = "Phase",
                Position = AxisPosition.RightTop
            };
            chartControl.AxisY.Add(magAxis);
            chartControl.AxisY.Add(phaseAxis);

            functions.CollectionChanged += functionsChanged;
            
          /*  var mapper = Mappers.Xy<ObservablePoint>()
                .X(point => Math.Log(point.X, Base)) //a 10 base log scale in the X axis
                .Y(point => point.Y);
                */
            sc = new SeriesCollection();
            chartControl.Series = sc;
            
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
                            LineSmoothness = 0,
                            PointGeometry = null,
                            PointGeometrySize = 0,
                            Title = "Magnitude"
                        };

                        sPhase = new LineSeries()
                        {
                            Values = new ChartValues<ObservablePoint>(),
                            StrokeThickness = 2,
                            Stroke = new SolidColorBrush(Colors.Gray),
                            LineSmoothness = 0,
                            PointGeometry = null,
                            PointGeometrySize = 0,
                            Title = "Phase"
                        };

                        seriesMag.Add(function, sMag);
                        seriesPhase.Add(function, sPhase);
                        sc.Add(sMag);
                        sc.Add(sPhase);

                        foreach (var pair in function.Points)
                        {
                            sMag.Values.Add(new ObservablePoint(Convert.ToDouble(pair.Key), Convert.ToDouble(pair.Value.Magnitude)));
                            sPhase.Values.Add(new ObservablePoint(Convert.ToDouble(pair.Key), Convert.ToDouble(pair.Value.Phase * 180 / DecimalMath.Pi)));
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

            setScale();

        }

        public void setScale()
        {
            foreach (LineSeries ser in seriesMag.Values)
            {
                if ((FrequencyScale == AxisScale.Linear) && (MagnitudeScale == AxisScale.Linear))
                    ser.Configuration = mpLinMagLinFreq;
                if ((FrequencyScale == AxisScale.Linear) && (MagnitudeScale == AxisScale.Logarithmic))
                    ser.Configuration = mpLogMagLinFreq;
                if ((FrequencyScale == AxisScale.Linear) && (MagnitudeScale == AxisScale.dB))
                    ser.Configuration = mpdBMagLinFreq;

                if ((FrequencyScale == AxisScale.Logarithmic) && (MagnitudeScale == AxisScale.Linear))
                    ser.Configuration = mpLinMagLogFreq;
                if ((FrequencyScale == AxisScale.Logarithmic) && (MagnitudeScale == AxisScale.Logarithmic))
                    ser.Configuration = mpLogMagLogFreq;
                if ((FrequencyScale == AxisScale.Logarithmic) && (MagnitudeScale == AxisScale.dB))
                    ser.Configuration = mpdBMagLogFreq;
            }

            foreach (LineSeries ser in seriesPhase.Values)
            {
                if (FrequencyScale == AxisScale.Linear)
                    ser.Configuration = mpPhaseLinFreq;
                if (FrequencyScale == AxisScale.Logarithmic)
                    ser.Configuration = mpPhaseLogFreq;
            }
        }

        private void FunctionChanged(Function func)
        {
            foreach (KeyValuePair<decimal, ComplexDecimal> point in func.Points)
            {
                alterOrAddPoint(Convert.ToDouble(point.Key), Convert.ToDouble(point.Value.Magnitude), seriesMag[func]);
                alterOrAddPoint(Convert.ToDouble(point.Key), Convert.ToDouble(point.Value.Phase * 180 / DecimalMath.Pi), seriesPhase[func]);
            }

            bool exists;

            foreach (ObservablePoint point in seriesMag[func].Values.OfType<ObservablePoint>())
            {
                exists = false;
                foreach (var kvp in func.Points)
                {
                    if (Convert.ToDouble(kvp.Key) == point.X)
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
