using Server.Services;
using System.Numerics;

namespace Server
{
    public class AssymetrycAlgorithmEncryption
    {
        internal static SymKeyReply EncryptSymmetricKey(SymKeyRequest request)
        {
            SymKeyReply enctyptedSymmetricKeyReply = new SymKeyReply();
            var p = new BigInteger(request.P.ToByteArray());
            var g = new BigInteger(request.G.ToByteArray());
            var y = new BigInteger(request.Y.ToByteArray());
            var k = new BigInteger(request.K.ToByteArray());
            var encryptedKey = AssymetrycAlgorithmEncryption.Encryption(CustomerService.symKey, p, g, y, k);
            var encryptedIV = AssymetrycAlgorithmEncryption.Encryption(CustomerService.IV, p, g, y, k);
            enctyptedSymmetricKeyReply.EncryptedSymmetrikKeyMessageA =
                Google.Protobuf.ByteString.CopyFrom(encryptedKey[0].ToByteArray());
            enctyptedSymmetricKeyReply.EncryptedSymmetrikKeyMessageB =
                Google.Protobuf.ByteString.CopyFrom(encryptedKey[1].ToByteArray());
            enctyptedSymmetricKeyReply.EncryptedinitialVectorMessageA =
                Google.Protobuf.ByteString.CopyFrom(encryptedIV[0].ToByteArray());
            enctyptedSymmetricKeyReply.EncryptedinitialVectorMessageB =
                Google.Protobuf.ByteString.CopyFrom(encryptedIV[1].ToByteArray());
            return enctyptedSymmetricKeyReply;
        }
        internal static BigInteger[] Encryption(byte[] value, BigInteger p, BigInteger g, BigInteger y, BigInteger k)
        {
            BigInteger[] encryptedText = new BigInteger[2];
            BigInteger openText = new BigInteger(value);
            //BigInteger k = ReciprocallySimple(p - 1);
            encryptedText[0] = BigInteger.ModPow(g, k, p);
            encryptedText[1] = (BigInteger.ModPow(y, k, p) * (openText % p)) % p;
            return encryptedText;
        }
    }
}
