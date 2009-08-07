using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laan.ContentMatters.Models
{
    public class ItemDefinition : IItem, IIdentifiable
    {
        public ItemDefinition()
        {
            MasterPage = "Site";
            Fields = new List<FieldDefinition>();
        }

        public virtual string MasterPage { get; set; }
        public virtual string Namespace { get; set; }
        public virtual string Description { get; set; }
        public virtual List<FieldDefinition> Fields { get; set; }

        #region IIdentifiable Members

        public virtual int ID { get; set; }

        #endregion

        #region IItem Members

        public virtual string TypeName
        {
            get { return GetType().Name; }
        }

        public virtual string Name { get; set; }

        public virtual string Title
        {
            get { return Description; }
        }

        #endregion

    }
}
