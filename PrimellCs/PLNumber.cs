using System.Globalization;
using System.Numerics;

namespace dpenner1.Primell
{
    // Always in normalized form. When numerator and denominator are non-zero, it is fully reduced.
    public struct PLNumber : IComparable<PLNumber>, IEquatable<PLNumber>
    {
        public static PLNumber Zero = new PLNumber(0, 1, false);
        public static PLNumber One = new PLNumber(1, 1, false);
        public static PLNumber PositiveInfinity = new PLNumber(1, 0, false);
        public static PLNumber NegativeInfinity = new PLNumber(-1, 0, false);
        public static PLNumber NaN = new PLNumber(0, 0, false);

        public BigInteger Numerator { get; }
        public BigInteger Denominator { get; }

        private BigInteger? wholePart;
        private BigInteger fractionalNumerator;

        private BigInteger WholePart
        {
            get
            {
                if (wholePart == null) SetWholeAndFractionalParts();
                return wholePart.Value;
            }
        }

        private PLNumber FractionalPart
        {
            get
            {
                if (wholePart == null) SetWholeAndFractionalParts();
                return new PLNumber(fractionalNumerator, Denominator, false);
            }
        }

        private void SetWholeAndFractionalParts()
        {
            wholePart = BigInteger.DivRem(Numerator, Denominator, out fractionalNumerator);
        }

        public int Sign => Numerator.Sign;

        // BigInteger.IsZero, .IsOne is documented as being more performant than ==, Equals(). Copy that here.
        public bool IsInfinity => !Numerator.IsZero & Denominator.IsZero;
        public bool IsPositiveInfinity => Denominator.IsZero && Numerator > BigInteger.Zero;
        public bool IsNegativeInfinity => Denominator.IsZero && Numerator < BigInteger.Zero;
        public bool IsNaN => Numerator.IsZero & Denominator.IsZero;
        public bool IsZero => Numerator.IsZero & !Denominator.IsZero;
        public bool IsOne => Numerator.IsOne & Denominator.IsOne;
        public bool IsInteger => Denominator.IsOne;

        public PLNumber(BigInteger integer) : this(integer, BigInteger.One, false) { }
        public PLNumber(BigInteger numerator, BigInteger denominator) : this(numerator, denominator, true) { }

        // For performance, we can prevent reduce when it is known the PLNumber will necessarily be reduced.
        internal PLNumber(BigInteger numerator, BigInteger denominator, bool reduce)
        {
            Denominator = denominator;
            Numerator = numerator;
            wholePart = null;
            fractionalNumerator = BigInteger.Zero;

            if (reduce)
            {
                if (IsNaN) { } // nothing to do here
                else if (IsPositiveInfinity) Numerator = BigInteger.One;
                else if (IsNegativeInfinity) Numerator = BigInteger.MinusOne;
                else if (IsZero) Denominator = BigInteger.One * Denominator.Sign; // signed zero
                else
                {
                    var gcd = BigInteger.GreatestCommonDivisor(Numerator, Denominator) * Denominator.Sign;
                    Numerator /= gcd;
                    Denominator /= gcd;
                }
            }
        }


        #region Math methods
        public static PLNumber Abs(PLNumber value)
            => new PLNumber(BigInteger.Abs(value.Numerator), value.Denominator, false);

        public static PLNumber Negate(PLNumber value)
            => new PLNumber(-value.Numerator, value.Denominator, false);

        public static PLNumber Reciprocal(PLNumber value)
        {
            return new PLNumber(value.Denominator, value.Numerator, false);
        }

        public static PLNumber Add(PLNumber left, PLNumber right)
        {
            if (left.IsNaN || right.IsNaN) return NaN;
            if (left.IsPositiveInfinity && right.IsNegativeInfinity) return NaN;
            if (left.IsNegativeInfinity && right.IsPositiveInfinity) return NaN;
            if (left.IsInfinity) return left;
            if (right.IsInfinity) return right;

            if (left.Denominator == right.Denominator)// try to avoid performance hit
                return new PLNumber(left.Numerator + right.Numerator, left.Denominator);
            return new PLNumber(left.Numerator * right.Denominator + right.Numerator * left.Denominator, left.Denominator * right.Denominator);
        }
        public static PLNumber Multiply(PLNumber left, PLNumber right)
            => new PLNumber(left.Numerator * right.Numerator, left.Denominator * right.Denominator,
                reduce: left.Numerator != right.Numerator && left.Denominator != right.Denominator);
        public static PLNumber Subtract(PLNumber left, PLNumber right)
        {
            return Add(left, Negate(right));
        }
        public static PLNumber Divide(PLNumber left, PLNumber right)
        {
            return Multiply(left, Reciprocal(right));
        }

