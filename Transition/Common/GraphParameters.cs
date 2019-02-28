using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Easycoustics.Transition.Common
{
    public class GraphParameters : BindableBase, ICloneable
    {
        private AxisScale horizontalScale;
        public AxisScale HorizontalScale { get => horizontalScale; set { SetProperty(ref horizontalScale, value); } }

        private AxisScale verticalScale;
        public AxisScale VerticalScale { get => verticalScale; set { SetProperty(ref verticalScale, value); } }


        private decimal minimumHorizontal;
        public decimal MinimumHorizontal { get => minimumHorizontal; set { SetProperty(ref minimumHorizontal, value); } }

        private decimal maximumHorizontal;
        public decimal MaximumHorizontal { get => maximumHorizontal; set { SetProperty(ref maximumHorizontal, value); } }

        private decimal minimumMag;
        public decimal MinimumMag { get => minimumMag; set { SetProperty(ref minimumMag, value); } }

        private decimal maximumMag;
        public decimal MaximumMag { get => maximumMag; set { SetProperty(ref maximumMag, value); } }

        private decimal minimumPhase;
        public decimal MinimumPhase { get => minimumPhase; set { SetProperty(ref minimumPhase, value); } }

        private decimal maximumPhase;
        public decimal MaximumPhase { get => maximumPhase; set { SetProperty(ref maximumPhase, value); } }

        private decimal maximumdB;
        public decimal MaximumdB { get => maximumdB; set { SetProperty(ref maximumdB, value); } }

        private double borderThickness;
        public double BorderThickness { get => borderThickness; set { SetProperty(ref borderThickness, value); } }

        private Color borderColor;
        public Color BorderColor { get => borderColor; set { SetProperty(ref borderColor, value); } }

        private double majorDivStrokeThickness;
        public double MajorDivStrokeThickness { get => majorDivStrokeThickness; set { SetProperty(ref majorDivStrokeThickness, value); } }

        private double minorDivStrokeThickness;
        public double MinorDivStrokeThickness { get => minorDivStrokeThickness; set { SetProperty(ref minorDivStrokeThickness, value); } }


        private Color minorDivColor;
        public Color MinorDivColor { get => minorDivColor; set { minorDivColor = value; SetProperty(ref minorDivColor, value); } }

        private Color majorDivColor;
        public Color MajorDivColor { get => majorDivColor; set { majorDivColor = value; SetProperty(ref majorDivColor, value); } }

        private Color gridColor;
        public Color GridColor { get => gridColor; set { gridColor = value; SetProperty(ref gridColor, value); } }

        private Color frameColor;
        public Color FrameColor { get => frameColor; set { frameColor = value; SetProperty(ref frameColor, value); } }

        private int quantityOfMinorDivsHorizontal;
        public int QuantityOfMinorDivsHorizontal { get => quantityOfMinorDivsHorizontal; set { SetProperty(ref quantityOfMinorDivsHorizontal, value); } }

        private int quantityOfMajorDivsHorizontal;
        public int QuantityOfMajorDivsHorizontal { get => quantityOfMajorDivsHorizontal; set { SetProperty(ref quantityOfMajorDivsHorizontal, value); } }

        private int quantityOfMinorDivsVertical;
        public int QuantityOfMinorDivsVertical { get => quantityOfMinorDivsVertical; set { SetProperty(ref quantityOfMinorDivsVertical, value); } }

        private int quantityOfMajorDivsVertical;
        public int QuantityOfMajorDivsVertical { get => quantityOfMajorDivsVertical; set { SetProperty(ref quantityOfMajorDivsVertical, value); } }

        private int quantityOfdBDivs;
        public int QuantityOfdBDivs { get => quantityOfdBDivs; set { SetProperty(ref quantityOfdBDivs, value); } }

        private int dBperDiv;
        public int DBPerDiv { get => dBperDiv; set { SetProperty(ref dBperDiv, value); } }

        private int horizontalPrefix; /* -3 for m, -6 for u, etc*/
        public int HorizontalPrefix { get => horizontalPrefix; set { SetProperty(ref horizontalPrefix, value); } }

        private int verticalPrefix;
        public int VerticalPrefix { get => verticalPrefix; set { SetProperty(ref verticalPrefix, value); } }

        private dBReference dBZeroRef; 
        public dBReference DBZeroRef { get => dBZeroRef; set { SetProperty(ref dBZeroRef, value); } }

        private double labelFontSize;
        public double LabelFontSize { get => labelFontSize; set { SetProperty(ref labelFontSize, value); } }

        public GraphParameters()
        {
            horizontalScale = AxisScale.Logarithmic;
            verticalScale = AxisScale.dB;

            quantityOfMinorDivsHorizontal = 5;
            quantityOfMajorDivsHorizontal = 12;

            quantityOfMinorDivsVertical = 5;
            quantityOfMajorDivsVertical = 12;

            minorDivStrokeThickness = 1;
            majorDivStrokeThickness = 1;
            
            dBperDiv = 5;
            dBZeroRef = dBReference.dBV;

            maximumdB = 10;
            quantityOfdBDivs = 12;

            horizontalPrefix = 0;
            verticalPrefix = 0;

            majorDivColor = Color.FromArgb(255, 140, 140, 140);
            minorDivColor = Color.FromArgb(255, 235, 235, 235);

            gridColor = Colors.White;
            frameColor = Color.FromArgb(255, 230, 230, 230);

            minimumHorizontal = 10;
            maximumHorizontal = 40000;

            minimumMag = 0;
            maximumMag = 100;

            maximumPhase = 180;
            minimumPhase = -180;

            borderColor = Colors.Black;
            borderThickness = 1;

            PropertyChanged += checkValues;

        }

        private void checkValues(object sender, PropertyChangedEventArgs e)
        {
            if (MinimumHorizontal > MaximumHorizontal) throw new ArgumentException("Minimum Horizontal is greater than Maximum");
            if (MinimumMag > MaximumMag) throw new ArgumentException("Minimum Vertical is greater than Maximum");
            if (MinimumHorizontal == MaximumHorizontal) throw new ArgumentException("Minimum Horizontal is equal than Maximum");
            if (MinimumMag == MaximumMag) throw new ArgumentException("Minimum Vertical is equal than Maximum");
        }

        public object Clone()
        {
            var output = new GraphParameters()
            {
                BorderColor = this.BorderColor,
                BorderThickness = this.BorderThickness,
                FrameColor = this.FrameColor,
                GridColor = this.GridColor,
                DBPerDiv = this.DBPerDiv,
                DBZeroRef = this.DBZeroRef,
                HorizontalPrefix = this.HorizontalPrefix,
                HorizontalScale = this.HorizontalScale,
                MajorDivColor = this.MajorDivColor,
                MajorDivStrokeThickness = this.MajorDivStrokeThickness,
                MaximumdB = this.MaximumdB,
                MaximumHorizontal = this.MaximumHorizontal,
                MaximumMag = this.MaximumMag,
                MaximumPhase = this.MaximumPhase,
                MinimumHorizontal = this.MinimumHorizontal,
                MinimumMag = this.MinimumMag,
                MinimumPhase = this.MinimumPhase,
                MinorDivColor = this.MinorDivColor,
                MinorDivStrokeThickness =  this.MinorDivStrokeThickness,
                QuantityOfdBDivs = this.QuantityOfdBDivs,
                QuantityOfMajorDivsHorizontal = this.QuantityOfMajorDivsHorizontal,
                QuantityOfMajorDivsVertical = this.QuantityOfMajorDivsVertical,
                QuantityOfMinorDivsHorizontal = this.QuantityOfMinorDivsHorizontal,
                QuantityOfMinorDivsVertical = this.QuantityOfMinorDivsVertical,
                VerticalPrefix = this.VerticalPrefix,
                VerticalScale = this.VerticalScale
            };

            return output;
        }
    }
}
