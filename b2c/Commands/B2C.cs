using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace b2c.Commands
{
    [Command(Name = "b2c",Description = "Azure AD Graph Commands for B2C directories.")]
    [Subcommand(typeof(Users),typeof(Configure))]
    public class B2C
    {
        private const string help = "-?|-h|--help";




    }
}
