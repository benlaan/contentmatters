using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Laan.ContentMatters.Models
{
    [ Serializable ]
    [ XmlRoot( "definitions" ) ]
    public class ItemDefinition : IItem, IIdentifiable
    {
        public ItemDefinition()
        {
            MasterPage = "Site";
            Fields = new List<FieldDefinition>();
        }

        [ XmlAttribute( "masterPage" ) ]
        public virtual string MasterPage { get; set; }

        [ XmlAttribute( "namespace" ) ]
        public virtual string Namespace { get; set; }

        [ XmlAttribute( "description" ) ]
        public virtual string Description { get; set; }

        [XmlIgnore]
        public virtual IList<FieldDefinition> Fields { get; set; }

        [XmlArray( "fields" ), XmlArrayItem( "field" )]
        public virtual List<FieldDefinition> FieldList
        {
            get { return new List<FieldDefinition>( Fields ); }
            set { Fields = value; }
        }

        #region IIdentifiable Members

        [ XmlIgnore ]
        public virtual int ID { get; set; }

        #endregion

        #region IItem Members

        [XmlIgnore]
        public virtual string TypeName
        {
            get { return GetType().Name; }
        }

        [XmlAttribute( "name" )]
        public virtual string Name { get; set; }

        [XmlIgnore]
        public virtual string Title
        {
            get { return Description; }
        }

        #endregion

    }
}
