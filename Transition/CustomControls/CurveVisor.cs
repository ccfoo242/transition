using Easycoustics.Transition.Common;
using Easycoustics.Transition.Design;
using Easycoustics.Transition.Functions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Easycoustics.Transition.CustomControls
{
    public class CurveVisor : Canvas
    {
        public ObservableCollection<Function> Curves = new ObservableCollection<Function>();

        private Dictionary<Function, Polyline> dictPolylinesMag = new Dictionary<Function, Polyline>();
        private Dictionary<Function, Polyline> dictPolylinesPhase = new Dictionary<Function, Polyline>();

        public ScaleParameters scaleParams = new ScaleParameters();

        private List<Line> VerticalMajorDivs = new List<Line>();
        private List<Line> VerticalMinorDivs = new List<Line>();

        private List<Line> HorizontalMajorDivs = new List<Line>();
        private List<Line> HorizontalMinorDivs = new List<Line>();

        private double CanvasWidth => 1800;
        private double CanvasHeight => 1000;

        private double CurvesCanvasMarginTop = 50;
        private double CurvesCanvasMarginLeft = 50;
        private double CurvesCanvasMarginRight = 50;
        private double CurvesCanvasMarginBottom = 100;

        private Viewbox ViewBoxCurves = new Viewbox();
        private Border BorderCurves = new Border();
        private SolidColorBrush MajorDivBrush;
        private SolidColorBrush MinorDivBrush;

        private Canvas CurvesCanvas = new Canvas();
        
        private GraphParameters GraphParams { get => UserDesign.CurrentDesign.CurveGraphParameters; }


        public CurveVisor() : base()
        {
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;

            CurvesCanvas.Width = CanvasWidth;
            CurvesCanvas.Height = CanvasHeight;

            Curves.CollectionChanged += CurveCollectionChanged;
            scaleParams.PropertyChanged += GraphPropertyChanged;

            this.SizeChanged += cnvSizeChanged;

            ViewBoxCurves.Child = CurvesCanvas;
            ViewBoxCurves.Stretch = Stretch.Fill;

            BorderCurves.Child = ViewBoxCurves;

            Children.Add(BorderCurves);
          

            ReDraw();

        }
        
        private void CurveCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var addfunc in e.NewItems)
                         AddCurve((Function)addfunc);
                    
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var addfunc in e.OldItems)
                        RemoveCurve((Function)addfunc);
                    
                    break;

                case NotifyCollectionChangedAction.Replace:

                    break;

                case NotifyCollectionChangedAction.Reset:

                    break;

            }
        }



        private void GraphPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ReDraw();
        }

        private void cnvSizeChanged(object sender, SizeChangedEventArgs e)
        {
            BorderCurves.Width = ((ActualWidth - CurvesCanvasMarginLeft - CurvesCanvasMarginRight) >= 0) ?
                                    (ActualWidth - CurvesCanvasMarginLeft - CurvesCanvasMarginRight) : 0;
            Canvas.SetLeft(BorderCurves, CurvesCanvasMarginLeft);

            BorderCurves.Height = ((ActualHeight - CurvesCanvasMarginTop - CurvesCanvasMarginBottom) >= 0) ?
                                    (ActualHeight - CurvesCanvasMarginTop - CurvesCanvasMarginBottom) : 0;
            Canvas.SetTop(BorderCurves, CurvesCanvasMarginTop);
        }

        private void ReDraw()
        {
            
            this.Background = new SolidColorBrush(GraphParams.FrameColor);
            CurvesCanvas.Background = new SolidColorBrush(GraphParams.GridBackgroundColor);

            BorderCurves.BorderThickness = new Thickness(GraphParams.BorderThickness);
            BorderCurves.BorderBrush = new SolidColorBrush(GraphParams.BorderColor);

            MajorDivBrush = new SolidColorBrush(GraphParams.MajorDivColor);
            MinorDivBrush = new SolidColorBrush(GraphParams.MinorDivColor);

            if (scaleParams.HorizontalScale == AxisScale.Logarithmic) DrawVerticalLogDivs();
            if (scaleParams.HorizontalScale == AxisScale.Linear) DrawVerticalLinDivs();

            if (scaleParams.VerticalScale == AxisScale.dB) DrawHorizontaldBDivs();
            if (scaleParams.VerticalScale == AxisScale.Logarithmic) DrawHorizontalLogDivs();
            if (scaleParams.VerticalScale == AxisScale.Linear) DrawHorizontalLinDivs();


        }

        private void DrawHorizontaldBDivs()
        {
            
            decimal maximum = scaleParams.MaximumdB;

            int QmajorDivs = scaleParams.QuantityOfdBDivs;
            int QminorDivs = scaleParams.QuantityOfMinorDivsVertical;

            double majorStep = CanvasHeight / QmajorDivs;
            double minorStep = majorStep / QminorDivs;


            for (int maj = 0; maj < QmajorDivs; maj++)
            {
                for (int min = 1; min < QminorDivs; min++)
                    CurvesCanvas.Children.Add(new Line()
                    {
                        X1 = 0,
                        X2 = CanvasWidth,
                        Y1 = (majorStep * maj) + (minorStep * min),
                        Y2 = (majorStep * maj) + (minorStep * min),
                        StrokeThickness = GraphParams.MinorDivStrokeThickness,
                        Stroke = MinorDivBrush
                    });

                if (maj != 0)
                    CurvesCanvas.Children.Add(new Line()
                    {
                        X1 = 0,
                        X2 = CanvasWidth,
                        Y1 = (majorStep * maj),
                        Y2 = (majorStep * maj),
                        StrokeThickness = GraphParams.MajorDivStrokeThickness,
                        Stroke = MajorDivBrush
                    });

            }

        }

        private void DrawHorizontalLinDivs()
        {
            decimal minimum = scaleParams.MinimumMag;
            decimal maximum = scaleParams.MaximumMag;

            int QmajorDivs = scaleParams.QuantityOfMajorDivsVertical;
            int QminorDivs = scaleParams.QuantityOfMinorDivsVertical;

            double majorStep = CanvasHeight / QmajorDivs;
            double minorStep = majorStep / QminorDivs;


            for (int maj = 0; maj < QmajorDivs; maj++)
            {
                for (int min = 1; min < QminorDivs; min++)
                    CurvesCanvas.Children.Add(new Line()
                    {
                        X1 = 0,
                        X2 = CanvasWidth,
                        Y1 = (majorStep * maj) + (minorStep * min),
                        Y2 = (majorStep * maj) + (minorStep * min),
                        StrokeThickness = GraphParams.MinorDivStrokeThickness,
                        Stroke = MinorDivBrush
                    });

                if (maj != 0)
                    CurvesCanvas.Children.Add(new Line()
                    {
                        X1 = 0,
                        X2 = CanvasWidth,
                        Y1 = (majorStep * maj),
                        Y2 = (majorStep * maj),
                        StrokeThickness = GraphParams.MajorDivStrokeThickness,
                        Stroke = MajorDivBrush
                    });

            }

        }

        private void DrawHorizontalLogDivs()
        {
            decimal minimum = scaleParams.MinimumMag;
            int minExponent = (int)Math.Floor(Math.Log10(Math.Abs(Convert.ToDouble(minimum))));
            decimal minOneDigit = minimum / DecimalMath.PowerN(10, minExponent);
            int minOneDigitClean = (int)Math.Floor(minOneDigit);
            decimal minimumClean = minOneDigitClean * DecimalMath.PowerN(10, minExponent);


            decimal maximum = scaleParams.MaximumMag;
            int maxExponent = (int)Math.Floor(Math.Log10(Math.Abs(Convert.ToDouble(maximum))));
            decimal maxOneDigit = maximum / DecimalMath.PowerN(10, maxExponent);
            int maxOneDigitClean = (int)Math.Floor(maxOneDigit);
            decimal maximumClean = maxOneDigitClean * DecimalMath.PowerN(10, maxExponent);

            double expSpace = Math.Log10(Convert.ToDouble(maximumClean) / Convert.ToDouble(minimumClean));

            bool IsInteger(decimal dec) => dec % 1 == 0;

            decimal currentOneDigit = minOneDigitClean;
            int currentExp = minExponent;

            decimal intMinorDivStep = 1m / scaleParams.QuantityOfMinorDivsVertical;
            double domainPosition;
            double canvasPosition;

            if ((minOneDigitClean == maxOneDigitClean) && (minExponent == maxExponent))
            {
                maxOneDigitClean++;
                if (maxOneDigitClean == 10) { maxOneDigitClean = 1; maxExponent++; }
            }

            while ((currentOneDigit != maxOneDigitClean) || (currentExp != maxExponent))
            {
                if (!(((currentOneDigit == minOneDigitClean) && (currentExp == minExponent)) ||
                    ((currentOneDigit == maxOneDigitClean) && (currentExp == maxExponent))))
                {
                    domainPosition = Convert.ToDouble(currentOneDigit) * Math.Pow(10, currentExp);
                    canvasPosition = Math.Log10(domainPosition / Convert.ToDouble(minimumClean)) * CanvasHeight / expSpace;

                    CurvesCanvas.Children.Add(new Line()
                    {
                        X1 = 0,
                        X2 = CanvasWidth,
                        Y1 = CanvasHeight - canvasPosition,
                        Y2 = CanvasHeight - canvasPosition,
                        StrokeThickness = IsInteger(currentOneDigit) ? 
                            GraphParams.MajorDivStrokeThickness : GraphParams.MinorDivStrokeThickness,
                        Stroke = IsInteger(currentOneDigit) ? MajorDivBrush : MinorDivBrush
                    }
                    );
                }

                currentOneDigit += intMinorDivStep;

                if (currentOneDigit == 10)
                {
                    currentOneDigit = 1;
                    currentExp++;
                }
            }
        }

        private void DrawVerticalLogDivs()
        {
            decimal minimum = scaleParams.MinimumHorizontal;
            int minExponent = (int)Math.Floor(Math.Log10(Math.Abs(Convert.ToDouble(minimum))));
            decimal minOneDigit = minimum / DecimalMath.PowerN(10, minExponent);
            int minOneDigitClean = (int)Math.Floor(minOneDigit);
            decimal minimumClean = minOneDigitClean * DecimalMath.PowerN(10, minExponent);


            decimal maximum = scaleParams.MaximumHorizontal;
            int maxExponent = (int)Math.Floor(Math.Log10(Math.Abs(Convert.ToDouble(maximum))));
            decimal maxOneDigit = maximum / DecimalMath.PowerN(10, maxExponent);
            int maxOneDigitClean = (int)Math.Floor(maxOneDigit);
            decimal maximumClean = maxOneDigitClean * DecimalMath.PowerN(10, maxExponent);

            double expSpace = Math.Log10(Convert.ToDouble(maximumClean) / Convert.ToDouble(minimumClean));

            bool IsInteger(decimal dec) => dec % 1 == 0;
            
            decimal currentOneDigit = minOneDigitClean;
            int currentExp = minExponent;

            decimal intMinorDivStep = 1m / scaleParams.QuantityOfMinorDivsVertical;
            double domainPosition;
            double canvasPosition;

            if ((minOneDigitClean == maxOneDigitClean) && (minExponent == maxExponent))
            {
                maxOneDigitClean++;
                if (maxOneDigitClean == 10) { maxOneDigitClean = 1; maxExponent++; }
            }

            while ((currentOneDigit != maxOneDigitClean) || (currentExp != maxExponent))
            {
                if (!(((currentOneDigit == minOneDigitClean) && (currentExp == minExponent)) ||
                    ((currentOneDigit == maxOneDigitClean) && (currentExp == maxExponent))))
                {
                    domainPosition = Convert.ToDouble(currentOneDigit) * Math.Pow(10, currentExp);
                    canvasPosition = Math.Log10(domainPosition / Convert.ToDouble(minimumClean)) * CanvasWidth / expSpace;

                    CurvesCanvas.Children.Add(new Line()
                    {
                        X1 = canvasPosition,
                        X2 = canvasPosition,
                        Y1 = 0,
                        Y2 = CanvasHeight,
                        StrokeThickness = IsInteger(currentOneDigit) ? 
                            GraphParams.MajorDivStrokeThickness : GraphParams.MinorDivStrokeThickness,
                        Stroke = IsInteger(currentOneDigit) ? MajorDivBrush : MinorDivBrush
                    }
                    );
                }

                currentOneDigit += intMinorDivStep;

                if (currentOneDigit == 10)
                {
                    currentOneDigit = 1;
                    currentExp++;
                }
            }
        }


        private void DrawVerticalLinDivs()
        {
            decimal minimum = scaleParams.MinimumHorizontal;
            decimal maximum = scaleParams.MaximumHorizontal;

            int QmajorDivs = scaleParams.QuantityOfMajorDivsHorizontal;
            int QminorDivs = scaleParams.QuantityOfMinorDivsHorizontal;

            double majorStep = CanvasWidth / QmajorDivs;
            double minorStep = majorStep / QminorDivs;
            

            for (int maj = 0; maj < QmajorDivs; maj++)
            {
                for (int min = 1; min < QminorDivs; min++)
                    CurvesCanvas.Children.Add(new Line()
                    {
                        X1 = (majorStep * maj) + (minorStep * min),
                        X2 = (majorStep * maj) + (minorStep * min),
                        Y1 = 0,
                        Y2 = CanvasHeight,
                        StrokeThickness = GraphParams.MinorDivStrokeThickness,
                        Stroke = MinorDivBrush
                    });
                
                if (maj != 0)
                    CurvesCanvas.Children.Add(new Line()
                    {
                        X1 = (majorStep * maj),
                        X2 = (majorStep * maj),
                        Y1 = 0,
                        Y2 = CanvasHeight,
                        StrokeThickness = GraphParams.MajorDivStrokeThickness,
                        Stroke = MajorDivBrush
                    });
                
            }
        }

        private void ClearAll()
        {
            Children.Clear();
            VerticalMajorDivs.Clear();
            VerticalMinorDivs.Clear();
            HorizontalMinorDivs.Clear();
            HorizontalMajorDivs.Clear();
        }

        private void AddCurve(Function func)
        {
            var newPolyLineMag = new Polyline()
            {
                Stroke = new SolidColorBrush(func.StrokeColor),
                StrokeThickness = func.StrokeThickness
            };

            var newPolyLinePhase = new Polyline()
            {
                Stroke = new SolidColorBrush(func.StrokeColorLighter),
                StrokeThickness = func.StrokeThickness
            };

            dictPolylinesMag.Add(func, newPolyLineMag);
            dictPolylinesPhase.Add(func, newPolyLinePhase);

            double X;

            SampledFunction sampledFunc = (func is SampledFunction) ? (SampledFunction)func : SampledFunction.RenderFunction(func);
                        
            foreach (var sample in sampledFunc.Data)
            {
                X = mapX(sample.X);
                newPolyLineMag.Points.Add(new Point(X, mapPointMag(sample.Y)));
                newPolyLinePhase.Points.Add(new Point(X, mapPointPhase(sample.Y)));
            }

            CurvesCanvas.Children.Add(newPolyLineMag);
            CurvesCanvas.Children.Add(newPolyLinePhase);

            func.FunctionChanged += functionChanged;
        }

        private void functionChanged(Function func, FunctionChangedEventArgs args)
        {
            var lineMag = dictPolylinesMag[func];
            var linePhase = dictPolylinesPhase[func];
            
            double X;

            SampledFunction sampledFunc = (func is SampledFunction) ? (SampledFunction)func : SampledFunction.RenderFunction(func);

            int originalPointQuantity = lineMag.Points.Count;
            int newQuantity = sampledFunc.Data.Count;

            if (originalPointQuantity == newQuantity)
            {
                int sampleNumber = 0;
                foreach (var sample in sampledFunc.Data)
                {
                    X = mapX(sample.X);
                    lineMag.Points[sampleNumber] = new Point(X, mapPointMag(sample.Y));
                    linePhase.Points[sampleNumber] = new Point(X, mapPointPhase(sample.Y));
                    sampleNumber++;
                }
            }
            else
            {
                lineMag.Points.Clear();
                linePhase.Points.Clear();
                foreach (var sample in sampledFunc.Data)
                {
                    X = mapX(sample.X);
                    lineMag.Points.Add(new Point(X, mapPointMag(sample.Y)));
                    linePhase.Points.Add(new Point(X, mapPointPhase(sample.Y)));
                }
            }

        }

        private void RemoveCurve(Function func)
        {
            dictPolylinesMag.Remove(func);
            dictPolylinesPhase.Remove(func);
        }

        private double mapX(decimal X)
        {
            var Xdbl = Convert.ToDouble(X) / Math.Pow(10, scaleParams.HorizontalPrefix);
            var maxHor = Convert.ToDouble(scaleParams.MaximumHorizontal);
            var minHor = Convert.ToDouble(scaleParams.MinimumHorizontal);
              
            if (scaleParams.HorizontalScale == AxisScale.Linear)
                return CanvasWidth * (Xdbl - minHor) / (maxHor - minHor);
            else
            {   /* log scale*/
                if (Xdbl <= 0) return double.MinValue;
                else
                    return CanvasWidth * Math.Log10(Xdbl / minHor) / Math.Log10(maxHor / minHor);
            }
        }

        private double mapPointMag(ComplexDecimal Y)
        {
            double Ymag;

            var maxVer = Convert.ToDouble(scaleParams.MaximumMag);
            var minVer = Convert.ToDouble(scaleParams.MinimumMag);
            
            if (scaleParams.VerticalScale == AxisScale.Linear)
            {
                Ymag = Y.MagnitudeDouble / Math.Pow(10, scaleParams.VerticalPrefix);
                return CanvasHeight - (CanvasHeight * (Ymag - minVer) / (maxVer - minVer));
            }
            else
            if (scaleParams.VerticalScale == AxisScale.Logarithmic)
            {
                Ymag = Y.MagnitudeDouble / Math.Pow(10, scaleParams.VerticalPrefix);
                return CanvasHeight - (CanvasHeight * Math.Log10(Ymag / minVer) / Math.Log10(maxVer / minVer));
            }
            else
            {
                /* dB scale */
                switch (scaleParams.DBZeroRef)
                {
                    case dBReference.dBV: Ymag = Y.TodBV; break;
                    case dBReference.dBSPL: Ymag = Y.TodBSPL; break;
                    case dBReference.dBm: Ymag = Y.TodBm; break;
                    default: Ymag = 0; break;
                }
                double mindB = Convert.ToDouble(scaleParams.MaximumdB) - Convert.ToDouble(scaleParams.DBPerDiv * scaleParams.QuantityOfdBDivs);
                double maxdB = Convert.ToDouble(scaleParams.MaximumdB);
                return CanvasHeight - (CanvasHeight * (Ymag - mindB) / (maxdB - mindB));
            }
        }

        private double mapPointPhase(ComplexDecimal Y)
        {
            var maxVer = Convert.ToDouble(scaleParams.MaximumPhase);
            var minVer = Convert.ToDouble(scaleParams.MinimumPhase);
            
            return CanvasHeight - (CanvasHeight * (Y.PhaseDegDouble - minVer) / (maxVer - minVer));
        }
    }
}
