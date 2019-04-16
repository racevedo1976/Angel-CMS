using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface IContextAccessor<TContext> where TContext : class
    {
        TContext GetContext();
    }
}
