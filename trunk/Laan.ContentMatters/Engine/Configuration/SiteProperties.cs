using System;
using System.Collections.Generic;

using Laan.ContentMatters.Engine.Interfaces;

namespace Laan.ContentMatters.Configuration
{
    public class SiteProperties : ISiteProperties
    {
        public SiteProperties( IDictionary<string, object> properties )
        {
            Values = properties;
        }

        public IDictionary<string, object> Values { get; private set; }
    }
}
