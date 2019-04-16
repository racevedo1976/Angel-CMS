using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.Data.Mock
{
    public class MockMessageCategory
    {
        public string CategoryName { get; set; }

        public int MessageCount { get; set; }
    }

    public class MockMessage
    {
        public string CategoryName { get; set; }

        public string Title { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
