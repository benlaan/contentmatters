using System;
using System.Collections.Generic;
using Laan.ContentMatters.Engine.Interfaces;
using System.Collections;

namespace Laan.ContentMatters.Configuration
{
    public class SiteProperties : ISiteProperties
    {
        public SiteProperties( IDictionary properties )
        {
            Values = properties;

        }

        public IDictionary<string, object> Values { get; private set; }
    }
}
