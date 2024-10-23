using System;
using System.Net.Sockets;
using System.Text;
using System.Net.Http;
using System.Text.Json;

class Client
{
    static void Main()
    {
        using (TcpClient client = new TcpClient("127.0.0.1" , 65432))
        using (NetworkStream stream = client.GetStream())
        {
            Commands cmds = new Commands();

            while (true)
            {
                byte[] buffmen = new byte[1024];
                int bytesRead = stream.Read(buffmen, 0, buffmen.Length);
                string command = Encoding.UTF8.GetString(buffmen, 0, bytesRead);

                if (command == "get-ip")
                {
                    byte[] send = Encoding.UTF8.GetBytes(cmds.IP());
                    stream.Write(send, 0, send.Length);
                } else if (command == "get-geo")
                {
                    byte[] send = Encoding.UTF8.GetBytes(cmds.GEO());
                    stream.Write(send, 0, send.Length);
                } else {
                    byte[] send = Encoding.UTF8.GetBytes("[!] Invalid Command.");
                    stream.Write(send, 0, send.Length);
                }
            }
        }
    }
}

class Commands
{
    public string IP()
    {
        var u = "https://api.ipify.org?format=json";
        HttpClient req = new HttpClient();

        var r = req.GetAsync(u).Result;
        var resp = r.Content.ReadAsStringAsync().Result;
        var data = JsonDocument.Parse(resp);
        var ip = data.RootElement.GetProperty("ip").GetString();

        return ip;
    }

    public string GEO()
    {
        string ip = IP();
        var u = $"https://ipinfo.io/{ip}";
        HttpClient req = new HttpClient();

        var r = req.GetAsync(u).Result;
        var resp = r.Content.ReadAsStringAsync().Result;
        var data = JsonDocument.Parse(resp);

        var country = data.RootElement.GetProperty("country").GetString();
        var state = data.RootElement.GetProperty("region").GetString();
        var city = data.RootElement.GetProperty("city").GetString();
        var isp = data.RootElement.GetProperty("org").GetString();

        string geo = $"IP: {ip} | Country: {country} | State: {state} | City: {city} | ISP: {isp}";
        return geo;
    }
}
