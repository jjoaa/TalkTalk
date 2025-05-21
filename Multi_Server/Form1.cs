using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace TCPServer
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private readonly List<TcpClient> clients = new List<TcpClient>();
        private readonly Dictionary<TcpClient, string> clientNames = new Dictionary<TcpClient, string>();
        private readonly object locker = new object();
        private int clientCounter = 1;

        private delegate void AddTextDelegate(string text);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread serverThread = new Thread(Listen);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        private void Listen()
        {
            AddTextDelegate addText = txt => txtView.Invoke(new Action(() => txtView.AppendText(txt)));

            try
            {
                server = new TcpListener(IPAddress.Any, 8000);
                server.Start();
                addText("서버 시작됨 (다중 클라이언트 지원)" + Environment.NewLine);

                while (true)
                {
                    TcpClient newClient = server.AcceptTcpClient();

                    string clientName = $"Client {clientCounter++}";

                    lock (locker)
                    {
                        clients.Add(newClient);
                        clientNames[newClient] = clientName;
                       // addText($"[{clientName}] 이게? 연결됨" + Environment.NewLine);
                    }

                    Thread clientThread = new Thread(() => HandleClient(newClient));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                addText($"[서버 오류] {ex.Message}"+ Environment.NewLine);
            }
        }

        private void HandleClient(TcpClient client)
        {
            AddTextDelegate addText = txt => txtView.Invoke(new Action(() => txtView.AppendText(txt)));

            string clientName = $"Client {clientCounter++}";

            try
            {
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    // 최초 닉네임 받기
                    string nickname = reader.ReadLine();
                    if (!string.IsNullOrEmpty(nickname))
                        clientName = nickname;

                    lock (locker)
                    {
                       // clients.Add(client);
                        clientNames[client] = clientName;
                    }

                    addText($"[{clientName}] 연결됨"+ Environment.NewLine);

                    // 일반 메시지 수신 루프
                    while (true)
                    {
                        string message = reader.ReadLine();
                        if (message == null) break;

                        addText($"[{clientName}] {message}" + Environment.NewLine);
                        BroadcastMessage(client, message);
                    }
                }
            }
            catch (Exception ex)
            {
                addText($"[{clientName} 오류] {ex.Message}" + Environment.NewLine);
            }
            finally
            {
                lock (locker)
                {
                    clients.Remove(client);
                    clientNames.Remove(client);
                }

                client.Close();
                addText($"[{clientName}] 연결 종료됨"+ Environment.NewLine);
            }
        }

        private void BroadcastMessage(TcpClient sender, string message)
        {
            string senderName;
            lock (locker)
            {
                senderName = clientNames.ContainsKey(sender) ? clientNames[sender] : "알 수 없음";
            }

            foreach (var client in clients.ToList())
            {
                try
                {
                    if (client.Connected)
                    {
                        NetworkStream stream = client.GetStream();
                        StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };
                        writer.WriteLine($"[{senderName}] {message}");
                    }
                }
                catch (Exception ex)
                {
                    txtView.Invoke(new Action(() =>
                        txtView.AppendText($"[전송 오류] {clientNames[client]}: {ex.Message}" + Environment.NewLine)
                    ));
                }
            }
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = txtInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(message)) return;

            txtView.AppendText($"[서버] {message}" + Environment.NewLine);

            lock (locker)
            {
                foreach (var client in clients.ToList())
                {
                    try
                    {
                        if (client.Connected)
                        {
                            NetworkStream stream = client.GetStream();
                            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };
                            writer.WriteLine($"[서버] {message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        txtView.AppendText($"[전송 오류] {clientNames[client]}: {ex.Message}" + Environment.NewLine);
                    }
                }
            }

            txtInput.Clear();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            CloseServer();
        }

        private void CloseServer()
        {
            lock (locker)
            {
                foreach (var client in clients)
                {
                    client.Close();
                }

                clients.Clear();
                clientNames.Clear();
            }

            server?.Stop();
        }
    }
}
