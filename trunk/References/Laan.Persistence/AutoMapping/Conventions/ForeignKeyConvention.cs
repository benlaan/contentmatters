using System;
using System.Reflection;

namespace Laan.Persistence.AutoMapping
{
    public class ForeignKeyConvention : FluentNHibernate.Conventions.ForeignKeyConvention
    {
        protected override string GetKeyName( PropertyInfo property, Type type )
        {
            return type.Name + "ID";
        }
    }
}