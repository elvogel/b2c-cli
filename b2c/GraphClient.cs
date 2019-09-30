using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using b2c.Data;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Formatting = System.Xml.Formatting;

namespace b2c
{
    public class GraphClient
    {
        private static string aadGraphResourceId = "https://graph.windows.net/";
        private static string aadInstance = "https://login.microsoftonline.com/";
        private static string aadGraphEndpoint = "https://graph.windows.net/";
        private static string aadGraphVersion = "api-version=1.6";

        private string AppId;
        private string Tenant;
        private string Secret;

        private static HttpClient client;
        private static ClientCredential credential;
        private static AuthenticationContext authContext;
        private static AuthenticationResult result;
        private IConsole console;
        private Config config;
        public GraphClient(Config config, IConsole console)
        {
            this.console = console;
            this.config = config;
            authContext = new AuthenticationContext(aadInstance + Tenant);
            credential = new ClientCredential(AppId,Secret);
        }

        private async Task Initialize()
        {
            client = new HttpClient();
            result = await authContext.AcquireTokenAsync(aadGraphResourceId, credential);
        }

        public async Task<string> CreateUser(string displayName, string mailNickname, string password)
        {
            var uri = "https://graph.windows.net/myorganization/users?" + aadGraphVersion;

            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent("{\n  \"accountEnabled\": true,\n  " +
                                            "\"displayName\": \"" + displayName + "\",\n  " +
                                            "\"mailNickname\": \"" + mailNickname + "\",\n  " +
                                            "\"passwordProfile\": {\n    " +
                                                "\"password\": \"" + password + "\",\n    " +
                                                "\"forceChangePasswordNextLogin\": false\n  },\n  " +
                                            "\"userPrincipalName\": \"" + mailNickname + "@" + Tenant + "\"\n}")
            };

            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            var obj = JsonConvert.DeserializeObject(await SendRequest(request)) as JObject;

            //return obj["objectId"].ToString();
            return obj.ToString();
        }

        public async Task<string> AddUserToGroup(string userId, string groupId)
        {
            var uri = $"https://graph.windows.net/myorganization/groups/{groupId}/$links/members?api-version=1.6";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent("{\"url\": \"https://graph.windows.net/myorganization/directoryObjects/" + userId + "\"}")
            };
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            return await SendRequest(request);
        }

        private async Task<string> SendRequest(HttpRequestMessage request)
        {
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync();

            var error = await response.Content.ReadAsStringAsync();
            var formatted = JsonConvert.DeserializeObject(error) as JObject;
            throw new WebException("Error Calling the Graph API: \n" + JsonConvert.SerializeObject(formatted,Newtonsoft.Json.Formatting.Indented));
        }

        public async Task<string> DeleteUser(string userId)
        {
            var uri = $"https://graph.windows.net/myorganization/users/{userId}?{aadGraphVersion}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, uri);
            return await SendRequest(request);
        }
    }
}
