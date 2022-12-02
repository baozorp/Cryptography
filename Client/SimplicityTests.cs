using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Client
{
    internal class SimplicityTests
    {


        internal static bool FermaTest(BigInteger value)
        {
            if (value == 2 || value == 3)
            {
                return true;
            }
            if (value < 2 || value % 2 == 0)
            {
                return false;
            }
            for (int i = 0; i < 10; i++)
            {
                BigInteger a = GetBigIntegerRandom(value);
                if (a > value - 2)
                {
                    continue;
                }
                if (BigInteger.GreatestCommonDivisor(a, value) != 1)
                {
                    return false;
                }
                if (BigInteger.ModPow(a, value - 1, value) != 1)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool MillerRabinTest(BigInteger value)
        {

            if (value == 2 || value == 3)
                return true;
            if (value < 2 || value % 2 == 0)
                return false;
            int s = 0;

            BigInteger y = value - 1;
            while (y % 2 == 0)
            {
                y /= 2;
                s += 1;
            }
            for (int i = 0; i < 10; i++)
            {
                BigInteger a = GetBigIntegerRandom(value);
                BigInteger x = BigInteger.ModPow(a, y, value);
                if (x == 1 || x == value - 1)
                    continue;
                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, value);
                    if (x == 1)
                        return false;
                    if (x == value - 1)
                        break;
                }
                if (x != value - 1)
                    return false;
            }
            return true;
        }

        internal static bool SoloveyShtrassen(BigInteger value)
        {
            if (value == 2 || value == 3)
            {
                return true;
            }
            if (value < 2 || value % 2 == 0)
            {
                return false;
            }

            for (int i = 0; i < 10; i++)
            {
                BigInteger a = GetBigIntegerRandom(value);
                if (BigInteger.GreatestCommonDivisor(a, value) > 1)
                    return false;
                BigInteger x = YakobiSymbol(a, value);
                BigInteger y = BigInteger.ModPow(a, (value - 1) / 2, value);

                if (x < 0)
                {
                    x += value;
                }

                if (y != x % value)
                {
                    return false;
                }

            }

            return true;
        }

        private static BigInteger YakobiSymbol(BigInteger a, BigInteger value)
        {

            if (BigInteger.GreatestCommonDivisor(a, value) != 1)
                return 0;
            int r = 1;
            if (a < 0)
            {
                a = -a;
                if (value % 4 == 3)
                    r = -r;
            }

            do
            {
                int y = 0;
                while (a % 2 == 0)
                {
                    y++;
                    a = a / 2;
                }
                if (y % 2 == 1)
                {
                    var mod = value % 8;
                    if (mod == 3 || mod == 5)
                        r = -r;
                }

                var amod4 = a % 4;

                if (amod4 == value % 4 && amod4 == 3)
                {
                    r = -r;
                }

                var c = a;
                a = value % c;
                value = c;
            } while (a != 0);

            return r;
        }

        internal static BigInteger GetBigIntegerRandom(BigInteger value)
        {
            var rand = new Random();
            var byteArray = new byte[128];
            BigInteger randomNumber;
            while (true)
            {
                rand.NextBytes(byteArray);
                randomNumber = new(byteArray);
                if (randomNumber > value - 2 || randomNumber < 2)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            return randomNumber;
        }
    }
}
