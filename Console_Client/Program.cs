using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Console_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("客户端窗口");
            IPAddress ip = Dns.GetHostAddresses(Dns.GetHostName()).First(m => m.AddressFamily == AddressFamily.InterNetwork);
            IPEndPoint point = new IPEndPoint(ip, 5600);
            TcpClient client = new TcpClient();
            client.Connect(point);
            NetworkStream stream = client.GetStream();

            Thread thread = new Thread(() => {
                
                while (true)
                {                     
                    BinaryReader br = new BinaryReader(stream);                    
                    string msg = br.ReadString();
                    Console.WriteLine(msg);
                }                
            });
            thread.Start();

            BinaryWriter bw = new BinaryWriter(stream);
            new Thread(() =>
            {
                while (true)
                {                    
                    string msg = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        byte[] result = Encoding.Default.GetBytes(msg);
                        stream.Write(result, 0, result.Length);
                    }
                }
            }).Start();
        }
    }
}
