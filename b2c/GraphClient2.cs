using System.Net.Http;
using System.Threading.Tasks;
using b2c.Data;
using b2c.Graph;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace b2c
{
    //https://blazorhelpwebsite.com/ViewBlogPost/43
    public class GraphClient2
    {
        private B2CConfig config;
        private IConsole console;
        private IConfidentialClientApplication app;
        private AcquireTokenForClientParameterBuilder builder;
        private AuthenticationResult result;
        protected ProtectedApiCallHelper helper;
        public GraphClient2(B2CConfig b2CConfig, IConsole console)
        {
            config = b2CConfig;
            this.console = console;

            app =
                ConfidentialClientApplicationBuilder
                    .Create(b2CConfig.AppId)
                    .WithTenantId(b2CConfig.Tenant)
                    .WithClientSecret(b2CConfig.Secret)
                    .Build();
            // With client credentials flows the scopes is ALWAYS of the shape
            // "resource/.default", as the
            // application permissions need to be set statically
            // (in the portal or by PowerShell),
            // and then granted by a tenant administrator
            string[] scopes = { "https://graph.microsoft.com/.default" };
             result = app.AcquireTokenForClient(scopes)
                .ExecuteAsync().Result;

             helper = new ProtectedApiCallHelper(new HttpClient());
        }

        public async Task<ListGroupsResponse> ListGroups()
        {
            return null;
        }
        public async Task<ListUsersResponse> ListUsers(string filterName = null, string filterValue = null)
        {
            return null;

        }
    }
}
