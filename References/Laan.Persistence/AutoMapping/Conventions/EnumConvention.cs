using System;
using System.Linq;

using FluentNHibernate.Conventions;
using FluentNHibernate.Mapping;

namespace Laan.Persistence.AutoMapping
{
    public class EnumConvention : IUserTypeConvention
    {
        public bool Accept( IProperty target )
        {
            return target.PropertyType.IsEnum;
        }

        public void Apply( IProperty target )
        {
            target.CustomTypeIs( target.PropertyType );
        }

        public bool Accept( Type type )
        {
            return type.IsEnum;
        }
    }
}