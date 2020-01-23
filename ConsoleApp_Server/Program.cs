using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace ConsoleApp_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("服务端窗口");
            IPAddress ip = Dns.GetHostAddresses(Dns.GetHostName()).First(m => m.AddressFamily == AddressFamily.InterNetwork);
            IPEndPoint point = new IPEndPoint(ip, 5600);
            TcpListener tcp = new TcpListener(point);
            tcp.Start();

            TcpClient client = tcp.AcceptTcpClient();
            BinaryWriter bw;
            BinaryReader br;
            NetworkStream stream = client.GetStream();            

            new Thread(() => 
            {                
                while (true)
                {
                    byte[] result = new byte[client.ReceiveBufferSize];
                    int datalen = stream.Read(result, 0, result.Length);
                    string msg = Encoding.Default.GetString(result,0,datalen);
                    Console.WriteLine(string.Format("消息：{0}  长度：{1}", msg, datalen));
                }
            }).Start();

            new Thread(() =>
            {
                while (true)
                {
                    bw = new BinaryWriter(stream);
                    string msg = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(msg))
                        bw.Write(msg);
                }
            }).Start();
        }
    }
}
