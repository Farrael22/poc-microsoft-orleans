using System;
using System.Linq;
using System.Threading.Tasks;
using Orleans.Grains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace OrleansSiloHost
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var silosToStart = 1;
            if (args.Any()) silosToStart = int.Parse(args[0]);
            return RunMainAsync(silosToStart).Result;
        }

        private static async Task<int> RunMainAsync(int silosToStart)
        {
            try
            {
                var hostTasks = Enumerable
                    .Range(2, silosToStart)
                    .Select(id => StartSilo(id - 1));

                var hosts = await Task.WhenAll(hostTasks);

                Console.WriteLine("Press Enter to terminate...");
                Console.ReadLine();


                foreach (var host in hosts)
                {
                    await host.StopAsync();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo(int id)
        {
            var builder = new SiloHostBuilder()
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
                .Configure<GrainCollectionOptions>(options =>
                {
                    options.CollectionAge = new TimeSpan(00, 00, 40);
                    options.CollectionQuantum = new TimeSpan(00, 00, 30);
                })
                .ConfigureEndpoints(siloPort: 11111 + id, gatewayPort: 30000 + id)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(BotGrain).Assembly).WithReferences())
                .UseDashboard(options => { options.Port = 8080 + id; })
                .ConfigureLogging(logging => logging
                    .AddConsole()
                    .AddFilter("Orleans", LogLevel.Warning)
                    .AddFilter("Runtime", LogLevel.Warning)
                    .AddFilter("Microsoft", LogLevel.Warning));

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
