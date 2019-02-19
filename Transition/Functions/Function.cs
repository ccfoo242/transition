using Easycoustics.Transition.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using static Easycoustics.Transition.Design.UserDesign;

namespace Easycoustics.Transition.Functions
{
    public abstract class Function
    {
        public string Title { get; set; }

        public abstract ComplexDecimal Calculate(decimal point);
        public Brush StrokeColor { get; set; }
        public double StrokeThickness { get; set; }
        public DoubleCollection StrokeArray { get; set; }

        public abstract Dictionary<decimal, ComplexDecimal> Points { get; } 
        
        public string FunctionUnit { get; set; }
        public string VariableUnit { get; set; }

        public static decimal MinimumFrequency => CircuitEditor.CircuitEditor.StaticCurrentDesign.MinimumFrequency;
        public static decimal MaximumFrequency => CircuitEditor.CircuitEditor.StaticCurrentDesign.MaximumFrequency;

        public static int NumberOfFrequencyPoints => CircuitEditor.CircuitEditor.StaticCurrentDesign.QuantityOfFrequencyPoints;

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

        public event Action<Function, FunctionChangedEventArgs> FunctionChanged;
        
        protected void RaiseFunctionChanged(FunctionChangedEventArgs args) => FunctionChanged?.Invoke(this, args);

        public void SubmitChange()
        {
            RaiseFunctionChanged(new FunctionChangedEventArgs() { Action = FunctionChangedEventArgs.FunctionChangeAction.Reset });
        }
    }


    public class FunctionChangedEventArgs : EventArgs
    {
        public FunctionChangeAction Action;
        public decimal X;
        public ComplexDecimal Y;

        public enum FunctionChangeAction { PointChanged, PointAdded, PointRemoved, Reset }
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
                RaiseFunctionChanged(new FunctionChangedEventArgs() { Action = FunctionChangedEventArgs.FunctionChangeAction.Reset });
            }
        }

        public override Dictionary<decimal, ComplexDecimal> Points
        {
            get
            {
                var output = new Dictionary<decimal, ComplexDecimal>();

                output.Add(MinimumFrequency, FixedValue);
                output.Add(MaximumFrequency, FixedValue);

                return output;
            }
        }
        
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

        public decimal GetMinimumDomain
        {
            get
            {
                var output = decimal.MaxValue;
                if (Data.Count == 0) throw new InvalidOperationException("Data array is empty");

                foreach (var kvp in Data)
                    if (kvp.Key < output) output = kvp.Key;

                return output;
            }
        }

        public decimal GetMaximumDomain
        {
            get
            {
                var output = decimal.MinValue;
                if (Data.Count == 0) throw new InvalidOperationException("Data array is empty");

                foreach (var kvp in Data)
                    if (kvp.Key > output) output = kvp.Key;

                return output;
            }
        }

        public SampledFunction()
        {
            InterpolationMode = InterpolationModes.Linear;
        }

        public void Clear()
        {
            Data.Clear();
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

        private ComplexDecimal InterpolateLinear(decimal x)
        {
            if (Data.Count < 2) throw new InvalidOperationException();

            if (isPointOutsideRange(x)) return getValueOutsideRange(x);

            var x1 = GetPreviuosAbscissa(x);
            var x2 = GetNextAbscissa(x);
           
            var y1 = Data[x1];
            var y2 = Data[x2];

            return y1 + ((x - x1) * (y2 - y1) / (x2 - x1));
        }

        private int getPointIndex(decimal point)
        {
            int output = 0;

            var sortedKeys = Data.Keys.ToList();
            sortedKeys.Sort();

            foreach (var k in sortedKeys)
                if (k == point) return output; else output++;

            return -1;
        }

        private ComplexDecimal InterpolateQuadratic(decimal x)
        {
            if (Data.Count < 3) throw new InvalidOperationException();

            if (isPointOutsideRange(x)) return getValueOutsideRange(x);

            var t0 = GetPreviuosAbscissa(x);
            var t1 = GetNextAbscissa(x);
            var y0 = Data[t0];
           
            var index0 = getPointIndex(t0);
            var index1 = getPointIndex(t1);

            var sortedKeys = Data.Keys.ToList();
            sortedKeys.Sort();

            var quantityOfPoints = sortedKeys.Count;

            ComplexDecimal z0 = 0;
            ComplexDecimal z1 = 0;

            int currentIndex = 0;

            while (currentIndex <= index0)
            {
                z0 = z1;
                z1 = (-1 * z0) + 2 * ((Data[sortedKeys[currentIndex + 1]] - Data[sortedKeys[currentIndex]]) / (sortedKeys[currentIndex + 1] - sortedKeys[currentIndex]));
                currentIndex++;
            }

            ComplexDecimal output = ((z1 - z0) / (2 * (t1 - t0)));
            output *= (x - t0) * (x - t0);
            output += z0 * (x - t0);
            output += y0;

            return output;
        }

        private ComplexDecimal InterpolateCubic(decimal point)
        {
            return 0;
        }

        public void addOrChangeSample(decimal abscissa, ComplexDecimal value)
        {
            FunctionChangedEventArgs args;

            if (Data.Keys.Contains(abscissa))
            {
                Data[abscissa] = value;
                args = new FunctionChangedEventArgs()
                {
                    Action = FunctionChangedEventArgs.FunctionChangeAction.PointChanged,
                    X = abscissa,
                    Y = value
                };
            }
            else
            {
                Data.Add(abscissa, value);
                args = new FunctionChangedEventArgs()
                {
                    Action = FunctionChangedEventArgs.FunctionChangeAction.PointAdded,
                    X = abscissa,
                    Y = value
                };
            }

           // RaiseFunctionChanged(args);
        }

        public bool removeSample(decimal abscissa)
        {
            if (Data.Keys.Contains(abscissa))
            {
                Data.Remove(abscissa);
              /*  RaiseFunctionChanged(new FunctionChangedEventArgs()
                {
                    Action = FunctionChangedEventArgs.FunctionChangeAction.PointRemoved,
                    X = abscissa
                });*/
                return true;
            }
            else
                return false;
        }

       
        public bool isCompatible(decimal minimumDomain, decimal maximumDomain, int quantityOfPoints, AxisScale scale)
        {
            if (Data.Count == 0) return false;

            if (GetMinimumDomain != minimumDomain) return false;
           // if (GetMaximumDomain != maximumDomain) return false;

            if (Data.Count != quantityOfPoints) return false;

            var domainPoints = getFrequencyPoints(scale, maximumDomain, minimumDomain, quantityOfPoints);

            foreach (var point in domainPoints)
                if (!Data.Keys.Contains(point)) return false;

            return true;

        }

        public bool isCompatible(SampledFunction func2)
        {
            /* if the two curves have the same domain points, we say they are compatible */

            if (Data.Count != func2.Data.Count) return false;

            foreach (var kvp in Data)
                if (!func2.Data.ContainsKey(kvp.Key)) return false;

            return true;
        }

    }

    public enum InterpolationModes { NearestNeighbor, Linear, Quadratic, Cubic };

}
