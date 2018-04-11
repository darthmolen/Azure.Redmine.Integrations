using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Redmine.Integrations
{
    public class DtoIssue
    {
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TrackerId { get; set; }
        public int AssigneeId { get; set; }
        public int StatusId { get; set; }
        public int PriorityId { get; set; }

        public List<DtoCustomField> CustomFields { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static DtoIssue FromJson(string json)
        {
            return JsonConvert.DeserializeObject<DtoIssue>(json);
        }
    }
}
