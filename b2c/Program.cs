using System;
using System.Threading.Tasks;
using b2c.Commands;
using EPS.Extensions.B2CGraphUtil;
using EPS.Extensions.B2CGraphUtil.Config;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace b2c
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()

                .AddJsonFile("b2c.json", false, false)
                .Build();
            Configuration = config;

            
            var services = new ServiceCollection()
                .AddLogging(l => l.AddConsole());

            var cfg = new GraphUtilConfig();
            Configuration.Bind(cfg);
            services.AddSingleton(cfg);
            services.AddSingleton<UserRepo>();
            services.AddSingleton<GroupsRepo>();


            await CommandLineApplication.ExecuteAsync<B2C>(args);
        }
    }
}
