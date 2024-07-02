using System.Numerics;

namespace dpenner1.Primell
{
    public static class PrimeLib
    {
        private static List<BigInteger> Primes = new List<BigInteger> { 2, 3 };

        // TODO - Can probably be improved for performance
        public static bool IsPrime(PLNumber number)
        {
            var n = number.Numerator;
            var lastPrime = Primes[Primes.Count - 1];

            if (!number.IsInteger) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false; // filter an easy case
            if (n <= lastPrime) return Primes.BinarySearch(n) >= 0;

            for (BigInteger i = lastPrime; i <= n; i += 2)
            {
                bool isPrime = true;
                foreach (var p in Primes)
                {
                    // Here we could do the sqrt optimization (but need a BigInteger sqrt function)
                    // if (p > sqrt(i)) break;
                    if (i % p == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime) Primes.Add(i);
            }

            return Primes[Primes.Count - 1] == n;
        }

        // TODO - Can probably be improved for performance
        public static PLNumber NextHighestPrime(PLNumber number)
        {
            if (number.IsNaN) return PLNumber.NaN;
            if (number.IsPositiveInfinity) return PLNumber.PositiveInfinity;

            if (number < 2) return 2;

            var start = PLNumber.Ceiling(number);
            if (number.IsInteger) start += 1;

            for (PLNumber i = start; ; i += 1)
            {
                if (IsPrime(i)) return i;
            }
        }

        // TODO - Can probably be improved for performance
        public static PLNumber NextLowestPrime(PLNumber number)
        {
            if (number.IsNaN) return PLNumber.NaN;
            if (number.IsPositiveInfinity) return PLNumber.PositiveInfinity;

            if (number <= 2) return PLNumber.NaN;

            var start = PLNumber.Floor(number);
            if (number.IsInteger) start -= 1;
            for (PLNumber i = start; ; i -= 1)
            {
                if (IsPrime(i)) return i;
            }
        }

        // TODO - Can probably be improved for performance
        // TODO - Rounding mode
        public static PLNumber RoundToPrime(PLNumber number)
        {
            if (number.IsNaN) return PLNumber.NaN;
            if (number.IsPositiveInfinity) return PLNumber.PositiveInfinity;

            if (number < 2) return 2;

            if (IsPrime(number)) return number;

            var low = NextLowestPrime(number);
            var lowDiff = number - low;
            var high = NextHighestPrime(number);
            var highDiff = high - number;

            return highDiff > lowDiff ? low : high;
        }

        public static PLObject PrimeRange(PLNumber start, PLNumber end, bool isInclusive)
        {
            var realEnd = end;
            if (isInclusive && end.IsInteger) realEnd += 1; // hack

            return new PLObject(new PrimePLGenerator(start, realEnd));
        }
    }
}
