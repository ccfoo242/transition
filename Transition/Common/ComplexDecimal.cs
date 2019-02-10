using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Transition.Common
{
    public struct ComplexDecimal : IEquatable<ComplexDecimal>, IFormattable
    {
        public decimal RealPart { get; }
        public decimal ImaginaryPart { get; }

        public decimal Magnitude { get => Modulus(this); }
        public decimal Phase { get => Argument(this); }

        public ComplexDecimal Reciprocal { get => 1 / this; }
        public ComplexDecimal Conjugate { get => new ComplexDecimal(RealPart, -1 * ImaginaryPart); }

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
        
        public static decimal Modulus(ComplexDecimal number)
        {
            return DecimalMath.Sqrt(DecimalMath.Power(number.RealPart, 2) + DecimalMath.Power(number.ImaginaryPart, 2));
        }

        public static decimal Argument(ComplexDecimal number)
        {
            return DecimalMath.Atan2(number.ImaginaryPart, number.RealPart);
        }

        public static ComplexDecimal Parallel(ComplexDecimal z1, ComplexDecimal z2)
        {
            if (z1.Magnitude == 0m || z2.Magnitude == 0m) return 0m;

            return (z1.Reciprocal + z2.Reciprocal).Reciprocal;
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
        
        public static ComplexDecimal operator |(ComplexDecimal n1, ComplexDecimal n2) { return Parallel(n1, n2); }

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

        public static ComplexDecimal Parse(string input)
        {
            return Parse(input, CultureInfo.CurrentCulture, "i");
        }

        public static ComplexDecimal Parse(string input, IFormatProvider formatProvider)
        {
            return Parse(input, formatProvider, "i");
        }

        public static ComplexDecimal Parse(string input, string imaginaryUnit)
        {
            return Parse(input, CultureInfo.CurrentCulture, imaginaryUnit);
        }
        
        public static ComplexDecimal Parse(string input, IFormatProvider formatProvider, string imaginarySymbol)
        {
            /* this can ONLY parse first real part, second imaginary part 
             the other order is NOT allowed.
             but it is possible to enter only the imaginary part, or only real part. 
             only ONE number each, this method cannot perform a sum 
             */
            /* examples of supported input:
             2+2i
             +30-20i
             +0.3i
             i
             3
             -5
             .23+.45i
             0.97-0.15i
             -.78E03-2.02E-03i 
             */

            var numberFormat = NumberFormatInfo.GetInstance(formatProvider);

            var decimalSymbol = numberFormat.NumberDecimalSeparator;
            var positiveSymbol = numberFormat.PositiveSign;
            var negativeSymbol = numberFormat.NegativeSign;
            var exponentSymbol = "E";
            var numberSymbol = "#";

            string realSt = "";
            string imagSt = "";
            string tmpSt = "";

            int currentState = 0;

            input = input.Trim();

            var westernNumbers = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", };

            var states = new Dictionary<int, Dictionary<string, int>>();

            /* input symbol => next state */
            /* if a symbol is entered that has not a next state in the current state
               that is an invalid symbol, is an error, and an exception is thrown*/

            /* init state */
            states.Add(0, new Dictionary<string, int>()
                { { numberSymbol, 1 } ,
                  { positiveSymbol, 2 } ,
                  { negativeSymbol, 2 },
                  { decimalSymbol, 3 } ,
                  { imaginarySymbol, 5 } });

            /* the first number from init state has entered,
             * machine is parsing the integer part of the real part
             from now on, if a +- enters, the machine starts to
             parse the imaginary portion of the complex number */
            states.Add(1, new Dictionary<string, int>()
                { { numberSymbol, 1 },
                  { decimalSymbol, 3 },
                  { imaginarySymbol, 5 },
                  { exponentSymbol, 6},
                  { positiveSymbol, 10 },
                  { negativeSymbol, 10 }, });

            /* a +- symbol entered from init state,
             * this symbol is the sign of the real portion of the
             complex number */
            states.Add(2, new Dictionary<string, int>()
                { { numberSymbol, 1 },
                  { decimalSymbol, 3 } });

            /* integer part of the real part finished parsing, a 
             decimal separator entered, machine starts to parse
             the decimal part of the real portion of the complex number
             only a number is expected in this state */
            states.Add(3, new Dictionary<string, int>()
                { { numberSymbol, 4 } });

            /* first number has entered,  machine is now
             parsing the decimal part of the
             real portion of the complex number
             machine now can accept an exponential, or a +-
             symbol for start to parse the imaginary portion of
             the complex number */
            states.Add(4, new Dictionary<string, int>()
                { { numberSymbol, 4 },
                  { imaginarySymbol, 5 },
                  { exponentSymbol, 6 },
                  { positiveSymbol, 10 },
                  { negativeSymbol, 10 } });

            /* and imaginary unit symbol has entered, 
             * only thing expected here is the end of the string */
            states.Add(5, new Dictionary<string, int>());

            /* and exponential symbol entered, machine is now
             * parsing the exponent of the real portion of the complex,
               the exponent can have a sign*/
            states.Add(6, new Dictionary<string, int>()
                { { positiveSymbol, 7 },
                  { negativeSymbol, 7 },
                  { numberSymbol, 8 } });

            /* a sign of the exponent of the real portion entered,
               from here, only a number can be entered, the first
               digit of the exponent of the real portion of the complex number*/
            states.Add(7, new Dictionary<string, int>()
                { { numberSymbol, 8} });

            /* machine is parsing the exponent of the real portion
               of the complex*/
            states.Add(8, new Dictionary<string, int>()
                { { imaginarySymbol, 5 },
                  { numberSymbol, 8 },
                  { positiveSymbol, 10 },
                  { negativeSymbol, 10 } });

            /* parsing the real portion has finished. machine starts to parse
               the imaginary portion */
            states.Add(10, new Dictionary<string, int>()
                { { imaginarySymbol, 5},
                  { numberSymbol, 11 },
                  { decimalSymbol, 15 } });

            /* a number entered, we are parsing the integer part
             * of the imaginary portion*/
            states.Add(11, new Dictionary<string, int>()
                { { imaginarySymbol, 5 },
                  { numberSymbol, 11 },
                  { exponentSymbol, 12 } ,
                  { decimalSymbol, 15 } });

            /* an exponent symbol entered in the imaginary portion */
            states.Add(12, new Dictionary<string, int>()
                { { positiveSymbol, 13 },
                  { negativeSymbol, 13 },
                  { numberSymbol, 14 } });

            /* the sign of the exponent of the imaginary portion entered,
             only a number is expected */
            states.Add(13, new Dictionary<string, int>()
                { { numberSymbol, 14 } });

            /* we are parsing the exponent of the imaginary portion */
            states.Add(14, new Dictionary<string, int>()
                { { numberSymbol, 14 },
                  { imaginarySymbol, 5 } });

            /* a decimal separator entered, machine starts to 
             parse the decimal part of the imaginary portion */
            states.Add(15, new Dictionary<string, int>()
                { { numberSymbol, 16 } });

            /* we are parsing the decimal part of the imaginary portion */
            states.Add(16, new Dictionary<string, int>()
                { { numberSymbol, 16 },
                  { imaginarySymbol, 5 },
                  { exponentSymbol, 12 } });

            string tmpChar;

            foreach (char a in input)
            {
                tmpChar = a.ToString();

                if (!String.IsNullOrWhiteSpace(tmpChar))
                {
                    if (westernNumbers.Contains(tmpChar)) tmpChar = "#";

                    if (!states[currentState].Keys.Contains(tmpChar))
                        throw new FormatException();

                    if ((currentState == 1) || (currentState == 4) || (currentState == 8))
                        if ((tmpChar == positiveSymbol) || (tmpChar == negativeSymbol))
                        {
                            realSt = tmpSt;
                            tmpSt = "";
                        }

                    if (a.ToString() != imaginarySymbol) tmpSt += a.ToString();

                    currentState = states[currentState][tmpChar];
                }
            }

            if (!((currentState == 1) ||
                (currentState == 4) ||
                (currentState == 5) ||
                (currentState == 8)))
            /* string finished in a invalid moment */
            { throw new FormatException(); }

            if ((currentState == 1) ||
                (currentState == 4) ||
                (currentState == 8))
            {
                realSt = tmpSt;
            }

            if (currentState == 5)
                imagSt = tmpSt;

            var real = decimal.Parse(realSt, System.Globalization.NumberStyles.Float, formatProvider);
            var imag = decimal.Parse(imagSt, System.Globalization.NumberStyles.Float, formatProvider);

            return new ComplexDecimal(real, imag);
        }

    }


    public class DecimalEngrConverter : IValueConverter
    {
        public bool ShortString { get; set; }
        public bool AllowNegativeNumber { get; set; }

        private Dictionary<string, int> prefixes = new Dictionary<string, int>()
                {{ "y", -24 },
                 { "z", -21 },
                 { "a", -18 },
                 { "f", -15 },
                 { "p", -12 },
                 { "n", -9 },
                 { "μ", -6 },
                 { "u", -6 },
                 { "m", -3 },
                 { "k", 3 },
                 { "K", 3 }, 
                 { "M", 6 }, 
                 { "G", 9 }, 
                 { "T", 12 }, 
                 { "P", 15 }, 
                 { "E", 18 }, 
                 { "Z", 21 },
                 { "Y", 24 }};


        private string[] numbers = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        public object Convert(object value, Type targetType, object parameter, string language)
        {   
            //number to string
            if (value == null) return "";
            decimal d = (decimal)value;
            if (d == 0m) return (0.0000m).ToString();

            decimal exponent = (int)Math.Floor(Math.Log10((double)Math.Abs(d)));
            decimal output = 0m;
            string prefix = "";
            bool hasPrefix = false;
            bool exponentOutsidePrefixes = false;
            bool negativeExponent = false;


            if (exponent >= 0)
            {
                switch (exponent)
                {
                    case 0:
                    case 1:
                    case 2:
                        output = d; prefix = ""; hasPrefix = false; break;
                    case 3:
                    case 4:
                    case 5:
                        output = (d / 1e3m); prefix = "K"; hasPrefix = true; break;
                    case 6:
                    case 7:
                    case 8:
                        output = (d / 1e6m); prefix = "M"; hasPrefix = true; break;
                    case 9:
                    case 10:
                    case 11:
                        output = (d / 1e9m); prefix = "G"; hasPrefix = true; break;
                    case 12:
                    case 13:
                    case 14:
                        output = (d / 1e12m); prefix = "T"; hasPrefix = true; break;
                    case 15:
                    case 16:
                    case 17:
                        output = (d / 1e15m); prefix = "P"; hasPrefix = true; break;
                    case 18:
                    case 19:
                    case 20:
                        output = (d / 1e18m); prefix = "E"; hasPrefix = true; break;
                    case 21:
                    case 22:
                    case 23:
                        output = (d / 1e21m); prefix = "Z"; hasPrefix = true; break;
                    case 24:
                    case 25:
                    case 26:
                        output = (d / 1e24m); prefix = "Y"; hasPrefix = true; break;
                }

                if (exponent >= 27)
                {
                    exponentOutsidePrefixes = true;
                    output = d / (decimal)Math.Pow(10, (int)exponent);
                }
            }
            else if (exponent < 0)
            {
                negativeExponent = true;

                switch (exponent)
                {
                    case -1:
                    case -2:
                    case -3:
                        output = (d * 1e3m); prefix = "m"; hasPrefix = true; break;
                    case -4:
                    case -5:
                    case -6:
                        output = (d * 1e6m); prefix = "μ"; hasPrefix = true; break;
                    case -7:
                    case -8:
                    case -9:
                        output = (d * 1e9m); prefix = "n"; hasPrefix = true; break;
                    case -10:
                    case -11:
                    case -12:
                        output = (d * 1e12m); prefix = "p"; hasPrefix = true; break;
                    case -13:
                    case -14:
                    case -15:
                        output = (d * 1e15m); prefix = "f"; hasPrefix = true; break;
                    case -16:
                    case -17:
                    case -18:
                        output = (d * 1e18m); prefix = "a"; hasPrefix = true; break;
                    case -19:
                    case -20:
                    case -21:
                        output = (d * 1e21m); prefix = "z"; hasPrefix = true; break;
                    case -22:
                    case -23:
                    case -24:
                        output = (d * 1e24m); prefix = "y"; hasPrefix = true; break;
                }
                if (exponent <= -25)
                {
                    exponentOutsidePrefixes = true;
                    output = d * (decimal)Math.Pow(10, Math.Abs((int)exponent));
                }
            }

            //output = decimal.Round(output, 4);
            string stOutput;

            if (ShortString)
                stOutput = output.ToString("###.####");
            else
                stOutput = output.ToString("##0.0000");

            if (!exponentOutsidePrefixes)
            {
                if (hasPrefix) stOutput += " " + prefix;
            }
            else
                stOutput += "E" + exponent.ToString();

            return stOutput;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            //string to decimal
            /* here we accept general, engineering and scientific notations */

            bool isEngrNotation = false;

            if (value == null) throw new ArgumentException();
            
            var valueString = (string)value;
            CultureInfo culture;

            if (language == null)
                culture = CultureInfo.InvariantCulture;
            else
            {
                if (language == "")
                    culture = CultureInfo.InvariantCulture;
                else
                    culture = CultureInfo.GetCultureInfo(language);
            }
            
            valueString = valueString.Trim();
            if (valueString.Length == 0) throw new ArgumentException();

            string lastChar = valueString.Substring(valueString.Length - 1, 1);

            if (!prefixes.Keys.Contains(lastChar) && !numbers.Contains(lastChar))
                throw new FormatException("Invalid SI Prefix");

            int exponent;

            if (numbers.Contains(lastChar))
                exponent = 0;
            else
            {
                exponent = prefixes[lastChar];
                isEngrNotation = true;
            }

            string valueString2 = isEngrNotation ? valueString.Substring(0, valueString.Length - 1) : valueString;

            decimal result;
            bool didParse = decimal.TryParse(valueString2, NumberStyles.Float, culture, out result);

            if (didParse)
               return result * DecimalMath.PowerN(10m, exponent); 
            else
               throw new ArgumentException(); 

        }
    }
}
