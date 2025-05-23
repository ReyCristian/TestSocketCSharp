using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using TestSocket;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            if (args[0] == "--server")
            {
                RunServer();
                return;
            }
            else if (args[0] == "--client")
            {
                string host = args.Length > 1 ? args[1] : "127.0.0.1";
                RunClient(host);
                return;
            }
        }

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Clear();
        // Menú interactivo
        Console.WriteLine("Seleccioná una opción:");
        Console.WriteLine("1. Crear servidor");
        Console.WriteLine("2. Unirse a servidor");
        var opcion = Console.ReadKey(intercept: true).KeyChar.ToString();

        if (opcion == "1")
            RunServer();
        else if (opcion == "2")
        {
            Console.Write("IP del servidor (default 127.0.0.1): ");
            string host = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(host))
                host = "127.0.0.1";
            RunClient(host);
        }
    }

    static void RunServer()
    {
        int port = 12345;
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Servidor escuchando en puerto {port}...");

        while (true)
        {
            using var client = listener.AcceptTcpClient();
            using var stream = client.GetStream();

            var response = new { message = "¡Hola mundo!" };
            string json = JsonSerializer.Serialize(response);
            byte[] data = Encoding.UTF8.GetBytes(json);

            stream.Write(data, 0, data.Length);
            Console.WriteLine("Mensaje enviado.");
        }
    }

    static void RunClient(string host)
    {
        int port = 12345;
        try
        {
            using var client = new TcpClient(host, port);
            using var stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            var data = JsonSerializer.Deserialize<Respuesta>(json);
            Console.WriteLine("Servidor dice: " + data?.message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al conectar: " + ex.Message);
        }
    }
}
