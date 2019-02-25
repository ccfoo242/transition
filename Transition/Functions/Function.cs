using Easycoustics.Transition.Common;
using Easycoustics.Transition.Design;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using static Easycoustics.Transition.Design.UserDesign;

namespace Easycoustics.Transition.Functions
{
    public abstract class Function
    {
        public string Title { get; set; }

        public abstract ComplexDecimal Calculate(decimal point);
        public abstract SampledFunction RenderToSampledFunction { get; }
        public abstract void SubmitAllSamplesChanged();

        public Brush StrokeColor { get; set; }
        public double StrokeThickness { get; set; }
        public DoubleCollection StrokeArray { get; set; }

         
        public string FunctionUnit { get; set; } /* Volt, Pascal, Amper, Ohm, etc */
        public string VariableUnit { get; set; } /* Hz, KHz., etc */
        public string FunctionQuantity { get; set; } /* Voltage, Current, Impedance, Pressure, Sec, etc.*/
        public string VariableQuantity { get; set; } /* Frequency, time, etc */

        public static decimal MinimumFrequency => CurrentDesign.MinimumFrequency;
        public static decimal MaximumFrequency => CurrentDesign.MaximumFrequency;

        public static int NumberOfFrequencyPoints => CircuitEditor.CircuitEditor.StaticCurrentDesign.QuantityOfFrequencyPoints;

        public static string[] PhysicalUnits = new string[] {
            "Amper",
            "Kg",
            "Hz",
            "Joule",
            "Meter",
            "Newton",
            "Ohm",
            "Pascal",
            "Sec",
            "Tesla",
            "Volt",
            "Watt",
            ""
            };

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

        public Function()
        {
            VariableUnit = "Hz";
            VariableQuantity = "Frequency";
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
                RaiseFunctionChanged(new FunctionChangedEventArgs()
                    { Action = FunctionChangedEventArgs.FunctionChangeAction.Reset });
            }
        }
        

        public override SampledFunction RenderToSampledFunction
        {
            get
            {
                var output = new SampledFunction(this);

                output.addOrChangeSample(MinimumFrequency, FixedValue);
                output.addOrChangeSample(MinimumFrequency, fixedValue);

                return output;
            }
        }

        public ConstantValueFunction(Complex fixedValue) : base()
        {
            FixedValue = fixedValue;
        }

        public override ComplexDecimal Calculate(decimal point)
        {
            return FixedValue;
        }

