using System;
namespace Client
{
    internal class SHACAL
    {
        private readonly ShiftType _shiftType;
        private readonly UInt32[] _roundKeysArray;
        private static readonly byte _separator = 20;
        private byte[] _initVector;
        private bool firstRDH = true;


        internal SHACAL(byte[] key, byte[] initVector, ShiftType shiftType)
        {
            _roundKeysArray = SymmetricKey.RoundKeyGenerator(key);
            if (shiftType == ShiftType.CTR)
            {
                _initVector = new byte[_separator];
            }
            else
            {
                _initVector = (byte[])initVector.Clone();
            }
            _shiftType = shiftType;

        }

        internal byte[] Encryption(byte[] value)
        {
            switch (_shiftType)
            {
                case ShiftType.ECB:
                    EncryptionMode.EncryptECB(value, _roundKeysArray, _separator);
                    break;

                case ShiftType.CBC:
                    _initVector = EncryptionMode.EncryptCBC(value, _initVector, _roundKeysArray, _separator);
                    break;

                case ShiftType.CFB:
                    _initVector = EncryptionMode.EncryptCFB(value, _initVector, _roundKeysArray, _separator);
                    break;

                case ShiftType.OFB:
                    _initVector = EncryptionMode.EncryptOFB(value, _initVector, _roundKeysArray, _separator);
                    break;

                case ShiftType.CTR:
                    _initVector = EncryptionMode.EncryptCTR(value, _initVector, _roundKeysArray, _separator);
                    break;

                case ShiftType.RD:
                    _initVector = EncryptionMode.EncryptRD(value, _initVector, _roundKeysArray, _separator);
                    break;

                case ShiftType.RDH:
                    if (firstRDH)
                    {
                        firstRDH = false;
                        var hashSum = new byte[_separator];
                        BitConverter.GetBytes((UInt64)Math.Pow(2, _separator) - (UInt64)Math.Pow(3, _separator)).CopyTo(hashSum, 0);
                        BitConverter.GetBytes((UInt64)Math.Pow(3, _separator) - (UInt64)Math.Pow(2, _separator)).CopyTo(hashSum, 8);
                        _initVector = EncryptionMode.EncryptRDH(hashSum, _initVector, _roundKeysArray, _separator);
                        _initVector = EncryptionMode.EncryptRDH(value, _initVector, _roundKeysArray, _separator);
                        return hashSum.Concat(value).ToArray();
                    }
                    _initVector = EncryptionMode.EncryptRDH(value, _initVector, _roundKeysArray, _separator);
                    break;
            }
            return value;
        }

        internal byte[] Decryption(byte[] value)
        {
            switch (_shiftType)
            {
                case ShiftType.ECB:
                    EncryptionMode.DecryptECB(value, _roundKeysArray, _separator);
                    break;

                case ShiftType.CBC:
                    _initVector = EncryptionMode.DecryptCBC(value, _initVector, _roundKeysArray, _separator);
                    break;

                case ShiftType.CFB:
                    _initVector = EncryptionMode.DecryptCFB(value, _initVector, _roundKeysArray, _separator);
                    break;

                case ShiftType.OFB:
                    _initVector = EncryptionMode.EncryptOFB(value, _initVector, _roundKeysArray, _separator);
                    break;

                case ShiftType.CTR:
                    _initVector = EncryptionMode.DecryptCTR(value, _initVector, _roundKeysArray, _separator);
                    break;

                case ShiftType.RD:
                    _initVector = EncryptionMode.DecryptRD(value, _initVector, _roundKeysArray, _separator);
                    break;

                case ShiftType.RDH:
                    if (firstRDH)
                    {
                        firstRDH = false;
                        var hashSum = new byte[_separator];
                        BitConverter.GetBytes((UInt64)Math.Pow(2, _separator) - (UInt64)Math.Pow(3, _separator)).CopyTo(hashSum, 0);
                        BitConverter.GetBytes((UInt64)Math.Pow(3, _separator) - (UInt64)Math.Pow(2, _separator)).CopyTo(hashSum, 8);
                        var hash = value.Take(_separator).ToArray();
                        _initVector = EncryptionMode.DecryptRDH(hash, _initVector, _roundKeysArray, _separator);
                        if (Enumerable.SequenceEqual(hashSum, hash))
                        {
                            value.Skip(_separator).Take(_separator).ToArray().CopyTo(hash, 0);
                            value = hash;
                        }
                        else
                        {
                            throw new InvalidDataException();
                        }
                    }
                    _initVector = EncryptionMode.DecryptRDH(value, _initVector, _roundKeysArray, _separator);
                    break;
            }
            return value;
        }


    }
}

