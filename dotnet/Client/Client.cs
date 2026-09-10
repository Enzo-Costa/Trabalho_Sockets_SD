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
            bool executando = true;

            while (executando)
            {
                Console.WriteLine("\n==================================================");
                Console.WriteLine("=== MENU DE OPÇÕES (SELECIONE ENTRE 0, 1, 2, X ou S) ===");
                Console.WriteLine("0: INT");
                Console.WriteLine("1: CHAR");
                Console.WriteLine("2: STRING");
                Console.WriteLine("X: DEMONSTRAÇÃO COM OS TRÊS TIPOS");
                Console.WriteLine("S: SAIR");
                Console.Write("Opção: ");
                
                string? tipo_vari = Console.ReadLine()?.Trim();
                
                if (string.Equals(tipo_vari, "S", StringComparison.OrdinalIgnoreCase))
                {
                    executando = false;
                    Console.WriteLine("Encerrando o cliente...");
                    continue;
                }

                // Cria o lote de testes baseado na escolha do usuário
                var testes = ObterTestes(tipo_vari);

                Console.WriteLine("\n=== INICIANDO TRANSMISSÃO TCP (1 CON POR ENVIO) ===");
                
                foreach (var teste in testes)
                {
                    EnviarReceberPayload(host, port, teste.Tipo, teste.Val);
                }
            }

            Console.WriteLine("=== APLICAÇÃO FINALIZADA ===");
        }

        private static (string Tipo, string Val)[] ObterTestes(string? opcao)
        {
            switch (opcao)
            {
                case "0":
                    Console.Write("=== AGUARDANDO INPUT DE INT ===\nDigite um valor: ");
                    string? inputInt = Console.ReadLine();
                    if (!int.TryParse(inputInt, out _))
                    {
                        Console.WriteLine("=== INT FORA DE ESCALA/INVÁLIDO, USANDO 0 ===");
                        inputInt = "0";
                    }
                    return new[] { (Tipo: "int", Val: inputInt) };

                case "1":
                    Console.Write("=== AGUARDANDO INPUT DE CHAR ===\nDigite um caractere: ");
                    string? inputChar = Console.ReadLine();
                    if (string.IsNullOrEmpty(inputChar) || inputChar.Length != 1)
                    {
                        Console.WriteLine("=== CHAR INVÁLIDO, USANDO WHITESPACE ===");
                        inputChar = " ";
                    }
                    return new[] { (Tipo: "char", Val: inputChar) };

                case "2":
                    Console.Write("=== AGUARDANDO INPUT DE STRING ===\nDigite uma string: ");
                    string? inputStr = Console.ReadLine();
                    if (string.IsNullOrEmpty(inputStr))
                    {
                        Console.WriteLine("=== STRING VAZIA, USANDO WHITESPACE ===");
                        inputStr = " ";
                    }
                    return new[] { (Tipo: "string", Val: inputStr) };

                case "X":
                case "x":
                default:
                    Console.WriteLine("=== SEGUINDO COM VARIÁVEIS DE TESTE PADRÃO ===");
                    return new[]
                    {
                        (Tipo: "int", Val: "37"),
                        (Tipo: "char", Val: "m"),
                        (Tipo: "string", Val: "Sistemas Distribuidos UERJ")
                    };
            }
        }

        private static void EnviarReceberPayload(string host, int port, string tipo, string val)
        {
            try
            {
                // TCP 1 con: Conexão criada e descartada a cada mensagem enviada
                using TcpClient client = new TcpClient(host, port);
                using NetworkStream stream = client.GetStream();
                using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };
                using StreamReader reader = new StreamReader(stream);

                var req = new JsonObject
                {
                    ["tipo"] = tipo,
                    ["val"] = val
                };

                Stopwatch stopwatch = Stopwatch.StartNew();
                writer.WriteLine(req.ToJsonString());

                string? responseLine = reader.ReadLine();
                stopwatch.Stop();

                double rttMs = stopwatch.Elapsed.TotalMilliseconds;

                Console.WriteLine($"[{tipo.ToUpper()}] Enviado: {req.ToJsonString()} | Resposta: {responseLine} | RTT: {rttMs:F3} ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar ou transmitir tipo {tipo}: {ex.Message}");
            }
        }
    }
}