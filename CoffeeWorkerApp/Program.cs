using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeWorkerApp.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoffeeWorkerApp
{
    public class Program
    {
        public static void Main (string[] args)
        {
            CreateHostBuilder (args).Build ().Run ();
        }

        public static IHostBuilder CreateHostBuilder (string[] args)
        {
            var builtConfig = new ConfigurationBuilder ()
                .AddJsonFile ("appsettings.json")
                .AddJsonFile ("appsettings.Development.json", true)
                .AddCommandLine (args)
                .Build ();

            return Host.CreateDefaultBuilder (args)
                .ConfigureServices ((hostContext, services) =>
                {
                    services.AddOptions ();
                    services.Configure<KafkaConsumerConfiguration> (builtConfig.GetSection ("KafkaConfiguration"));
                    services.AddHostedService<Worker> ();
                })
                .ConfigureLogging (logging =>
                {
                    logging.AddConsole ();
                });
        }
    }
}