        private static double Log(PLNumber value)
            => BigInteger.Log(value.Numerator) - BigInteger.Log(value.Denominator);

        private static double Log(PLNumber value, PLNumber baseValue)
        {
            throw new NotImplementedException();
        }
        private static double Log10(PLNumber value)
        {
            throw new NotImplementedException();
        }
        public static PLNumber Pow(PLNumber value, int exponent)
        {
            if (value.IsNaN) return NaN;
            if (exponent == 0) return One; // mirror rest of .NET framework for 0^0           

            // reduce is not possible: numerator and denominator do not share prime factors; exponentiation will not change that
            if (exponent < 0)
                return new PLNumber(BigInteger.Pow(value.Denominator, -exponent), BigInteger.Pow(value.Numerator, -exponent), false);
            return new PLNumber(BigInteger.Pow(value.Numerator, exponent), BigInteger.Pow(value.Denominator, exponent), false);
        }

        public static PLNumber Ceiling(PLNumber value)
        {
            if (value.IsNaN || value.IsInfinity) return value;
            if (value.IsInteger) return value.Numerator;

            var adjust = 0;
            if (value < 0) adjust = -1;
            return value.WholePart + 1 + adjust;
        }

        public static PLNumber Floor(PLNumber value)
        {
            if (value.IsNaN || value.IsInfinity) return value;
            if (value.IsInteger) return value.Numerator;

            var adjust = 0;
            if (value < 0) adjust = -1;
            return value.WholePart + adjust;
        }

        public static PLNumber RoundToInteger(PLNumber value)
        {
            if (value.IsInteger) return value;
            if (value.FractionalPart > new PLNumber(1, 2, false))
                return Ceiling(value);
            else return Floor(value);

        }

        #endregion Math methods

        #region Comparison methods
        public bool Equals(PLNumber obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            return Numerator == obj.Numerator && Denominator == obj.Denominator;
        }

        public static int Compare(PLNumber left, PLNumber right)
        {
            var signCompare = left.Sign.CompareTo(right.Sign);
            if (signCompare != 0) return signCompare;

            if (left.Denominator == right.Denominator) return BigInteger.Compare(left.Numerator, right.Numerator); // try to avoid a performance hit
            return BigInteger.Compare(left.Numerator * right.Denominator, right.Numerator * left.Denominator);
        }

        public static PLNumber Max(PLNumber left, PLNumber right) => Compare(left, right) > 0 ? left : right;
        public static PLNumber Min(PLNumber left, PLNumber right) => Compare(left, right) < 0 ? left : right;

        public int CompareTo(PLNumber other) => Compare(this, other);

        #endregion Comparison methods

        #region Parsing

        private const NumberStyles DefaultStyle = NumberStyles.Integer | NumberStyles.AllowExponent;
        public static PLNumber Parse(string value)
            => Parse(value, DefaultStyle, null);
        public static PLNumber Parse(string value, NumberStyles style)
            => Parse(value, style, null);
        public static PLNumber Parse(string value, IFormatProvider provider)
            => Parse(value, DefaultStyle, provider);
        public static PLNumber Parse(string value, NumberStyles style, IFormatProvider provider)
        {
            PLNumber retval;
            if (!TryParse(value, style, provider, out retval)) throw new FormatException("Unrecognized format");
            return retval;
        }

        public static bool TryParse(string value, out PLNumber result)
            => TryParse(value, DefaultStyle, null, out result);

        public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out PLNumber result)
            => TryParse(value, style, style & ~NumberStyles.AllowTrailingWhite, style & ~NumberStyles.AllowLeadingWhite, provider, new string[] { "/", "\u2044" }, out result);

