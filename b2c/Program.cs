using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using b2c.Commands;
using EPS.Extensions.B2CGraphUtil;
using EPS.Extensions.B2CGraphUtil.Config;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.ExternalConnectors;


namespace b2c
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        static async Task Main(string[] args)
        {
            var oc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("B2C command-line app");
            Console.WriteLine("Endpoint Systems");
            Console.WriteLine("https://endpointsystems.com");
            Console.WriteLine("---------------------------");
            Console.ForegroundColor = oc;
            var config = new ConfigurationBuilder()

                .AddJsonFile("b2c.json", false, false)
                .Build();
            Configuration = config;

            
            var services = new ServiceCollection()
                .AddLogging(l => l.AddConsole());


            services.AddSingleton<IConfiguration>(config);
            services.Configure<GraphUtilConfig>(Configuration.GetSection("GraphUtilConfig"));
            services.AddSingleton<UserRepo>();
            services.AddSingleton<GroupsRepo>();

            var p = services.BuildServiceProvider();
            var app = new CommandLineApplication<B2C>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(p);
            await app.ExecuteAsync(args);
        }
    }
}
