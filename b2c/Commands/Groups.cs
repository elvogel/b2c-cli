using System;
using System.Diagnostics;
using System.Threading.Tasks;
using EPS.Extensions.B2CGraphUtil;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace b2c.Commands
{
    [Command("groups", Description = "operations on groups")]
    [Subcommand(typeof(ListGroups), typeof(AddUserToGroup), 
        typeof(ListGroupMembers), typeof(CreateGroup), typeof(DeleteGroup))]
    class Groups
    {
        //the command for the subcommands
        public void OnExecute(IConsole console)
        {
            console.Error.WriteLine("You must specify a command. See --help for details.");
        }

    }
    
    [Command("list",Description = "list all available groups")]
    class ListGroups: BaseCommand
    {
        [Option(ShortName = "csv", Description = "output key values in CSV format")]
        public bool Csv { get; set; }

        [Option(ShortName = "h", LongName = "head", Description = "add a header to CSV output")]
        public bool Head { get; set; }

        [Option(ShortName = "json", Description = "output object list in JSON format")]
        public bool Json { get; set; }

        public ListGroups(IConsole console): base(console) {}
        public async Task OnExecuteAsync()
        {
            var sw = Stopwatch.StartNew();
            RepoInit();
            if (!Csv && !Json) write("getting groups...");
            var groupList = await groups.GetAllGroups();
            if (groupList.Count == 0)
            {
                write("no groups");
            }
            else if (Csv)
            {
                if (Head) console.WriteLine($"oid,displayName,mail,description");
                foreach (var group in groupList)
                {
                    console.WriteLine($"{group.Id},{group.DisplayName},{group.Mail},{group.Description}");
                }
            }
            else if (Json)
            {
                var x = JsonConvert.SerializeObject(groupList, Formatting.Indented);
                console.WriteLine(x);
            }
            else
            {
                foreach (var g in groupList)
                {
                    console.WriteLine($"{g.Id},{g.DisplayName}");
                }
            }
         
            if (!Csv && !Json) record(sw);
        }
    }

    [Command("create",Description = "create a new group")]
    class CreateGroup : BaseCommand
    {
        [Option(ShortName = "d",LongName = "desc", Description = "group description")]
        public string Description { get; set; }

        public CreateGroup(IConsole iconsole) : base(iconsole)
        {
        }
        [Option(ShortName = "n",Description = "group name",LongName = "groupName")]
        public string GroupName { get; set; }

        public async Task OnExecuteAsync()
        {
            var sw = Stopwatch.StartNew();
            RepoInit();
            var ret = await groups.CreateGroup(GroupName);
            write(ret.Id);
            record(sw);
        }
    }

    [Command("delete",Description = "delete a group")]
    class DeleteGroup: BaseCommand
    {
        [Option(ShortName = "id",Description = "The group objectId.")]
        public string Id { get; set; }
        public async Task OnExecuteAsync()
        {
            var sw = Stopwatch.StartNew();
            RepoInit();
            await groups.DeleteGroup(Id);
            record(sw);
        }

        public DeleteGroup(IConsole iconsole) : base(iconsole)
        {
        }
    }

    [Command("add", Description = "Add user to a group")]
    class AddUserToGroup: BaseCommand
    {
        [Option(ShortName = "g", LongName = "gid", Description = "the group's object ID")]
        public string GroupId { get; set; }

        [Option(ShortName = "u", LongName = "uid", Description = "the user's object ID")]
        public string UserId { get; set; }

        public UserRepo Users { get; set; }

        public AddUserToGroup(IConsole iConsole): base(iConsole) { }

    public async Task OnExecuteAsync()
        {
            var sw = Stopwatch.StartNew();
            RepoInit();
            if (string.IsNullOrEmpty(GroupId) || string.IsNullOrEmpty(UserId))
                throw new ArgumentException("need groupId and userId");
            verbose($"adding user {UserId} to group {GroupId}...");
            await Users.AddToGroup(UserId, GroupId);
            record(sw);
        }
    }

    [Command("listUsers", Description = "List users in a group")]
    class ListGroupMembers: BaseCommand
    {
        public ListGroupMembers(IConsole iConsole) : base(iConsole) { }

        [Option(ShortName = "g", LongName = "group", Description = "the group's object ID")]
        public string GroupId { get; set; }
        
        public async Task OnExecuteAsync()
        {
            var sw = Stopwatch.StartNew();
            RepoInit(); 
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
