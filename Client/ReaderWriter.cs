using System;
namespace Client
{
	internal class ReaderWriter
	{
		internal static async Task EncryptionFile(string path, string sendFilesDirectory)
        {
			if (!Directory.Exists(sendFilesDirectory))
			{
				Directory.CreateDirectory(sendFilesDirectory);
			}
            SHACAL shacal = new SHACAL(Form1.symKey, (byte[])Form1.iv.Clone(), ShiftType.ECB);
			int blockLength = 1000;
			byte[] dataByteArray = new byte[blockLength];
			using (Stream sourse = File.OpenRead(path))
            {
				using (Stream destination = File.OpenWrite(sendFilesDirectory + Path.GetFileName(path)))
                {   Form2 ifrm = new Form2();
                    ifrm.Text = "Encrypting \"" + Path.GetFileName(path) + "\"";
                    var step = sourse.Length / 100;
					var stepCount = 0;
					if (blockLength * 100 < sourse.Length)
                    {
                        ifrm.Show();
                    }
                    while (sourse.Position + blockLength < sourse.Length)
                    {
						if (sourse.Position > step * stepCount)
						{
							stepCount++;
                            ifrm.doStep();
                        }
						await sourse.ReadAsync(dataByteArray, 0, dataByteArray.Length);
						await Task.Run(()=>shacal.Encryption(dataByteArray));
						await destination.WriteAsync(dataByteArray);
					}
					dataByteArray = new byte[sourse.Length - sourse.Position];
					sourse.Read(dataByteArray, 0, dataByteArray.Length);
					dataByteArray = PaddingAdd(dataByteArray);
                    await Task.Run(() => shacal.Encryption(dataByteArray));
                    destination.Write(dataByteArray);
				}
			}

			
        }

		internal static async Task DecryptionFile(string path, string getFilesDirectory)
        {
            SHACAL shacal = new SHACAL(Form1.symKey, (byte[])Form1.iv.Clone(), ShiftType.ECB);
			int blockLength = 1000;
			byte[] dataByteArray = new byte[blockLength];
            using (Stream sourse = File.OpenRead(getFilesDirectory + Path.GetFileName(path)))
			{
				using (Stream destination = File.OpenWrite(path))
				{
					Form2 ifrm = new Form2();
					ifrm.Text = "Decrypting \"" + Path.GetFileName(path) + "\"";
					var step = sourse.Length / 100;
					var stepCount = 0;
					if (blockLength * 100 < sourse.Length)
					{
						ifrm.Show();
					}
					while (sourse.Position + blockLength < sourse.Length)
					{
                        if (sourse.Position > step * stepCount)
                        {
                            stepCount++;
                            ifrm.doStep();
                        }
                        await sourse.ReadAsync(dataByteArray, 0, dataByteArray.Length);
						await Task.Run(()=>shacal.Decryption(dataByteArray));
						await destination.WriteAsync(dataByteArray);
					}
					dataByteArray = new byte[sourse.Length - sourse.Position];
					sourse.Read(dataByteArray, 0, dataByteArray.Length);
                    await Task.Run(() => shacal.Decryption(dataByteArray));
                    dataByteArray = PaddingDelete(dataByteArray);
					destination.Write(dataByteArray);
				}
			}
            File.Delete(getFilesDirectory + Path.GetFileName(path));
        }

		private static byte[] PaddingAdd(byte[] data)
        {
			int saveLength = data.Length;
			int difference = 20 - saveLength % 20;
			if (difference == data[data.Length - 1])
				difference++;
			Array.Resize(ref data, saveLength + difference);
			for (int i = 0; i < difference; i++)
            {
				data[saveLength + i] = (byte)(difference);
            }
			return data;
        }

		private static byte[] PaddingDelete(byte[] data)
		{
			byte saveLast = data[data.Length-1];
			for (int i = 1; i < 21; i++)
            {
				if (data[data.Length - i] != saveLast)
                {
					Array.Resize(ref data, data.Length  + 1 - i);
					break;
				}
            }
			return data;
		}

		internal static void IsDirectoriesExist(string sendFilesDirectory, string getFilesDirectory)
		{
            if (Directory.Exists(sendFilesDirectory))
            {
                string[] existFilesList = Directory.GetFiles(sendFilesDirectory);
                foreach (var item in existFilesList)
                {
                    File.Delete(item);
                }
            }
            if (Directory.Exists(getFilesDirectory))
            {
                string[] existFilesList = Directory.GetFiles(getFilesDirectory);
                foreach (var item in existFilesList)
                {
                    File.Delete(item);
                }
            }
        }

	}
}


