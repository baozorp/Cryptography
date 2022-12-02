using Grpc.Core;
using Grpc.Net.Client;
using System.Windows.Forms;


namespace Client
{
    public partial class Form1 : Form
    {
        internal static GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");
        internal static Customer.CustomerClient client;
        internal static byte[] symKey = new byte[64];
        internal static byte[] iv = new byte[20];
        internal static readonly string sendFilesDirectory = "C:\\KursData\\SendingFiles\\";
        internal static readonly string getFilesDirectory = "C:\\KursData\\GettingFiles\\";
        internal static bool isNewConnection = true;

        public Form1()
        {
            InitializeComponent();
            client = new Customer.CustomerClient(channel);
            this.Text = "SHACAL (инициализация)";
            _ = SymmetricKey.GetSymmetricKey();
            timer1.Interval = 100;
            timer1.Start();
            timer1.Tick += new EventHandler(this.timer1_TickAsync!);
            ReaderWriter.IsDirectoriesExist(sendFilesDirectory, getFilesDirectory);
        }

        private void timer1_TickAsync(object Sender, EventArgs e)
        {
            if (channel.State == ConnectivityState.Ready)
            {

                if (isNewConnection)
                {
                    ReaderWriter.IsDirectoriesExist(sendFilesDirectory, getFilesDirectory);
                    isNewConnection = false;
                }
                this.Text = "SHACAL";
                getListOfServerObjects();
                if (button1.Enabled == false)
                {
                    button1.Enabled = button2.Enabled = listBox1.Enabled = true;
                    _ = SymmetricKey.GetSymmetricKey();
                }

            }
            else
            {
                isNewConnection = true;
                this.Text = "SHACAL (Ожидаем соединения с сервером)";
                if (Directory.Exists(sendFilesDirectory))
                {
                    Directory.Delete(sendFilesDirectory, true);

                }
                if (Directory.Exists(getFilesDirectory))
                {
                    Directory.Delete(getFilesDirectory, true);

                }
                button1.Enabled = button2.Enabled = listBox1.Enabled = false;
                channel.ConnectAsync();
            }
            

        }

        private async void getListOfServerObjects()
        {
            try
            {
                var request = new ListOfObjectsRequest { };
                var listOfServerObjects = await client.GetListOfObjectsAsync(request);
                Invoke(() => listBox1.BeginUpdate());
                foreach (var item in listOfServerObjects.ListOfObjects)
                {
                    if (listBox1.FindStringExact(item) == -1)
                    {
                        Invoke(() => listBox1.Items.Add(item));
                    }
                }
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    if (!listOfServerObjects.ListOfObjects.Contains(listBox1.Items[i]))
                    {
                        Invoke(() => listBox1.Items.Remove(listBox1.Items[i]));
                    }
                }
                Invoke(() => listBox1.EndUpdate());
            }
            catch
            {
            };

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            openFileDialog1.Filter = "All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                ExistsReply existReply = await client.isExistAsync(new ExistsRequest { FileName = (Path.GetFileName(filePath)) });
                if (existReply.Exist == false)
                {
                    Task EF = ReaderWriter.EncryptionFile(filePath, sendFilesDirectory);
                    await EF;
                    await Task.Run(() => SenderGetter.SenderFileAsync(Path.GetFileName(filePath), sendFilesDirectory));
                }
                else
                {
                    await Task.Run(() => Warning("Файл c таким именем уже существует на сервере"));
                }

            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string filename = listBox1.SelectedItem.ToString()!;
                saveFileDialog1.InitialDirectory = "c:\\";
                saveFileDialog1.FileName = filename;
                saveFileDialog1.Filter = "Type (" + Path.GetExtension(filename) + ")|" + Path.GetExtension(filename) + "|All files (*.*)|*.*";
                saveFileDialog1.AddExtension = false;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (!File.Exists(getFilesDirectory + filename))
                    {
                        try
                        {
                            Task GF = SenderGetter.GetterFileAsync(filename, getFilesDirectory);
                            await GF;
                            Task DF = ReaderWriter.DecryptionFile(saveFileDialog1.FileName, getFilesDirectory);
                            await DF;
                        }
                        catch
                        {
                            await Task.Run(() => Warning("Во время скачивания сервер был отключен"));
                        }
                    }
                    else
                    {
                        await Task.Run(() => Warning("Файл c таким именем уже скачивается и дешифруется, попробуйте позже"));
                    }
                }  
            }
        }

        private void Warning(string message)
        {
            MessageBox.Show(
                message,
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);

        }
    }
}