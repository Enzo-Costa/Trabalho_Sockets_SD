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
            Console.WriteLine("=== AGUARDANDO TIPO DE INPUT (SELECIONE ENTRE  0,1 E 2) ===");
            Console.WriteLine("0: INT");
            Console.WriteLine("1: CHAR");
            Console.WriteLine("2: STRING");
            Console.WriteLine("X: DEMONSTRACAO COM OS TRES TIPOS");
            var tipo_vari = Console.ReadLine();
            var input_tipo = 0;
            switch (tipo_vari)
            {
                case "0":
                    Console.WriteLine("INT");
                    input_tipo = 0;
                    break;
                case "1":
                    Console.WriteLine("CHAR");
                    input_tipo = 1;
                    break;
                case "2":
                    Console.WriteLine("STRING");
                    input_tipo = 2;
                    break;
                default:
                    input_tipo = 3;
                    Console.WriteLine("DEMONSTACAO");
                    break;
            }
            if (input_tipo == 0)
            {
                Console.WriteLine("=== AGUARDANDO INPUT DE INT ===");
                var returner = Console.ReadLine();
                int x = 0;
                if (Int32.TryParse(returner, out x))
                {
                    testes = new[]
                    {
                        new { Tipo = "int", Val = returner }
                    };
                }
                else
                {
                    Console.WriteLine("=== INT FORA DE ESCALA, RETORNANDO A 0 ===");
                    testes = new[]
                    {
                        new { Tipo = "int", Val = "0" }
                    };
                }
            }
            else if (input_tipo == 1)
            {
                Console.WriteLine("=== AGUARDANDO INPUT DE CHAR ===");
                var char_vazio = " ";
                var returner = Console.ReadLine();
                if (returner.Length == 1)
                {
                    testes = new[]
                    {
                        new { Tipo = "char", Val = returner }
                    };
                }
                else
                {
                    Console.WriteLine("=== CHAR FORA DE ESCALA, RETORNANDO WHITESPACE ===");
                    testes = new[]
                    {
                        new { Tipo = "char", Val = char_vazio }
                    };
                }
            }
            else if (input_tipo == 2)
            {
                Console.WriteLine("=== AGUARDANDO INPUT DE STRING ===");
                var char_vazio = " ";
                var returner = Console.ReadLine();
                if (returner.Length > 0)
                {
                    testes = new[]
                    {
                        new { Tipo = "string", Val = returner }
                    };
                }
                else
                {
                    Console.WriteLine("=== STRING VAZIA, RETORNANDO WHITESPACE ===");
                    testes = new[]
                    {
                        new { Tipo = "string", Val = char_vazio }
                    };
                }
            }
            else
            {
                Console.WriteLine("=== SEGUINDO COM VARIÁVEIS DE TESTE ===");
            }
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