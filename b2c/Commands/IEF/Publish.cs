using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using b2c.Data;
using McMaster.Extensions.CommandLineUtils;

namespace b2c.Commands.IEF;

[Command(Name = "publish", Description = "publish policy files (in the sequence given) the specified environment")]
public class Publish : BaseCommand
{
    [Option(ShortName = "p", LongName = "path", Description = "path to the folder containing the XML files")]
    public string Folder { get; set; }

    [Option(ShortName = "s", LongName = "sequence",
        Description = "a comma-separated, sequential list of file names for uploading")]
    public string FileSequence { get; set; }


    public Publish(IConsole iconsole) : base(iconsole) { }

    public async Task OnExecuteAsync()
    {
        Execute();
        if (string.IsNullOrEmpty(Folder) || string.IsNullOrEmpty(FileSequence) || string.IsNullOrEmpty(envName))
            throw new ArgumentException("need directory path, environment name and file sequence!");

        var tenant = env.Settings["Tenant"];
        if (string.IsNullOrEmpty(tenant))
            throw new ArgumentException($"Tenant setting needs to be specified in {envName} environment!");

        var sw = Stopwatch.StartNew();
        if (!tenant.Contains(".onmicrosoft.com")) tenant += ".onmicrosoft.com";
        var clientId = guc.AppId;
        var secret = guc.Secret;

        var seq = FileSequence.Split(",");

        var list = new List<KeyValuePair<string, string>>
        {
            new("scope", "https://graph.microsoft.com/.default"),
            new("grant_type", "client_credentials"),
            new("client_id", clientId),
            new("client_secret", secret)
        };
        var client = new HttpClient();
        var tr = await client.PostAsync($"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token",
            new FormUrlEncodedContent(list));
        var json = await JsonDocument.ParseAsync(await tr.Content.ReadAsStreamAsync());
        var token = json.Deserialize<Token>();
        var xs = new XmlSerializer(typeof(TrustFrameworkPolicy));
        foreach (var file in seq)
        {
            var fpath = Path.Combine(Folder, file);
            if (!File.Exists(fpath))
            {
                write($"path {fpath} does not exist");
                continue;
            }

            await using var stm = File.OpenRead(fpath);
            stm.Seek(0, SeekOrigin.Begin);
            var policy = xs.Deserialize(stm) as TrustFrameworkPolicy;
            if (policy == null)
            {
                write($"failed to deserialize policy from {fpath}");
                continue;
            }

            var policyId = policy.PolicyId;
            write($"uploading policy {policyId}...");
            stm.Seek(0, SeekOrigin.Begin);

            using var requestMsg = new HttpRequestMessage(HttpMethod.Put,
                $"https://graph.microsoft.com/beta/trustframework/policies/{policyId}/$value");
            requestMsg.Content = new StreamContent(stm);
            requestMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            requestMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token!.accessToken);
            //requestMsg.Headers.Add("Content-Type", "application/xml");
            var rsp = await client.SendAsync(requestMsg);
            if (!rsp.IsSuccessStatusCode)
            {
                json = await JsonDocument.ParseAsync(await rsp.Content.ReadAsStreamAsync());
                var err = json.Deserialize<PolicyError>();
                write("---");
                write($"{rsp.StatusCode}: {rsp.ReasonPhrase}");
                write($"Error Code: {err!.Error.Code}");
                write(formatErr(err.Error.Message));
                write("---");
                write("command canceled due to errors.");
                return;
            }
            else
            {
                write($"{policyId} published successfully");
            }
        }

        record(sw);
    }

    private string formatErr(string errMsg)
    {
        return errMsg.Replace(".The following error", ".\r\n\r\nThe following error");
    }

}
