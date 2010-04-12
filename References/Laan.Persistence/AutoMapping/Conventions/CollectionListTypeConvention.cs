using System;
using System.Linq;

using FluentNHibernate.Conventions;
using FluentNHibernate.Mapping;
using System.Collections;

namespace Laan.Persistence.AutoMapping
{
    public class CollectionListTypeConvention<T> : IHasManyConvention where T : IList
    {

        #region IConvention<IOneToManyPart> Members

        public bool Accept( IOneToManyPart target )
        {
            return true;
        }

        public void Apply( IOneToManyPart target )
        {
            var argumentType = target.GetType().GetGenericArguments().First();
            Type type = typeof( T ).MakeGenericType( argumentType );
            target.CollectionType( type );
        }

        #endregion
    }
}