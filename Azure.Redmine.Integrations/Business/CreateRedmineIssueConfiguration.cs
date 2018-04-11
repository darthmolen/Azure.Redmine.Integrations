using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Redmine.Integrations.Business
{
    public class CreateRedmineIssueConfiguration
    {
        public string RedmineBaseUrl { get; set; }
        public string RedmineApiTokenHeaderName { get; set; }
        public string RedmineApiTokenValue { get; set; }
        public string RedmineIssuesEndPoint { get; set; } = "issues.json";

        private static string _environRedmineUrlName = "RedmineBaseUrl";
        private static string _environRedmineApiKeyName = "RedmineApiHeaderKeyName";
        private static string _environRedmineApiValueName = "RedmineApiKeyValue";
        private static string _environRedmineIssuesEndPointName = "RedmineIssuesEndPoint";
        
        public CreateRedmineIssueConfiguration()
        {
            RedmineBaseUrl = System.Environment.GetEnvironmentVariable(_environRedmineUrlName);
            RedmineApiTokenHeaderName = System.Environment.GetEnvironmentVariable(_environRedmineApiKeyName);
            RedmineApiTokenValue = System.Environment.GetEnvironmentVariable(_environRedmineApiValueName);
        }
    }
}
