using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Transition.Common
{
    public struct ComplexDecimal : IEquatable<ComplexDecimal>, IFormattable
    {
        public decimal RealPart { get; }
        public decimal ImaginaryPart { get; }

        public decimal Magnitude { get => Modulus(this); }
        public decimal Phase { get => Argument(this); }

        public static readonly ComplexDecimal Zero = new ComplexDecimal(0, 0);
        public static readonly ComplexDecimal One = new ComplexDecimal(1, 0);
        public static readonly ComplexDecimal ImaginaryOne = new ComplexDecimal(1, 0);

        public ComplexDecimal(decimal real, decimal imag)
        {
            RealPart = real;
            ImaginaryPart = imag;
        }

        public ComplexDecimal(decimal real)
        {
            RealPart = real;
            ImaginaryPart = 0m;
        }


        public ComplexDecimal(double real, double imag)
        {
            RealPart = (decimal)real;
            ImaginaryPart = (decimal)imag;
        }

        public ComplexDecimal(double real)
        {
            RealPart = (decimal)real;
            ImaginaryPart = 0;
        }

        public ComplexDecimal(int real, int imag)
        {
            RealPart = (decimal)real;
            ImaginaryPart = (decimal)imag;
        }

        public ComplexDecimal(int real)
        {
            RealPart = (decimal)real;
            ImaginaryPart = 0m;
        }

        public static ComplexDecimal Add(ComplexDecimal n1, ComplexDecimal n2)
        {
            return new ComplexDecimal(n1.RealPart + n2.RealPart, n1.ImaginaryPart + n2.ImaginaryPart);
        }

        public static ComplexDecimal Subtract(ComplexDecimal n1, ComplexDecimal n2)
        {
            return new ComplexDecimal(n1.RealPart - n2.RealPart, n1.ImaginaryPart - n2.ImaginaryPart);
        }

        public static ComplexDecimal Product(ComplexDecimal n1, ComplexDecimal n2)
        {
            decimal a = n1.RealPart;
            decimal b = n1.ImaginaryPart;
            decimal c = n2.RealPart;
            decimal d = n2.ImaginaryPart;

            return new ComplexDecimal((a * c) - (b * d), (a * d) + (b * c));
        }

        public static ComplexDecimal Divide(ComplexDecimal dividend, ComplexDecimal divisor)
        {
            decimal a = dividend.RealPart;
            decimal b = dividend.ImaginaryPart;
            decimal c = divisor.RealPart;
            decimal d = divisor.ImaginaryPart;

            decimal real = ((a * c) + (b * d)) / ((c * c) + (d * d));
            decimal imag = ((b * c) - (a * d)) / ((c * c) + (d * d));

            return new ComplexDecimal(real, imag);
        }

        public static ComplexDecimal Reciprocal(ComplexDecimal number)
        {
            return 1 / number;
        }

        public static decimal Modulus(ComplexDecimal number)
        {
            return DecimalMath.Sqrt(DecimalMath.Power(number.RealPart, 2) + DecimalMath.Power(number.ImaginaryPart, 2));
        }

        public static decimal Argument(ComplexDecimal number)
        {
            return DecimalMath.Atan2(number.ImaginaryPart, number.RealPart);
        }

        public static ComplexDecimal Exp(ComplexDecimal number)
        {
            decimal realPart = DecimalMath.Exp(number.RealPart);

            ComplexDecimal imaginaryPart = new ComplexDecimal(DecimalMath.Cos(number.ImaginaryPart), DecimalMath.Sin(number.ImaginaryPart));

            return Product(realPart, imaginaryPart);
        }

        public static ComplexDecimal Pow(ComplexDecimal value, ComplexDecimal power)
        {
            decimal a = value.RealPart;
            decimal b = value.ImaginaryPart;
            decimal c = power.RealPart;
            decimal d = power.ImaginaryPart;

            decimal p = DecimalMath.Power((a * a) + (b * b), c / 2) * DecimalMath.Exp(-d * ComplexDecimal.Argument(value));
            decimal q = (c * ComplexDecimal.Argument(value)) + (0.5m * d * DecimalMath.Log((a * a) + (b * b)));

            return new ComplexDecimal(p * DecimalMath.Cos(q), p * DecimalMath.Sin(q));
        }

        public static ComplexDecimal Sqrt(ComplexDecimal value)
        {
            return ComplexDecimal.Pow(value, .5m);
        }

        public static ComplexDecimal operator +(ComplexDecimal n1, ComplexDecimal n2) { return Add(n1, n2); }
        public static ComplexDecimal operator -(ComplexDecimal n1, ComplexDecimal n2) { return Subtract(n1, n2); }
        public static ComplexDecimal operator *(ComplexDecimal n1, ComplexDecimal n2) { return Product(n1, n2); }
        public static ComplexDecimal operator /(ComplexDecimal n1, ComplexDecimal n2) { return Divide(n1, n2); }

        public static bool operator ==(ComplexDecimal n1, ComplexDecimal n2)
            { return (n1.RealPart == n2.RealPart) && (n1.ImaginaryPart == n2.ImaginaryPart); }

        public static bool operator !=(ComplexDecimal n1, ComplexDecimal n2)
            { return (n1.RealPart != n2.RealPart) || (n1.ImaginaryPart != n2.ImaginaryPart); }

        public static implicit operator ComplexDecimal(decimal value)
        {
            return new ComplexDecimal(value, 0m);
        }

        public static implicit operator ComplexDecimal(double value)
        {
            return new ComplexDecimal(value, 0);
        }

        public static implicit operator ComplexDecimal(int value)
        {
            return new ComplexDecimal(value, 0);
        }

        public static implicit operator ComplexDecimal(Complex value)
        {
            return new ComplexDecimal(value.Real, value.Imaginary);
        }

        public static explicit operator Complex(ComplexDecimal value)
        {
            return new Complex(Convert.ToDouble(value.RealPart), Convert.ToDouble(value.ImaginaryPart));
        }

        public static ComplexDecimal FromPolar(decimal modulus, decimal argument)
        {
            decimal realPart = modulus * DecimalMath.Cos(argument);
            decimal imagPart = modulus * DecimalMath.Sin(argument);

            return new ComplexDecimal(realPart, imagPart);
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            
            return obj is ComplexDecimal && Equals((ComplexDecimal)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (RealPart.GetHashCode() * 397) ^ Convert.ToInt32(ImaginaryPart * 398);
            }
        }
        
        public bool Equals(ComplexDecimal other)
        {
            return this == other;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString(format, formatProvider, "i");
        }

        public string ToString(string format, IFormatProvider formatProvider, string imaginarySymbol)
        {
            if (formatProvider == null) formatProvider = CultureInfo.CurrentCulture;
            if (imaginarySymbol == null) imaginarySymbol = "i";

            string output = "";
            output += RealPart.ToString(format, formatProvider) + " ";

            if (ImaginaryPart < 0m)
                output += ImaginaryPart.ToString(format, formatProvider);
            else
                output += "+" + ImaginaryPart.ToString(format, formatProvider) + imaginarySymbol;

            return output;
        }

        public override string ToString()
        {
            return ToString("G", CultureInfo.CurrentCulture);
        }

        public string ToString(string format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        public string ToString(IFormatProvider formatProvider)
        {
            return ToString("G", formatProvider);
        }
    }
}
