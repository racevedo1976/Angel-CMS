using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Icons;
using Angelo.Connect.Configuration;
using Angelo.Connect.Menus;

namespace Angelo.Connect.Abstractions
{
    public interface IMenuItemProvider
    {
        string MenuName { get; }

        IEnumerable<IMenuItem> MenuItems { get; }
    }
}
