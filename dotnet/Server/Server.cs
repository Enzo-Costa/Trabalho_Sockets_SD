using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json.Nodes;

namespace SocketServer
{
    class Server
    {
        static void Main()
        {
            int port = 5000;
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine($"Servidor C# TCP rodando na porta {port}");

            while (true)
            {
                using TcpClient client = server.AcceptTcpClient();
                using NetworkStream stream = client.GetStream();
                using StreamReader reader = new StreamReader(stream);
                using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

                string? inputLine = reader.ReadLine();
                if (inputLine != null)
                {
                    var json = JsonNode.Parse(inputLine);
                    string tipo = json!["tipo"]!.ToString();
                    string val = json["val"]!.ToString();

                    var resp = new JsonObject { ["tipo"] = tipo };

                    switch (tipo)
                    {
                        case "int":
                            resp["val"] = int.Parse(val) + 1;
                            break;
                        case "char":
                            char c = val[0];
                            resp["val"] = (char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c)).ToString();
                            break;
                        case "string":
                            char[] arr = val.ToCharArray();
                            Array.Reverse(arr);
                            resp["val"] = new string(arr);
                            break;
                    }

                    writer.WriteLine(resp.ToJsonString());
                }
            }
        }
    }
}