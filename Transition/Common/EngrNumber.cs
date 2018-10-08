using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Transition
{

    [Windows.Foundation.Metadata.CreateFromString(MethodName = "Transition.EngrNumber.Parse")]
    public struct EngrNumber : IComparable, IEquatable<EngrNumber>, IComparable<EngrNumber>
    {
        /* this struct stores numbers in engineering notation */
        // it is immutable

        public static readonly EngrNumber One = new EngrNumber(1M, "");

        public decimal Mantissa { get;  }
        public string Prefix { get; }

        public bool NegativeSign { get { return Mantissa < 0; } }
        public bool IsZero { get { return Mantissa == 0; } }

        public double ValueDouble { get { return (double)Mantissa * Math.Pow(10, getExponent(Prefix)); } }
        public decimal ValueDecimal { get { return Mantissa * (decimal)Math.Pow(10, getExponent(Prefix)); } }
        
        public static readonly Dictionary<String, int> mapPrefixes =
            new Dictionary<string, int> {
                { "Y",  24 },
                { "Z",  21 },
                { "E",  18 },
                { "P",  15 },
                { "T",  12 },
                { "G",   9 },
                { "M",   6 },
                { "K",   3 },
                { "k" ,  3 },
                { "" ,   0 },
                { "m",  -3 },
                { "u",  -6 },
                { "n",  -9 },
                { "p", -12 },
                { "f", -15 },
                { "a", -18 },
                { "z", -21 },
                { "y", -24 }
            };
        /* if you notice that the prefix Kilo appears two times,
           it is because both upper and lower case are allowed
           k and K, in case of inverse function, (getting prefix
           from exponent), upper case character K is returned.             
        */
        public EngrNumber(decimal mantissa, string prefix)
        {
            if (prefix == null) prefix = "";
            
            this.Mantissa = mantissa;
            this.Prefix = prefix;

            /* Normalize the number, 
            this must be done in the constructor only
           
            normalize means to have one, two or three digits
             in the left side of the point, and adjust the prefix accordingly
             e.g. :
              0.01234  ==>  12.3400m
              1234567K ==>   1.2346G
              12345    ==>  12.3450K
             this operation does not alter the total value of the number
            */
            if (Mantissa == 0) return;

             if (!NegativeSign)
             {
                 //positive sign
                 while ((Mantissa < 1) || (Mantissa >= 1000))
                 {
                     if (Mantissa < 1)
                     {
                         Prefix = getPrefix(getExponent() - 3);
                         Mantissa *= 1000;
                     }

                     if (Mantissa >= 1000)
                     {
                         Prefix = getPrefix(getExponent() + 3);
                         Mantissa /= 1000;
                     }
                 }
             }
             else
             {
                 //negative sign
                 while ((Mantissa > -1) || (Mantissa <= -1000))
                 {
                     if (Mantissa > -1)
                     {
                         Prefix = getPrefix(getExponent() - 3);
                         Mantissa *= 1000;
                     }

                     if (Mantissa <= -1000)
                     {
                         Prefix = getPrefix(getExponent() + 3);
                         Mantissa /= 1000;
                     }
                 }
             }

             Mantissa = decimal.Round(Mantissa, 4);
             
        }

        public int CompareTo(Object obj)
        {
            if (obj is EngrNumber)
                return CompareTo((EngrNumber)obj);

            return 0;
        }

        public int CompareTo(EngrNumber other)
        {
            return (this.ValueDouble < other.ValueDouble) ? -1 :
                   ((this.ValueDouble > other.ValueDouble) ? 1 : 0);
        }

        public EngrNumber(decimal input) : this(input, "") { }
        public EngrNumber(int input) : this((decimal)input, "") { }
        public EngrNumber(double input) : this((decimal)input, "") { }
        public EngrNumber(string input)
        {
            EngrNumber parsed = Parse(input);
            this.Mantissa = parsed.Mantissa;
            this.Prefix = parsed.Prefix;

        }
        
        public override string ToString()
        {
            if (Prefix == null)
                return Mantissa.ToString("##0.0000");
            else
                return Mantissa.ToString("##0.0000") + Prefix;
        }

        public string ToShortString()
        {
            if (Prefix == null)
                return Mantissa.ToString();
            else
                return Mantissa.ToString() + Prefix;
        }

        public static string getPrefix(int exponent)
        {
            if (mapPrefixes.Values.Contains(exponent))
            {
                foreach (KeyValuePair<String, int> pair in mapPrefixes)
                    if (pair.Value == exponent) return pair.Key;
                throw new ArgumentException();
            }
            else
                throw new ArgumentException();
        }

        public static int getExponent(string prefix)
        {
            if (!validPrefix(prefix)) throw new ArgumentException();

            return mapPrefixes[prefix];
           
        }

        public static bool validPrefix(string prefix)
        {
            if (prefix == null) return false;

            return mapPrefixes.Keys.Contains(prefix);
        }

        public static string getInversePrefix(string prefix)
        {
            if (!validPrefix(prefix))
                throw new ArgumentException();

            return getPrefix(getExponent(prefix) * -1);
        }

        public int getExponent()
        {
            if (Prefix != null)
                return getExponent(Prefix);
            else return 0;
        }
        
        
        public decimal getOneDigitMantissa()
        {
            decimal output = Mantissa;

            if (output == 0) return 0M;

            if (!NegativeSign)

                //positive sign
                while ((output < 1) || (output >= 10))
                {
                    if (output < 1) output *= 10;
                    if (output >= 10) output /= 10;
                }
            else
                //negative sign
                while ((output > -1) || (output <= -10))
                {
                    if (output > -1) output *= 10;
                    if (output <= -10) output /= 10;
                }
            return output;
        }

        public static EngrNumber Parse(string rawString)
        {
            rawString.Trim();

            if (rawString.Length == 0)
                throw new ArgumentException();

            string lastChar = rawString.Substring(rawString.Length - 1, 1);
           
            bool prefixExists = validPrefix(lastChar);

            string stringMantissa;
            string prefix;

            if (prefixExists)
            {
                stringMantissa = rawString.Substring(0, rawString.Length - 1);
                stringMantissa.Trim();
                prefix = lastChar;
            }
            else
            {
                stringMantissa = rawString;
                prefix = "";
            }
            
            decimal parsedMantissa = 0;

            try   { parsedMantissa = decimal.Parse(stringMantissa); }
            catch { throw new ArgumentException(); }

            if (!validPrefix(prefix))
                throw new ArgumentException();

            return new EngrNumber(parsedMantissa, prefix);
        }

        public bool Equals(EngrNumber other)
        {
            if (other.Mantissa.Equals(Mantissa))
                if (other.Prefix == Prefix)
                    return true;

            return false;
        }

        public override bool Equals(object obj)
        {
            return ((obj is EngrNumber) && Equals((EngrNumber)obj));
        }

        public static EngrNumber Add(EngrNumber n1, EngrNumber n2)
        {
            return new EngrNumber(n1.ValueDecimal + n2.ValueDecimal);
        }

        public static EngrNumber Multiply(EngrNumber n1, EngrNumber n2)
        {
            return new EngrNumber(n1.ValueDecimal * n2.ValueDecimal);
        }

        public static EngrNumber Substract(EngrNumber n1, EngrNumber n2)
        {
            return Add(n1, Negate(n2));
        }

        public static EngrNumber Divide(EngrNumber n1, EngrNumber n2)
        {
            return Multiply(n1, Reciprocal(n2));
        }

        public static EngrNumber Negate(EngrNumber n)
        {
            return new EngrNumber(n.Mantissa * -1, n.Prefix);
        }

        public static EngrNumber Reciprocal(EngrNumber n)
        {
            return new EngrNumber(n.Mantissa, getInversePrefix(n.Prefix));
        }

        public static EngrNumber operator + (EngrNumber n1, EngrNumber n2) { return Add(n1, n2); }
        public static EngrNumber operator - (EngrNumber n1, EngrNumber n2) { return Substract(n1, n2); }
        public static EngrNumber operator * (EngrNumber n1, EngrNumber n2) { return Multiply(n1, n2); }
        public static EngrNumber operator / (EngrNumber n1, EngrNumber n2) { return Divide(n1, n2); }
        public static bool operator == (EngrNumber n1, EngrNumber n2) { return n1.Equals(n2); }
        public static bool operator != (EngrNumber n1, EngrNumber n2) { return !n1.Equals(n2); }
        public static bool operator < (EngrNumber n1, EngrNumber n2) { return (n1.CompareTo(n2) < 0); }
        public static bool operator > (EngrNumber n1, EngrNumber n2) { return (n1.CompareTo(n2) > 0); }
        public static bool operator <= (EngrNumber n1, EngrNumber n2){ return (n1.CompareTo(n2) <= 0); }
        public static bool operator >= (EngrNumber n1, EngrNumber n2){ return (n1.CompareTo(n2) >= 0); }

        public static explicit operator decimal(EngrNumber n1) { return n1.ValueDecimal; }
        public static explicit operator double(EngrNumber n1)  { return n1.ValueDouble; }

        public static implicit operator EngrNumber(decimal n1) { return new EngrNumber(n1); }
        public static implicit operator EngrNumber(double n1)  { return new EngrNumber(n1); }
        public static implicit operator EngrNumber(int n1)     { return new EngrNumber(n1); }
        public static implicit operator EngrNumber(string n1)  { return new EngrNumber(n1); }

        public override int GetHashCode()
        {
            return Mantissa.GetHashCode() ^ getExponent(Prefix).GetHashCode();
        }

    }


    public class EngrNumberHolder
    {
        public EngrNumber Value { get; set; }
    }



    public class EngrConverter : IValueConverter
    {
        public bool AllowNegativeNumber { get; set; }
        public bool ShortString { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //number to string
            if (value == null) return "";
            EngrNumber engrNumber = (EngrNumber)value;
            
            return ShortString ? engrNumber.ToShortString() : engrNumber.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            //string to number
            String valueString = (String)value;

            if (valueString == "")
                return EngrNumber.One;
            else
            {
                EngrNumber output;
                try
                {
                    output = EngrNumber.Parse(valueString);
                }
                catch { output = EngrNumber.One; }

              
                if (!AllowNegativeNumber && output.NegativeSign) output = output * -1;

                return output;
            }
        }
    }
    
    

}
