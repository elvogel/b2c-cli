using System;
using System.Threading.Tasks;
using b2c.Commands;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.HelpText;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace b2c
{
    
    [Command(Name = "b2c",Description = "Azure AD Graph Commands for B2C directories.")]
    [Subcommand(typeof(Users), typeof(Groups), typeof(Ief))]
    class B2C
    {
        public static IConfiguration Configuration { get; set; }

        static async Task Main(string[] args) => await CommandLineApplication.ExecuteAsync<B2C>(args);

        public int OnExecute(CommandLineApplication app, IConsole console)
        {
            var oc = Console.ForegroundColor;
            console.ForegroundColor = ConsoleColor.Yellow;
            console.WriteLine("B2C command-line app");
            console.WriteLine("Endpoint Systems");
            console.WriteLine("https://endpointsystems.com");
            console.WriteLine("---------------------------");
            console.ForegroundColor = oc;

            console.WriteLine("you must specify a command.");
            app.ShowHelp();
            return 1;
        }
    }
}
