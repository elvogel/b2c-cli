using System;
using System.IO;
using Newtonsoft.Json;

namespace b2c.Data
{
    public class Config
    {
        public static B2CConfig GetConfig()
        {
            var data = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "b2c.json"));
            var config = JsonConvert.DeserializeObject<B2CConfig>(data);
            return config;
        }
    }
}
