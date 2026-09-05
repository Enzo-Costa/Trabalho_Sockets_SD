# Trabalho 1: Sockets TCP Interoperáveis (Java & C#)

**Disciplina:** Sistemas Distribuídos (2026/1)  
**Professor:** Alexandre Sztajnberg  
**Alunos:** Enzo Moraes da Costa e Marcos Alves   

---

## 1. Descrição do Projeto

Este projeto consiste na implementação de uma aplicação cliente-servidor utilizando **Sockets TCP puros** para comunicação interprocessos (IPC). O objetivo principal é demonstrar a interoperabilidade entre duas linguagens de programação diferentes (**Java** e **C#**) trocando mensagens padronizadas em JSON.

### Modelo de Comunicação: TCP 1 con
A aplicação segue o modelo `TCP 1 con`, onde uma nova conexão TCP é aberta pelo cliente a cada requisição, processada pelo servidor, e finalizada após o envio da resposta.

---

## 2. Protocolo da Camada de Aplicação

Todas as requisições e respostas utilizam o formato JSON delimitado por uma quebra de linha (`\n`) para controle de enquadramento (*framing*).

### Formato da Requisição
```json
{"tipo": "int" | "char" | "string", "val": "<valor>"}
```

### Regras de Negócio do Servidor
* **`int`**: Converte o valor para número inteiro, incrementa +1 e retorna.
* **`char`**: Inverte a caixa do caractere (maiúscula <-> minúscula) e retorna.
* **`string`**: Inverte a ordem da cadeia de caracteres (*string reversal*) e retorna.

---

## 3. Estrutura do Repositório

```text
trabalho-sockets/
├── dotnet/                  # Projetos em C# (.NET 8)
│   ├── Client/              # Cliente C#
│   │   ├── Client.cs
│   │   └── Client.csproj
│   └── Server/              # Servidor C#
│       ├── Server.cs
│       └── Server.csproj
└── java/                    # Projetos em Java
    ├── Client.java          # Cliente Java
    ├── Server.java          # Servidor Java
    └── json-20231013.jar    # Biblioteca org.json
```

---

## 4. Instruções de Compilação e Execução

### Pré-requisitos
* **.NET SDK 8.0+** (para compilar C#)
* **JDK 17+** (para compilar Java)

---

### Cenário A: Servidor C# + Cliente Java

1. **Iniciar o Servidor C#:**
   ```bash
   cd dotnet/Server
   dotnet run
   ```

2. **Executar o Cliente Java (em outro terminal):**
   * **Linux/macOS:**
     ```bash
     cd java
     javac -cp .:json-20231013.jar Client.java
     java -cp .:json-20231013.jar Client
     ```
   * **Windows (PowerShell):**
     ```powershell
     cd java
     javac -cp ".;json-20231013.jar" Client.java
     java -cp ".;json-20231013.jar" Client
     ```

---

### Cenário B: Servidor Java + Cliente C#

1. **Iniciar o Servidor Java:**
   * **Linux/macOS:**
     ```bash
     cd java
     javac -cp .:json-20231013.jar Server.java
     java -cp .:json-20231013.jar Server
     ```
   * **Windows (PowerShell):**
     ```powershell
     cd java
     javac -cp ".;json-20231013.jar" Server.java
     java -cp ".;json-20231013.jar" Server
     ```

2. **Executar o Cliente C# (em outro terminal):**
   ```bash
   cd dotnet/Client
   dotnet run
   ```

---

## 5. Medição de Desempenho (RTT)

O *Round Trip Time* (RTT) é medido no lado do cliente isolando o tempo gasto no estabelecimento da conexão socket, envio da mensagem e recepção completa do retorno:
* **Java:** Medido via `System.nanoTime()`.
* **C#:** Medido via `System.Diagnostics.Stopwatch`.

### Resultados Obtidos (Ambiente Localhost)

| Cenário (Servidor <- Cliente) | Tipo de Dados | RTT Médio (ms) |
| :--- | :--- | :--- |
| **C# Server <- Java Client** | `int` | ~1.85 ms |
| **C# Server <- Java Client** | `char` | ~0.75 ms |
| **C# Server <- Java Client** | `string` | ~0.82 ms |
| **Java Server <- C# Client** | `int` | ~1.62 ms |
| **Java Server <- C# Client** | `char` | ~0.68 ms |
| **Java Server <- C# Client** | `string` | ~0.71 ms |

---

## 6. Considerações de Projeto

* **Codificação:** A codificação **UTF-8** foi padronizada em ambas as pontas para prevenir corrupção de caracteres especiais ou acentuados.
* **Overhead de Conexão:** A primeira requisição apresenta um RTT ligeiramente mais alto devido ao custo do *handshake* TCP em conexões individuais (`TCP 1 con`).