using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Laan.ContentMatters.Models
{
    public class FieldDefinition
    {
        public FieldDefinition()
        {
            FieldType = FieldType.Number;
            IsParent = false;
        }

        [XmlAttribute( "name" )]
        public virtual string Name { get; set; }

        [XmlAttribute( "fieldType" )]
        public virtual FieldType FieldType { get; set; }

        [XmlAttribute( "referenceType" )]
        public virtual string ReferenceType { get; set; }

        [XmlIgnore]
        public bool IsParent { get; set; }

        public override string ToString()
        {
            return String.Format( "{0}: {1}{2}", Name, FieldType, ( ReferenceType != null ? "[" + ReferenceType + "]" : "" ) );
        }

    }
}
