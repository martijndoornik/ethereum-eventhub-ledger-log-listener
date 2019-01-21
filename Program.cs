using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AzureEventhubProtocol.Connect;
using AzureEventhubProtocol.Send;

namespace LogListener
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Initiating database connection now...");
                //var connectionString = "Server=tcp:qnh-ledger.database.windows.net,1433;Initial Catalog=qnh-ledger-log-listener;Persist Security Info=False;User ID=martijndoornik;Password=7D&2Bw9Jo%&p#76O;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                LogDatabase.Connect(args);
                Console.WriteLine("Connection established.");

                Console.WriteLine("Initiating Eventhub connection now...");
                var template = EventhubConnectionTemplate.FromArgumentList(args, "log-listener", "0.1.0");
                var connection = EventhubConnection.Connect(template, async (message) =>
                {
                    Console.WriteLine($"{message.DateTime.ToString()}: received {message.EventName}");
                    await LogDatabase.Instance.SaveEvent(message);
                });
                Console.WriteLine("Connection established.");
                var con = true;
                string input;
                Console.WriteLine("Log Listener now active");
                while (con)
                {
                    input = Console.ReadLine();
                    switch (input?.ToLower())
                    {
                        case null:
                            break;
                        case "exit":
                            con = false;
                            break;
                        case "status":
                            Console.WriteLine("Attempting to send status...");
                            connection.SendStatusMessage().GetAwaiter().GetResult();
                            Console.WriteLine("Status was sent.");
                            break;
                        default:

                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught in {e.Source}.");
                Console.WriteLine($"Message: {e.Message}");
                Console.WriteLine("Press any button to close the application...");
                Console.ReadKey();
            }
        }
    }
}
