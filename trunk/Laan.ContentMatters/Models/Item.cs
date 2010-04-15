using System;
using System.Linq;

using Laan.ContentMatters.Models.Extensions;

namespace Laan.ContentMatters.Models
{
    public abstract class Item : IItem, IIdentifiable
    {
        public Item()
        {
            Type type = GetType();

            string typeName = GetTypeName( type );
            Title = typeName;
            TypeName = type.IsGenericType ? "List" : typeName;
        }

        private string GetTypeName( Type type )
        {
            if ( type.IsGenericType )
                return type.GetGenericArguments().First().Name + "List";
            else
                return type.Name;
        }

        public virtual ItemDefinition ItemDefinition { get; set; }

        public virtual int ID { get; set; }
        public virtual string TypeName { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual User Author { get; set; }
        public virtual string Description { get; set; }
        public virtual string Title { get; set; }
        
        protected virtual string GetLink()
        {
            return Title;
        }

        public virtual string Link { get { return GetLink(); } }
    }
}