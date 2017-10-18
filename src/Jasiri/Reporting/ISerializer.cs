﻿using OpenTracing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jasiri.Reporting
{
    public interface ISerializer
    {
        string MediaType { get; }
        string Serialize(IReadOnlyList<ISpan> spans);
    }
}