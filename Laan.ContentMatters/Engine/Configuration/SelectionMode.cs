using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Laan.ContentMatters.Configuration
{
    public enum SelectionMode
    {
        [XmlEnum( "all" )]
        All,

        [XmlEnum( "parent" )]
        Parent,

        [XmlEnum( "key" )]
        Key,

        [XmlEnum( "random" )]
        Random

    }
}
