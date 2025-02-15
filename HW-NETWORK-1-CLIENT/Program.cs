using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HW_NETWORK_1_CLIENT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string ip = "127.0.0.5";
            const int port = 5050;
            const int fromLength = 4;
            const int toLength = 3;

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("------ Available convertions ------\n\t-UAH USD\n\t-USD UAH\n\t-USD EUR\n\t-EUR USD\n\t-EUR GBP\n\t-GBP EUR\n\t-SEK UAH\n\t-UAH SEK\n");
            Console.ResetColor();

            while (true)
            {
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                
                try
                {
                    Console.Write("Enter convertion currencies (FROM -> TO)\nFROM('Exit' to exit the program): ");
                    string? fromCurrency = (Console.ReadLine() + ' ').ToUpper();
                    Console.Write("TO: ");
                    string? toCurrency = (Console.ReadLine())?.ToUpper().Trim();
                    if (fromCurrency.Trim() == "EXIT")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Exiting . . .");
                        Console.ResetColor();
                        break;
                    }
                    if (string.IsNullOrWhiteSpace(fromCurrency) || string.IsNullOrEmpty(toCurrency))
                    {
                        throw new Exception("Currencies cannot be null");
                    }
                    if (toCurrency.Length > toLength || fromCurrency.Length > fromLength)
                    {
                        throw new Exception("Currency index is invalid");
                    }
                    
                    byte[] fromData = Encoding.UTF8.GetBytes(fromCurrency);
                    byte[] toData = Encoding.UTF8.GetBytes(toCurrency);

                    client.Connect(endPoint);
                    client.Send(fromData);
                    client.Send(toData);

                    byte[] buffer = new byte[1024];
                    int size = 0;
                    StringBuilder reply = new StringBuilder();

                    do
                    {
                        size = client.Receive(buffer);
                        reply.Append(Encoding.UTF8.GetString(buffer, 0, size));
                    }
                    while (client.Available > 0);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(reply + "\n");
                    Console.ReadKey();
                    Console.ResetColor();


                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
            }
            

        }
    }
}
