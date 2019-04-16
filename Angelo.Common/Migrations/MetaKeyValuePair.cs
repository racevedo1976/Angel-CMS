using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Common.Migrations
{
    public class MetaKeyValuePair
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public MetaKeyValuePair()
        {

        }

        public MetaKeyValuePair(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
