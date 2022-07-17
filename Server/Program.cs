using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class serv
    {
        static void Main(string[] args)
        {
            IPAddress ip;
            int port;
            string path;

            if (args.Length > 0)
            {
                ip = IPAddress.Parse(args[0]);
                port = Convert.ToInt32(args[1]);
                path = args[2];
            }

            else
            {
                ip = IPAddress.Parse("127.0.0.1");
                port = 5555;
                path = "temp";
            }
            TcpListener serv = new TcpListener(ip, port);
            serv.Start();
            while (true)
            {
                TcpClient client = serv.AcceptTcpClient();
                byte[] data = new byte[256];
                NetworkStream stream = client.GetStream();
                int bytes = stream.Read(data);
                StringBuilder builder = new StringBuilder();
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                string[] rec = builder.ToString().Split('|');
                int portrec = Convert.ToInt32(rec[1]);
                stream.Write(Encoding.UTF8.GetBytes("ok"));
                string name = rec[0];
                UdpClient recive = new UdpClient(portrec);
                IPEndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);
                ICollection<string> text = new List<string>();
                byte[] con;
                bool check = false;
                recive.Client.ReceiveTimeout = 500;
                while (check == false)
                {
                    con = new byte[3];
                    stream.Read(con);
                    if (Encoding.UTF8.GetString(con).Contains('1'))
                    {
                        stream.Write(Encoding.UTF8.GetBytes("s"));
                        check = true;
                    }
                    try
                    {
                        data = recive.Receive(ref remoteIp);
                        stream.Write(Encoding.UTF8.GetBytes("ok"));
                        string[] part = Encoding.UTF8.GetString(data).Split('|');
                        text.Add(part[1]);
                    }
                    catch { IOException e; }
                }
                StreamWriter file = new StreamWriter(path + "/" + name);
                foreach (string t in text)
                {
                    file.Write(t);
                }
                file.Close();
                recive.Close();
                stream.Close();
                client.Close();
            }
        }
    }
}