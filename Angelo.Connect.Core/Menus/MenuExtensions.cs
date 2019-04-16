using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Menus
{
    public static class MenuExtensions
    {
        public static void InvokeRecursive(this IMenuItem menuItem, Action<IMenuItem> action)
        {
            action.Invoke(menuItem);

            if (menuItem.MenuItems != null)
            {
                foreach (var child in menuItem.MenuItems)
                    InvokeRecursive(child, action);
            }
        }

    }
}
