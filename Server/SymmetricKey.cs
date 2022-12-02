using System;
using System.Collections;

namespace Kurs_work_without_graphics
{
    internal class SymmetricKey
    {
        internal static byte[] KeyGenerator()
        {
            var rand = new Random();
            var keyInBytes = new byte[64];
            string keySourse = "C:\\KursData\\ServerKey\\SHACAL.key";
            string keyFileDirectory = "C:\\KursData\\ServerKey";
            string serverDirectory = "C:\\KursData\\ServerData";

            if (!File.Exists(keySourse))
            {
                if (!Directory.Exists(keyFileDirectory))
                {
                    Directory.CreateDirectory(keyFileDirectory);
                }

                if (!Directory.Exists(serverDirectory))
                {
                    Directory.CreateDirectory(serverDirectory);
                }
                string[] existFilesList = Directory.GetFiles(serverDirectory);
                foreach(var item in existFilesList)
                {
                    File.Delete(item);
                }
                rand.NextBytes(keyInBytes);
                File.WriteAllBytes(keySourse, keyInBytes);

            }
            else
            {
                if (new FileInfo(keySourse).Length != 64)
                {
                    File.Delete(keySourse);
                    rand.NextBytes(keyInBytes);
                    File.WriteAllBytes(keySourse, keyInBytes);
                }
                else
                {
                    keyInBytes = File.ReadAllBytes(keySourse);
                }
            }
            return keyInBytes;
        }
    }
}

