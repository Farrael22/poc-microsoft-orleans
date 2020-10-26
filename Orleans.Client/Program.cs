using Orleans.Interfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrleansClient
{
    /// <summary>
    /// Orleans test silo client
    /// </summary>
    public class Program
    {
        const int initializeAttemptsBeforeFailing = 5;
        private static int attempt = 0;

        static int Main(string[] args)
        {
            return RunMainAsync(args).Result;
        }

        private static async Task<int> RunMainAsync(string[] args)
        {
            try
            {
                using (var client = await StartClientWithRetries())
                {
                    var option = default(int);

                    do
                    {
                        Console.WriteLine("Choose your option number:");
                        Console.WriteLine("1 - Set attendant online");
                        Console.WriteLine("2 - Create Ticket");
                        Console.WriteLine("3 - Close Ticket");

                        option = int.Parse(Console.ReadLine());

                        switch (option)
                        {
                            case 1:
                                await SetAttendantOnlineOptionChoosenAsync(client);
                                break;
                            case 2:
                                await CreateTicketOptionChoosenAsync(client);
                                break;
                            case 3:
                                await CloseTicketOptionChosenAsync(client);
                                break;
                            default:
                                Console.WriteLine("Invalid option");
                                break;
                        }

                    } while (option > 0);
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> StartClientWithRetries()
        {
            attempt = 0;
            IClusterClient client;
            client = new ClientBuilder()
                .UseAdoNetClustering(options =>
                {
                    options.Invariant = "System.Data.SqlClient";
                    options.ConnectionString = "Server=DESKTOP-RS8V952\\SQLSERVER;Database=Orleans;Trusted_Connection=True;";
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "poc";
                    options.ServiceId = "Orleans";
                })
                .ConfigureLogging(logging => logging
                    .AddConsole()
                    .AddFilter("Orleans", LogLevel.Warning)
                    .AddFilter("Runtime", LogLevel.Warning)
                    .AddFilter("Microsoft", LogLevel.Warning))
                .Build();

            await client.Connect(RetryFilter);
            Console.WriteLine("Client successfully connect to silo host");
            return client;
        }

        private static async Task<bool> RetryFilter(Exception exception)
        {
            if (exception.GetType() != typeof(SiloUnavailableException))
            {
                Console.WriteLine($"Cluster client failed to connect to cluster with unexpected error.  Exception: {exception}");
                return false;
            }
            attempt++;
            Console.WriteLine($"Cluster client attempt {attempt} of {initializeAttemptsBeforeFailing} failed to connect to cluster.  Exception: {exception}");
            if (attempt > initializeAttemptsBeforeFailing)
            {
                return false;
            }
            await Task.Delay(TimeSpan.FromSeconds(4));
            return true;
        }

        private static async Task CreateTicketOptionChoosenAsync(IClusterClient client)
        {
            Console.WriteLine("What is bot`s name?");
            var botName = Console.ReadLine();

            Console.WriteLine("What is queue`s name?");
            var queueName = Console.ReadLine();

            Console.WriteLine("What is ticket`s id?");
            var ticketId = int.Parse(Console.ReadLine());

            await CreateTicketsAsync(client, botName, queueName, ticketId);
        }

        private static async Task SetAttendantOnlineOptionChoosenAsync(IClusterClient client)
        {
            var attendant = new Attendant
            {
                AttendantName = "attendant",
                BotsInformation = new Dictionary<string, IList<string>>
                {
                    { 
                        "test", 
                        new List<string>{ "queue1", "queue2" } 
                    }
                },
                CurrentTickets = new List<Ticket>(),
                LastTicketReceivedDate = default,
                MaxSlots = 3
            };

            var attendantGrain = client.GetGrain<IAttendantGrain>($"{attendant.AttendantName}");
            await attendantGrain.GoOnlineAsync(attendant);
        }

        private static async Task CloseTicketOptionChosenAsync(IClusterClient client)
        {
            Console.WriteLine("What is attendant`s name?");
            var attendantName = Console.ReadLine();

            Console.WriteLine("What is bot`s name?");
            var botName = Console.ReadLine();

            Console.WriteLine("What is ticket`s id?");
            var ticketId = int.Parse(Console.ReadLine());

            await CloseTicketAsync(client, attendantName, botName, ticketId);
        }

        private static async Task CloseTicketAsync(IClusterClient client, string attendantName, string botName, int ticketId)
        {
            var attendantGrain = client.GetGrain<IAttendantGrain>($"{attendantName}");
            await attendantGrain.FinishTicketAsync(ticketId);
        }

        private async static Task CreateTicketsAsync(IClusterClient client, string botName, string queueName, int ticketId)
        {
            var ticket = new Ticket(ticketId, queueName, null, botName);

            var owner = client.GetGrain<IBotGrain>("botname");

            try
            {
                await owner.FowardTicketAsync(ticket);
            }
            catch (Exception e)
            {
                Console.WriteLine("#CreateTicketsAsync - failed", e);
            }
        }
    }
}
