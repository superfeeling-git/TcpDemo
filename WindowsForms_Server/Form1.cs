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
using System.Threading;

namespace WindowsForms_Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Dictionary<string, NetworkStream> tcplist = new Dictionary<string, NetworkStream>();
        private void Form1_Load(object sender, EventArgs e)
        {
            IPAddress ip = Dns.GetHostAddresses(Dns.GetHostName()).First(m => m.AddressFamily == AddressFamily.InterNetwork);
            IPEndPoint point = new IPEndPoint(ip, 6104);
            TcpListener tcp = new TcpListener(point);
            tcp.Start();
            Thread thread = new Thread(() => 
            {
                while(true)
                {
                    TcpClient client_tcp = tcp.AcceptTcpClient();
                    NetworkStream stream = client_tcp.GetStream();
                    IPEndPoint ip_point = (IPEndPoint)client_tcp.Client.RemoteEndPoint;                    
                    tcplist.Add($"{ip_point.Address}:{ip_point.Port}", stream);
                    this.Invoke(new Action(() => 
                    {
                        ClientList.Items.Add($"{ip_point.Address}:{ip_point.Port}");
                    }));
                    
                    Thread curr_thread = new Thread((obj) => 
                    {
                        while(true)
                        {
                            TcpClient curr_client = (TcpClient)obj;
                            NetworkStream curr_stream = curr_client.GetStream();
                            try
                            {                                 
                                byte[] result = new byte[curr_client.ReceiveBufferSize];
                                int datalen = curr_stream.Read(result, 0, result.Length);
                                string rec_msg = Encoding.Default.GetString(result);
                                this.Invoke(new Action(() => {                                
                                    ReceiveMsg.Items.Add(string.Format("消息：{0}", rec_msg));
                                    Log.Items.Add($"消息时间：{DateTime.Now}，消息内容：{rec_msg}");
                                }));
                            }
                            catch(Exception ex)
                            {
                                curr_stream.Close();
                                curr_stream.Dispose();
                                curr_client.Close();
                                curr_client.Dispose();
                                Log.Items.Add($"发生异常：{ex.Message}");
                                break;
                            }
                        }
                    });
                    curr_thread.IsBackground = true;
                    curr_thread.Start(client_tcp);
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ClientList.SelectedItems.Count <= 0)
                MessageBox.Show("请选择要发送到的客户端");
            else
            { 
                string ip = ClientList.SelectedItem.ToString();
                NetworkStream stream = (NetworkStream)tcplist.First(m => m.Key == ip).Value;
                byte[] msg = Encoding.Default.GetBytes(textBox1.Text.Trim());
                stream.Write(msg, 0, msg.Length);
                textBox1.Text = string.Empty;
            }
        }
    }
}