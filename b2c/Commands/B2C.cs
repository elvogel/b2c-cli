using McMaster.Extensions.CommandLineUtils;

namespace b2c.Commands
{
    [Command(Name = "b2c",Description = "Azure AD Graph Commands for B2C directories.")]
    [Subcommand(typeof(Users),typeof(Configure), typeof(Groups), typeof(Ief))]
    public class B2C
    {
        private const string help = "-?|-h|--help";
    }
}
