using System;
using System.Threading.Tasks;
using b2c.Commands;
using McMaster.Extensions.CommandLineUtils;


namespace b2c
{
    class Program
    {
        static async Task Main(string[] args) => await CommandLineApplication.ExecuteAsync<B2C>(args);
    }
}
