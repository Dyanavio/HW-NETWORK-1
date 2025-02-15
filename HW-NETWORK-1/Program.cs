using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HW_NETWORK_1
{
    

    internal class Program
    {
        static void Main(string[] args)
        {
            SortedDictionary<string, double> converters = new SortedDictionary<string, double>()
            {
                ["UAH USD"] = 0.024042,
                ["USD UAH"] = 41.5943,
                ["USD EUR"] = 0.953,
                ["EUR USD"] = 1.0493,
                ["EUR GBP"] = 0.8337,
                ["GBP EUR"] = 1.1995,
                ["SEK UAH"] = 3.889984,
                ["UAH SEK"] = 0.25707
            };
            
            const string ip = "127.0.0.5";
            const int port = 5050;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(endPoint);
            server.Listen(5);
            while (true)
            {
                string log = "";

                Socket listener = server.Accept();

                log += $"Connection time: {DateTime.Now}\n";
                byte[] buffer = new byte[1024];
                int size = 0;
                StringBuilder data = new StringBuilder();

                do
                {
                    size = listener.Receive(buffer);
                    data.Append(Encoding.UTF8.GetString(buffer, 0, size));
                   // data.Append(' ');
                } 
                while (listener.Available > 0);
                log += $"  -{data}\n";
                //Console.WriteLine(data.ToString());
                try
                {
                    string? reply = $"{data}\n1 -> {converters[data.ToString()]}";
                    byte[] buff = Encoding.UTF8.GetBytes(reply);

                    listener.Send(buff);

                }
                catch(KeyNotFoundException e)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }
                catch(Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }

                log += $"Disconnect time: {DateTime.Now}\n";
                listener.Shutdown(SocketShutdown.Both);
                listener.Close();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(log);

            }
        }
    }
}
