using System;
using System.IO;
using Newtonsoft.Json;

namespace b2c.Data
{
    public class Config
    {
        public Config()
        {
            var data = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "b2c.json"));
            var c = JsonConvert.DeserializeObject<Config>(data);
            AppId = c.AppId;
            Tenant = c.Tenant;
            Secret = c.Secret;
        }

        public Config(string appId, string tenant, string secret)
        {
            AppId = appId;
            Tenant = tenant;
            Secret = secret;
        }

        public static Config GetConfig()
        {
            var data = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "b2c.json"));
            return JsonConvert.DeserializeObject<Config>(data);
        }

        public string AppId { get; set; }
        public string Tenant { get; set; }
        public string Secret { get; set; }
    }
}
