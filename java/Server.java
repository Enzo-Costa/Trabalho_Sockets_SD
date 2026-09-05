import java.io.*;
import java.net.*;
import org.json.JSONObject;

public class Server {
    public static void main(String[] args) {
        int port = 5000;
        try (ServerSocket serverSocket = new ServerSocket(port)) {
            System.out.println("Servidor Java TCP rodando na porta " + port);

            while (true) {
                try (Socket clientSocket = serverSocket.accept();
                     BufferedReader in = new BufferedReader(new InputStreamReader(clientSocket.getInputStream()));
                     PrintWriter out = new PrintWriter(clientSocket.getOutputStream(), true)) {

                    String inputLine = in.readLine();
                    if (inputLine != null) {
                        JSONObject req = new JSONObject(inputLine);
                        String tipo = req.getString("tipo");
                        String val = req.get("val").toString();

                        JSONObject resp = new JSONObject();
                        resp.put("tipo", tipo);

                        switch (tipo) {
                            case "int":
                                int num = Integer.parseInt(val) + 1;
                                resp.put("val", num);
                                break;
                            case "char":
                                char c = val.charAt(0);
                                char resultChar = Character.isUpperCase(c) ? Character.toLowerCase(c) : Character.toUpperCase(c);
                                resp.put("val", String.valueOf(resultChar));
                                break;
                            case "string":
                                String reversed = new StringBuilder(val).reverse().toString();
                                resp.put("val", reversed);
                                break;
                        }

                        out.println(resp.toString());
                    }
                } catch (Exception e) {
                    System.err.println("Erro na conexão: " + e.getMessage());
                }
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}