using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Common.Models
{
    public interface IMappableViewModel
    {
        void CopyFrom(object data);
        void CopyTo(object data);
    }
}
