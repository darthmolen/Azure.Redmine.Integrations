using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.Redmine.Integrations
{
    public class RedmineCustomField
    {
        public string value { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string internal_name { get; set; }
    }
}
