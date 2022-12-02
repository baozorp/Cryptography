using System;
namespace Client
{
	internal class SymmetricAlgorithm
	{
		internal static byte[] Encryption(byte[] value, UInt32[] roundKeysArray)
        {
			if (value.Length % 20 != 0)
            {
				throw new InvalidDataException();
            }
			UInt32[] blocksArray = new UInt32[5];
			UInt32[] blocksArrayNext = new UInt32[5];
			UInt32 funcResult = 0;
			UInt32 MRoundConst = 0;
            for (int i = 0; i < blocksArray.Length; i++)
            {
                blocksArray[i] = BitConverter.ToUInt32(value, i * 4);
            }

            for (int i = 0; i < 80; i++)
            {
				if (i < 20)
				{
					funcResult = (blocksArray[1] & blocksArray[2]) | (blocksArray[1] & blocksArray[3]);
					MRoundConst = MConst[0];
				}
				else if (i > 19 && i < 40)
                {
					funcResult = blocksArray[1] ^ blocksArray[2] ^ blocksArray[3];
					MRoundConst = MConst[1];
				}
                else if (i > 39 && i < 60)
                {
					funcResult = (blocksArray[1] ^ blocksArray[2]) | (blocksArray[1] ^ blocksArray[3]) | (blocksArray[2] ^ blocksArray[3]);
					MRoundConst = MConst[2];
				}
                else
                {
					funcResult = blocksArray[1] ^ blocksArray[2] ^ blocksArray[3];
					MRoundConst = MConst[3];
				}

				blocksArrayNext[0] = roundKeysArray[i] +
					((blocksArray[0] << 5) | (blocksArray[0] >> 27)) +
					funcResult +
					blocksArray[4]+
					MRoundConst;
				blocksArrayNext[1] = blocksArray[0];
				blocksArrayNext[2] = (blocksArray[1] << 30) | (blocksArray[1] >> 2);
				blocksArrayNext[3] = blocksArray[2];
				blocksArrayNext[4] = blocksArray[3];
				blocksArrayNext.CopyTo(blocksArray, 0);
			}
			Buffer.BlockCopy(blocksArray, 0, value, 0, value.Length);
			return value;
		}

		internal static byte[] Decryption(byte[] value, UInt32[] roundKeysArray)
		{
			UInt32[] blocksArray = new UInt32[5];
			UInt32[] blocksArrayNext = new UInt32[5];
			UInt32 funcResult = 0;
			UInt32 MRoundConst = 0;
			for (int i = 0; i < blocksArray.Length; i++)
			{
				blocksArrayNext[i] = BitConverter.ToUInt32(value, i * 4);
			}

			for (int i = 79; i > -1; i--)
			{
				blocksArray[3] = blocksArrayNext[4];
				blocksArray[2] = blocksArrayNext[3];
				blocksArray[1] = (blocksArrayNext[2] >> 30) | (blocksArrayNext[2] << 2);
				blocksArray[0] = blocksArrayNext[1];

				if (i < 20)
				{
					funcResult = (blocksArray[1] & blocksArray[2]) | (blocksArray[1] & blocksArray[3]);
					MRoundConst = MConst[0];
				}
				else if (i > 19 && i < 40)
				{
					funcResult = blocksArray[1] ^ blocksArray[2] ^ blocksArray[3];
					MRoundConst = MConst[1];
				}
				else if (i > 39 && i < 60)
				{
					funcResult = (blocksArray[1] ^ blocksArray[2]) | (blocksArray[1] ^ blocksArray[3]) | (blocksArray[2] ^ blocksArray[3]);
					MRoundConst = MConst[2];
				}
				else
				{
					funcResult = blocksArray[1] ^ blocksArray[2] ^ blocksArray[3];
					MRoundConst = MConst[3];
				}

				blocksArray[4] = blocksArrayNext[0] -
					roundKeysArray[i] -
					((blocksArray[0] << 5) | (blocksArray[0] >> 27)) -
					funcResult -
					MRoundConst;
				blocksArray.CopyTo(blocksArrayNext, 0);
			}

			Buffer.BlockCopy(blocksArray, 0, value, 0, value.Length);
			return value;
		}

		internal static readonly UInt32[] MConst = new UInt32[4] {
				(UInt32)(0x5A827999 % Math.Pow(2, 32)),
				(UInt32)(0x6ED9EBA1 % Math.Pow(2, 32)),
				(UInt32)(0x8F1BBCDC % Math.Pow(2, 32)),
				(UInt32)(0xCA62C1D6 % Math.Pow(2, 32))
			};
	}
}