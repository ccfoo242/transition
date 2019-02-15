using Easycoustics.Transition.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Easycoustics.Transition.Functions
{
    public abstract class Function
    {
        public abstract ComplexDecimal Calculate(decimal point);
        public Brush StrokeColor;
        public double StrokeThickness;
        public DoubleCollection strokeArray;

        public abstract Dictionary<decimal, ComplexDecimal> Points { get; } 
        
        public string DependentVariableUnit { get; set; }
        public string IndependentVariableUnit { get; set; }

        public static decimal MinimumFrequency => CircuitEditor.CircuitEditor.StaticCurrentDesign.MinimumFrequency;
        public static decimal MaximumFrequency => CircuitEditor.CircuitEditor.StaticCurrentDesign.MaximumFrequency;

        public static int NumberOfFrequencyPoints => CircuitEditor.CircuitEditor.StaticCurrentDesign.NumberOfFrequencyPoints;

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

        public event Action<Function> FunctionChanged;

        protected void RaiseFunctionChanged() => FunctionChanged?.Invoke(this);
        
    }

    public class ConstantValueFunction : Function
    {
        private ComplexDecimal fixedValue;
        public ComplexDecimal FixedValue
        {
            get => fixedValue;
            set
            {
                fixedValue = value;
                RaiseFunctionChanged();
            }
        }

        public override Dictionary<decimal, ComplexDecimal> Points { get
            {
                var output = new Dictionary<decimal, ComplexDecimal>();

                output.Add(MinimumFrequency, FixedValue);
                output.Add(MaximumFrequency, FixedValue);

                return output;
            } }
        
        public ConstantValueFunction(Complex fixedValue)
        {
            FixedValue = fixedValue;
        }

        public override ComplexDecimal Calculate(decimal point)
        {
            return FixedValue;
        }
        
    }






    public class SampledFunction : Function
    {
        public Dictionary<decimal, ComplexDecimal> Data { get; } = new Dictionary<decimal, ComplexDecimal>();

        public override Dictionary<decimal, ComplexDecimal> Points => Data;

        public InterpolationModes InterpolationMode;

        public SampledFunction()
        {
            InterpolationMode = InterpolationModes.Linear;
        }

        public override ComplexDecimal Calculate(decimal point)
        {
            if (pointExistsInDomain(point))
                return Data[point]; 
            else
                return Interpolate(point);
            
        }

        public bool pointExistsInDomain(decimal point)
        {
            foreach (var pData in Data.Keys)
                if (pData == point) return true;

            return false;
        }

        public ComplexDecimal Interpolate(decimal point)
        {
            return Interpolate(point, InterpolationMode);
        }

        public ComplexDecimal Interpolate(decimal point, InterpolationModes interpolationMode)
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

        public decimal GetNextAbscissa(decimal point)
        {
            var current = decimal.MaxValue;

            foreach (var p in Data.Keys)
                if ((p > point) && (p < current))
                    current = p;

            if (current == decimal.MaxValue)
                throw new ArgumentException();

            return current;
        }

        public decimal GetPreviuosAbscissa(decimal point)
        {
            var current = decimal.MinValue;

            foreach (var p in Data.Keys)
                if ((p < point) && (p > current))
                    current = p;

            if (current == decimal.MinValue)
                throw new ArgumentException(); 

            return current;
        }
        
        public bool isThereNextAbscissa(decimal point)
        {
            try { var x = GetNextAbscissa(point); }
            catch { return false; }
            return true;
        }

        public bool isTherePreviousAbscissa(decimal point)
        {
            try { var x = GetPreviuosAbscissa(point); }
            catch { return false; }
            return true;
        }

        public bool isPointOutsideRange(decimal point)
        {
            if (Data.Count == 0) return true;
            if (!isThereNextAbscissa(point)) return true;
            if (!isTherePreviousAbscissa(point)) return true;

            return false;
        }

        public ComplexDecimal getValueOutsideRange(decimal point)
        {
            if (Data.Count == 0) throw new InvalidOperationException();
            
            bool ThereIsNextAbscissa = isThereNextAbscissa(point);
            bool ThereIsPreviousAbscissa = isTherePreviousAbscissa(point);

            if (!ThereIsPreviousAbscissa)
                return Data[GetNextAbscissa(point)];
            else
                return Data[GetPreviuosAbscissa(point)];
        }

        private ComplexDecimal InterpolateNearestNeighbor(decimal point)
        {
            if (Data.Count < 1) throw new InvalidOperationException();

            if (isPointOutsideRange(point)) return getValueOutsideRange(point);

            var distanceToNext = GetNextAbscissa(point) - point;
            var distanceToPrevious = point - GetPreviuosAbscissa(point);

            if (distanceToNext < distanceToPrevious)
                return Data[GetNextAbscissa(point)];
            else
                return Data[GetPreviuosAbscissa(point)];
        }

        private ComplexDecimal InterpolateLinear(decimal point)
        {
            if (Data.Count < 2) throw new InvalidOperationException();

            if (isPointOutsideRange(point)) return getValueOutsideRange(point);

            var x1 = GetPreviuosAbscissa(point);
            var x2 = GetNextAbscissa(point);
            var x = point;
            var y1 = Data[x1];
            var y2 = Data[x2];

            return y1 + ((x - x1) * (y2 - y1) / (x2 - x1));
        }

        private ComplexDecimal InterpolateQuadratic(decimal point)
        {
            return 0;
        }

        private ComplexDecimal InterpolateCubic(decimal point)
        {
            return 0;
        }

        public void addSample(decimal abscissa, ComplexDecimal value)
        {
            if (Data.Keys.Contains(abscissa))
                Data[abscissa] = value;
            else
                Data.Add(abscissa, value);

            RaiseFunctionChanged();
        }

        public bool removeSample(decimal abscissa)
        {
            if (Data.Keys.Contains(abscissa))
            {
                Data.Remove(abscissa);
                RaiseFunctionChanged();
                return true;
            }
            else
                return false;
        }
    }

    public enum InterpolationModes { NearestNeighbor, Linear, Quadratic, Cubic };

}
