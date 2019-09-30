using System;
using System.Threading.Tasks;
using b2c.Data;
using McMaster.Extensions.CommandLineUtils;

namespace b2c.Commands
{
    [Command("users", Description = "user commands")]
    [Subcommand(typeof(CreateUser),typeof(DeleteUser))]
    class Users
    {
        private async Task OnExecute(IConsole console)
        {
        }
    }

    [Command(Name="create",Description = "create the user")]
    class CreateUser
    {
        [Option(ShortName = "display",Description = "the user's display name")]
        public string displayName { get; set; }

        [Option(ShortName = "pass")]
        public string password { get; set; }

        [Option(ShortName = "upn")]
        public string userPrincipalName { get; set; }

        private async Task OnExecute(IConsole console)
        {
            try
            {
                var config = Config.GetConfig();
                var client = new GraphClient(config, console);
                console.WriteLine($"creating user {displayName}...");
                var str = await client.CreateUser(displayName, userPrincipalName, password);
                console.WriteLine(str);
            }
            catch (Exception ex)
            {
                console.WriteLine($"A {ex.GetType()} was caught: {ex.Message}");
            }
        }
    }

    [Command(Name="delete", Description = "delete the user")]
    class DeleteUser
    {
        [Option(Description = "The user ID (guid).")]
        public string userId { get; set; }

        private async Task OnExecute(IConsole console)
        {
            var config = Config.GetConfig();
            var client = new GraphClient(config, console);
            console.WriteLine($"deleting user {userId}...");
            var ret = await client.DeleteUser(userId);
            console.WriteLine(ret);
        }
    }
}
