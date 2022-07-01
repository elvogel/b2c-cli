using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using b2c.Data;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Graph;
using Directory = System.IO.Directory;
using File = System.IO.File;
using TrustFrameworkPolicy = b2c.Data.TrustFrameworkPolicy;

namespace b2c.Commands.IEF;

[Command(Name = "publish", Description = "publish policy files (in the sequence given) the specified environment")]
public class Publish : BaseCommand
{
    [Option(ShortName = "p", LongName = "path", Description = "path to the folder containing the XML files")]
    public string Folder { get; set; }

    public Publish(IConsole iconsole) : base(iconsole)
    {
    }
    
    public string[] FileSequence { get; set; }

    public async Task OnExecuteAsync()
    {
        verbose("reading config data...");
        EnvInit();
        if (string.IsNullOrEmpty(envName))
            throw new ArgumentException("Environment name need to be set!");

        if (string.IsNullOrEmpty(Folder) || string.IsNullOrEmpty(envName))
            throw new ArgumentException("need directory path, environment name!");

      
        var tenant = env.Settings["Tenant"];
        if (string.IsNullOrEmpty(tenant))
            throw new ArgumentException($"Tenant setting needs to be specified in {envName} environment!");

        var sw = Stopwatch.StartNew();
        if (!tenant.Contains(".onmicrosoft.com")) tenant += ".onmicrosoft.com";
        var clientId = guc.AppId;
        var secret = guc.Secret;

        var seq = SortAndListFiles(Folder);
        verbose("---");
        verbose("files sequenced:");
        foreach (var item in seq)
        {
            verbose($"{item}");
        }
        verbose("---");

        FileSequence = seq.ToArray();
        
        var list = new List<KeyValuePair<string, string>>
        {
            new("scope", "https://graph.microsoft.com/.default"),
            new("grant_type", "client_credentials"),
            new("client_id", clientId),
            new("client_secret", secret)
        };
        var client = new HttpClient();
        try
        {
            verbose("authenticating with the tenant...");
            var tr = await client.PostAsync($"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token",
                new FormUrlEncodedContent(list));
            var json = await JsonDocument.ParseAsync(await tr.Content.ReadAsStreamAsync());
            var token = json.Deserialize<Token>();
            verbose("token received and parsed");

            var xs = new XmlSerializer(typeof(TrustFrameworkPolicy));
            foreach (var file in FileSequence)
            {
                if (!File.Exists(file))
                {
                    write($"path {file} does not exist");
                    continue;
                }

                await using var stm = File.OpenRead(file);
                stm.Seek(0, SeekOrigin.Begin);
                var policy = xs.Deserialize(stm) as TrustFrameworkPolicy;
                if (policy == null)
                {
                    write($"failed to deserialize policy from {file}");
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
                    write($"{err!.Error.Code}: {rsp.ReasonPhrase}\r\n");
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
        }
        catch (ServiceException se)
        {
            write($"{se.StatusCode.ToString()}: {se.Message}");
        }
        record(sw);
    }

    /// <summary>
    /// Build our deployment sequence list based on policy inheritance.
    /// </summary>
    /// <param name="folder">The folder containing all of our policy XML files</param>
    /// <returns>The deployment sequence list.</returns>
    private List<string> SortAndListFiles(string folder)
    {
        var files = Directory.GetFiles(folder,"*.xml");
        Dictionary<string, TrustFrameworkPolicy> tfPolicies = new Dictionary<string, TrustFrameworkPolicy>();
        var ol = new List<string>();
        var serializer = new XmlSerializer(typeof(TrustFrameworkPolicy));
        
        foreach (var file in files)
        {
            try
            {
                tfPolicies.Add(file, serializer.Deserialize(File.OpenRead(file)) as TrustFrameworkPolicy);
            }
            catch (Exception)
            {
                write($"parsing failure for {file}!");
                throw;
            }
        }

        var first = tfPolicies.FirstOrDefault(p => p.Value.BasePolicy == null || string.IsNullOrEmpty(p.Value.BasePolicy.PolicyId));
        var pid = first.Value.PolicyId;
        ol.Add(first.Key);
        tfPolicies.Remove(first.Key);
        var children = new Stack<string>();
        children.Push(pid);
        
        while (tfPolicies.Count > 0)
        {
            var policyId = children.Pop();
            var kids = getChildrenOf(tfPolicies, policyId);
            foreach (var kid in kids)
            {
                if (!children.Contains(kid.Value)) children.Push(kid.Value);
                ol.Add(kid.Key);
            }
        }

        return ol;
    }

    private Dictionary<string,string> getChildrenOf(Dictionary<string,TrustFrameworkPolicy> policies, string name)
    {
        var list = new Dictionary<string,string>();
        foreach (var policy in policies)
        {
            if (policy.Value.BasePolicy.PolicyId == name)
            {
                list.Add(policy.Key,policy.Value.PolicyId);
                policies.Remove(policy.Key);
            }
        }

        return list;
    }
    
    private string formatErr(string errMsg)
    {
        return errMsg
                .Replace(".The following error", ".\r\n\r\nThe following error")
                .Replace(".Claim", ".\r\n\r\nClaim")
                .Replace(".Technical",".\r\n\r\nTechnical")
                .Replace(".Policy",".\r\n\r\nPolicy")
                .Replace(".Invalid",".\r\n\r\nInvalid")
                .Replace(".The",".\r\n\r\nThe")
                .Replace(". List", ".\r\n\r\nList")
                .Replace(".Schema",".\r\n\r\nSchema")
                .Replace(".A",".\r\n\r\nA")
            ;
    }

}
