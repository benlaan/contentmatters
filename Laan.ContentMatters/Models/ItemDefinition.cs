using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Laan.ContentMatters.Models
{
    [ Serializable ]
    [ XmlRoot( "definitions" ) ]
    [ DebuggerDisplay("{Namespace}.{Name}") ]
    public class ItemDefinition : IItem
    {
        public ItemDefinition()
        {
            Fields = new List<FieldDefinition>();
        }

        [ XmlAttribute( "parent" ) ]
        public virtual string Parent { get; set; }

        [ XmlAttribute( "namespace" ) ]
        public virtual string Namespace { get; set; }

        [ XmlAttribute( "description" ) ]
        public virtual string Description { get; set; }

        [XmlArray( "fields" ), XmlArrayItem( "field" )]
        public virtual List<FieldDefinition> Fields { get; set; }

        [XmlAttribute( "name" )]
        public virtual string Name { get; set; }

    }
}
