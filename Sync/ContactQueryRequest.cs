using Sync;
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

    public class StateFilter
    {
        public StateFilter(string state)
        {
            Parameter = "State";
            Operator = "Is";
            Value = state;
        }

        public string Parameter { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }

    public class RootFilter
    {
        public RootFilter(object filter)
        {
            Conditions = new List<object>
            {
                filter
            };
        }

        public List<object> Conditions { get; set; }
    }
}
