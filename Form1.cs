using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Capture
{
    public partial class Form1 : Form
    {
        public const string divideStr = "/";
        public Server server;
        public Client client;
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            server = new Server();
            server.Start();
        }

        public async Task DoPaint(byte[] _imageData)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            MemoryStream ms = new MemoryStream(_imageData);
            Image image = Image.FromStream(ms);
            pictureBox1.Image = image;

        }


        public async void DoPaint()
        {
            ImgCapture capture = new ImgCapture(0, 0, 1920, 1080);

            while (true)
            {
                await DoPaintTask(capture);
            }
        }

        private async Task DoPaintTask(ImgCapture _capture)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            Bitmap bitMap = _capture.GetBit();
            pictureBox1.Image = (bitMap);
            
            await Task.Run(
                () =>
                {
                    Thread.Sleep(1);
                }
                );
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            int xPos = Cursor.Position.X;
            int yPos = Cursor.Position.Y;
            if(server != null)
            {
                server.SendMousInfo(xPos, yPos);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client = new Client();
            client.Connect();
        }

        public async Task<byte[]> GetScreen(ImgCapture _capture)
        {
            byte[] result = null;
            MemoryStream stream = new MemoryStream();
            Bitmap bitmap = _capture.GetBit();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            // bitmap.Save(stream, bitmap.RawFormat); //이방식은 stream 이 null남 
        
            result = stream.ToArray();
            await Task.Run(
                () =>
                Thread.Sleep(100)
                );
            return result;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            ImgCapture capture = new ImgCapture(0, 0, 1920, 1080);
            Task<byte[]> tt = GetScreen(capture);
            await tt;
            byte[] result = tt.Result;
        }

        public void ShowMouseControl(int x, int y)
        {
            MessageBox.Show($"x 좌표 {x}\ny 좌표 {y}", "마우스 조작");
            
        }
    }
}
