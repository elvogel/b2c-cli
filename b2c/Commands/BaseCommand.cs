using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using EPS.Extensions.B2CGraphUtil;
using EPS.Extensions.B2CGraphUtil.Config;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

namespace b2c.Commands
{
    public abstract class BaseCommand
    {
        [Option(ShortName = "c",LongName = "cfg",Description = "config path")]
        public string configPath { get; set; }
        
        [Option(ShortName = "v", LongName = "isVerbose", Description = "verbose output")]
        public bool isVerbose { get; set; }

        [Option(ShortName = "t", LongName = "time", Description = "put the timestamp in the output")]
        public bool time { get; set; }

        [Option(ShortName = "e", LongName = "environment", Inherited = true,
            Description = "the environment (from configuration) to publish into")]
        public string envName { get; set; }

        protected IConsole console { get; set; }
        
        protected UserRepo users { get; set; }
        
        protected GroupsRepo groups { get; set; }
        protected IConfiguration config;

        protected List<Data.Environment> envs;
        protected Data.Environment env;
        protected GraphUtilConfig guc;

        protected BaseCommand(IConsole iconsole)
        {
            var cfgPath = configPath ?? Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "b2c.json");
            if (!File.Exists(cfgPath))
                throw new ArgumentException($"config file {cfgPath} does not exist.");
            config = new ConfigurationBuilder()

                .AddJsonFile(cfgPath, false, false)
                .Build();
           
            console = iconsole;
        }

        protected void Execute()
        {
            if (string.IsNullOrEmpty(envName))
                throw new ArgumentException("Environment name need to be set!");

            envs = new List<Data.Environment>();
            config.GetSection("Environments").Bind(envs);

            env = envs.FirstOrDefault(x => x.Name.ToLower().Equals(envName.ToLower()));

            guc = env ?? throw new ArgumentException($"environment '{envName}' is not found in config");

            users = new UserRepo(guc);
            groups = new GroupsRepo(guc);

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
