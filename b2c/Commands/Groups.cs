using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using b2c.Data;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace b2c.Commands
{
    [Command("groups", Description = "operations on groups")]
    [Subcommand(typeof(ListGroups), typeof(AddUserToGroup))]
    class Groups
    {
        //the command for the subcommands
        private Task OnExecute(IConsole console)
        {
            return Task.CompletedTask;
        }

    }
    [Command("list",Description = "list all available groups")]
    class ListGroups: BaseCommand
    {
        public async Task OnExecute(IConsole console)
        {
            var sw = Stopwatch.StartNew();
            var config = Config.GetConfig();
            var client = new GraphClient(config, console);
            console.WriteLine("getting groups...");
            var x = await client.ListGroups();

            if (verbose)
            {
                var y = JsonConvert.SerializeObject(x);
                console.WriteLine(y);
            }
            else if (verboseFormat)
            {
                var y = JsonConvert.SerializeObject(x,Formatting.Indented);
                console.WriteLine(y);
            }
            else
            {
                foreach (var g in x.value)
                {
                    console.WriteLine($"{g.displayName} {g.id}");
                }
            }

            sw.Stop();
            if (!time) return;
            console.WriteLine($"operation completed in {sw.ElapsedMilliseconds}ms");
        }
    }


    [Command("user",Description = "Add user to a group")]
    class AddUserToGroup
    {
        [Option(ShortName = "g",LongName = "group",Description = "the group's object ID")]
        public string GroupId { get; set; }

        [Option(ShortName = "u",LongName = "user",Description = "the user's object ID")]
        public string UserId { get; set; }

        public async Task OnExecute(IConsole console)
        {
            var sw = Stopwatch.StartNew();
            var config = Config.GetConfig();
            var client = new GraphClient(config, console);
            console.WriteLine("adding user to group...");
            var ret = await client.AddUserToGroup(UserId, GroupId);
            sw.Stop();
            console.WriteLine($"operation completed in {sw.ElapsedMilliseconds}ms");


        }
    }
}
