﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface IContentDescriptor
    {
        string ContentType { get; }

        string ContentId { get; }

        string VersionCode { get; }

    }
}
