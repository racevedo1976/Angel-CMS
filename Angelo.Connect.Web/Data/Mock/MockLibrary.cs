using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.Data.Mock
{
    public class MockFile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string PhysicalPath { get; set; }
        public string FolderId { get; set; }
    }

    public class MockFolder
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string VirtualPath { get; set; }
        public string ParentId { get; set; }

    }

}
