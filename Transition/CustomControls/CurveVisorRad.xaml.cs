using Easycoustics.Transition.Common;
using Easycoustics.Transition.Functions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Telerik.UI.Xaml.Controls.Chart;
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
    public sealed partial class CurveVisorRad : UserControl
    {
        public ObservableCollection<Function> Curves { get; } = new ObservableCollection<Function>();

        private AxisScale magScale;
        public AxisScale MagScale { get => magScale; set { magScale = value; scaleMagnitudeChanged(); } }

       

        private decimal dBReference;
        public decimal DBReference { get => dBReference; set { dBReference = value; scaleMagnitudeChanged(); } }

        NumericalAxis currentMagAxis { get {
                switch (MagScale)
                {
                    case AxisScale.Linear: return magLinearAxis;
                    case AxisScale.Logarithmic: return magLogAxis;
                    case AxisScale.dB: return magLinearAxis;
                    default: return magLinearAxis;
                }
        } }
        LogarithmicAxis magLogAxis = new LogarithmicAxis()
        {
            Title = "Magnitude"
        };

        LinearAxis magLinearAxis = new LinearAxis()
        {
            Title = "Magnitude"
        };

        LinearAxis phaseAxis = new LinearAxis()
        {
            Title = "Phase"
        };

        public CurveVisorRad()
        {
            this.InitializeComponent();

            dBReference = 1;
            magScale = AxisScale.dB;

            Curves.CollectionChanged += colFunctionsChanged;

        }

        private void colFunctionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            LineSeries lMag;
            LineSeries lPhase;

            Function func;


            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var function in e.NewItems)
                    {
                        func = ((Function)function);
                        var funcSampled = ((Function)function).RenderToSampledFunction;
                        var dataMagdB = funcSampled.DataMagdB(DBReference);
                        var dataMag = funcSampled.DataMagLin();
                        var dataPh = funcSampled.DataPhaseDeg();

                        func.FunctionChanged += functionChanged;

                        /*    lMag = new LineSeries()
                            {
                                XBindingPath = "Key",
                                YBindingPath = "Value",
                                XAxis = FreqAxis,
                                YAxis = currentMagAxis,
                                StrokeThickness = func.StrokeThickness,
                                Stroke = func.StrokeColor
                            };*/
                        lMag = new LineSeries()
                        {
                            CategoryBinding = new PropertyNameDataPointBinding(""),
                            ValueBinding = new PropertyNameDataPointBinding(""),
                            HorizontalAxis
                        };

                        lMag.ItemsSource = (MagScale == AxisScale.dB) ? dataMagdB : dataMag;

                        FuncSeriesMag.Add(func, lMag);


                        lPhase = new LineSeries()
                        {
                            ItemsSource = dataPh,
                            XBindingPath = "Key",
                            YBindingPath = "Value",
                            XAxis = FreqAxis,
                            YAxis = phaseAxis,
                            StrokeThickness = func.StrokeThickness,
                            Stroke = func.StrokeColor
                        };

                        FuncSeriesPh.Add(func, lPhase);

                        sfChart.Series.Add(lMag);
                        sfChart.Series.Add(lPhase);

                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var function in e.OldItems)
                    {
                        func = (Function)function;
                        // var funcSampled = FuncSampl[func];

                        sfChart.Series.Remove(FuncSeriesMag[func]);
                        sfChart.Series.Remove(FuncSeriesPh[func]);

                        FuncSeriesMag.Remove(func);
                        FuncSeriesPh.Remove(func);

                        func.FunctionChanged -= functionChanged;
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    sfChart.Series.Clear();
                    FuncSeriesMag.Clear();
                    FuncSeriesPh.Clear();
                    foreach (var function in e.OldItems)
                        ((Function)function).FunctionChanged -= functionChanged;


                    break;

            }
        }

        private void scaleMagnitudeChanged()
        {
        }

        private void functionChanged(Function arg1, FunctionChangedEventArgs arg2)
        {
            throw new NotImplementedException();
        }

        private void curvesTap(object sender, TappedRoutedEventArgs e)
        {

        }

        private void graphParamTap(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
