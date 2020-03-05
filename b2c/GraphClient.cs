#pragma warning disable 649
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using b2c.Data;
using b2c.Graph;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace b2c
{
    public class GraphClient
    {
        private static string aadInstance = "https://login.microsoftonline.com/";
        private static string aadGraphVersion = "api-version=1.6";

        private static HttpClient client;
        private static ClientCredential credential;
        private static AuthenticationContext authContext;
        private static AuthenticationResult result;
        private IConsole console;
        private B2CConfig config;
        public GraphClient(B2CConfig config, IConsole console)
        {
            this.console = console;
            this.config = config;
            authContext = new AuthenticationContext(aadInstance + config.Tenant,true);
            credential = new ClientCredential(config.AppId,config.Secret);
            client = new HttpClient();
            result = authContext.AcquireTokenAsync("https://graph.windows.net/", credential).Result;

        }

        public async Task<ListGroupsResponse> ListGroups()
        {
            var uri = "https://graph.windows.net/myorganization/groups?" + aadGraphVersion;
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            return JsonConvert.DeserializeObject<ListGroupsResponse>(await SendRequest(request));
        }

        public async Task<ListUsersResponse> ListUsers(string filterName = null, string filterValue = null)
        {
            var uri = "https://graph.windows.net/myorganization/users?" + aadGraphVersion;
            if (!string.IsNullOrEmpty(filterName))
            {
                uri += $"$starts" +
                       $"with({filterName},{filterValue})";
            }
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            var ret = await SendRequest(request);

            return JsonConvert.DeserializeObject<ListUsersResponse>(ret);

        }

        public async Task<JObject> CreateUser(string displayName, string mailNickname, string password)
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
                                            "\"userPrincipalName\": \"" + mailNickname + "@" + config.Tenant + "\"\n}")
            };

            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            return JsonConvert.DeserializeObject<JObject>(await SendRequest(request));
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
            throw new WebException("Error Calling the Graph API: \n" +
                                   JsonConvert.SerializeObject(formatted,Formatting.Indented));
        }

        public async Task<string> DeleteUser(string userId)
        {
            try
            {
                var uri = $"https://graph.windows.net/myorganization/users/{userId}?{aadGraphVersion}";

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, uri);
                await SendRequest(request);
                return "operation completed successfully";
            }
            catch (Exception ex)
            {
                return $"{ex.GetType()}: {ex.Message}";
            }
        }
    }
}
