import java.io.*;
import java.net.*;
import java.util.Scanner;
import org.json.JSONObject;

public class Client {
    public static void main(String[] args) {
        String host = "127.0.0.1";
        int port = 5000;
        Scanner scanner = new Scanner(System.in);
        boolean executando = true;

        while (executando) {
            System.out.println("\n==================================================");
            System.out.println("=== MENU DE OPÇÕES (SELECIONE ENTRE 0, 1, 2, X ou S) ===");
            System.out.println("0: INT");
            System.out.println("1: CHAR");
            System.out.println("2: STRING");
            System.out.println("X: DEMONSTRAÇÃO COM OS TRÊS TIPOS");
            System.out.println("S: SAIR");
            System.out.print("Opção: ");

            String opcao = scanner.nextLine().trim();

            if (opcao.equalsIgnoreCase("S")) {
                executando = false;
                System.out.println("Encerrando o cliente Java...");
                continue;
            }

            // Obtém os testes formatados conforme a opção escolhida
            String[][] testes = obterTestes(opcao, scanner);

            System.out.println("\n=== INICIANDO TRANSMISSÃO TCP (1 CON POR ENVIO) ===");
            for (String[] teste : testes) {
                enviarReceberPayload(host, port, teste[0], teste[1]);
            }
        }

        scanner.close();
        System.out.println("=== APLICAÇÃO FINALIZADA ===");
    }

    private static String[][] obterTestes(String opcao, Scanner scanner) {
        switch (opcao) {
            case "0":
                System.out.print("=== AGUARDANDO INPUT DE INT ===\nDigite um valor: ");
                String inputInt = scanner.nextLine();
                try {
                    Integer.parseInt(inputInt);
                } catch (NumberFormatException e) {
                    System.out.println("=== INT FORA DE ESCALA/INVÁLIDO, USANDO 0 ===");
                    inputInt = "0";
                }
                return new String[][]{{"int", inputInt}};

            case "1":
                System.out.print("=== AGUARDANDO INPUT DE CHAR ===\nDigite um caractere: ");
                String inputChar = scanner.nextLine();
                if (inputChar.length() != 1) {
                    System.out.println("=== CHAR INVÁLIDO, USANDO WHITESPACE ===");
                    inputChar = " ";
                }
                return new String[][]{{"char", inputChar}};

            case "2":
                System.out.print("=== AGUARDANDO INPUT DE STRING ===\nDigite uma string: ");
                String inputStr = scanner.nextLine();
                if (inputStr.isEmpty()) {
                    System.out.println("=== STRING VAZIA, USANDO WHITESPACE ===");
                    inputStr = " ";
                }
                return new String[][]{{"string", inputStr}};

            case "X":
            case "x":
            default:
                System.out.println("=== SEGUINDO COM VARIÁVEIS DE TESTE PADRÃO ===");
                return new String[][]{
                    {"int", "37"},
                    {"char", "m"},
                    {"string", "Sistemas Distribuidos UERJ"}
                };
        }
    }

    private static void enviarReceberPayload(String host, int port, String tipo, String val) {
        // TCP 1 con: Abre e fecha o socket a cada mensagem enviada
        try (Socket socket = new Socket(host, port);
             PrintWriter out = new PrintWriter(socket.getOutputStream(), true);
             BufferedReader in = new BufferedReader(new InputStreamReader(socket.getInputStream()))) {

            JSONObject req = new JSONObject();
            req.put("tipo", tipo);
            req.put("val", val);

            long start = System.nanoTime();
            out.println(req.toString());

            String responseLine = in.readLine();
            long end = System.nanoTime();

            double rttMs = (end - start) / 1_000_000.0;

            System.out.printf("[%s] Enviado: %s | Resposta: %s | RTT: %.3f ms\n",
                              tipo.toUpperCase(), req.toString(), responseLine, rttMs);

        } catch (IOException e) {
            System.err.println("Erro ao conectar ou transmitir tipo " + tipo + ": " + e.getMessage());
        }
    }
}