using System;
using System.Threading.Tasks;
using b2c.Commands;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;


namespace b2c
{
    
    
    [Command(Name = "b2c",Description = "Azure AD Graph Commands for B2C directories.")]
    [Subcommand(typeof(Users), typeof(Groups), typeof(Ief))]
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        static async Task<int> Main(string[] args)
        {
            return await CommandLineApplication.ExecuteAsync<Program>(args);
            /*
            var app = new CommandLineApplication<B2C>();
            app.Conventions
                .UseDefaultConventions()
                .UseArgumentAttributes()
                ;

            app.Parse(args);
            await app.ExecuteAsync(args);
            */
        }

        public int OnExecute(CommandLineApplication app, IConsole console)
        {
            var oc = Console.ForegroundColor;
            console.ForegroundColor = ConsoleColor.Yellow;
            console.WriteLine("B2C command-line app");
            console.WriteLine("Endpoint Systems");
            console.WriteLine("https://endpointsystems.com");
            console.WriteLine("---------------------------");
            console.ForegroundColor = oc;

            console.Error.WriteLine("you must specify a command.");
            app.ShowHelp();
            return 1;
        }
    }
}
