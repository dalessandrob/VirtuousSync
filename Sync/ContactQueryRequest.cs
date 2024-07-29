using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync
{
    public class ContactQueryRequest
    {
        public ContactQueryRequest()
        {
            Groups = new List<object>();
            SortBy = "Id";
            Descending = false;
        }

        public List<object> Groups { get; set; }

        public string SortBy { get; set; }

        public bool Descending { get; set; }
    }

    public class Condition
    {
        public string Parameter { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        //public string secondaryValue { get; set; }
        //public List<string> values { get; set; }
    }

    public class Root
    {
        public Root()
        {
            Conditions = new List<Condition>();
        }

        public List<Condition> Conditions { get; set; }
    }
}