        // Method not written for performance. If that turns out to be an issue, change.
        // fractionStyle applies to the fraction as a whole. i.e. if the fraction is N/D, then entireStyle applies to the left of N and to the right of D.

        // Anything specified on the fractionStyle applies to both numerator and denominator with the following exceptions:
        // 1. If entireStyle has ANY of AllowParentheses, AllowLeadingSign, AllowTrailingSign, then ALL of those flags are ignored in numerator and denominator style
        //    i.e. You can specify sign as a whole or on each of the components, but not both.
        // 2. Whitespace: To allow control of presence of whitespace around the slash, whitespace is not included
        // Also in keeping with rest of .NET framework, there can be no whitespace between parentheses/sign and the fraction itself
        // Currency not supported. Maybe in future... but why?
        // Hex not supported, maybe in future?
        // TODO - do RTL cultures write fractions RTL?
        // TODO - apparently Farsi uses slash as decimal separator?
        public static bool TryParse(string value, NumberStyles fractionStyle, NumberStyles numeratorStyle, NumberStyles denominatorStyle, IFormatProvider provider, string[] fractionSeparators, out PLNumber result)
        {
            // General parse idea:
            // 0. Adjust styles to match style rules
            // 1. Parse anything related to fraction style
            // 2. Remove those parts from the string
            // 3. Split on the slash
            // 4. Parse numerator and denominator separately (if not float)

            // Wow, parsing this was more finicky than I anticipated

            result = Zero;

            // 0. Adjust styles ------ //
            NumberStyles unsupportedStyles = NumberStyles.AllowCurrencySymbol | NumberStyles.AllowHexSpecifier;
            NumberStyles signStyles = NumberStyles.AllowParentheses | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign;

            if (((fractionStyle | numeratorStyle | denominatorStyle) & unsupportedStyles) != 0) throw new ArgumentException("Unsupported number style");

            // Generally fractionStyle applies to both numerator and denominator
            numeratorStyle |= fractionStyle & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);
            denominatorStyle |= fractionStyle & ~(NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite);

            // deal with sign exception
            bool signOnWhole = (fractionStyle & signStyles) != 0;
            if (signOnWhole) // Sign specified on whole rather than specific parts
            {
                numeratorStyle &= ~signStyles;
                denominatorStyle &= ~signStyles;
            }

            NumberFormatInfo info = provider?.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
            if (info == null) info = NumberFormatInfo.CurrentInfo;

            // 1. Parse fraction style portion ------ //
            int sign = 1;
            string toSplit = value;
            if (signOnWhole)
            {
                bool signFoundAtStart;
                bool signFoundAtEnd;
                if (!TryParseForSign(value, fractionStyle, info, out sign, out toSplit, out signFoundAtStart, out signFoundAtEnd)) return false;

                if (signFoundAtStart) numeratorStyle &= ~NumberStyles.AllowLeadingWhite;
                if (signFoundAtEnd) denominatorStyle &= ~NumberStyles.AllowTrailingWhite;
            }

            string[] parts = toSplit.Split(fractionSeparators, int.MaxValue, StringSplitOptions.None);
            if (parts.Length > 2) return false;

            // components could itself be a fraction though decimal point or exponential notation
            PLNumber numerator = Zero;
            PLNumber denominator = One;

            // 
            if (!TryParseDecimal(parts[0], numeratorStyle, info, out numerator)) return false;
            if (parts.Length == 2)
            {
                if (!TryParseDecimal(parts[1], denominatorStyle, info, out denominator)) return false;
                if (denominator.IsZero) return false;
            }

