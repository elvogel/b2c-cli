using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using b2c.Data;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace b2c.Commands
{
    [Command("users", Description = "user commands")]
    [Subcommand(typeof(CreateUser),typeof(DeleteUser), typeof(ListUsers))]
    class Users
    {
        private Task OnExecute(IConsole console)
        {
            return Task.CompletedTask;
        }
    }

    [Command(Name="create",Description = "create the user")]
    class CreateUser: BaseCommand
    {
        [Option(ShortName = "display",Description = "the user's display name")]
        public string displayName { get; set; }

        [Option(ShortName = "pass")]
        public string password { get; set; }

        [Option(ShortName = "upn", Description = "username for the tenant")]
        public string userPrincipalName { get; set; }

        public async Task OnExecute(IConsole console)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var config = Config.GetConfig();
                var client = new GraphClient(config, console);
                console.WriteLine($"creating user {displayName}...");
                var user = await client.CreateUser(displayName, userPrincipalName, password);

                if (verboseFormat)
                {
                    var str = JsonConvert.SerializeObject(user,Formatting.Indented);
                    console.WriteLine(str);
                }
                else if (verbose)
                {
                    var str = JsonConvert.SerializeObject(user,Formatting.None);
                    console.WriteLine(str);
                }
                else
                    console.WriteLine($"displayName: {user["displayName"]}, objectId: {user["objectId"]}, userPrincipalName: {user["userPrincipalName"]}");

                if (!time) return;
                sw.Stop();
                console.WriteLine($"operation completed in {sw.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                console.WriteLine($"A {ex.GetType()} was caught: {ex.Message}");
            }
        }
    }

    [Command(Name="delete", Description = "delete the user")]
    class DeleteUser: BaseCommand
    {
        [Option(Description = "The user ID (guid).")]
        public string userId { get; set; }

        public async Task OnExecute(IConsole console)
        {
            var config = Config.GetConfig();
            var client = new GraphClient(config, console);
            console.WriteLine($"deleting user {userId}...");
            var ret = await client.DeleteUser(userId);
            console.WriteLine(ret);
        }
    }

    [Command(Name="list", Description = "list all users")]
    class ListUsers : BaseCommand
    {
        public async Task OnExecute(IConsole console)
        {
            var sw = Stopwatch.StartNew();
            var config = Config.GetConfig();
            var client = new GraphClient(config, console);
            console.WriteLine($"getting users...");
            var ret = await client.ListUsers();
            if (verboseFormat)
            {
                var x = JsonConvert.SerializeObject(ret,Formatting.Indented);
                console.WriteLine(ret);
            }
            else if (verbose)
            {
                var x = JsonConvert.SerializeObject(ret,Formatting.None);
                console.WriteLine(ret);
            }
            else
            {
                foreach (var user in ret.value)
                {
                    console.WriteLine(
                        $"{user.displayName} objectId: {user.objectId}, userPrincipalName: {user.userPrincipalName}, mailNickname: {user.mailNickname}");
                }
            }
            sw.Stop();
            if (!time) return;
            console.WriteLine($"\noperation completed in {sw.ElapsedMilliseconds}ms");
        }
    }
}
