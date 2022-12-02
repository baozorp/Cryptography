using System;
namespace Client
{
    internal enum ShiftType
    {
        ECB,
        CBC,
        CFB,
        OFB,
        CTR,
        RD,
        RDH
    };

    internal class EncryptionMode
	{
        internal static void EncryptECB(byte[] value, UInt32[] roundKeysArray, int separator)
        {
            Parallel.For(0, value.Length / separator, i =>
            {
                var encryptRes = SymmetricAlgorithm.Encryption(value.Skip(i * separator).Take(separator).ToArray(), roundKeysArray);
                Array.Copy(encryptRes, 0, value, i * separator, separator);
            });
        }

        internal static void DecryptECB(byte[] value, UInt32[] roundKeysArray, int separator)
        {
            Parallel.For(0, value.Length / separator, i =>
            {
                var decryptRes = SymmetricAlgorithm.Decryption(value.Skip(i * separator).Take(separator).ToArray(), roundKeysArray);
                Array.Copy(decryptRes, 0, value, i * separator, separator);
            });
        }

        internal static byte[] EncryptCBC(byte[] value, byte[] initVector, UInt32[] roundKeysArray, int separator)
        {
            var xorRes = XOR(value.Take(separator).ToArray(), initVector);
            var encryptionRes = SymmetricAlgorithm.Encryption(xorRes, roundKeysArray);
            Array.Copy(encryptionRes, 0, value, 0, separator);
            for (int i = 1; i < value.Length / separator; i++)
            {
                xorRes = XOR(value.Skip(i * separator).Take(separator).ToArray(), value.Skip((i - 1) * separator).Take(separator).ToArray());
                encryptionRes = SymmetricAlgorithm.Encryption(xorRes, roundKeysArray);
                Array.Copy(encryptionRes, 0, value, i * separator, separator);
            }
            return encryptionRes;
        }

        internal static byte[] DecryptCBC(byte[] value, byte[] initVector, UInt32[] roundKeysArray, int separator)
        {
            var initVectorNext = value.Skip(value.Length - separator).Take(20).ToArray();
            var copyArray = new byte[value.Length];
            value.CopyTo(copyArray, 0);
            var xorRes = XOR(SymmetricAlgorithm.Decryption(value.Take(separator).ToArray(), roundKeysArray), initVector);
            Array.Copy(xorRes, 0, value, 0, separator);
            Parallel.For(1, value.Length / separator, i =>
            {
                xorRes = XOR(SymmetricAlgorithm.Decryption(value.Skip(i * separator).Take(separator).ToArray(), roundKeysArray),
                    copyArray.Skip((i - 1) * separator).Take(separator).ToArray());
                Array.Copy(xorRes, 0, value, i * separator, separator);
            });
            return initVectorNext;
        }

        internal static byte[] EncryptCFB(byte[] value, byte[] initVector, UInt32[] roundKeysArray, int separator)
        {
            var encryptRes = SymmetricAlgorithm.Encryption((byte[])initVector.Clone(), roundKeysArray);
            var xorRes = XOR(encryptRes, value.Take(separator).ToArray());
            Array.Copy(xorRes, 0, value, 0, separator);
            for (int i = 1; i < value.Length / separator; i++)
            {
                encryptRes = SymmetricAlgorithm.Encryption(value.Skip((i - 1) * separator).Take(separator).ToArray(), roundKeysArray);
                xorRes = XOR(encryptRes, value.Skip(i * separator).Take(separator).ToArray());
                Array.Copy(xorRes, 0, value, i * separator, separator);
            }
            return xorRes;
        }

        internal static byte[] DecryptCFB(byte[] value, byte[] initVector, UInt32[] roundKeysArray, int separator)
        {
            var initVectorNext = value.Skip(value.Length - 20).Take(20).ToArray();
            var copyArray = new byte[value.Length];
            value.CopyTo(copyArray, 0);
            var decryptRes = SymmetricAlgorithm.Encryption((byte[])initVector.Clone(), roundKeysArray);
            var xorRes = XOR(decryptRes, copyArray.Take(separator).ToArray());
            Array.Copy(xorRes, 0, value, 0, separator);
            for (int i = 1; i < value.Length / separator; i++)
            {
                decryptRes = SymmetricAlgorithm.Encryption(copyArray.Skip((i - 1) * separator).Take(separator).ToArray(), roundKeysArray);
                xorRes = XOR(decryptRes, copyArray.Skip(i * separator).Take(separator).ToArray());
                Array.Copy(xorRes, 0, value, i * separator, separator);
            }
            return initVectorNext;
        }

        internal static byte[] EncryptOFB(byte[] value, byte[] initVector, UInt32[] roundKeysArray, int separator)
        {
            var copyInitVector = (byte[])initVector.Clone();
            var xorRes = new byte[separator];
            for (int i = 0; i < value.Length / separator; i++)
            {
                xorRes = XOR(value.Skip(i * separator).Take(separator).ToArray(), SymmetricAlgorithm.Encryption(copyInitVector, roundKeysArray));
                Array.Copy(xorRes, 0, value, i * separator, separator);
            }
            return copyInitVector;
        }

