using System.Numerics;


namespace Client
{
    internal class AssymetricAlgorithm
    {
        internal static void GoEncrypt(byte[] value)
        {
            BigInteger[] key = KeyGenerator();
        }
        internal static BigInteger[] KeyGenerator()
        {
            BigInteger[] keyArray = new BigInteger[5];
            keyArray[0] = NewSimpleBigInteger();
            keyArray[1] = ReciprocallySimple(keyArray[0]);
            keyArray[2] = BigIntegerGenerator(keyArray[0]);
            keyArray[3] = BigInteger.ModPow(keyArray[1], keyArray[2], keyArray[0]);
            keyArray[4] = ReciprocallySimple(keyArray[0] - 1);
            return keyArray;
        }
        internal static BigInteger ReciprocallySimple(BigInteger value)
        {
            byte[] byteArray = new byte[64];
            Random rand = new Random();
            BigInteger a;
            while (true)
            {
                rand.NextBytes(byteArray);
                a = new BigInteger(byteArray);
                if (a > (value) || a < 1)
                {
                    continue;
                }
                if (BigInteger.GreatestCommonDivisor(a, value - 1) == 1)
                {
                    return a;
                }
            }
        }


        internal static BigInteger[] Encryption(byte[] value, BigInteger p, BigInteger g, BigInteger y, BigInteger k)
        {
            BigInteger[] encryptedText = new BigInteger[2];
            BigInteger openText = new BigInteger(value);
            encryptedText[0] = BigInteger.ModPow(g, k, p);
            encryptedText[1] = (BigInteger.ModPow(y, k, p) * (openText % p)) % p;
            return encryptedText;
        }

        internal static byte[] Decryption(BigInteger[] value, BigInteger p, BigInteger x)
        {
            BigInteger openText = ((value[1] % p) * BigInteger.ModPow(value[0], p - 1 - x, p)) % p;
            return openText.ToByteArray();
        }

        internal static BigInteger NewSimpleBigInteger()
        {
            var rand = new Random();
            var byteArray = new byte[128];
            BigInteger simpleBigInteger = new BigInteger(byteArray);
            while (true)
            {
                rand.NextBytes(byteArray);
                simpleBigInteger = new BigInteger(byteArray);
                if (SimplicityTests.FermaTest(simpleBigInteger)
                    && SimplicityTests.SoloveyShtrassen(simpleBigInteger)
                    && SimplicityTests.MillerRabinTest(simpleBigInteger))
                {
                    break;
                }
            };
            return simpleBigInteger;
        }

        internal static BigInteger BigIntegerGenerator(BigInteger value)
        {
            var rand = new Random();
            var byteArray = new byte[128];
            BigInteger bigInteger;
            while (true)
            {
                rand.NextBytes(byteArray);
                bigInteger = new(byteArray);
                if (bigInteger > (value - 2) || bigInteger <= 2)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            return bigInteger;
        }
    }
}
