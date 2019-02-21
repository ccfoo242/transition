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

        NumericalAxis magLinearAxis = new NumericalAxis() { Header = "Magnitude" };
        LogarithmicAxis magLogAxis = new LogarithmicAxis() { Header = "Magnitude" };

        NumericalAxis phaseAxis = new NumericalAxis()
        {
            Header = "Phase",
            Minimum = -180,
            Maximum = 180
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
                    
                        func.FunctionChanged += functionChanged;

                        lMag = new LineSeries()
                        {                            
                            XBindingPath = "Key",
                            XAxis = FreqAxis,
                            YAxis = currentMagAxis,
                            StrokeThickness = func.StrokeThickness,
                            Stroke = func.StrokeColor
                        };

                        lMag.ItemsSource = (MagScale == AxisScale.dB) ? funcSampled.DataIndB(DBReference) : funcSampled.Data;
                        lMag.YBindingPath = (MagScale == AxisScale.dB) ? "Value.RealPart" : "Value.Magnitude";

                        FuncSeriesMag.Add(func, lMag);


                        lPhase = new LineSeries()
                        {
                            ItemsSource = funcSampled.Data,
                            XBindingPath = "Key",
                            YBindingPath = "Value.PhaseDeg",
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
            var serie = FuncSeriesMag[obj];
            SampledFunction func = obj.RenderToSampledFunction;

            serie.ItemsSource = (MagScale == AxisScale.dB) ? func.DataIndB(DBReference) : func.Data;

        }

        private void scaleMagnitudeChanged()
        {
            foreach (var kvp in FuncSeriesMag)
            {
                kvp.Value.YAxis = currentMagAxis;
                kvp.Value.ItemsSource = (MagScale == AxisScale.dB) ? kvp.Key.RenderToSampledFunction.DataIndB(DBReference) : kvp.Key.RenderToSampledFunction.Data;

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
