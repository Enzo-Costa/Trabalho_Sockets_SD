import java.io.*;
import java.net.*;
import org.json.JSONObject;

public class Client {
    public static void main(String[] args) {
        String host = "127.0.0.1";
        int port = 5000;

        // Estrutura com os payloads para teste
        String[][] testes = {
            {"int", "37"},
            {"char", "m"},
            {"string", "Sistemas Distribuidos UERJ"}
        };

        System.out.println("=== INICIANDO BATERIA DE TESTES (CLIENTE JAVA) ===");

        for (String[] teste : testes) {
            String tipo = teste[0];
            String val = teste[1];

            // Para 'TCP 1 con', abre uma nova conexão TCP por requisição
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
                System.err.println("Erro ao testar tipo " + tipo + ": " + e.getMessage());
            }
        }

        System.out.println("=== TESTES CONCLUÍDOS ===");
    }
}