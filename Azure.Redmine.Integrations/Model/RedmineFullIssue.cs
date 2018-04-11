using System;
using System.Collections.Generic;
using System.Net;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Azure.Redmine.Integrations
{
    public partial class RedmineFullIssue
    {
        [JsonProperty("issue")]
        public Issue Issue { get; set; }
    }

    public partial class Issue
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("project")]
        public AssignedTo Project { get; set; }

        [JsonProperty("tracker")]
        public AssignedTo Tracker { get; set; }

        [JsonProperty("status")]
        public AssignedTo Status { get; set; }

        [JsonProperty("priority")]
        public AssignedTo Priority { get; set; }

        [JsonProperty("author")]
        public AssignedTo Author { get; set; }

        [JsonProperty("assigned_to")]
        public AssignedTo AssignedTo { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("start_date")]
        public System.DateTimeOffset StartDate { get; set; }

        [JsonProperty("done_ratio")]
        public long DoneRatio { get; set; }

        [JsonProperty("spent_hours")]
        public double SpentHours { get; set; }

        [JsonProperty("custom_fields")]
        public CustomField[] CustomFields { get; set; }

        [JsonProperty("created_on")]
        public System.DateTimeOffset CreatedOn { get; set; }

        [JsonProperty("updated_on")]
        public System.DateTimeOffset UpdatedOn { get; set; }
    }

    public partial class AssignedTo
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class CustomField
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("internal_name")]
        public object InternalName { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class RedmineFullIssue
    {
        public static RedmineFullIssue FromJson(string json) => JsonConvert.DeserializeObject<RedmineFullIssue>(json, Azure.Redmine.Integrations.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this RedmineFullIssue self) => JsonConvert.SerializeObject(self, Azure.Redmine.Integrations.Converter.Settings);
    }

    internal class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
