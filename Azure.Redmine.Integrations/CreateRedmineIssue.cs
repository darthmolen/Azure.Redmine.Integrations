
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Azure.Redmine.Integrations.Business;
using System.Collections.Generic;

namespace Azure.Redmine.Integrations
{
    public static class CreateRedmineIssue
    {
        [FunctionName("CreateRedmineIssue")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            try
            {
                // {"issue":{"project_id":165,"subject":"Test Ticket","tracker_id":2,"priority_id":4,"assigned_to_id":127,"status_id":1,"description":"My Test Description"}}
                //var (wrapper, issue) = CreateTestData();
                //return new OkObjectResult(wrapper.SerializeToJson());

                var wrapper = new RedmineIssueWrapper(req);
                var ret = CreateIssue(wrapper);


                if (ret.IsCompleted && ret.Result.Success)
                {
                    var content = ret.Result.Content;
                    log.Info(content);
                    return new OkObjectResult(content); // also add the predicted URL of the new issue
                }
                else
                {
                    return new BadRequestObjectResult(ret.Result.Content);
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        private static async Task<(bool Success, string Content)> CreateIssue(RedmineIssueWrapper issue)
        {
            //var issuesEndPoint = "issues.json";

            // Read values from config
            // https://docs.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings
            // https://docs.microsoft.com/en-us/azure/azure-functions/functions-reference-csharp#environment-variables

            var config = new CreateRedmineIssueConfiguration();

            var headers = new List<KeyValuePair<string,string>>(new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>(config.RedmineApiTokenHeaderName, config.RedmineApiTokenValue)
            });

            HttpClientInstance.Init(config.RedmineBaseUrl, headers);

            var _client = HttpClientInstance.Instance;

            var resp = await _client.PostAsync(config.RedmineIssuesEndPoint, new StringContent(issue.SerializeToJson(), System.Text.Encoding.UTF8, "application/json"));

            var success = resp.IsSuccessStatusCode;

            string ret;

            if (success)
                ret = await resp.Content.ReadAsStringAsync();
            else
            {
                ret = resp.ReasonPhrase + ". ";

                var content = await resp.Content.ReadAsStringAsync();
                if (!String.IsNullOrWhiteSpace(content))
                {
                    try
                    {
                        dynamic json = JObject.Parse(content);
                        ret += json?.errors[0];
                    }
                    catch { }
                }
            }


            return (Success: success, Content: ret);
        }

        #region Testing

        private static (RedmineIssueWrapper wrapper, DtoIssue issue) CreateTestData()
        {
            //for testing
            var issue = CreateTestRedmineIssue();
            var wrapper = new RedmineIssueWrapper() { issue = issue };
            var i = CreateTestDtoIssue();
 
            return (wrapper: wrapper, issue: i);
        }

        private static DtoIssue CreateTestDtoIssue()
        {
            var i = new DtoIssue()
            {
                ProjectId = 165,
                Title = "My Test Ticket",
                Description = "Test Description for Ticket",
                TrackerId = 2,
                StatusId = 1,
                PriorityId = 4,
                AssigneeId = 127
            };

            return i;
        }

        private static RedmineIssue CreateTestRedmineIssue()
        {
            // https://url/issue_statuses.json?key=
            // https://url/trackers.json?key=
            // https://url/projects/{id}.json?key=
            // https://url/projects.json?key=

            var projectId = 165; // Pando Roadmap
            var ticketSubject = "My Test Ticket";
            var trackerId = 2; // Feature
            var statusId = 1; // new
            var priorityId = 4; // normal
            var assignedId = 127; //Steven Molen 
            var desc = "Test Description";

            RedmineIssue issue = new RedmineIssue
            {
                project_id = projectId,
                subject = ticketSubject,
                tracker_id = trackerId,
                status_id =  statusId,
                priority_id = priorityId,
                assigned_to_id = assignedId,
                description = desc
            };

            return issue;
        }

        #endregion Testing
    }
}
