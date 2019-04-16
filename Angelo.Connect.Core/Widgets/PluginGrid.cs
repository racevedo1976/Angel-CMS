using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using System.Collections.Generic;

namespace Angelo.Connect.Widgets
{
    public class PluginGrid
    {
        public ICollection<string> Columns { get; } = new List<string>();
    }
}