        internal static byte[] DecryptOFB(byte[] value, byte[] initVector, UInt32[] roundKeysArray, int separator)
        {
            var copyInitVector = (byte[])initVector.Clone();
            var xorRes = new byte[separator];
            for (int i = 0; i < value.Length / separator; i++)
            {
                xorRes = XOR(value.Skip(i * separator).Take(separator).ToArray(), SymmetricAlgorithm.Encryption(copyInitVector, roundKeysArray));
                Array.Copy(xorRes, 0, value, i * separator, separator);
            }
            return copyInitVector;
        }

        internal static byte[] EncryptCTR(byte[] value, byte[] initVector, UInt32[] roundKeysArray, int separator)
        {
            var xorRes = new byte[separator];
            Parallel.For(0, value.Length / separator, i =>
            {
                xorRes = XOR(SymmetricAlgorithm.Encryption(AddDelta((UInt64)i, initVector), roundKeysArray), value.Skip(i * separator).Take(separator).ToArray());
                Array.Copy(xorRes, 0, value, i * separator, separator);
            });
            return AddDelta((UInt64)(value.Length / separator), initVector);
        }

        internal static byte[] DecryptCTR(byte[] value, byte[] initVector, UInt32[] roundKeysArray, int separator)
        {
            var xorRes = new byte[separator];
            Parallel.For(0, value.Length / separator, i =>
            {
                xorRes = XOR(SymmetricAlgorithm.Encryption(AddDelta((UInt64)i, initVector), roundKeysArray), value.Skip(i * separator).Take(separator).ToArray());
                Array.Copy(xorRes, 0, value, i * separator, separator);
            });
            return AddDelta((UInt64)(value.Length / separator), initVector);
        }

        internal static byte[] EncryptRD(byte[] value, byte[] initVector, UInt32[] roundKeysArray, int separator)
        {
            var delta = BitConverter.ToUInt64(initVector.Skip(initVector.Length - 8).Take(8).ToArray());
            var xorRes = new byte[separator];
            Parallel.For(0, value.Length / separator, i =>
            {
                xorRes = XOR(AddDelta(delta * (UInt64)i, initVector), value.Skip(i * separator).Take(separator).ToArray());
                Array.Copy(SymmetricAlgorithm.Encryption(xorRes, roundKeysArray), 0, value, i * separator, separator);
            });
            return AddDelta(delta * (UInt64)(value.Length / separator), initVector);
        }

        internal static byte[] DecryptRD(byte[] value, byte[] initVector, UInt32[] roundKeysArray, int separator)
        {
            var delta = BitConverter.ToUInt64(initVector.Skip(initVector.Length - 8).Take(8).ToArray());
            var xorRes = new byte[separator];
            var decryptRes = new byte[separator];
            Parallel.For(0, value.Length / separator, i =>
            {
                decryptRes = SymmetricAlgorithm.Decryption(value.Skip(i * separator).Take(separator).ToArray(), roundKeysArray);
                xorRes = XOR(AddDelta(delta * (UInt64)i, initVector), decryptRes);
                Array.Copy(xorRes, 0, value, i * separator, separator);
            });
            return AddDelta(delta * (UInt64) (value.Length / separator), initVector);
        }

        internal static byte[] EncryptRDH(byte[] value, byte[] initVector, UInt32[] roundKeysArray, int separator)
        {
            var delta = BitConverter.ToUInt64(initVector.Skip(initVector.Length - 8).Take(8).ToArray());
            var xorRes = new byte[separator];
            Parallel.For(0, value.Length / separator, i =>
            {
                xorRes = XOR(AddDelta(delta * (UInt64)(i + 1), initVector), value.Skip(i * separator).Take(separator).ToArray());
                Array.Copy(SymmetricAlgorithm.Encryption(xorRes, roundKeysArray), 0, value, i * separator, separator);
            });
            return AddDelta(delta * (UInt64)(value.Length / separator), initVector);
        }

        internal static byte[] DecryptRDH(byte[] value, byte[] initVector, UInt32[] roundKeysArray, int separator)
        {
            var delta = BitConverter.ToUInt64(initVector.Skip(initVector.Length - 8).Take(8).ToArray());
            var xorRes = new byte[separator];
            var decryptRes = new byte[separator];
            Parallel.For(0, value.Length / separator, i =>
            {
                decryptRes = SymmetricAlgorithm.Decryption(value.Skip(i * separator).Take(separator).ToArray(), roundKeysArray);
                xorRes = XOR(AddDelta(delta * (UInt64)(i + 1), initVector), decryptRes);
                Array.Copy(xorRes, 0, value, i * separator, separator);
            });
            return AddDelta(delta * (UInt64)(value.Length / separator), initVector);
        }

        private static byte[] AddDelta(UInt64 delta, byte[] array)
        {
            var arrayCopy = (byte[])array.Clone();
            for (int i = 0; i < arrayCopy.Length; i++)
            {
                delta += arrayCopy[i];
                arrayCopy[i] = (byte)delta;
                delta >>= 8;
            }
            return arrayCopy;
        }


        private static byte[] XOR(byte[] valueLeft, byte[] valueRight)
        {
            for (int i = 0; i < valueLeft.Length; i++)
            {
                valueLeft[i] = (byte)(valueLeft[i] ^ valueRight[i]);
            }
            return valueLeft;
        }

    }
}