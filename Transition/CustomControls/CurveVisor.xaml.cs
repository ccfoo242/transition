using Easycoustics.Transition.Common;
using Easycoustics.Transition.Design;
using Easycoustics.Transition.Functions;
using Syncfusion.UI.Xaml.Charts;
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

        public Dictionary<Function, LineSeries> FuncSeriesMag { get; } = new Dictionary<Function, LineSeries>();
        public Dictionary<Function, LineSeries> FuncSeriesPh { get; } = new Dictionary<Function, LineSeries>();
        // public Dictionary<Function, SampledFunction> FuncSampl { get; } = new Dictionary<Function, SampledFunction>();

        private AxisScale magScale;
        public AxisScale MagScale { get => magScale; set { magScale = value; scaleMagnitudeChanged(); } }

        private decimal dBReference;
        public decimal DBReference { get => dBReference; set { dBReference = value; scaleMagnitudeChanged(); } }

        NumericalAxis magLinearAxis = new NumericalAxis()
        {
            Header = "Magnitude",
            SmallTickLineSize = 2,
            TickLineSize = 6,
            SmallTicksPerInterval = 4,
            Interval = 5
        };

        LogarithmicAxis magLogAxis = new LogarithmicAxis()
        {
            Header = "Magnitude",
            SmallTickLineSize = 2,
            TickLineSize = 6,
            SmallTicksPerInterval = 4,
            Interval = 5
        };

        NumericalAxis phaseAxis = new NumericalAxis()
        {
            Header = "Phase",
            Minimum = -180,
            Maximum = 180,
            TickLineSize = 0,
            OpposedPosition = true
        };

        RangeAxisBase currentMagAxis { get {
                switch (MagScale)
                {
                    case AxisScale.Linear: return magLinearAxis;
                    case AxisScale.Logarithmic: return magLogAxis;
                    case AxisScale.dB: return magLinearAxis;
                    default: return magLinearAxis;
                }
            } }

        public FrequencyCurveVisor()
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

                        lMag = new LineSeries()
                        {                            
                            XBindingPath = "Key",
                            YBindingPath = "Value",
                            XAxis = FreqAxis,
                            YAxis = currentMagAxis,
                            StrokeThickness = func.StrokeThickness,
                            Stroke = func.StrokeColor
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

        private void functionChanged(Function obj, FunctionChangedEventArgs args)
        {
            var serieMag = FuncSeriesMag[obj];
            var seriePh = FuncSeriesPh[obj];
            SampledFunction func = obj.RenderToSampledFunction;

            var dataMagdB = func.DataMagdB(DBReference);
            var dataMag = func.DataMagLin();
            var dataPh = func.DataPhaseDeg();

            serieMag.ItemsSource = (MagScale == AxisScale.dB) ? dataMagdB : dataMag;
            seriePh.ItemsSource = dataPh;
        }

        private void scaleMagnitudeChanged()
        {
            foreach (var kvp in FuncSeriesMag)
            {
                kvp.Value.YAxis = currentMagAxis;
                kvp.Value.ItemsSource = (MagScale == AxisScale.dB) ? 
                    kvp.Key.RenderToSampledFunction.DataMagdB(DBReference) : kvp.Key.RenderToSampledFunction.DataMagLin();

            }
        }


        private async void curvesTap(object sender, TappedRoutedEventArgs e)
        {
            var curveSelector = new CurveSelection(Curves.ToList());

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
