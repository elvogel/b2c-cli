using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Environment = b2c.Data.Environment;

namespace b2c.Commands.IEF;

[Command(Name = "compile", Description = "compile IEF files for all the different Environments")]
public class Compile: BaseCommand
{
    [Option(LongName = "inPath",ShortName = "i",
        Description = "the path containing the XML to compile",ShowInHelpText = true)]
    public string inPath { get; set; }
    
    [Option(LongName = "outPath",ShortName = "o",
        Description = "the destination path for the Environments (existing files will be overwritten",ShowInHelpText = true)]
    public string outPath { get; set; }
    
    
    public Compile(IConsole iConsole) : base(iConsole)
    {
    }

    public async Task OnExecuteAsync()
    {
        OnExecute();
        if (string.IsNullOrEmpty(inPath))
            throw new ArgumentException(nameof(inPath));
        var sw = Stopwatch.StartNew();
        inPath = Path.GetFullPath(inPath);
        verbose($"inbound path: {inPath}");

        if (string.IsNullOrEmpty(outPath))
        {
            outPath = inPath;
            verbose($"outbound path not specified, using {inPath}");
        }
        
        CreateDirectories(envs);

        int i = 0;
        foreach (var e in envs)
        {
            i++;
            await compileFiles(e);
        }
        verbose($"completed processing {i} environments");
        record(sw);
    }

    protected void CreateDirectories(List<Environment> environments)
    {
        foreach (var e in environments)
        {
            if (string.IsNullOrEmpty(e.Name))
                throw new ArgumentException("Every environment needs an environment name!");

            var path = Path.Combine(outPath, e.Name);
            if (Directory.Exists(path)) verbose($"{path} exists");
            else
            {
                verbose("creating path {path}");
                Directory.CreateDirectory(path);
            }
        }
    }

    protected async Task compileFiles(Environment environment)
    {
        var files = Directory.GetFiles(inPath, "*.xml");
        foreach (var file in files)
        {
            var fName = Path.GetFileName(file);
            verbose($"compiling {file}...");
            var txt = await File.ReadAllTextAsync(file);

            var i = 0;
            if (environment.Settings == null)
            {
                write($"no settings detected for {environment.Name}...");
                continue;
            }
            foreach (var setting in environment.Settings)
            {
                var count = txt.Split(setting.Key).Length - 1;
                var s = "{Settings:" + setting.Key + "}";
                verbose($"found {count} instances of {s}");
                if (count <= 0) continue;
                txt = txt.Replace(s, setting.Value);
                i++;
            }

            var outFile = Path.Combine(outPath,environment.Name, fName);
            verbose($"{i} found settings processed, saving {file} to {outFile}");
            await File.WriteAllTextAsync(outFile, txt);
        }
    }
}
