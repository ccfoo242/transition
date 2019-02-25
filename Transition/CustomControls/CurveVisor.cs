using Easycoustics.Transition.Common;
using Easycoustics.Transition.Functions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Easycoustics.Transition.CustomControls
{
    public class CurveVisor : Canvas
    {
        public ObservableCollection<Function> Curves = new ObservableCollection<Function>();

        public GraphParameters GraphParams = new GraphParameters();

        private List<Line> VerticalMajorDivs = new List<Line>();
        private List<Line> VerticalMinorDivs = new List<Line>();

        private List<Line> HorizontalMajorDivs = new List<Line>();
        private List<Line> HorizontalMinorDivs = new List<Line>();

        private double CanvasWidth => 1800;
        private double CanvasHeight => 1000;

        private SolidColorBrush MajorDivBrush;
        private SolidColorBrush MinorDivBrush;

        public CurveVisor() : base()
        {
            Width = CanvasWidth;
            Height = CanvasHeight;

            Curves.CollectionChanged += CurveCollectionChanged;
            GraphParams.PropertyChanged += GraphPropertyChanged;

            this.SizeChanged += cnvSizeChanged;

            ReDraw();

        }

        private void GraphPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ReDraw();
        }

        private void cnvSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ReDraw();
        }

        private void ReDraw()
        {
            ClearAll();

            this.Background = new SolidColorBrush(GraphParams.BackgroundColor);

            MajorDivBrush = new SolidColorBrush(GraphParams.MajorDivColor);
            MinorDivBrush = new SolidColorBrush(GraphParams.MinorDivColor);

            if (GraphParams.HorizontalScale == AxisScale.Logarithmic) DrawVerticalLogDivs();
            if (GraphParams.HorizontalScale == AxisScale.Linear) DrawVerticalLinDivs();

        }

        private void DrawVerticalLogDivs()
        {
            decimal minimum = GraphParams.MinimumHorizontal;
            int minExponent = (int)Math.Floor(Math.Log10(Math.Abs(Convert.ToDouble(minimum))));
            decimal minOneDigit = minimum / DecimalMath.PowerN(10, minExponent);
            int minOneDigitClean = (int)Math.Floor(minOneDigit);
            decimal minimumClean = minOneDigitClean * DecimalMath.PowerN(10, minExponent);


            decimal maximum = GraphParams.MaximumHorizontal;
            int maxExponent = (int)Math.Floor(Math.Log10(Math.Abs(Convert.ToDouble(maximum))));
            decimal maxOneDigit = maximum / DecimalMath.PowerN(10, maxExponent);
            int maxOneDigitClean = (int)Math.Floor(maxOneDigit);
            decimal maximumClean = maxOneDigitClean * DecimalMath.PowerN(10, maxExponent);

            double expSpace = Math.Log10(Convert.ToDouble(maximumClean) / Convert.ToDouble(minimumClean));

            bool IsInteger(decimal dec) => dec % 1 == 0;
            
            decimal currentOneDigit = minOneDigitClean;
            int currentExp = minExponent;

            decimal intMinorDivStep = 1m / GraphParams.QuantityOfMinorDivsVertical;
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

                    Children.Add(new Line()
                    {
                        X1 = canvasPosition,
                        X2 = canvasPosition,
                        Y1 = 0,
                        Y2 = CanvasHeight,
                        StrokeThickness = GraphParams.MajorDivStrokeThickness,
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

        }

        private void ClearAll()
        {
            Children.Clear();
            VerticalMajorDivs.Clear();
            VerticalMinorDivs.Clear();
            HorizontalMinorDivs.Clear();
            HorizontalMajorDivs.Clear();
        }

        private void CurveCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
