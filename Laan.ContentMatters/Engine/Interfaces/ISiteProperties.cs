using System;
using System.Collections.Generic;

namespace Laan.ContentMatters.Engine.Interfaces
{
    public interface ISiteProperties
    {
        IDictionary<string, object> Values { get; }
    }
}
