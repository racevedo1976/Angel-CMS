using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface IDocument: IContentType
    {
        string DocumentId { get; set; }
        string Title { get; set; }
    }
}
