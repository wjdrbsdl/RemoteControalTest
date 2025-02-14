using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Capture
{
    public class Client
    {
        List<string> textList = new List<string>();
        Socket mainSock;
        int m_port = 5000;

        #region 마우스 조작 변수
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;      // The left button is down.
        private const uint MOUSEEVENTF_LEFTUP = 0x004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;    // 오른쪽 마우스 버튼 눌림
        private const uint MOUSEEVENTF_RIGHTUP = 0x00010;
        #endregion

        public void Connect()
        {
            Console.WriteLine("클라 컨넥");
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string parseIP = "192.168.0.41";
            if (parseIP == null)
            {
                Console.WriteLine("IP 받아오기 실패");
                return;
            }
            IPAddress serverAddr = IPAddress.Parse(parseIP);
            IPEndPoint clientEP = new IPEndPoint(serverAddr, m_port);
            mainSock.BeginConnect(clientEP, new AsyncCallback(ConnectCallback), mainSock);
        }
        public void Close()
        {
            Console.WriteLine("클라에서 끊음");
            if (mainSock != null)
            {
                mainSock.Close();
                mainSock.Dispose();
            }
        }
        public class AsyncObject
        {
            public int numbering = 0;
            public byte[] Buffer;
            public Socket WorkingSocket;
            public readonly int BufferSize;
            public AsyncObject(int bufferSize)
            {
                BufferSize = bufferSize;
                Buffer = new byte[(long)BufferSize];
            }

            public void ClearBuffer()
            {
                Array.Clear(Buffer, 0, BufferSize);
            }
        }
        void ConnectCallback(IAsyncResult ar)
        {
            Console.WriteLine("한번인가 두번인가 클라이언트 연결 콜백");


            try
            {
                Console.WriteLine("클라 연결 콜백");
                Socket client = (Socket)ar.AsyncState;
                // client.EndConnect(ar);
                AsyncObject obj = new AsyncObject(Server.bufferSize); //버퍼 사이즈
                //서버에서 보낼때 사이즈를 256으로 보내고
                //기존 사이즈가 255 라서 계속 1번더 받음

                //서버에서 보낼때는 사이즈가 8 이라서 1번만에 받음. 
                obj.numbering = 100;
                obj.WorkingSocket = mainSock;
                SendScreenShot();
                mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);
            }
            catch (Exception e)
            {
                Console.WriteLine("받을거 없음?");
                Connect();
            }
        }

        void DataReceived(IAsyncResult ar)
        {
            //  Console.WriteLine("클라 데이터 리시브");
            try
            {
                AsyncObject obj = (AsyncObject)ar.AsyncState;

                int received = obj.WorkingSocket.EndReceive(ar);
                if (received == 0)
                {
                    obj.WorkingSocket.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);
                    return;
                }
                byte[] buffer = new byte[received];

                Array.Copy(obj.Buffer, 0, buffer, 0, received);

                ControlMouse(buffer);

                obj.WorkingSocket.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);
            }
            catch (Exception e)
            {
                Console.WriteLine("먼가 이상 재접속 시도" + e.HResult);
                Connect();
            }
        }
     
        async void SendScreenShot()
        {
            ImgCapture capture = new ImgCapture(0, 0, 1920, 1080);
            while (true)
            {
                Task<byte[]> bitScreen = Program.form.GetScreen(capture);
                byte[] result = await bitScreen;
                if (result == null)
                    continue;

                if (mainSock.Connected == false)
                {
                    Connect();
                    break;
                }
                    
                mainSock.Send(result);
            }
            
        }

        private void ControlMouse(byte[] buff)
        {
            string code = Encoding.ASCII.GetString(buff);
            string[] poses = code.Split(Form1.divideStr);
            int x = int.Parse(poses[0]);
            int y = int.Parse(poses[1]);
            int isLeft = int.Parse(poses[2]); //0이면 우클 , 1이면 좌클
            float correctX = x * 0.01f;
            float correctY = y * 0.01f;
            //클라이언트의 해상도로 마우스 위치를 조정 
            int curX = (int)(1920 * correctX);
            int curY = (int)(1080 * correctY);
           
            Cursor.Position = new Point(curX, curY);
            if (isLeft != 0)
            {
                //좌클
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
            else
            {
                //우클
                mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
            }
            
        }
    }
}
