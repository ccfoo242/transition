using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Transition.Functions
{
    public abstract class Function
    {
        public abstract Complex Calculate(double point);
        public Brush StrokeColor;
        public double StrokeThickness;
        public DoubleCollection strokeArray;

        public string DependentVariableQuantity { get; set; }
        public string IndependentVariableQuantity { get; set; }

        public static string[] PhysicalQuantities = new string[] {
            "Acceleration",
            "Acoustic Impedance",
            "Admittance",
            "Area",
            "Electric Voltage",
            "Electric Current",
            "Force",
            "Frequency",
            "Impedance",
            "Length",
            "Magnetic B",
            "Magnetic H",
            "Mass",
            "Mechanical Impedance",
            "Phase",
            "Power",
            "Pressure",
            "Ratio",
            "Time",
            "Velocity",
            "Volume Velocity",
        };
    }

    public class ConstantValueFunction : Function
    {
        public Complex FixedValue { get; set; }

        public ConstantValueFunction(Complex fixedValue)
        {
            FixedValue = fixedValue;
        }

        public override Complex Calculate(double point)
        {
            return FixedValue;
        }
    }

    public class SampledCurve : Function
    {
        public Dictionary<double, Complex> Data { get; set; }
        public InterpolationModes InterpolationMode;

        public SampledCurve()
        {
            Data = new Dictionary<double, Complex>();
        }

        public override Complex Calculate(double point)
        {
            bool pointExists = false;

            foreach (double pData in Data.Keys)
                if (pData == point) pointExists = true;

            if (pointExists)
                return Data[point]; 
            else
                return Interpolate(point);
            
        }

        public Complex Interpolate(double point)
        {
            if (Data.Count == 0) throw new InvalidOperationException();
            
            try { double x1 = GetPreviuosAbscissa(point); }
            catch { return Data[GetNextAbscissa(point)]; }

            try { double x2 = GetNextAbscissa(point); }
            catch { return Data[GetPreviuosAbscissa(point)]; }
            
            if (InterpolationMode == InterpolationModes.Linear)
                return InterpolateLinear(point);
            else 
            if (InterpolationMode == InterpolationModes.Quadratic)
                return InterpolateQuadratic(point);
            else
                return InterpolateCubic(point);
        }

        public double GetNextAbscissa(double point)
        {
            double current = double.MaxValue;

            foreach (double p in Data.Keys)
                if ((p > point) && (p < current))
                    current = p;

            if (current == double.MaxValue)
                throw new ArgumentException();

            return current;
        }

        public double GetPreviuosAbscissa(double point)
        {
            double current = double.MinValue;

            foreach (double p in Data.Keys)
                if ((p < point) && (p > current))
                    current = p;

            if (current == double.MinValue)
                throw new ArgumentException(); 

            return current;
        }

        public Complex InterpolateLinear(double point)
        {
          
            double x1 = GetPreviuosAbscissa(point); 
            double x2 = GetNextAbscissa(point);
            double x = point;
            Complex y1 = Data[x1];
            Complex y2 = Data[x2];

            return y1 + ((x - x1) * (y2 - y1) / (x2 - x1));
        }

        public Complex InterpolateQuadratic(double point)
        {
            return 0;
        }

        public Complex InterpolateCubic(double point)
        {
            return 0;
        }
    }

    public enum InterpolationModes { Linear, Quadratic, Cubic };

}
