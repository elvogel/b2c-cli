using System;
using System.Diagnostics;
using System.Threading.Tasks;
using EPS.Extensions.B2CGraphUtil;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace b2c.Commands;


    [Command("users", Description = "user commands")]
    [Subcommand(typeof(CreateUser),typeof(DeleteUser), typeof(ListUsers))]
    class Users
    {
        private static Task OnExecute()
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
        [Option(ShortName = "vf", LongName = "verboseFormat", Description = "Display full, formatted JSON object")]
        public bool verboseFormat { get; set; }

        public UserRepo Users { get; set; }

        public CreateUser(IOptions<UserRepo> users, IConsole console): base(console)
        {
            Users = users.Value;
        }
        public async Task OnExecute()
        {
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
                else if (isVerbose)
                {
                    var str = JsonConvert.SerializeObject(user,Formatting.None);
                    console.WriteLine(str);
                }
                else
                    console.WriteLine($"displayName: {user.DisplayName}, objectId: {user.Id}, userPrincipalName: {user.UserPrincipalName}");

                record(sw);
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

        private readonly UserRepo users;
        public DeleteUser(IOptions<UserRepo> userRepo, IConsole iConsole) : base(iConsole)
        {
            users = userRepo.Value;
        }
        public async Task OnExecute(IConsole iconsole)
        {
            var sw = Stopwatch.StartNew();
            verbose("deleting user {userId}...");
            await users.DeleteUser(userId);
            verbose($"user {userId} deleted.");
            record(sw);
        }
    }

    [Command(Name = "list", Description = "list all users")]
    class ListUsers : BaseCommand
    {
        private readonly UserRepo users;

        public ListUsers(IOptions<UserRepo> userRepo, IConsole iConsole) : base(iConsole)
        {
            users = userRepo.Value;
        }

        [Option(ShortName = "csv", Description = "output key values in CSV format")]
        public bool Csv { get; set; }

        [Option(ShortName = "h", LongName = "head", Description = "add a header to CSV output")]
        public bool Head { get; set; }

        [Option(ShortName = "json", Description = "output object list in JSON format")]
        public bool Json { get; set; }

        public async Task OnExecute()
        {
            var sw = Stopwatch.StartNew();
            if (!Json && !Csv) write("getting users...");
            var ret = await users.GetAllUsers();

            if (Json)
            {
                var x = JsonConvert.SerializeObject(ret, Formatting.Indented);
                console.WriteLine(x);
            }
            else if (Csv)
            {
                if (Head) console.WriteLine("oid,upn,mailNickname");

                foreach (var user in ret)
                {
                    console.WriteLine($"{user.Id},{user.UserPrincipalName},{user.MailNickname}");
                }
            }
            else
            {
                foreach (var user in ret)
                {
                    write(
                        $"{user.DisplayName} objectId: {user.Id}, userPrincipalName: {user.UserPrincipalName}, mailNickname: {user.MailNickname}");
                }
            }

            if (!Csv && !Json) record(sw);
        }

    }
