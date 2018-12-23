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
        public abstract Complex Calculate(EngrNumber point);
        public Brush StrokeColor;
        public double StrokeThickness;
        public DoubleCollection strokeArray;

        public abstract Dictionary<EngrNumber, Complex> Points { get; } 
        
        public string DependentVariableUnit { get; set; }
        public string IndependentVariableUnit { get; set; }

        public static EngrNumber MinimumFrequency => CircuitEditor.CircuitEditor.StaticCurrentDesign.MinimumFrequency;
        public static EngrNumber MaximumFrequency => CircuitEditor.CircuitEditor.StaticCurrentDesign.MaximumFrequency;

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
        private Complex fixedValue;
        public Complex FixedValue
        {
            get => fixedValue;
            set
            {
                fixedValue = value;
                RaiseFunctionChanged();
            }
        }

        public override Dictionary<EngrNumber, Complex> Points { get
            {
                var output = new Dictionary<EngrNumber, Complex>();

                output.Add(MinimumFrequency, FixedValue);
                output.Add(MaximumFrequency, FixedValue);

                return output;
            } }
        
        public ConstantValueFunction(Complex fixedValue)
        {
            FixedValue = fixedValue;
        }

        public override Complex Calculate(EngrNumber point)
        {
            return FixedValue;
        }
        
    }






    public class SampledFunction : Function
    {
        public Dictionary<EngrNumber, Complex> Data { get; } = new Dictionary<EngrNumber, Complex>();

        public override Dictionary<EngrNumber, Complex> Points => Data;

        public InterpolationModes InterpolationMode;

        public SampledFunction()
        {
            InterpolationMode = InterpolationModes.Linear;
        }

        public override Complex Calculate(EngrNumber point)
        {
            if (pointExistsInDomain(point))
                return Data[point]; 
            else
                return Interpolate(point);
            
        }

        public bool pointExistsInDomain(EngrNumber point)
        {
            foreach (var pData in Data.Keys)
                if (pData == point) return true;

            return false;
        }

        public Complex Interpolate(EngrNumber point)
        {
            return Interpolate(point, InterpolationMode);
        }

        public Complex Interpolate(EngrNumber point, InterpolationModes interpolationMode)
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

        public EngrNumber GetNextAbscissa(EngrNumber point)
        {
            var current = EngrNumber.MaxValue;

            foreach (var p in Data.Keys)
                if ((p > point) && (p < current))
                    current = p;

            if (current == EngrNumber.MaxValue)
                throw new ArgumentException();

            return current;
        }

        public EngrNumber GetPreviuosAbscissa(EngrNumber point)
        {
            var current = EngrNumber.MinValue;

            foreach (double p in Data.Keys)
                if ((p < point) && (p > current))
                    current = p;

            if (current == EngrNumber.MinValue)
                throw new ArgumentException(); 

            return current;
        }
        
        public bool isThereNextAbscissa(EngrNumber point)
        {
            try { EngrNumber x = GetNextAbscissa(point); }
            catch { return false; }
            return true;
        }

        public bool isTherePreviousAbscissa(EngrNumber point)
        {
            try { EngrNumber x = GetPreviuosAbscissa(point); }
            catch { return false; }
            return true;
        }

        public bool isPointOutsideRange(EngrNumber point)
        {
            if (Data.Count == 0) return true;
            if (!isThereNextAbscissa(point)) return true;
            if (!isTherePreviousAbscissa(point)) return true;

            return false;
        }

        public Complex getValueOutsideRange(EngrNumber point)
        {
            if (Data.Count == 0) throw new InvalidOperationException();
            
            bool ThereIsNextAbscissa = isThereNextAbscissa(point);
            bool ThereIsPreviousAbscissa = isTherePreviousAbscissa(point);

            if (!ThereIsPreviousAbscissa)
                return Data[GetNextAbscissa(point)];
            else
                return Data[GetPreviuosAbscissa(point)];
        }

        private Complex InterpolateNearestNeighbor(EngrNumber point)
        {
            if (Data.Count < 1) throw new InvalidOperationException();

            if (isPointOutsideRange(point)) return getValueOutsideRange(point);

            EngrNumber distanceToNext = GetNextAbscissa(point) - point;
            EngrNumber distanceToPrevious = point - GetPreviuosAbscissa(point);

            if (distanceToNext < distanceToPrevious)
                return Data[GetNextAbscissa(point)];
            else
                return Data[GetPreviuosAbscissa(point)];
        }

        private Complex InterpolateLinear(EngrNumber point)
        {
            if (Data.Count < 2) throw new InvalidOperationException();

            if (isPointOutsideRange(point)) return getValueOutsideRange(point);

            EngrNumber x1 = GetPreviuosAbscissa(point);
            EngrNumber x2 = GetNextAbscissa(point);
            double x = point;
            Complex y1 = Data[x1];
            Complex y2 = Data[x2];

            return y1 + ((x - x1) * (y2 - y1) / (x2 - x1));
        }

        private Complex InterpolateQuadratic(EngrNumber point)
        {
            return 0;
        }

        private Complex InterpolateCubic(EngrNumber point)
        {
            return 0;
        }

        public void addSample(EngrNumber abscissa, Complex value)
        {
            if (Data.Keys.Contains(abscissa))
                Data[abscissa] = value;
            else
                Data.Add(abscissa, value);

            RaiseFunctionChanged();
        }

        public bool removeSample(EngrNumber abscissa)
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
