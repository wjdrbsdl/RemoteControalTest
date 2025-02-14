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
            pictureBox1.MouseUp += OnClickPickTureClick;
        }

        private void OnClickPickTureClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                PictureBoxLeftClick(sender, e, true);
            }
            else if(e.Button == MouseButtons.Right)
            {
                PictureBoxLeftClick(sender, e, false);
            }
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

        private void PictureBoxLeftClick(object sender, EventArgs e, bool _isLeft)
        {
            Point a = pictureBox1.PointToClient(Cursor.Position);
            Point b = pictureBox1.PointToScreen(Cursor.Position);
            Size size = pictureBox1.Size;
            
            float ratioX = (float)a.X / (float)size.Width;
            int closeX = (int)(ratioX * 100);
            float ratioY = (float)a.Y / (float)size.Height;
            int closeY = (int)(ratioY * 100);
            //������ 2�ڸ� �����ؼ� ����
            // Program.form.ShowMouseControl(closeX, closeY);

            int isLeft = 1;
            if(_isLeft == false)
            {
                isLeft = 0; //��Ŭ�̸� 0
            }
            if (server != null)
            {
                server.SendMousInfo(closeX, closeY, isLeft);
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
            // bitmap.Save(stream, bitmap.RawFormat); //�̹���� stream �� null�� 
        
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
            MessageBox.Show($"x ��ǥ {x}\ny ��ǥ {y}", "���콺 ����");
            
        }
        public void ShowMouseControl(Point a, Point b)
        {
            MessageBox.Show($"a: x ��ǥ {a.X} :y ��ǥ {a.Y}\nb : x ��ǥ {b.X} :y ��ǥ {b.Y}", "���콺 ����");

        }
    }
}
