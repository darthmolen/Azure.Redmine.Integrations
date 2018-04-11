using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Redmine.Integrations
{
    public class RedmineIssueWrapper
    {
        public RedmineIssue issue { get; set; }

        public RedmineIssueWrapper()
        { }

        public RedmineIssueWrapper(HttpRequest req)
        {
            issue = new RedmineIssue(req);
        }

        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
    }
}
