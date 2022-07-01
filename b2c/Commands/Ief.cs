using b2c.Commands.IEF;
using McMaster.Extensions.CommandLineUtils;

namespace b2c.Commands;

[Command("ief", Description = "Identity Experience Framework (IEF) commands")]
[Subcommand(typeof(ValidateSchema), typeof(Compile), typeof(Publish), typeof(CompilePublish))]
class Ief
{
    public void OnExecute(IConsole console)
    {
        console.Error.WriteLine("You must specify a command. See --help for details.");
    }    
}