        public override void SubmitAllSamplesChanged()
        {
            throw new NotImplementedException();
        }
    }







    public class SampledFunction : Function, INotifyPropertyChanged, ICloneable
    {
       // public Dictionary<decimal, ComplexDecimal> Data { get; } = new Dictionary<decimal, ComplexDecimal>();
        public ObservableCollection<Tuple<decimal, ComplexDecimal>> Data { get; } = new ObservableCollection<Tuple<decimal, ComplexDecimal>>();
        
        public InterpolationModes InterpolationMode = InterpolationModes.Linear;
        public AxisScale VariableScale { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        /* (Data collection must be ordered by domain/frequency/Item1, asc) */
        public decimal GetMinimumDomain => Data.First().Item1;
        public decimal GetMaximumDomain => Data.Last().Item1;

        //public decimal dBReference;

        public override SampledFunction RenderToSampledFunction => this;

      
        public SampledFunction() : base()
        {
            var rnd = new Random();
            StrokeColor = new SolidColorBrush(Color.FromArgb(255, (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255)));
            StrokeThickness = 2;

        }

        public SampledFunction(Function other)
        {
            FunctionQuantity = other.FunctionQuantity;
            FunctionUnit = other.FunctionUnit;
            VariableQuantity = other.VariableQuantity;
            VariableUnit = other.VariableUnit;
            StrokeArray = other.StrokeArray;
            StrokeThickness = other.StrokeThickness;
            StrokeColor = other.StrokeColor;
            Title = other.Title;
        }

        public void Clear()
        {
            //Data.Clear();
            Data.Clear();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data"));
        }

        public override ComplexDecimal Calculate(decimal point)
        {
            if (pointExistsInDomain(point))
                return getPointValue(point); 
            else
                return Interpolate(point);
            

        }

        public bool pointExistsInDomain(decimal point)
        {/*
            foreach (var pData in Data.Keys)
                if (pData == point) return true;

            return false;*/
            var output = Data.Where(data => data.Item1 == point).ToList();

            return (output.Count != 0);
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

        public Tuple<decimal,ComplexDecimal> GetNextAbscissa(decimal point)
        {
       
            var current = decimal.MaxValue;
            Tuple<decimal, ComplexDecimal> output = new Tuple<decimal, ComplexDecimal>(0, 0);


            foreach (var p in Data)
                if ((p.Item1 > point) && (p.Item1 < current))
                {
                    current = p.Item1;
                    output = p;
                }

            if (current == decimal.MaxValue)
                throw new ArgumentException();

            return output;
        }

        public Tuple<decimal, ComplexDecimal> GetPreviuosAbscissa(decimal point)
        {
            var current = decimal.MinValue;
            Tuple<decimal, ComplexDecimal> output = new Tuple<decimal, ComplexDecimal>(0, 0);


            foreach (var p in Data)
                if ((p.Item1 < point) && (p.Item1 > current))
                {
                    current = p.Item1;
                    output = p;
                }


            if (current == decimal.MinValue)
                throw new ArgumentException(); 

            return output;
        }

        public Tuple<decimal, ComplexDecimal> getTuple(decimal abs)
        {
            var output = Data.Where(tup => tup.Item1 == abs).ToList();

            if (output.Count != 0) return output[0];
            else
                throw new InvalidOperationException();
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
                return GetNextAbscissa(point).Item2;
            else
                return GetPreviuosAbscissa(point).Item2;
                
            /*
                return Data[GetNextAbscissa(point)];
            else
                return Data[GetPreviuosAbscissa(point)];*/

        }

        private ComplexDecimal InterpolateNearestNeighbor(decimal point)
        {
            if (Data.Count < 1) throw new InvalidOperationException();

            if (isPointOutsideRange(point)) return getValueOutsideRange(point);
            /*
            var distanceToNext = GetNextAbscissa(point) - point;
            var distanceToPrevious = point - GetPreviuosAbscissa(point);

            if (distanceToNext < distanceToPrevious)
                return Data[GetNextAbscissa(point)];
            else
                return Data[GetPreviuosAbscissa(point)];
                */

            var distanceToNext = GetNextAbscissa(point).Item1 - point;
            var distanceToPrevious = point - GetPreviuosAbscissa(point).Item1;

            if (distanceToNext < distanceToPrevious)
                return GetNextAbscissa(point).Item2;
            else
                return GetPreviuosAbscissa(point).Item2;
        }

        private ComplexDecimal InterpolateLinear(decimal x)
        {
            if (Data.Count < 2) throw new InvalidOperationException();

            if (isPointOutsideRange(x)) return getValueOutsideRange(x);

            var x1 = GetPreviuosAbscissa(x).Item1;
            var x2 = GetNextAbscissa(x).Item1;
           
            var y1 = GetPreviuosAbscissa(x).Item2;
            var y2 = GetNextAbscissa(x).Item2;

            return y1 + ((x - x1) * (y2 - y1) / (x2 - x1));
        }

        private int getPointIndex(decimal point)
        {
            int output = 0;

            var sortedKeys = getSortedKeys();

            foreach (var k in sortedKeys)
                if (k == point) return output; else output++;

            return -1;
        }

        private ComplexDecimal getPointValue(decimal point)
        {
            var output = Data.Where(tup => tup.Item1 == point).ToList();

            if (output.Count != 0) return output[0].Item2;

            throw new InvalidOperationException();
        }

        private ComplexDecimal getPointIndex(int index)
        {
            return Data[index].Item2;
        }

        private List<decimal> getSortedKeys()
        {
            var sortedKeys = new List<decimal>();

            foreach (var tup in Data)
                sortedKeys.Add(tup.Item1);

            sortedKeys.Sort();

            return sortedKeys;
        }

        private ComplexDecimal InterpolateQuadratic(decimal x)
        {
            if (Data.Count < 3) throw new InvalidOperationException();

            if (isPointOutsideRange(x)) return getValueOutsideRange(x);

            var t0 = GetPreviuosAbscissa(x).Item1;
            var t1 = GetNextAbscissa(x).Item1;
            var y0 = GetPreviuosAbscissa(x).Item2; 
           
            var index0 = getPointIndex(t0);
            var index1 = getPointIndex(t1);

            var sortedKeys = getSortedKeys();

            var quantityOfPoints = sortedKeys.Count;

            ComplexDecimal z0 = 0;
            ComplexDecimal z1 = 0;

            int currentIndex = 0;

            while (currentIndex <= index0)
            {
                z0 = z1;
                z1 = (-1 * z0) + 2 * ((Data[currentIndex + 1].Item2 - Data[currentIndex].Item2) / (Data[currentIndex + 1].Item1 - Data[currentIndex].Item1));
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

            if (pointExistsInDomain(abscissa))
            {
                int index = getPointIndex(abscissa);

                Data[index] = new Tuple<decimal, ComplexDecimal>(abscissa,value);
                args = new FunctionChangedEventArgs()
                {
                    Action = FunctionChangedEventArgs.FunctionChangeAction.PointChanged,
                    X = abscissa,
                    Y = value
                };
            }
            else
            {
                Data.Add(new Tuple<decimal, ComplexDecimal>(abscissa, value));
                args = new FunctionChangedEventArgs()
                {
                    Action = FunctionChangedEventArgs.FunctionChangeAction.PointAdded,
                    X = abscissa,
                    Y = value
                };
            }
            
            RaiseFunctionChanged(args);
        }

        public bool removeSample(decimal abscissa)
        {
            if (pointExistsInDomain(abscissa))
            {
                Data.Remove(getTuple(abscissa));
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
            if (VariableScale != scale) return false;

            return true;
        }



        public bool isCompatible(SampledFunction func2)
        {
            /* if the two curves have the same domain points, we say they are compatible */

            if (Data.Count != func2.Data.Count) return false;
            if (GetMinimumDomain != func2.GetMinimumDomain) return false;
            if (VariableScale != func2.VariableScale) return false;

            return true;
        }


        public void AdaptFunctionTo(decimal minimumDomain, decimal maximumDomain, int quantityOfPoints, AxisScale scale)
        {
            var ObjectivePoints = UserDesign.getFrequencyPoints(scale, maximumDomain, minimumDomain, quantityOfPoints);

            var toDelete = new List<Tuple<decimal,ComplexDecimal>>();

            foreach (var obs in Data)
                if (!ObjectivePoints.Contains(obs.Item1))
                    toDelete.Add(obs);

            foreach (var del in toDelete)
                Data.Remove(del);

        }

        public override void SubmitAllSamplesChanged()
        {
            RaiseFunctionChanged(new FunctionChangedEventArgs()
            { Action = FunctionChangedEventArgs.FunctionChangeAction.Reset });
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data"));
        }

        public object Clone()
        {
            var output = new SampledFunction(this);

            output.VariableScale = VariableScale;
            output.InterpolationMode = InterpolationMode;

            foreach (var tup in Data)
                output.Data.Add(tup);
            

            return output;
        }
    }

    public enum InterpolationModes { NearestNeighbor, Linear, Quadratic, Cubic };
 

}
 