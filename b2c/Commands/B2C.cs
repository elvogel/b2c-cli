using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace b2c.Commands;

[Command(Name = "b2c",Description = "Azure AD Graph Commands for B2C directories.")]
[Subcommand(typeof(Users), typeof(Groups), typeof(Ief))]
public class B2C
{
    private const string help = "-?|-h|--help";
    public Task OnExecuteAsync()
    {
        return Task.CompletedTask;
    }
}

