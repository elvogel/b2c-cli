using System;
using System.IO;
using System.Text;
using McMaster.Extensions.CommandLineUtils;

namespace b2c.Commands
{
    [Command("configure",Description = "configure the graph settings")]
    public class Configure
    {
        [Option(ShortName = "appId",
            Description = "The application ID of the application within the Azure AD B2C directory")]
        public string AppId { get; set; }
        [Option(ShortName = "tenant",
        Description = "The tenant id, i.e. <tenantname>.onmicrosoft.com")]
        public string Tenant { get; set; }
        [Option(ShortName = "secret",Description = "The application secret")]
        public string Secret { get; set; }

        private void OnExecute(IConsole console)
        {
            var destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "b2c.json");
            var sb = new StringBuilder();
            var tn = Tenant.Contains("onmicrosoft.com") ? Tenant : Tenant + ".onmicrosoft.com";
            sb.AppendLine("{");
            sb.AppendLine($"\"AppId\": \"{AppId}\",");
            sb.AppendLine($"\"Tenant\": \"{Tenant}\",");
            sb.AppendLine($"\"Secret\": \"{Secret}\"");
            sb.AppendLine("}");
            File.WriteAllText(destPath,sb.ToString());
            console.WriteLine($"Graph configuration successfully written to {destPath}");
        }
    }
}
