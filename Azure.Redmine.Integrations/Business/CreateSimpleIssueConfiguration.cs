using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Redmine.Integrations.Business
{
    public class CreateSimpleIssueConfiguration
    {
        public DtoIssue ConfiguredProject { get; set; }
        public string CreateRedmineIssueUrl { get; set; }
        public string CreateRedmineIssueEndPoint { get; set; }

        private static string _environconfiguredProject = "ConfiguredProject";
        private static string _environCreateRedmineIssueUrl = "CreateRedmineIssueUrl";
        private static string _environCreateRedmineIssueEndPoint = "CreateRedmineIssueEndpoint";

        public CreateSimpleIssueConfiguration()
        {
            CreateRedmineIssueUrl = System.Environment.GetEnvironmentVariable(_environconfiguredProject);
            CreateRedmineIssueEndPoint = System.Environment.GetEnvironmentVariable(_environCreateRedmineIssueEndPoint);
            // Deserialize configured project from the system variable. This will fill in all the necessary redmine ids to push to the configured project
            var configured = System.Environment.GetEnvironmentVariable(_environconfiguredProject);

            if (String.IsNullOrWhiteSpace(configured))
                throw new Exception("No Configured Project Found");

            ConfiguredProject = DtoIssue.FromJson(configured);

        }
    }
}
