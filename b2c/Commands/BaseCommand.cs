using McMaster.Extensions.CommandLineUtils;

namespace b2c.Commands
{
    public abstract class BaseCommand
    {
        [Option(ShortName = "vf", LongName = "verboseFormat", Description = "Display full, formatted JSON object")]
        public bool verboseFormat { get; set; }

        [Option(ShortName = "v",LongName = "verbose", Description = "display full JSON object properties")]
        public bool verbose { get; set; }

        [Option(ShortName = "t",LongName = "time", Description = "time the operation")]
        public bool time { get; set; }

    }
}
