
using Grpc.Core;
using System.Windows.Forms.Design;

namespace Client
{
    internal class SenderGetter
    {
        internal static async Task SenderFileAsync(string filename, string sendFilesDirectory)
        {
            int blockLength = 50000;
            byte[] dataByteArray = new byte[blockLength];
            var requestInfo = new SendFileRequest { FileName = filename };
            using (Stream sourse = File.OpenRead((sendFilesDirectory + filename)))
            {
                while (sourse.Position + blockLength < sourse.Length)
                {
                    sourse.Read(dataByteArray, 0, dataByteArray.Length);
                    requestInfo.File = Google.Protobuf.ByteString.CopyFrom(dataByteArray);
                    await Form1.client.SendFileAsync(requestInfo);
                }
                dataByteArray = new byte[sourse.Length - sourse.Position];
                sourse.Read(dataByteArray, 0, dataByteArray.Length);
                requestInfo.File = Google.Protobuf.ByteString.CopyFrom(dataByteArray);
                await Form1.client.SendFileAsync(requestInfo);
            }
            File.Delete(sendFilesDirectory + filename);
        }

        internal static async Task GetterFileAsync(string filename, string getFilesDirectory)
        {
            if (!Directory.Exists(getFilesDirectory))
            {
                Directory.CreateDirectory(getFilesDirectory);
            }
            using (var call = Form1.client.GetFile(new GetFileRequest { FileName = filename }))
                {
                    using (FileStream fileWrtiter = new FileStream(getFilesDirectory + filename, FileMode.Append))
                    {
                        while (await call.ResponseStream.MoveNext())
                        {
                            var getFileReply = call.ResponseStream.Current;
                            fileWrtiter.Write(getFileReply.File.ToByteArray(), 0, getFileReply.File.ToByteArray().Length);
                        }
                    }
                }
            
        }
    }
}
