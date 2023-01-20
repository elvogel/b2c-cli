using System.Threading.Tasks;
using b2c.Commands.IEF;
using McMaster.Extensions.CommandLineUtils;

namespace b2c.Commands;

[Command("ief", Description = "Identity Experience Framework (IEF) commands")]
[Subcommand(typeof(ValidateSchema), typeof(Compile), typeof(Publish))]
public class Ief
{
    public Task OnExecuteAsync()
    {
        return Task.CompletedTask;
    }
    
}
