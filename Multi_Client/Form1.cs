using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace NickClient
{
    public partial class Form1 : Form
    {
        TcpClient client;
        NetworkStream stream;
        StreamReader reader;
        StreamWriter writer;
        Thread receiveThread;
        bool connected = false;
        string nickname;

        private delegate void AddTextDelegate(string text);

        public Form1()
        {
            InitializeComponent();
            txtNickname.ForeColor = Color.Gray;
            txtNickname.Text = "00지역 이름";

            txtNickname.GotFocus += RemovePlaceholder;
            txtNickname.LostFocus += SetPlaceholder;
        }

        private void RemovePlaceholder(object sender, EventArgs e)
        {
            if (txtNickname.ForeColor == Color.Gray)
            {
                txtNickname.Text = "";
                txtNickname.ForeColor = Color.Black;
            }
        }

        private void SetPlaceholder(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNickname.Text))
            {
                txtNickname.Text = "닉네임";
                txtNickname.ForeColor = Color.Gray;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnSend.Enabled = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = txtIP.Text.Trim();
                int port = int.Parse(txtPort.Text.Trim());
                nickname = txtNickname.Text.Trim();

                if (string.IsNullOrEmpty(nickname))
                {
                    MessageBox.Show("닉네임을 입력하세요.");
                    return;
                }

                client = new TcpClient();
                client.Connect(ip, port);

                stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream) { AutoFlush = true };

                // 닉네임 전송
                writer.WriteLine(nickname);

                if (receiveThread == null || !receiveThread.IsAlive)
                {
                    receiveThread = new Thread(Receive);
                    receiveThread.IsBackground = true;
                    receiveThread.Start();

                    connected = true;
                    txtView.AppendText("서버에 연결됨" + Environment.NewLine);
                    btnSend.Enabled = true;
                    btnConnect.Enabled = false;
                }
            }

                
            catch (Exception ex)
            {
                MessageBox.Show("연결 실패: " + ex.Message + Environment.NewLine);
            }
        }

        private void Receive()
        {
            AddTextDelegate addText = new AddTextDelegate(txtView.AppendText);
            try
            {
                while (client.Connected)
                {
                    if (stream.CanRead)
                    {
                        string msg = reader.ReadLine();

                        if (msg == null)
                        {
                            // 서버 연결 끊김
                            connected = false;
                            Invoke(addText, "[알림] 서버와의 연결이 종료되었습니다." + Environment.NewLine);
                            break;
                        }

                        if (!string.IsNullOrEmpty(msg))
                        {
                            if (!msg.StartsWith($"[{nickname}]"))
                            {
                                Invoke(addText, msg + Environment.NewLine);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Invoke(addText, $"[오류] 수신 실패: {ex.Message}" + Environment.NewLine);
            }
        }

        private void AppendText(string text)
        {
            if (txtView.InvokeRequired)
                txtView.Invoke(new AddTextDelegate(AppendText), text);
            else
                txtView.AppendText(text);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (connected && !string.IsNullOrEmpty(txtInput.Text))
            {
                string msg = txtInput.Text;
                writer.WriteLine(msg);
                txtView.AppendText($"[나] {msg}{Environment.NewLine}"); // 👈 이 라인 보이니?
                txtInput.Clear();
            }
            else
            {
                txtView.AppendText("[오류] 연결되지 않았거나 빈 메시지입니다" + Environment.NewLine);
            }
        
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            connected = false;
            try
            {
                receiveThread?.Join(500); // 최대 500ms 대기
                reader?.Close();
                writer?.Close();
                stream?.Close();
                client?.Close();
            }
            catch
            {
                // 종료 중 발생하는 예외 무시
            }
        }

    }
}
