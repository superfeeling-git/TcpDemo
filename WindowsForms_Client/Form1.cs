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

namespace WindowsForms_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        NetworkStream stream;
        TcpClient tcp;
        private void Form1_Load(object sender, EventArgs e)
        {
            IPAddress ip = Dns.GetHostAddresses(Dns.GetHostName()).First(m => m.AddressFamily == AddressFamily.InterNetwork);
            IPEndPoint point = new IPEndPoint(ip, 6104);
            tcp = new TcpClient();
            tcp.Connect(point);
            stream = tcp.GetStream();
            Thread thread = new Thread(() => 
            {
                while(true)
                {
                    byte[] result = new byte[tcp.ReceiveBufferSize];
                    int datalen = stream.Read(result, 0, result.Length);
                    this.Invoke(new Action(() => {
                        string rec_msg = Encoding.Default.GetString(result, 0, datalen);
                        ReceiveMsg.Items.Add(string.Format("消息：{0}", rec_msg));
                    }));
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string msg = textBox1.Text.Trim();
            byte[] len = Encoding.Default.GetBytes(msg);
            stream.Write(len, 0, len.Length);
            textBox1.Text = string.Empty;
        }
    }
}