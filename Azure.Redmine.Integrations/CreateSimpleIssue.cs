
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using Azure.Redmine.Integrations.Business;
using System.Threading.Tasks;
using System;
using System.Net.Http;

namespace Azure.Redmine.Integrations
{
    // http://devslice.net/2016/09/azure-functions-call-functions/
    
    public static class CreateSimpleIssue
    {
        private static CreateSimpleIssueConfiguration _config;

        private static CreateSimpleIssueConfiguration Config
        {
            get
            {
                if (_config == null)
                    _config = new CreateSimpleIssueConfiguration();

                return _config;
            }
        }

        // Takes fewer parameters and defaults to an assignee in configuration
        [FunctionName("CreateSimpleIssue")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // should be a soft SimpleIssue
            // { "Title": "Test Simple Ticket", "Description": "This is a Test Simple Ticket" }
            dynamic data = JsonConvert.DeserializeObject(new StreamReader(req.Body).ReadToEnd());

            var iTitle = WebUtility.UrlDecode(req.Query["Title"]) ?? data?.Title;
            var iDesc = WebUtility.UrlDecode(req.Query["Description"]) ?? data?.Description ;

            var simpleIssue = new SimpleIssue() { Title = iTitle, Description = iDesc };

            // TODO: Catch Validation Errors
            var dtoIssue = Map(simpleIssue);

            var resp = CreateIssue(dtoIssue);

            // TODO: Figure out format of return
            // return url
            return new BadRequestResult();
        }

        private static async Task<(bool Success, string ResponseMessage, RedmineFullIssue CreatedIssue)> CreateIssue(DtoIssue issue)
        {
            // TODO: Call CreateRedmineIssue
            HttpClientInstance.Init(_config.CreateRedmineIssueUrl, null);
            var _client = HttpClientInstance.Instance;

            var resp = await _client.PostAsJsonAsync(_config.CreateRedmineIssueEndPoint, issue);

            var success = resp.IsSuccessStatusCode;

            string json = null;

            if (success)
            {
                json = await resp.Content.ReadAsStringAsync();

                if (!String.IsNullOrWhiteSpace(json))
                {
                    try
                    {
                        var ret = JsonConvert.DeserializeObject<RedmineFullIssue>(json);
                        return (Success: success, ResponseMessage: "Successful", CreatedIssue: ret);
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        return (Success: success, ResponseMessage: ex.Message, CreatedIssue: null);
                    }
                }
                else
                {
                    success = false;
                    return (Success: success, ResponseMessage: "Empty Content from Redmine.", CreatedIssue: null);
                }
            }
            else
            {
                var error = await resp.Content.ReadAsStringAsync();
                return (Success: success, ResponseMessage: error, CreatedIssue: null);
            }            
        }



        private static DtoIssue Map(SimpleIssue issue)
        {

            // TODO: Validate Title
            // TODO: Validate Description

            var mappedIssue = new DtoIssue()
            {
                ProjectId = _config.ConfiguredProject.ProjectId,
                TrackerId = _config.ConfiguredProject.TrackerId,
                StatusId = _config.ConfiguredProject.StatusId,
                PriorityId = _config.ConfiguredProject.PriorityId,
                Title = issue.Title,
                Description = issue.Description
            };

            if (_config.ConfiguredProject.CustomFields != null)
            {
                mappedIssue.CustomFields = _config.ConfiguredProject.CustomFields;

            }

            return mappedIssue;
        }
    }
}
