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

    public class ClassConvention : IClassConvention
    {
        #region IConvention<IClassMap> Members

        public bool Accept( IClassMap target )
        {
            return true;
        }

        public void Apply( IClassMap target )
        {
            string @namespace = target.EntityType.Namespace.Split( '.' ).Last();
            target.WithTable( String.Format( "{0}_{1}", @namespace, target.EntityType.Name ) );
        }

        #endregion
    }
}
