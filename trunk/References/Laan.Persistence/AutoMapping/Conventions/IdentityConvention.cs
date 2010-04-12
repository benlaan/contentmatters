using System;
using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Mapping;

namespace Laan.Persistence.AutoMapping
{
    public class IdentityConvention : IIdConvention
    {

        public IdentityConvention()
        {

        }

        #region IConvention<IIdentityPart> Members

        public bool Accept( IIdentityPart target )
        {
            return true;
        }

        public void Apply( IIdentityPart target )
        {
            target.GeneratedBy.Native();
        }

        #endregion
    }
}
