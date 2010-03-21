using System;

using FluentNHibernate.Conventions;
using FluentNHibernate.Mapping;

namespace Laan.ContentMatters.Conventions
{
    public class ManyToOneConvention : IReferenceConvention
    {
        #region IConvention<IManyToOnePart> Members

        public bool Accept( IManyToOnePart target )
        {
            return true;
        }

        public void Apply( IManyToOnePart target )
        {
            target
                .ColumnName( target.Property.Name + "ID" )
                .SetAttribute( "fetch", "join" );
        }

        #endregion
    }
}