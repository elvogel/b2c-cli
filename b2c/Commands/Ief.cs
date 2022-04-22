using McMaster.Extensions.CommandLineUtils;

namespace b2c.Commands;

[Command("ief", Description = "Identity Experience Framework (IEF) commands")]
[Subcommand(typeof(ValidateSchema), typeof(Compile))]
public class Ief
{
    
}
