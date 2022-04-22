using System.Diagnostics;
using System.Threading.Tasks;
using EPS.Extensions.B2CGraphUtil;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;

namespace b2c.Commands
{
    [Command("groups", Description = "operations on groups")]
    [Subcommand(typeof(ListGroups), typeof(AddUserToGroup))]
    class Groups
    {
        //the command for the subcommands
        protected Task OnExecute()
        {
            return Task.CompletedTask;
        }

    }
    [Command("list",Description = "list all available groups")]
    class ListGroups: BaseCommand
    {
        public GroupsRepo Groups { get; set; }
        public ListGroups(IOptions<GroupsRepo> groups, IConsole console): base(console)
        {
            Groups = groups.Value;
        }
        public async Task OnExecute()
        {
            var sw = Stopwatch.StartNew();
            console.WriteLine("getting groups...");
            var groups = await Groups.GetAllGroups();

            foreach (var g in groups)
            {
                console.WriteLine($"{g.Id},{g.DisplayName}");
            }
            sw.Stop();
            if (!time) return;
            console.WriteLine($"operation completed in {sw.ElapsedMilliseconds} ms");
        }
    }


    [Command("user", Description = "Add user to a group")]
    class AddUserToGroup
    {
        [Option(ShortName = "g", LongName = "group", Description = "the group's object ID")]
        public string GroupId { get; set; }

        [Option(ShortName = "u", LongName = "user", Description = "the user's object ID")]
        public string UserId { get; set; }

        public UserRepo Users { get; set; }

        public AddUserToGroup(IOptions<UserRepo> users)
        {
            Users = users.Value;
        }

    public async Task OnExecute(IConsole console)
        {
            var sw = Stopwatch.StartNew();
            console.WriteLine("adding user to group...");
            await Users.AddToGroup(UserId, GroupId);
            sw.Stop();
            console.WriteLine($"operation completed in {sw.ElapsedMilliseconds}ms");


        }
    }
}
