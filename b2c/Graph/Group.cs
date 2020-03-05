using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace b2c.Graph
{
    public class ListGroupsResponse
    {
        [JsonPropertyName("odata.metadata")]
        public string odataMetadata { get; set; }

        public List<Group> value { get; set; }
    }
    public class Group
    {
        [JsonPropertyName("odata.type")]
        public string odataType { get; set; }
        public string objectType { get; set; }
        public string objectId { get; set; }
        public DateTime? deletionTimestamp { get; set; }
        public string description { get; set; }
        public string dirSyncEnabled { get; set; }
        public string displayName { get; set; }
        public DateTime? lastDirSyncTime { get; set; }
        public string mail { get; set; }
        public string mailNickname { get; set; }
        public string onPremisesSecurityIdentifier { get; set; }
        public string[] provisioningErrors { get; set; }
        public string[] proxyAddresses { get; set; }
        public bool securityEnabled { get; set; }
    }
}
