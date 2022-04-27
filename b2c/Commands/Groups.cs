using System.Diagnostics;
using System.Threading.Tasks;
using EPS.Extensions.B2CGraphUtil;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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
        
        [Option(ShortName = "csv", Description = "output key values in CSV format")]
        public bool Csv { get; set; }

        [Option(ShortName = "h", LongName = "head", Description = "add a header to CSV output")]
        public bool Head { get; set; }

        [Option(ShortName = "json", Description = "output object list in JSON format")]
        public bool Json { get; set; }

        public ListGroups(IOptions<GroupsRepo> groups, IConsole console): base(console)
        {
            Groups = groups.Value;
        }
        public async Task OnExecute()
        {
            var sw = Stopwatch.StartNew();
            if (!Csv && !Json) write("getting groups...");
            var groups = await Groups.GetAllGroups();
            
            if (Csv)
            {
                if (Head) console.WriteLine($"oid,displayName,mail,description");
                foreach (var group in groups)
                {
                    console.WriteLine($"{group.Id},{group.DisplayName},{group.Mail},{group.Description}");
                }
            }
            else if (Json)
            {
                var x = JsonConvert.SerializeObject(groups, Formatting.Indented);
                console.WriteLine(x);
            }
            else
            {
                foreach (var g in groups)
                {
                    console.WriteLine($"{g.Id},{g.DisplayName}");
                }
            }
         
            if (!Csv && !Json) record(sw);
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

    [Command("user", Description = "Add user to a group")]
    class ListGroupMembers: BaseCommand
    {
        private readonly GroupsRepo groups;
        public ListGroupMembers(IOptions<GroupsRepo> groupsRepo, IConsole iConsole) : base(iConsole)
        {
            groups = groupsRepo.Value;
        }

        [Option(ShortName = "g", LongName = "group", Description = "the group's object ID")]
        public string GroupId { get; set; }
        
        public async Task Execute()
        {
            var sw = Stopwatch.StartNew();
            var g = await groups.GetGroup(GroupId);
            verbose($"group {g.DisplayName} has {g.Members.Count} members:");
            
            foreach (var member in g.Members)
            {
                write(member.Id);
            }

            record(sw);
        }
    }
}
