using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace b2c.Graph
{
    public class ListGroupsResponse
    {
        [JsonPropertyName("odata.metadata")]
        public string odataMetadata { get; set; }

        public List<GraphGroup> value { get; set; }
    }

    //https://docs.microsoft.com/en-us/graph/api/resources/groups-overview?view=graph-rest-1.0
    public class GraphGroup
    {
        public string id { get; set; }
        public object deletedDateTime { get; set; }
        public object classification { get; set; }
        public DateTime createdDateTime { get; set; }
        public object[] creationOptions { get; set; }
        public string description { get; set; }
        public string displayName { get; set; }
        public object expirationDateTime { get; set; }
        public object[] groupTypes { get; set; }
        public object isAssignableToRole { get; set; }
        public object mail { get; set; }
        public bool mailEnabled { get; set; }
        public string mailNickname { get; set; }
        public object membershipRule { get; set; }
        public object membershipRuleProcessingState { get; set; }
        public object onPremisesDomainName { get; set; }
        public object onPremisesLastSyncDateTime { get; set; }
        public object onPremisesNetBiosName { get; set; }
        public object onPremisesSamAccountName { get; set; }
        public object onPremisesSecurityIdentifier { get; set; }
        public object onPremisesSyncEnabled { get; set; }
        public object preferredDataLocation { get; set; }
        public object preferredLanguage { get; set; }
        public object[] proxyAddresses { get; set; }
        public DateTime renewedDateTime { get; set; }
        public object[] resourceBehaviorOptions { get; set; }
        public object[] resourceProvisioningOptions { get; set; }
        public bool securityEnabled { get; set; }
        public string securityIdentifier { get; set; }
        public object theme { get; set; }
        public object visibility { get; set; }
        public object[] onPremisesProvisioningErrors { get; set; }
    }
}
