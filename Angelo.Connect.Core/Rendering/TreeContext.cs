using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Rendering
{
    public class TreeContext
    {
        public string TreeId { get; set; }
        public string NodeId { get; set; }
        public bool Editable { get; set; } = false;
        public bool AllowContainers = false;
        public ZoneContext Zone { get; set; }
    }

    public class ZoneContext
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public bool Embedded { get; set; } = false;
        public bool AllowContainers { get; set; } = false;
        public bool AllowPadding { get; set; } = true;
    }
}
