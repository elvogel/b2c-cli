using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Environment = b2c.Data.Environment;

namespace b2c.Commands;

[Command(Name = "compile", Description = "compile IEF files for all the different Environments")]
public class Compile: BaseCommand
{
    private readonly IConfiguration config;

    [Option(LongName = "inPath",ShortName = "i",
        Description = "the path containing the XML to compile",ShowInHelpText = true)]
    public string inPath { get; set; }
    
    [Option(LongName = "outPath",ShortName = "o",
        Description = "the destination path for the Environments (existing files will be overwritten",ShowInHelpText = true)]
    public string outPath { get; set; }
    
    
    public Compile(IConfiguration configuration, IConsole iConsole) : base(iConsole)
    {
        config = configuration;
    }

    public async Task OnExecute()
    {
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
        
        var envs = new List<Environment>();
        config.GetSection("Environments").Bind(envs);

        CreateDirectories(envs);

        int i = 0;
        foreach (var env in envs)
        {
            i++;
            await compileFiles(env);
        }
        verbose($"completed processing {i} environments");
        record(sw);
    }

    protected void CreateDirectories(List<Data.Environment> envs)
    {
        foreach (var env in envs)
        {
            if (string.IsNullOrEmpty(env.Name))
                throw new ArgumentException("Every environment needs an environment name!");

            var path = Path.Combine(outPath, env.Name);
            if (Directory.Exists(path)) verbose($"{path} exists");
            else
            {
                verbose("creating path {path}");
                Directory.CreateDirectory(path);
            }
        }
    }

    protected async Task compileFiles(Environment env)
    {
        var files = Directory.GetFiles(inPath, "*.xml");
        foreach (var file in files)
        {
            var fName = Path.GetFileName(file);
            verbose($"compiling {file}...");
            var txt = await File.ReadAllTextAsync(file);

            var i = 0;
            foreach (var setting in env.Settings)
            {
                var count = txt.Split(setting.Key).Length - 1;
                var s = "{Settings:" + setting.Key + "}";
                verbose($"found {count} instances of {s}");
                if (count <= 0) continue;
                txt = txt.Replace(s, setting.Value);
                i++;
            }

            var outFile = Path.Combine(outPath,env.Name, fName);
            verbose($"{i} found settings processed, saving {file} to {outFile}");
            await File.WriteAllTextAsync(outFile, txt);
        }
    }
}
