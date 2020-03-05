using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
// ReSharper disable UnusedMember.Global

namespace b2c.Graph
{
    public class ListUsersResponse
    {
        [JsonPropertyName("odata.metadata")]
        public string odataMetadata { get; set; }
        public List<User> value { get; set; }
    }
    public class User
    {
        [JsonPropertyName("odata.type")]
        public string odataType { get; set; }

        public string objectType { get; set; }
        public string objectId { get; set; }
        public DateTime? deletionTimestamp { get; set; }
        public bool accountEnabled { get; set; }
        //public string[] signInNames { get; set; }
        public AssignedLicenses[] assignedLicenses { get; set; }
        public AssignedPlans[] assignedPlans { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string creationType { get; set; }
        public string department { get; set; }
        public string dirSyncEnabled { get; set; }
        public string displayName { get; set; }
        public string facsimileTelephoneNumber { get; set; }
        public string givenName { get; set; }
        public string immutableId { get; set; }
        public string jobTitle { get; set; }
        public string lastDirSyncTime { get; set; }
        public string mail { get; set; }
        public string mailNickname { get; set; }
        public string mobile { get; set; }
        public string onPremisesSecurityIdentifier { get; set; }
        public string[] otherMails { get; set; }
        public string passwordPolicies { get; set; }
        //public string passwordProfile { get; set; }
        public string physicalDeliveryOfficeName { get; set; }
        public string postalCode { get; set; }
        public string preferredLanguage { get; set; }
        public ProvisionedPlan[] provisionedPlans { get; set; }
        public string[] provisioningErrors { get; set; }
        public string[] proxyAddresses { get; set; }
        public string sipProxyAddress { get; set; }
        public string state { get; set; }
        public string streetAddress { get; set; }
        public string surname { get; set; }
        public string usageLocation { get; set; }
        public string userPrincipalName { get; set; }
        public string userType { get; set; }
    }

    public class AssignedLicenses
    {
        public string[] disabledPlans { get; set; }
        public string skuId { get; set; }
    }

    public class AssignedPlans
    {
        public DateTime? assignedTimestamp { get; set; }
        public string capabilityStatus { get; set; }
        public string service { get; set; }
        public string servicePlanId { get; set; }
    }

    public class ProvisionedPlan
    {
        public string capabilityStatus { get; set; }
        public string provisioningStatus { get; set; }
        public string service { get; set; }
    }

}
