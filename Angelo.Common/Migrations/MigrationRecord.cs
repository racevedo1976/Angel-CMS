using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Common.Migrations
{
    public class MigrationRecord
    {
        public string Id { get; set; }
        public string Migration { get; set; }

        public DateTime Executed { get; set; }
        public int Duration { get; set; }
        public int StatusCode { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
    }
}
