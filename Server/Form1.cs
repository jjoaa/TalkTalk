using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace TCPServer1
{
    public partial class Form1 : Form
    {
        TcpListener Server; //서버 소켓
        TcpClient Client;   //클라이언트 소켓

        NetworkStream Stream;
        StreamReader Reader;
        StreamWriter Writer;

        Thread receiveThread;

        bool Conntected;

        private delegate void AddTextDelegate(string strText);
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            txtView.AppendText("나 : " + txtInput.Text + Environment.NewLine);
            Writer.WriteLine(txtInput.Text);//보내기
            Writer.Flush();
            txtInput.Clear();
        }


        private void Listen()
        {
            AddTextDelegate AddText = new AddTextDelegate(txtView.AppendText);

            //소켓 생성
            IPAddress addr = new IPAddress(0);
            int port = 8000;

            Server = new TcpListener(addr, port); //생성 및 바인딩
            Server.Start();//서버시작

            Invoke(AddText, "서버 시작" + Environment.NewLine);

            Client = Server.AcceptTcpClient();

            Conntected = true;

            Invoke(AddText, "클라이언트와 연결" + Environment.NewLine);

            Stream = Client.GetStream();
            Reader = new StreamReader(Stream);
            Writer = new StreamWriter(Stream);

            //수신을 위한 스레드
            ThreadStart ts = new ThreadStart(Receive);
            Thread rcvthread = new Thread(ts);
            rcvthread.Start();
        }

        private void Receive()
        {
            AddTextDelegate AddText = new AddTextDelegate(txtView.AppendText);
            while (Conntected)
            {
                if (Stream.CanRead)
                {
                    string temp = Reader.ReadLine();
                    if (temp.Length > 0)
                    {
                        Invoke(AddText, "상대방 : " + temp + Environment.NewLine);
                    }
                }
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            ThreadStart ts = new ThreadStart(Listen);
            Thread thread = new Thread(ts);
            thread.Start();
        }
    }
}
