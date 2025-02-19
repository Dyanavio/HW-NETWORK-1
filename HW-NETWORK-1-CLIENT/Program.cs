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
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            client.Connect(endPoint);
            while (true)
            {
                try
                {
                    Console.Write("Enter convertion currencies (FROM -> TO)\nFROM('Exit' to exit the program): ");
                    string? fromCurrency = (Console.ReadLine() + ' ').ToUpper();
                    if (fromCurrency.Trim() == "EXIT")
                    {
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Exiting . . .");
                        Console.ResetColor();
                        break;
                    }
                    Console.Write("TO: ");
                    string? toCurrency = (Console.ReadLine())?.ToUpper().Trim();

                    if (string.IsNullOrWhiteSpace(fromCurrency) || string.IsNullOrEmpty(toCurrency))
                    {
                        throw new Exception("Currencies cannot be null");

                    }
                    if (toCurrency.Length > toLength || fromCurrency.Length > fromLength)
                    {
                        throw new Exception("Currency index is invalid");
                    }

                    string message = fromCurrency + toCurrency;
                    byte[] buffer = Encoding.UTF8.GetBytes(message);

                    client.Send(buffer);

                    byte[] buff = new byte[1024];
                    int size = 0;
                    StringBuilder reply = new StringBuilder();

                    do
                    {
                        size = client.Receive(buff);
                        reply.Append(Encoding.UTF8.GetString(buff, 0, size));
                    }
                    while (client.Available > 0);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(reply + "\n");
                    Console.ResetColor();
                    if (reply[reply.Length - 1] == '0')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("No more attempts remaining");
                        Console.ResetColor();
                        Console.ReadLine();

                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                        break;
                    }
                }
                catch(SocketException)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("You were disconnected from the host");
                    Console.ResetColor();
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
