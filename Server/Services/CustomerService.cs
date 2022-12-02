using Grpc.Core;
using Kurs_work_without_graphics;
using System.Linq;
using System.Numerics;

namespace Server.Services
{
    public class CustomerService : Customer.CustomerBase
    {
        private readonly ILogger<CustomerService> _logger;
        internal static byte[] symKey = SymmetricKey.KeyGenerator();
        internal static byte[] IV = InitializationVector.IVGenerator();
        internal static List<string> filesList = new List<string>();
        internal static string serverDataDirectory = "C:\\KursData\\ServerData\\";
        private System.Timers.Timer _timer = new System.Timers.Timer(1000);


        public CustomerService(ILogger<CustomerService> logger)
        {
            _logger = logger;
            _timer.Elapsed += (_, _) => readExistFiles();
            _timer.Start();
        }

        internal static void readExistFiles()
        {
            if (!Directory.Exists(serverDataDirectory))
            {
                Directory.CreateDirectory(serverDataDirectory);
            }
            List<string> existFilesList = new List<string>(Directory.GetFiles(serverDataDirectory));
            for (int i = 0; i < existFilesList.Count; i++)
            {
                existFilesList[i] = Path.GetFileName(existFilesList[i]);
                if (!filesList.Contains(existFilesList[i]))
                {
                    filesList.Add(existFilesList[i]);
                }
            }
            for (int i = 0; i < filesList.Count; i++)
            {
                if (!existFilesList.Contains(filesList[i]))
                {
                    filesList.Remove(filesList[i]);
                }
            }

            filesList = new List<string>(existFilesList);
        }

        public override Task<ExistsReply> isExist(ExistsRequest request, ServerCallContext context)
        {
            ExistsReply reply = new ExistsReply();
            if (filesList.Contains(request.FileName))
            {
                reply.Exist = true;
                return Task.FromResult(reply);
            }
            reply.Exist = false;
            return Task.FromResult(reply);
        }

        public override Task<SendFileReply> SendFile(SendFileRequest request, ServerCallContext context)
        {
            SendFileReply reply = new SendFileReply();
            using (FileStream fileWrtiter = new FileStream(serverDataDirectory + request.FileName, FileMode.Append))
            {
                fileWrtiter.Write(request.File.ToByteArray(), 0, request.File.ToByteArray().Length);
            }
            return Task.FromResult(reply);
        }

        public override async Task GetFile(GetFileRequest request, IServerStreamWriter<GetFileReply> responseStream, ServerCallContext context)
        {
            try {
            GetFileReply reply = new GetFileReply();
            int blockLength = 50000;
            byte[] dataByteArray = new byte[blockLength];
                using (Stream sourse = File.OpenRead(serverDataDirectory + request.FileName))
                {
                    while (sourse.Position + blockLength < sourse.Length)
                    {
                        sourse.Read(dataByteArray, 0, dataByteArray.Length);
                        reply.File = Google.Protobuf.ByteString.CopyFrom(dataByteArray);
                        await responseStream.WriteAsync(reply);

                    }
                    dataByteArray = new byte[sourse.Length - sourse.Position];
                    sourse.Read(dataByteArray, 0, dataByteArray.Length);
                    reply.File = Google.Protobuf.ByteString.CopyFrom(dataByteArray);
                    await responseStream.WriteAsync(reply);
                }
            }
            catch
            {
                Console.WriteLine("Client was disconnected");
            }
        }

        public override Task<SymKeyReply> SendSymKey(SymKeyRequest request, ServerCallContext context)
        {
            SymKeyReply enctyptedSymmetricKeyReply = new SymKeyReply();
            var p = new BigInteger(request.P.ToByteArray());
            var g = new BigInteger(request.G.ToByteArray());
            var y = new BigInteger(request.Y.ToByteArray());
            var k = new BigInteger(request.K.ToByteArray());
            var encryptedKey = AssymetrycAlgorithmEncryption.Encryption(symKey, p, g, y, k);
            var encryptedIV = AssymetrycAlgorithmEncryption.Encryption(IV, p, g, y, k);

            enctyptedSymmetricKeyReply.EncryptedSymmetrikKeyMessageA =
                Google.Protobuf.ByteString.CopyFrom(encryptedKey[0].ToByteArray());
            enctyptedSymmetricKeyReply.EncryptedSymmetrikKeyMessageB =
                Google.Protobuf.ByteString.CopyFrom(encryptedKey[1].ToByteArray());
            enctyptedSymmetricKeyReply.EncryptedinitialVectorMessageA =
                Google.Protobuf.ByteString.CopyFrom(encryptedIV[0].ToByteArray());
            enctyptedSymmetricKeyReply.EncryptedinitialVectorMessageB =
                Google.Protobuf.ByteString.CopyFrom(encryptedIV[1].ToByteArray());
            
            return Task.FromResult(enctyptedSymmetricKeyReply);
        }

        public override Task<ListOfObjectsReply> GetListOfObjects(ListOfObjectsRequest request, ServerCallContext context)
        {
            ListOfObjectsReply listOfObjects = new ListOfObjectsReply();
            {                                        ;
                for (int i = 0; i < filesList.Count; i++)
                {
                    listOfObjects.ListOfObjects.Add(filesList[i]);
                }
            }
            return Task.FromResult(listOfObjects);
        }
    }
}