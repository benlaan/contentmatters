using System;
using System.Reflection;

namespace Laan.ContentMatters.Conventions
{
    public class ForeignKeyConvention : FluentNHibernate.Conventions.ForeignKeyConvention
    {
        protected override string GetKeyName( PropertyInfo property, Type type )
        {
            return type.Name + "ID";
        }
    }
}