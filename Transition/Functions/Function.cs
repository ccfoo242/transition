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

        public string DependentVariableUnit { get; set; }
        public string IndependentVariableUnit { get; set; }

        public static string[] PhysicalQuantities = new string[] {
            "Acceleration",
            "Acoustic Impedance",
            "Admittance",
            "Angular Frequency",
            "Area",
            "Electric Charge",
            "Electric Current",
            "Electric Voltage",
            "Energy",
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
            "Reactance",
            "Resistance",
            "Time",
            "Velocity",
            "Volume",
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






    public class SampledFunction : Function
    {
        public Dictionary<double, Complex> Data { get; } = new Dictionary<double, Complex>();
        public InterpolationModes InterpolationMode;

        public SampledFunction()
        {
            InterpolationMode = InterpolationModes.Linear;
        }

        public override Complex Calculate(double point)
        {
            if (pointExistsInDomain(point))
                return Data[point]; 
            else
                return Interpolate(point);
            
        }

        public bool pointExistsInDomain(Complex point)
        {
            
            foreach (var pData in Data.Keys)
                if (pData == point) return true;

            return false;
        }

        public Complex Interpolate(double point)
        {
            return Interpolate(point, InterpolationMode);
        }

        public Complex Interpolate(double point, InterpolationModes interpolationMode)
        {
            if (interpolationMode == InterpolationModes.NearestNeighbor)
                return InterpolateNearestNeighbor(point);
            else
            if (interpolationMode == InterpolationModes.Linear)
                return InterpolateLinear(point);
            else
            if (interpolationMode == InterpolationModes.Quadratic)
                return InterpolateQuadratic(point);
            else
                return InterpolateCubic(point);
        }

        public double GetNextAbscissa(double point)
        {
            var current = double.MaxValue;

            foreach (var p in Data.Keys)
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
        
        public bool isThereNextAbscissa(double point)
        {
            try { Complex x = GetNextAbscissa(point); }
            catch { return false; }
            return true;
        }

        public bool isTherePreviousAbscissa(double point)
        {
            try { Complex x = GetPreviuosAbscissa(point); }
            catch { return false; }
            return true;
        }

        public bool isPointOutsideRange(double point)
        {
            if (Data.Count == 0) return true;
            if (!isThereNextAbscissa(point)) return true;
            if (!isTherePreviousAbscissa(point)) return true;

            return false;
        }

        public Complex getValueOutsideRange(double point)
        {
            if (Data.Count == 0) throw new InvalidOperationException();
            
            bool ThereIsNextAbscissa = isThereNextAbscissa(point);
            bool ThereIsPreviousAbscissa = isTherePreviousAbscissa(point);

            if (!ThereIsPreviousAbscissa)
                return Data[GetNextAbscissa(point)];
            else
                return Data[GetPreviuosAbscissa(point)];
        }

        private Complex InterpolateNearestNeighbor(double point)
        {
            if (Data.Count < 1) throw new InvalidOperationException();

            if (isPointOutsideRange(point)) return getValueOutsideRange(point);

            double distanceToNext = GetNextAbscissa(point) - point;
            double distanceToPrevious = point - GetPreviuosAbscissa(point);

            if (distanceToNext < distanceToPrevious)
                return Data[GetNextAbscissa(point)];
            else
                return Data[GetPreviuosAbscissa(point)];
        }

        private Complex InterpolateLinear(double point)
        {
            if (Data.Count < 2) throw new InvalidOperationException();

            if (isPointOutsideRange(point)) return getValueOutsideRange(point);

            double x1 = GetPreviuosAbscissa(point); 
            double x2 = GetNextAbscissa(point);
            double x = point;
            Complex y1 = Data[x1];
            Complex y2 = Data[x2];

            return y1 + ((x - x1) * (y2 - y1) / (x2 - x1));
        }

        private Complex InterpolateQuadratic(double point)
        {
            return 0;
        }

        private Complex InterpolateCubic(double point)
        {
            return 0;
        }

        public void addSample(double abscissa, Complex value)
        {
            if (Data.Keys.Contains(abscissa))
                Data[abscissa] = value;
            else
                Data.Add(abscissa, value);
        }

        public bool removeSample(double abscissa)
        {
            if (Data.Keys.Contains(abscissa))
            {
                Data.Remove(abscissa);
                return true;
            }
            else
                return false;
        }
    }

    public enum InterpolationModes { NearestNeighbor, Linear, Quadratic, Cubic };

}