            result = sign * numerator / denominator;
            return true;
        }

        // could be useful to make public? if so, need to block hex and currency here too.
        // can't use .NET decimal parcer because of precision
        private static bool TryParseDecimal(string value, NumberStyles style, IFormatProvider provider, out PLNumber result)
        {
            result = Zero;

            int exponent = 0;
            BigInteger fraction = BigInteger.Zero;
            BigInteger whole = BigInteger.Zero;

            NumberFormatInfo info = provider?.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
            if (info == null) info = NumberFormatInfo.CurrentInfo;

            NumberStyles signStyles = NumberStyles.AllowParentheses | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign;

            int sign = 1;
            string mainStr = value;
            bool signFoundAtStart = false;
            bool signFoundAtEnd = false;

            // sign part
            if ((style & signStyles) != 0)
            {
                if (!TryParseForSign(value, style, info, out sign, out mainStr, out signFoundAtStart, out signFoundAtEnd)) return false;
                if (signFoundAtStart) style &= ~NumberStyles.AllowLeadingWhite;
                if (signFoundAtEnd) style &= ~NumberStyles.AllowTrailingWhite;
                style &= ~signStyles; //ensure we don't parse other signs
            }

            // exponent part
            if ((style & NumberStyles.AllowExponent) != 0)
            {
                var parts = mainStr.Split('e', 'E');
                if (parts.Length > 2) return false;
                mainStr = parts[0];
                if (parts.Length == 2) // actually have exponent to parse
                {
                    if (!int.TryParse(parts[1], (style & NumberStyles.AllowTrailingWhite) | NumberStyles.AllowLeadingSign, info, out exponent)) return false;
                    style &= ~NumberStyles.AllowTrailingWhite;
                }
                style &= ~NumberStyles.AllowExponent;
            }

            // fractional part
            if ((style & NumberStyles.AllowDecimalPoint) != 0)
            {
                var parts = mainStr.Split(new string[] { info.NumberDecimalSeparator }, int.MaxValue, StringSplitOptions.None);
                if (parts.Length > 2) return false;
                mainStr = parts[0];
                if (parts.Length == 2) //actually have fractional part to parse
                {
                    bool allowWhiteSpace = (style & NumberStyles.AllowTrailingWhite) != 0;
                    if (!(parts[1] == "" || (string.IsNullOrWhiteSpace(parts[1]) && allowWhiteSpace))) // fractional part allowed to be empty
                    {
                        if (!BigInteger.TryParse(parts[1], style & NumberStyles.AllowTrailingWhite, info, out fraction)) return false;
                    }

                    style &= ~NumberStyles.AllowTrailingWhite;
                }
                style &= ~NumberStyles.AllowDecimalPoint;
            }

            // integer part
            if (!BigInteger.TryParse(mainStr, style, info, out whole)) return false;

            // put everything together
            int fractionExponent = fraction.ToString().Length; // hack for number of decimal digits
            PLNumber fractionalValue = new PLNumber(fraction, BigInteger.Pow(10, fractionExponent));

            PLNumber exponentValue = new PLNumber(BigInteger.Pow(10, Math.Abs(exponent)));
            if (exponent < 0) exponentValue = Reciprocal(exponentValue);

            result = sign * (whole + fractionalValue) * exponentValue;
            return true;
        }

        private static bool TryParseForSign(string value, NumberStyles style, NumberFormatInfo info, out int sign, out string valueWithoutSign, out bool signFoundAtStart, out bool signFoundAtEnd)
        {
            sign = 0;
            valueWithoutSign = value;
            signFoundAtStart = false;
            signFoundAtEnd = false;

            int subStartIndex = 0;
            int subEndIndex = value.Length;

            bool open = false;
            bool breakLoop = false;
            bool leadWhiteSpace = false;
            bool trailWhiteSpace = false;

            bool allowLeadingWhite = (style & NumberStyles.AllowLeadingWhite) != 0;
            bool allowTrailingWhite = (style & NumberStyles.AllowTrailingWhite) != 0;
            bool allowParentheses = (style & NumberStyles.AllowParentheses) != 0;
            bool allowLeadingSign = (style & NumberStyles.AllowLeadingSign) != 0;
            bool allowTrailingSign = (style & NumberStyles.AllowTrailingSign) != 0;

            int i;

            // check front of string
            if (allowParentheses || allowLeadingSign)
            {
                for (i = 0; i < value.Length; i++)
                {
                    switch (value[i])
                    {
                        case '(':
                            if (!allowParentheses) return false;
                            open = true;
                            breakLoop = true;
                            sign = -1;
                            break;
                        case '\u0009':
                        case '\u000A':
                        case '\u000B':
                        case '\u000C':
                        case '\u000D':
                        case '\u0020':
                            leadWhiteSpace = true;
                            break;
                        default: // try to match with positive/negative sign
                            if (allowLeadingSign)
                            {
                                // Assumption: NegativeSign != PositiveSign...
                                if (TryMatchSign(value, i, info.NegativeSign, false)) sign = -1;
                                else if (TryMatchSign(value, i, info.PositiveSign, false)) sign = 1;
                            }
                            breakLoop = true;
                            break;
                    }
                    if (breakLoop) break;
                }

                if (sign != 0)
                {
                    if (leadWhiteSpace && !allowLeadingWhite) return false;
                    signFoundAtStart = true;
                    subStartIndex = i + 1; // loop was on character and then breaked, so i++ did not occur
                }
            }

            breakLoop = false;
            // check end of string if needed
            if ((sign == 0 || open) && (allowParentheses || allowTrailingSign))
            {
                for (i = value.Length - 1; i >= 0; i--)
                {
                    switch (value[i])
                    {
                        case ')':
                            if (!open) return false;
                            open = false;
                            breakLoop = true;
                            break;
                        case '\u0009':
                        case '\u000A':
                        case '\u000B':
                        case '\u000C':
                        case '\u000D':
                        case '\u0020':
                            trailWhiteSpace = true;
                            break;
                        default: // try to match with positive/negative sign
                            if (sign == 0 && !open && allowTrailingSign)
                            {
                                // Assumption: NegativeSign != PositiveSign...
                                if (TryMatchSign(value, i, info.NegativeSign, true)) sign = -1;
                                else if (TryMatchSign(value, i, info.PositiveSign, true)) sign = 1;
                            }
                            breakLoop = true;
                            break;
                    }
                    if (breakLoop) break;
                }

                if (sign != 0)
                {
                    if (trailWhiteSpace && !allowTrailingWhite) return false;
                    signFoundAtEnd = true;

                    subEndIndex = i; // i is on the character (but endIndex is exclusive)
                }
            }

            if (open) return false;
            if (sign == 0) sign = 1; //no sign found, implied positive

            valueWithoutSign = value.Substring(subStartIndex, subEndIndex - subStartIndex);
            return true;
        }

        private static bool TryMatchSign(string value, int startIndex, string sign, bool reverse)
        {
            int valueIndex = startIndex;
            int signIndex = reverse ? sign.Length - 1 : 0;
            int signEndIndex = reverse ? -1 : sign.Length;
            bool match = true;

            while (match && signIndex != signEndIndex)
            {
                if (value[valueIndex] != sign[signIndex]) match = false;
                if (reverse)
                {
                    valueIndex--;
                    signIndex--;
                }
                else
                {
                    valueIndex++;
                    signIndex++;
                }
            }

            return match;
        }

        #endregion Parsing

        #region Operators

        public static PLNumber operator +(PLNumber left, PLNumber right) => Add(left, right);
        public static PLNumber operator -(PLNumber left, PLNumber right) => Subtract(left, right);
        public static PLNumber operator *(PLNumber left, PLNumber right) => Multiply(left, right);
        public static PLNumber operator /(PLNumber left, PLNumber right) => Divide(left, right);

        public static PLNumber operator +(PLNumber x) => x;
        public static PLNumber operator -(PLNumber x) => Negate(x);


        public static implicit operator PLNumber(long value) => new PLNumber(value, 1, false);

        public static implicit operator PLNumber(BigInteger value) => new PLNumber(value, 1, false);

        public static implicit operator PLNumber(decimal value)
        {
            int[] bits = decimal.GetBits(value);

            decimal mantissa = new decimal(bits[0], bits[1], bits[2], value < 0, 0);
            int exponent = (bits[3] & 0x00FF0000) >> 16;

            return new PLNumber((BigInteger)mantissa, BigInteger.Pow(10, exponent));
        }

        // This one was fun figuring out.
        // Conversion from double is exact, without loss of precision. 
        // All doubles other than infinity and NaN should work.
        public static explicit operator PLNumber(double value)
        {
            if (double.IsPositiveInfinity(value)) return PositiveInfinity;
            if (double.IsNegativeInfinity(value)) return NegativeInfinity;
            if (double.IsNaN(value)) return NaN;
            if (value == 0) return Zero; // TODO - signed zero

            // CLR apparently does not allow subnormal numbers, so don't have to deal with that

            long bits = BitConverter.DoubleToInt64Bits(value);
            long lo = (bits & 0x00000000FFFFFFFF);
            long hi = ((bits & 0x000FFFFF00000000) >> 32) | (1L << 20); // adding implied bit

            int expbits = (int)((bits & 0x7FF0000000000000) >> 52);

            // expropriating useful decimal constructor
            decimal mantissa = new decimal((int)lo, (int)hi, 0, value < 0, 0);
            // actual offset is 1023, but we are using decimal constructor which offsets further
            int exponent = expbits - 1023 - 52;

            BigInteger factor = BigInteger.Pow(2, Math.Abs(exponent));
            if (exponent > 0)
                return new PLNumber((BigInteger)mantissa * factor);
            return new PLNumber((BigInteger)mantissa, factor);
        }

        public static explicit operator double (PLNumber value) => Math.Exp(Log(value));

        public static explicit operator BigInteger(PLNumber value) => value.WholePart;

        public static explicit operator int (PLNumber value) => (int)value.WholePart;
        public static bool operator ==(PLNumber left, PLNumber right) => left.Equals(right);
        public static bool operator !=(PLNumber left, PLNumber right) => !left.Equals(right);
        public static bool operator <(PLNumber left, PLNumber right) => Compare(left, right) < 0;
        public static bool operator >(PLNumber left, PLNumber right) => Compare(left, right) > 0;
        public static bool operator <=(PLNumber left, PLNumber right) => Compare(left, right) <= 0;
        public static bool operator >=(PLNumber left, PLNumber right) => Compare(left, right) >= 0;

        #endregion Operators

        #region Object overrides
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(PLNumber)) return false;

            var x = (PLNumber)obj;
            return Equals(x);
        }

        public override int GetHashCode() => Numerator.GetHashCode() ^ Denominator.GetHashCode(); // no idea if this is a good implementation

        public override string ToString()
        {
            if (IsZero) return "0";   // negative zero not printed as such.
            if (IsNaN) return "NaN";
            if (IsPositiveInfinity) return "∞";
            if (IsNegativeInfinity) return "-∞";
            return Numerator + (IsInteger ? "" : "/" + Denominator);
        }

        public string ToString(int @base)
        {
            if (IsZero) return "0";   // negative zero not printed as such.
            if (IsNaN) return "NaN";
            if (IsPositiveInfinity) return "∞";
            if (IsNegativeInfinity) return "-∞";


            return GetRepresentation(Numerator, @base) + (IsInteger ? "" : "/" + GetRepresentation(Denominator, @base));
        }

        private string GetRepresentation(BigInteger value, int @base)
        {
            string retval = "";
            var sign = "";
            if (value < 0) sign = "-";
            BigInteger rem;

            do
            {
                value = BigInteger.DivRem(BigInteger.Abs(value), @base, out rem);
                if (rem >= 0 && rem <= 9) retval += (char)('0' + rem);
                else if (rem >= 10 && rem <= 35) retval += (char)('A' + rem - 10);
                else if (rem >= 36 && rem <= 61) retval += (char)('a' + rem - 36);
                else if (rem == 62) retval += 'Þ';
                else if (rem == 63) retval += 'þ';

            } while (value != 0);

            var temp = retval.ToCharArray();
            Array.Reverse(temp);
           
            return sign + new string(temp);
        }
        /*
        public override PLObject NumericBinaryOperation(PLObject other, Func<PLNumber, PLNumber, PLObject> op, PLOperationOptions options)
        {
            var otherList = other as PLList;
            if (other is PLList)
            {
                var retval = new PLList();
                foreach (var plobj in otherList)
                {
                    retval.Add(NumericBinaryOperation(plobj, op, options));
                }

                return retval;
            }

            var otherNumber = other as PLNumber;
            return op.Invoke(this, otherNumber);
        }

        public override PLObject NumericUnaryOperation(Func<PLNumber, PLObject> op, PLOperationOptions options)
        {
            return op.Invoke(this);
        }*/

        #endregion Object overrides
    }
}
