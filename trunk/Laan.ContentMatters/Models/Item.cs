﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web;
using System.Xml.Serialization;

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

        public virtual string Link( string displayText )
        {
            return String.Format(
                "<a href=\"/{0}/{1}/{2}\">{2}</a>",
                GetType().Name, ID, displayText.HtmlEncoded()
            );
        }
        
        public virtual string Link()
        {
            return Link( Title );
        }
    }
}