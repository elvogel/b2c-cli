using System;
using System.Diagnostics;
using McMaster.Extensions.CommandLineUtils;

namespace b2c.Commands
{
    public abstract class BaseCommand
    {
        [Option(ShortName = "vf", LongName = "verboseFormat", Description = "Display full, formatted JSON object")]
        public bool verboseFormat { get; set; }

        [Option(ShortName = "v", LongName = "isVerbose", Description = "display full JSON object properties")]
        public bool isVerbose { get; set; }

        [Option(ShortName = "t", LongName = "time", Description = "put the timestamp in the output")]
        public bool time { get; set; }

        protected IConsole console { get; set; }

        protected BaseCommand(IConsole iconsole)
        {
            console = iconsole;
        }

        protected void record(Stopwatch sw)
        {
            sw.Stop();
            if (time) write($"command completed in {sw.ElapsedMilliseconds} ms");
        }

        protected void verbose(string line)
        {
            if (isVerbose) write(line);
        }
        
        protected void write(string line)
        {
            console.WriteLine(time ? $"{DateTime.Now:hh:mm:ss:fff}\t{line}" : line);
        }
    }
}
