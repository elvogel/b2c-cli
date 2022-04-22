using System;
using System.Diagnostics;
using System.Threading.Tasks;
using b2c.Data;
using EPS.Extensions.B2CGraphUtil;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;
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
        
        [Option(ShortName = "first", Description = "User's first name")]
        public string firstName { get; set; }

        [Option(ShortName = "last",Description = "Users' last name")]
        public string lastName { get; set; }

        [Option(ShortName = "display",Description = "the user's display name")]
        public string displayName { get; set; }

        [Option(ShortName = "pass")]
        public string password { get; set; }

        [Option(ShortName = "upn", Description = "username for the tenant")]
        public string userPrincipalName { get; set; }

        public UserRepo Users { get; set; }

        public CreateUser(IOptions<UserRepo> users, IConsole console): base(console)
        {
            Users = users.Value;
        }
        public async Task OnExecute(IConsole console)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                var config = Config.GetConfig();
                console.WriteLine($"creating user {displayName}...");
                var user = await Users.AddUser(firstName, lastName, displayName, password);

                if (verboseFormat)
                {
                    var str = JsonConvert.SerializeObject(user,Formatting.Indented);
                    console.WriteLine(str);
                }
                else if (isVerbose)
                {
                    var str = JsonConvert.SerializeObject(user,Formatting.None);
                    console.WriteLine(str);
                }
                else
                    console.WriteLine($"displayName: {user.DisplayName}, objectId: {user.Id}, userPrincipalName: {user.UserPrincipalName}");

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

        public DeleteUser(IConsole iConsole): base(iConsole){}
        public async Task OnExecute(IConsole iconsole)
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
        public ListUsers(IConsole iConsole): base(iConsole){}
        public async Task OnExecute(IConsole iConsole)
        {
            var sw = Stopwatch.StartNew();
            var config = Config.GetConfig();
            var client = new GraphClient2(config, console);
            console.WriteLine($"getting users...");
            var ret = await client.ListUsers();
            if (verboseFormat)
            {
                var x = JsonConvert.SerializeObject(ret,Formatting.Indented);
                console.WriteLine(ret);
            }
            else if (isVerbose)
            {
                var x = JsonConvert.SerializeObject(ret,Formatting.None);
                console.WriteLine(ret);
            }
            else
            {
                foreach (var user in ret.value)
                {
                    console.WriteLine(
                        $"{user.displayName} objectId: {user.id}, userPrincipalName: {user.userPrincipalName}, mailNickname: {user.displayName}");
                }
            }
            sw.Stop();
            if (!time) return;
            console.WriteLine($"\noperation completed in {sw.ElapsedMilliseconds}ms");
        }
    }
}
