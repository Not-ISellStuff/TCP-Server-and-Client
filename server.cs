using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 65432);
        server.Start();
        Console.WriteLine("Listening...\n");

        while (true)
        {
            using (TcpClient client = server.AcceptTcpClient())
            using (NetworkStream stream = client.GetStream())
            {
                while (true)
                {
                    Console.WriteLine("Commands: [get-ip, get-geo] | Command: ");
                    string command = Console.ReadLine();
                    byte[] cmd = Encoding.UTF8.GetBytes(command);
                    stream.Write(cmd, 0, cmd.Length);

                    byte[] buffmen = new byte[1024];
                    int br = stream.Read(buffmen, 0, buffmen.Length);
                    string resp = Encoding.UTF8.GetString(buffmen, 0, br);
                    Console.WriteLine("Response: " + resp);
                    if (resp == "")
                    {
                        Console.WriteLine("Client has disconnected.");
                        return;
                    }
                }
            }
        }
    }
}
