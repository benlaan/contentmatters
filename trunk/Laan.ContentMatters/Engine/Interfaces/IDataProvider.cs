using System;
using System.Collections.Generic;
using Laan.ContentMatters.Configuration;

namespace Laan.ContentMatters.Engine.Interfaces
{
    public interface IDataProvider
    {
        Dictionary<string, object> Build( Page page );
    }
}
