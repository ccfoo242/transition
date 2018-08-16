using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Transition
{

    [Windows.Foundation.Metadata.CreateFromString(MethodName = "Transition.EngrNumber.ConvertFromString")]
    public struct EngrNumber : IComparable
    {
        /* this struct stores numbers in engineering notation */

        public decimal Mantissa { get; set; }
        public String Prefix { get; set; }

        public bool NegativeSign { get { return Mantissa < 0; } }
        public double ValueDouble { get { return (double)Mantissa * Math.Pow(10, getExponent(Prefix)); } }

        public static Dictionary<String, int> mapPrefixes =
            new Dictionary<string, int> {
                {"Y", 24 },
                {"Z", 21 },
                {"E", 18 },
                {"P", 15 },
                {"T", 12 },
                {"G", 9 },
                {"M", 6 },
                {"K", 3 },
                {"", 0 },
                {"m", -3 },
                {"u", -6 },
                {"n", -9 },
                {"p", -12 },
                {"f", -15 },
                {"a", -18 },
                {"z", -21 },
                {"y", -24 }
            };

        public EngrNumber(decimal mantissa, String prefix)
        {
            this.Mantissa = mantissa;

            if (prefix == null)
                this.Prefix = "";
            else
                this.Prefix = prefix;

            normalize();
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

        public static EngrNumber one()
        {
            return new EngrNumber(1, "");
        }

        public EngrNumber(decimal input)
        {
            Mantissa = input;
            Prefix = "";
            normalize();
        }

        public EngrNumber(int input)
        {
            Mantissa = input;
            Prefix = "";
            normalize();
        }

        public EngrNumber(double input)
        {
            if (double.IsInfinity(input)) input = 1;
            if (double.IsNaN(input)) input = 1;

            Mantissa = (decimal)input;
            Prefix = "";
            normalize();
        }


        public void makePositiveSign()
        {
            if (NegativeSign) Mantissa *= -1;
        }

        public override string ToString()
        {
            if (Prefix == null)
                return Mantissa.ToString("##0.0000");
            else
                return Mantissa.ToString("##0.0000") + Prefix;
        }

        public String ToShortString()
        {
            if (Prefix == null)
                return Mantissa.ToString();
            else
                return Mantissa.ToString() + Prefix;
        }

        public static String getPrefix(int exponent)
        {
            if (mapPrefixes.Values.Contains(exponent))
            {
                foreach (KeyValuePair<String, int> pair in mapPrefixes)
                    if (pair.Value == exponent) return pair.Key;
                return null;
            }
            else
                return null;
        }

        public static int getExponent(String prefix)
        {
            if (prefix == null) return 0;

            if (mapPrefixes.Keys.Contains(prefix))
                return mapPrefixes[prefix];
            else
                return 0;
        }

        public int getExponent()
        {
            if (Prefix != null)
                return getExponent(Prefix);
            else return 0;
        }

        public void set(decimal mantissa, String prefix)
        {
            this.Mantissa = mantissa;
            this.Prefix = prefix;

            normalize();
        }

        public void set(decimal mantissa)
        {
            this.Mantissa = mantissa;
            normalize();
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

        public static EngrNumber ConvertFromString(String rawString)
        {
            rawString.Trim();

            if (rawString.Length == 0)
                throw new ArgumentException();

            String lastChar = rawString.Substring(rawString.Length - 1, 1);
            bool prefixExists = mapPrefixes.Keys.Contains(lastChar);

            String stringMantissa;
            String prefix;

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

            try { parsedMantissa = decimal.Parse(stringMantissa); }
            catch
            {
                throw new ArgumentException();
            }

            if (!EngrNumber.mapPrefixes.Keys.Contains(prefix))
                throw new ArgumentException();

            EngrNumber output = new EngrNumber(parsedMantissa, prefix);

            return output;
        }

        public void normalize()
        {
            /* normalize means to have one, two or three digits
               in the left side of the point, and adjust the prefix accordingly
               e.g. :
                0.01234  ==> 12.3400m
                1234567K ==>  1.2346G
            */
            if (Prefix == null) Prefix = "";

            String originalPrefix = Prefix;

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
                return EngrNumber.one();
            else
            {
                EngrNumber output;
                try
                {
                    output = EngrNumber.ConvertFromString(valueString);
                }
                catch { output = EngrNumber.one(); }

              
                if (!AllowNegativeNumber) output.makePositiveSign();

                return output;
            }
        }
    }
    
    

}
