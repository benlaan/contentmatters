using System;
using System.Linq;

using FluentNHibernate.Conventions;
using FluentNHibernate.Mapping;

using Laan.ContentMatters.Models;

namespace Laan.ContentMatters.Conventions
{
    public class CollectionListTypeConvention : IHasManyConvention
    {

        #region IConvention<IOneToManyPart> Members

        public bool Accept( IOneToManyPart target )
        {
            return true;
        }

        public void Apply( IOneToManyPart target )
        {
            var argumentType = target.GetType().GetGenericArguments().First();
            Type type = typeof( ItemList/*Type*/<> ).MakeGenericType( argumentType );
            target.CollectionType( type );
        }

        #endregion
    }
}