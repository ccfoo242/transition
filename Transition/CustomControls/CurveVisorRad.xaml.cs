using Easycoustics.Transition.Common;
using Easycoustics.Transition.Functions;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections;
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
    public sealed partial class CurveVisorRad : UserControl
    {
        public static CurveVisorRad staticCurveVisor;

        public ObservableCollection<Function> Curves { get; } = new ObservableCollection<Function>();

        private AxisScale magScale;
        public AxisScale MagScale { get => magScale; set { magScale = value; scaleMagnitudeChanged(); } }
        
        private dBReference dBReference;
        public dBReference DBReference { get => dBReference; set { dBReference = value; scaleMagnitudeChanged(); } }

        private Dictionary<Function, FastLineSeries> dictMag = new Dictionary<Function, FastLineSeries>();
        private Dictionary<Function, FastLineSeries> dictPhase = new Dictionary<Function, FastLineSeries>();

        NumericalAxis dbAxis = new NumericalAxis()
        {
            Header = "Gain (dB)",
        };

        NumericalAxis PhaseAxis = new NumericalAxis()
        {
            Header = "Phase",
            Minimum = -180,
            Maximum = 180,
            OpposedPosition = true,
        };

        public CurveVisorRad()
        {
            this.InitializeComponent();

            staticCurveVisor = this;

            dBReference = dBReference.dBV;
            magScale = AxisScale.dB;

            Curves.CollectionChanged += colFunctionsChanged;

            

        }

        private void colFunctionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            FastLineSeries lineMag;
            FastLineSeries linePh;
            Function func;
            
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var function in e.NewItems)
                    {
                        func = ((Function)function);
                        var funcSampled = ((Function)function).RenderToSampledFunction;
                       
                        func.FunctionChanged += functionChanged;
                        
                        string magBind;

                        if (MagScale == AxisScale.dB)
                        {
                            switch (DBReference)
                            {
                                case dBReference.dBV: magBind = "Item2.TodBV"; break;
                                case dBReference.dBm: magBind = "Item2.TodBm"; break;
                                case dBReference.dBSPL: magBind = "Item2.TodBSPL"; break;
                                default: magBind = "Item2.TodBV";break;
                            }
                        }
                        else
                            magBind = "Item2.Magnitude";
                        
                        lineMag = new FastLineSeries()
                        {
                            ItemsSource = funcSampled.Data,
                            XBindingPath = "Item1",
                            YBindingPath = magBind,
                            Stroke = funcSampled.StrokeColor,
                            StrokeThickness = funcSampled.StrokeThickness,
                            XAxis = freqAxis,
                            YAxis = dbAxis,
                            ShowTooltip = true,
                            ListenPropertyChange = true,
                        };

                        dictMag.Add(func, lineMag);

                        linePh = new FastLineSeries()
                        {
                            ItemsSource = funcSampled.Data,
                            XBindingPath = "Item1",
                            YBindingPath = "Item2.PhaseDegDouble",
                            Stroke = funcSampled.StrokeColor,
                            StrokeThickness = funcSampled.StrokeThickness,
                            XAxis = freqAxis,
                            YAxis = PhaseAxis,
                            ShowTooltip = true,
                            ListenPropertyChange = true,
                        };

                        dictPhase.Add(func, linePh);

                        sfchart.Series.Add(lineMag);
                        sfchart.Series.Add(linePh);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var function in e.OldItems)
                    {
                    
                        
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                   
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

        public void modify(object sender, TappedRoutedEventArgs e)
        {
        
        }
    }
    
}
