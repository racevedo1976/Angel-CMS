using System;
using System.Collections.Generic;

using Angelo.Connect.Configuration;
using Angelo.Connect.Icons;
using Angelo.Connect.Security;

namespace Angelo.Connect.Abstractions
{
    public interface IMenuItem
    {
        string Title { get; set; }
        IconType Icon { get; set; }
        bool Active { get; set; }
        string Url { get; set; }
        int SortOrder { get; set; }

        IEnumerable<IMenuItem> MenuItems { get; set; }

        bool Authorize(UserContext user);
    };
}
