using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text.Json.Nodes;

namespace SocketClient
{
    class Client
    {
        static void Main()
        {
            string host = "127.0.0.1";
            int port = 5000;

            // Estrutura com os payloads para teste
            var testes = new[]
            {
                new { Tipo = "int", Val = "37" },
                new { Tipo = "char", Val = "m" },
                new { Tipo = "string", Val = "Sistemas Distribuidos UERJ" }
            };

            Console.WriteLine("=== INICIANDO BATERIA DE TESTES (CLIENTE C#) ===");

            foreach (var teste in testes)
            {
                try
                {
                    using TcpClient client = new TcpClient(host, port);
                    using NetworkStream stream = client.GetStream();
                    using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };
                    using StreamReader reader = new StreamReader(stream);

                    var req = new JsonObject
                    {
                        ["tipo"] = teste.Tipo,
                        ["val"] = teste.Val
                    };

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    writer.WriteLine(req.ToJsonString());

                    string? responseLine = reader.ReadLine();
                    stopwatch.Stop();

                    double rttMs = stopwatch.Elapsed.TotalMilliseconds;

                    Console.WriteLine($"[{teste.Tipo.ToUpper()}] Enviado: {req.ToJsonString()} | Resposta: {responseLine} | RTT: {rttMs:F3} ms");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao testar tipo {teste.Tipo}: {ex.Message}");
                }
            }

            Console.WriteLine("=== TESTES CONCLUÍDOS ===");
        }
    }
}