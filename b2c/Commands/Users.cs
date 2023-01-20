using System;
using System.Diagnostics;
using System.Threading.Tasks;
using EPS.Extensions.B2CGraphUtil;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace b2c.Commands;


[Command("users", Description = "user commands")]
[Subcommand(typeof(CreateUser), typeof(DeleteUser), typeof(ListUsers))]
class Users
{
    public Task OnExecuteAsync()
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
        
        [Option(ShortName = "vf",Description = "display the user object in JSON")]
        public bool verboseFormat { get; set; }
        
        [Option(ShortName = "csv",Description = "display user object details in CSV format")]
        public bool CSV { get; set; }
        
        [Option(ShortName = "he",LongName = "header",Description = "display the header for CSV output")]
        public bool csvHeader { get; set; }

        [Option(ShortName = "pass")]
        public string password { get; set; }

        [Option(ShortName = "upn", Description = "username for the tenant")]
        public string userPrincipalName { get; set; }

        public UserRepo Users { get; set; }

        public CreateUser(IConsole console): base(console) { }
        public async Task OnExecuteAsync()
        {
            Execute();
            try
            {
                var sw = Stopwatch.StartNew();
                verbose($"creating user {displayName}...");
                var user = await Users.AddUser(firstName, lastName, displayName, password);

                if (verboseFormat)
                {
                    var str = JsonConvert.SerializeObject(user,Formatting.Indented);
                    console.WriteLine(str);
                }
                else if (CSV)
                {
                    if (csvHeader)
                        console.WriteLine("objectId,displayName,upn");
                    console.WriteLine($"{user.Id},{user.DisplayName},{user.UserPrincipalName}");
                }
                else
                    console.WriteLine($"displayName: {user.DisplayName}, objectId: {user.Id}, userPrincipalName: {user.UserPrincipalName}");

                record(sw);
            }
            catch (Exception ex)
            {
                write($"{ex.GetType()}: {ex.Message}");
            }
        }
    }

    [Command(Name="delete", Description = "delete the user")]
    class DeleteUser: BaseCommand
    {
        [Option(Description = "The user ID (guid).")]
        public string userId { get; set; }
        
        [Option(ShortName = "a", Description = "delete all users")]
        public bool allUsers { get; set; }

        public DeleteUser(IConsole iConsole) : base(iConsole) { }
        public async Task OnExecuteAsync()
        {
            Execute();
            var sw = Stopwatch.StartNew();

            if (allUsers)
            {
                var list = await users.GetAllUsers();
                verbose($"deleting {list.Count} users...");
                foreach (var user in list)
                {
                    if (user.UserPrincipalName.Contains("endpoint"))
                    {
                        write("----------SKIPPING ENDPOINT SYSTEMS----------");
                        continue;
                    }
                    verbose($"deleting {user.UserPrincipalName}");
                    await users.DeleteUser(user.Id);
                }

                record(sw);
                return;
            }

            if (string.IsNullOrEmpty(userId))
            {
                write("must provide userId to delete!");
                return;
            }
            verbose("deleting user {userId}...");
            await users.DeleteUser(userId);
            verbose($"user {userId} deleted.");
            record(sw);
        }
    }

    [Command(Name = "list", Description = "list all users")]
    class ListUsers : BaseCommand
    {
        public ListUsers(IConsole iConsole) : base(iConsole) { }

        [Option(ShortName = "csv", Description = "output key values in CSV format")]
        public bool Csv { get; set; }

        [Option(ShortName = "h", LongName = "head", Description = "add a header to CSV output")]
        public bool Head { get; set; }

        [Option(ShortName = "json", Description = "output object list in JSON format")]
        public bool Json { get; set; }

        public async Task OnExecuteAsync()
        {
            Execute();
            var sw = Stopwatch.StartNew();
            if (!Json && !Csv) write("getting users...");
            var ret = await users.GetAllUsers();
            base.write($"found {ret.Count} users.");
            if (Json)
            {
                var x = JsonConvert.SerializeObject(ret, Formatting.Indented);
                console.WriteLine(x);
            }
            else if (Csv)
            {
                if (Head) console.WriteLine("oid,upn,displayName,mailNickname");

                foreach (var user in ret)
                {
                    console.WriteLine($"{user.Id},{user.UserPrincipalName},{user.DisplayName},{user.MailNickname}");
                }
            }
            else
            {
                foreach (var user in ret)
                {
                    write(
                        $"{user.DisplayName} objectId: {user.Id}, userPrincipalName: {user.UserPrincipalName},displayName: {user.DisplayName}, mailNickname: {user.MailNickname}");
                }
            }

            if (!Csv && !Json) record(sw);
        }

    }
