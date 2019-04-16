using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.SlideShow.Models
{
    public interface IEffect<TTarget>
    {
        bool IsEnabled { get; set; }
        void Invoke(TTarget target);
    }
}
