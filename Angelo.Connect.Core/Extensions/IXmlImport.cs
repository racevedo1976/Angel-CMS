using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Angelo.Connect
{
    interface IXmlImport
    {
        Task ImportXmlAsync(XmlReader reader);
    }
}
