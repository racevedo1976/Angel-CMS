using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Common.Abstractions
{
    public interface IStartupAction
    {
        void Invoke();
    }  
}
