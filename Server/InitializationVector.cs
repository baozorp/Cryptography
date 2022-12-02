using System.Collections;

namespace Kurs_work_without_graphics
{
    public class InitializationVector
	{
		internal static byte[] IVGenerator()
        {
			var rand = new Random();
			var byteArray = new byte[20];
			for (int i = 0; i < byteArray.Length; i++)
            {
				byteArray[i] = (byte)rand.Next(0, 255);
			}
			return byteArray;
        }
	}
}

