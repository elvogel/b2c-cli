using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace b2c.Commands.IEF;

[Command(Name = "compub", Description = "compile AND publish IEF files")]
public class CompilePublish: BaseCommand
{
    public readonly Publish PublishCommand;
    public readonly Compile CompileCommand;
    public CompilePublish(IConsole iconsole) : base(iconsole)
    {
        CompileCommand = new Compile(iconsole);
        PublishCommand = new Publish(iconsole);
    }

    public async Task OnExecuteAsync()
    {
        CompileCommand.inPath = inPath;
        CompileCommand.outPath = outPath;
        CompileCommand.envName = envName;
        CompileCommand.isVerbose = isVerbose;
        CompileCommand.configPath = configPath;
        
        PublishCommand.Folder = Folder;
        PublishCommand.time = time;
        PublishCommand.configPath = configPath;
        PublishCommand.envName = envName;
        PublishCommand.isVerbose = isVerbose;
        
        try
        {
            await CompileCommand.OnExecuteAsync();
        }
        catch (Exception)
        {
            write("compile command failed.");
        }
        try
        {
            await PublishCommand.OnExecuteAsync();
        }
        catch (Exception)
        {
            write("publish command failed.");
        }
        
    }

    [Option(ShortName = "p", LongName = "path", Description = "path to the folder containing the XML files")]
    public string Folder { get; set; }
    
    [Option(LongName = "inPath",ShortName = "i",
        Description = "the path containing the XML to compile",ShowInHelpText = true)]
    public string inPath { get; set; }
    
    [Option(LongName = "outPath",ShortName = "o",
        Description = "the destination path for the Environments (existing files will be overwritten",ShowInHelpText = true)]
    public string outPath { get; set; }
}