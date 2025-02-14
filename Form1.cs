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
            Point a = pictureBox1.PointToClient(Cursor.Position);
            Point b = pictureBox1.PointToScreen(Cursor.Position);
            Size size = pictureBox1.Size;
            
            float ratioX = (float)a.X / (float)size.Width;
            int closeX = (int)(ratioX * 100);
            float ratioY = (float)a.Y / (float)size.Height;
            int closeY = (int)(ratioY * 100);
            //ºñÀ²À» 2ÀÚ¸® ±îÁöÇØ¼­ Àü´Þ
           // Program.form.ShowMouseControl(closeX, closeY);

            if (server != null)
            {
                server.SendMousInfo(closeX, closeY);
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
            // bitmap.Save(stream, bitmap.RawFormat); //ÀÌ¹æ½ÄÀº stream ÀÌ null³² 
        
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
            MessageBox.Show($"x ÁÂÇ¥ {x}\ny ÁÂÇ¥ {y}", "¸¶¿ì½º Á¶ÀÛ");
            
        }
        public void ShowMouseControl(Point a, Point b)
        {
            MessageBox.Show($"a: x ÁÂÇ¥ {a.X} :y ÁÂÇ¥ {a.Y}\nb : x ÁÂÇ¥ {b.X} :y ÁÂÇ¥ {b.Y}", "¸¶¿ì½º Á¶ÀÛ");

        }
    }
}
