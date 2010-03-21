using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using NHibernate.UserTypes;
using System.Collections;
using NHibernate.Collection.Generic;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace Laan.ContentMatters.Models
{
    public class ItemList<T> : List<T> //PersistentGenericBag<T>
                                       , IItemList<T>, IList<T>, IList where T : IItem
    {
        public ItemList()
        {
            TypeName = "List";
        }

        public ItemList( IEnumerable<T> list ) : base( list ?? new List<T>() )
        {
            TypeName = "List";
        }

//        public virtual ItemDefinition ItemDefinition { get; set; }

        public virtual string TypeName { get; set; }
        
        public string Title
        {
            get { return String.Format("{0}List", GetGenericArgName()); }
            set { throw new NotSupportedException(); }
        }

        public string Description
        {
            get { return Title; }
            set { throw new NotSupportedException(); }
        }

        private string GetGenericArgName()
        {
            return this.GetType().GetGenericArguments().First().Name;
        }
    }
}
