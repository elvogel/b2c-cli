using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
// ReSharper disable UnusedMember.Global

namespace b2c.Graph
{
    public class ListUsersResponse
    {
        [JsonPropertyName("odata.metadata")] public string odataMetadata { get; set; }
        public List<GraphUser> value { get; set; }
    }

    //https://docs.microsoft.com/en-us/graph/api/resources/users?view=graph-rest-1.0
    public class GraphUser
    {
        public string[] businessPhones { get; set; }
        public string displayName { get; set; }
        public string givenName { get; set; }
        public string jobTitle { get; set; }
        public string mail { get; set; }
        public string mobilePhone { get; set; }
        public object officeLocation { get; set; }
        public string preferredLanguage { get; set; }
        public string surname { get; set; }
        public string userPrincipalName { get; set; }
        public string id { get; set; }
    }
}
