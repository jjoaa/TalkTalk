using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace TCPServer1
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private TcpClient client;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;
        private Thread listenThread;
        private Thread receiveThread;
        private bool connected;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            listenThread = new Thread(Listen);
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        private void Listen()
        {
            try
            {
                AppendText("서버 시작"+ Environment.NewLine);

                server = new TcpListener(IPAddress.Any, 8000);
                server.Start();

                client = server.AcceptTcpClient();
                connected = true;

                AppendText("클라이언트와 연결됨"+ Environment.NewLine);

                stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);

                receiveThread = new Thread(Receive);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                AppendText("서버 오류: " + ex.Message + Environment.NewLine);
            }
        }

        private void Receive()
        {
            try
            {
                while (connected)
                {
                    if (stream.CanRead)
                    {
                        string message = reader.ReadLine();
                        if (!string.IsNullOrEmpty(message))
                        {
                            AppendText("상대방: " + message + Environment.NewLine);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                AppendText("수신 오류: " + ex.Message + Environment.NewLine);
            }
            finally
            {
                CloseConnection();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (writer != null && connected)
            {
                string message = txtInput.Text.Trim();
                if (!string.IsNullOrEmpty(message))
                {
                    AppendText("나: " + message + Environment.NewLine);
                    try
                    {
                        writer.WriteLine(message);
                        writer.Flush();
                    }
                    catch (IOException ex)
                    {
                        AppendText("전송 오류: " + ex.Message + Environment.NewLine);
                    }
                }
                txtInput.Clear();
            }
        }

        private void AppendText(string text)
        {
            if (txtView.InvokeRequired)
            {
                txtView.Invoke(new Action<string>(AppendText), text);
            }
            else
            {
                txtView.AppendText(text);
            }
        }

        private void CloseConnection()
        {
            connected = false;
            try
            {
                reader?.Close();
                writer?.Close();
                stream?.Close();
                client?.Close();
                receiveThread?.Join(500); // 최대 500ms 대기
            }
            catch { }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            CloseConnection();
            base.OnFormClosing(e);
        }
    }
}
