using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using b2c.Data;
using EPS.B2C.IEF;
using EPS.Extensions.B2CGraphUtil.Config;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace b2c.Commands.IEF;

public class Publish: BaseCommand
{
    [Option(ShortName = "p",LongName = "path",Description = "path to the folder containing the policy files")]
    public string Folder { get; set; }
    [Option(ShortName = "f", LongName = "files", Description = "comma-separated list of ordered files for publishing")]
    public string FileSequence { get; set; }
    [Option(ShortName = "e",LongName = "env", Description = "environment name from configuration to deploy into")]
    public string EnvironmentName { get; set; }
    protected readonly IConfiguration config;
    protected readonly GraphUtilConfig guc;

    public Publish(IConfiguration configuration, IOptions<GraphUtilConfig> graphUtilConfig,IConsole iconsole) : base(iconsole)
    {
        config = configuration;
        guc = graphUtilConfig.Value;
    }

    public async Task OnExecuteAsync()
    {
        if (string.IsNullOrEmpty(Folder) || string.IsNullOrEmpty(FileSequence) || string.IsNullOrEmpty(EnvironmentName))
        {
            throw new ArgumentException("path, files and environment are required fields");
        }

        var list = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "client_credentials"),
            new("scope", "https://graph.microsoft.com/.default"),
            new("client_id", guc.AppId),
            new("client_secret", guc.Secret)
        };

        var files = FileSequence.Split(",");
        var envs = new List<Data.Environment>();
        config.GetSection("Environments").Bind(envs);

        var env = envs.FirstOrDefault(x => x.Name == EnvironmentName);
        if (env == null) throw new ArgumentException($"Environment {EnvironmentName} wasn't found in config");
        var client = new HttpClient();
        var res = await client.PostAsync($"https://login.microsoftonline.com/{env.Settings["Tenant"]}/oauth2/v2.0/token",
            new FormUrlEncodedContent(list));
        var json = await JsonDocument.ParseAsync(await res.Content.ReadAsStreamAsync());
        var token = json.Deserialize<Token>();
        

        var xs = new XmlSerializer(typeof(TrustFrameworkPolicy));
        foreach (var file in files)
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

        }
        
    }

    private string formatErr(string errMsg)
    {
        return errMsg.Replace(".The following error", ".\r\n\r\nThe following error");
    }
}