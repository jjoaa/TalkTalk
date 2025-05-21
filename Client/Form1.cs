using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace TCPClient1
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;
        private Thread receiveThread;
        private bool connected;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string ip = "127.0.0.1";
                int port = 8000;

                client = new TcpClient();
                client.Connect(ip, port);

                stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
                connected = true;

                AppendText("서버에 접속 성공!"+ Environment.NewLine);

                receiveThread = new Thread(Receive);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                AppendText("서버 연결 실패: " + ex.Message + Environment.NewLine);
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

        private void btnSend_Click_1(object sender, EventArgs e)
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
