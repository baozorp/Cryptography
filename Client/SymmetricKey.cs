using System.Numerics;

namespace Client
{
	internal class SymmetricKey
	{

        internal static async Task GetSymmetricKey()
        {
            var keyAssym = AssymetricAlgorithm.KeyGenerator();
            var p = Google.Protobuf.ByteString.CopyFrom(keyAssym[0].ToByteArray());
            var g = Google.Protobuf.ByteString.CopyFrom(keyAssym[1].ToByteArray());
            var x = Google.Protobuf.ByteString.CopyFrom(keyAssym[2].ToByteArray());
            var y = Google.Protobuf.ByteString.CopyFrom(keyAssym[3].ToByteArray());
            var seanseKey = Google.Protobuf.ByteString.CopyFrom(keyAssym[4].ToByteArray());
            var reply = new SymKeyRequest { P = p, G = g, Y = y, K = seanseKey };
            var customer = await Form1.client.SendSymKeyAsync(reply);
            var messageKey = new BigInteger[2];
            var messageIV = new BigInteger[2];
            messageKey[0] = new BigInteger(customer.EncryptedSymmetrikKeyMessageA.ToByteArray());
            messageKey[1] = new BigInteger(customer.EncryptedSymmetrikKeyMessageB.ToByteArray());
            messageIV[0] = new BigInteger(customer.EncryptedinitialVectorMessageA.ToByteArray());
            messageIV[1] = new BigInteger(customer.EncryptedinitialVectorMessageB.ToByteArray());
            Form1.symKey = AssymetricAlgorithm.Decryption(messageKey, keyAssym[0], keyAssym[2]);
            Form1.iv = AssymetricAlgorithm.Decryption(messageIV, keyAssym[0], keyAssym[2]);
        }

        internal static UInt32[] RoundKeyGenerator(byte[] key)
		{
			UInt32[] roundKeysArray = new UInt32[80];
			for (int i = 0; i < 80; i++)
			{
				if (i < 16)
                {
					roundKeysArray[i] = BitConverter.ToUInt32(key, i * 4);
				}
                else
                {
					roundKeysArray[i] = (roundKeysArray[i - 3] ^ roundKeysArray[i - 8] ^ roundKeysArray[i - 14] ^ roundKeysArray[i - 16]) << 1;
				}		
			}
			return roundKeysArray;
		}

	}
}

