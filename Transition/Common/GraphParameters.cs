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

        private decimal minimumVertical;
        public decimal MinimumVertical { get => minimumVertical; set { SetProperty(ref minimumVertical, value); } }

        private decimal maximumVertical;
        public decimal MaximumVertical { get => maximumVertical; set { SetProperty(ref maximumVertical, value); } }



        private double majorDivStrokeThickness;
        public double MajorDivStrokeThickness { get => majorDivStrokeThickness; set { SetProperty(ref majorDivStrokeThickness, value); } }

        private double minorDivStrokeThickness;
        public double MinorDivStrokeThickness { get => minorDivStrokeThickness; set { SetProperty(ref minorDivStrokeThickness, value); } }


        private Color minorDivColor;
        public Color MinorDivColor { get => minorDivColor; set { minorDivColor = value; SetProperty(ref minorDivColor, value); } }

        private Color majorDivColor;
        public Color MajorDivColor { get => majorDivColor; set { majorDivColor = value; SetProperty(ref majorDivColor, value); } }

        private Color backgroundColor;
        public Color BackgroundColor { get => backgroundColor; set { backgroundColor = value; SetProperty(ref backgroundColor, value); } }


        private int quantityOfMinorDivsHorizontal;
        public int QuantityOfMinorDivsHorizontal { get => quantityOfMinorDivsHorizontal; set { SetProperty(ref quantityOfMinorDivsHorizontal, value); } }

        private int quantityOfMajorDivsHorizontal;
        public int QuantityOfMajorDivsHorizontal { get => quantityOfMajorDivsHorizontal; set { SetProperty(ref quantityOfMajorDivsHorizontal, value); } }

        private int quantityOfMinorDivsVertical;
        public int QuantityOfMinorDivsVertical { get => quantityOfMinorDivsVertical; set { SetProperty(ref quantityOfMinorDivsVertical, value); } }

        private int quantityOfMajorDivsVertical;
        public int QuantityOfMajorDivsVertical { get => quantityOfMajorDivsVertical; set { SetProperty(ref quantityOfMajorDivsVertical, value); } }

        private int dBperDiv;
        public int DBPerDiv { get => dBperDiv; set { SetProperty(ref dBperDiv, value); } }

        private int horizontalPrefix;
        public int HorizontalPrefix { get => horizontalPrefix; set { SetProperty(ref horizontalPrefix, value); } }

        private int verticalPrefix;
        public int VerticalPrefix { get => verticalPrefix; set { SetProperty(ref verticalPrefix, value); } }

        private decimal dBZeroRef;
        public decimal DBZeroRef { get => dBZeroRef; set { SetProperty(ref dBZeroRef, value); } }


        public GraphParameters()
        {
            horizontalScale = AxisScale.Logarithmic;
            verticalScale = AxisScale.dB;

            quantityOfMinorDivsHorizontal = 5;
            quantityOfMajorDivsHorizontal = 12;

            quantityOfMinorDivsVertical = 5;
            quantityOfMajorDivsVertical = 12;

            minorDivStrokeThickness = 1;
            majorDivStrokeThickness = 2;
            
            dBperDiv = 5;
            DBZeroRef = 1;

            horizontalPrefix = 0;
            verticalPrefix = 0;

            majorDivColor = Colors.Gray;
            minorDivColor = Colors.LightGray;
            backgroundColor = Colors.White;

            minimumHorizontal = 10;
            maximumHorizontal = 40000;

            minimumVertical = 0;
            maximumVertical = 100;

            PropertyChanged += checkValues;

        }

        private void checkValues(object sender, PropertyChangedEventArgs e)
        {
            if (MinimumHorizontal > MaximumHorizontal) throw new ArgumentException("Minimum Horizontal is greater than Maximum");
            if (MinimumVertical > MaximumVertical) throw new ArgumentException("Minimum Vertical is greater than Maximum");
            if (MinimumHorizontal == MaximumHorizontal) throw new ArgumentException("Minimum Horizontal is equal than Maximum");
            if (MinimumVertical == MaximumVertical) throw new ArgumentException("Minimum Vertical is equal than Maximum");
        }

        public object Clone()
        {
            var output = new GraphParameters()
            {
                BackgroundColor = this.BackgroundColor,
                DBPerDiv = this.DBPerDiv,
                DBZeroRef = this.DBZeroRef,
                HorizontalPrefix = this.HorizontalPrefix,
                HorizontalScale = this.HorizontalScale,
                MajorDivColor = this.MajorDivColor,
                MajorDivStrokeThickness = this.MajorDivStrokeThickness,
                MaximumHorizontal = this.MaximumHorizontal,
                MaximumVertical = this.MaximumVertical,
                MinimumHorizontal = this.MinimumHorizontal,
                MinimumVertical = this.MinimumVertical,
                MinorDivColor = this.MinorDivColor,
                MinorDivStrokeThickness =  this.MinorDivStrokeThickness,
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
