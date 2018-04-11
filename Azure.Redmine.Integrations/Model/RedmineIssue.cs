using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Linq;

namespace Azure.Redmine.Integrations
{
    public class RedmineIssue
    {
        public int project_id { get; set; } = 0;
        public string subject { get; set; }
        public int tracker_id { get; set; } = 0;
        public int priority_id { get; set; } = 0;
        public int assigned_to_id { get; set; } = 0;
        public int status_id { get; set; } = 0;
        public string description { get; set; }
        public RedmineCustomField[] custom_fields { get; set; }

        public RedmineIssue(HttpRequest req)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            Map(data, req.Query);

            var (IsValid, ValidationErrors) = Validate();

            if (!IsValid) throw new Exception(string.Concat(ValidationErrors));
        }

        public RedmineIssue()
        {
        }

        public (bool IsValid,List<string> ValidationErrors) Validate()
        {
            bool isValid = false;
            List<string> errors = new List<string>();

            if (project_id == 0)
                errors.Add("Project Not Set. ");

            if (String.IsNullOrWhiteSpace(subject))
                errors.Add("Please provide a subject. ");

            if (String.IsNullOrWhiteSpace(description))
                errors.Add("Please provide a ticket description. ");

            if (tracker_id == 0)
                errors.Add("Please provide a tracker. ");

            if (priority_id == 0)
                errors.Add("Please provide a priority. ");

            if (assigned_to_id == 0)
                errors.Add("Please provide an assignee. ");

            if (status_id == 0)
                errors.Add("Please provide an appropriate status. ");

            if (errors.Count == 0)
                isValid = true;

            var ret = (IsValid: isValid, ValidationErrors: errors);

            return ret;
        }

        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        private void Map(dynamic data, IQueryCollection query)
        {
            var iTitle = query["Title"];
            var iDesc = query["Description"];

            var sProj = int.TryParse(query["ProjectId"], out int iProject);
            var sTrack = int.TryParse(query["TrackerId"], out int iTracker);
            var sPrior = int.TryParse(query["PriorityId"], out int iPriority);
            var sAssign = int.TryParse(query["AssigneeId"], out int iAssignee);
            var sStatus = int.TryParse(query["StatusId"], out int iStatus);

            project_id = (sProj) ? iProject : data?.ProjectId ?? 0;
            tracker_id = (sTrack) ? iTracker : data?.TrackerId ?? 0;
            priority_id = (sPrior) ? iPriority : data?.PriorityId ?? 0;
            assigned_to_id = (sAssign) ? iAssignee : data?.AssigneeId ?? 0;
            status_id = (sStatus) ? iStatus : data?.StatusId ?? 0;

            subject = (!String.IsNullOrEmpty(iTitle)) ? WebUtility.UrlDecode(iTitle) : data?.Title;
            description = (!String.IsNullOrEmpty(iDesc)) ? WebUtility.UrlDecode(iDesc) : data?.Description;

            if (data?.CustomFields != null && data.CustomFields.Count > 0)
            {
                custom_fields = MapFields(data.CustomFields.ToObject<List<DtoCustomField>>());
            }
        }

        private RedmineCustomField[] MapFields(List<DtoCustomField> fields)
        {

            var mappedFields = fields.Select(f => new RedmineCustomField() {
                id = f.ID,
                name = f.Name,
                internal_name = null,
                value = f.Value
            });

            return mappedFields?.ToArray();
        }
    }
}
