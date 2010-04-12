using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Mapping;

namespace Laan.Persistence.AutoMapping
{
    public class OneToManyConvention : IHasManyConvention
    {

        public OneToManyConvention()
        {

        }

        #region IConvention<IOneToManyPart> Members

        public bool Accept( IOneToManyPart target )
        {
            return true;
        }

        public void Apply( IOneToManyPart target )
        {
            target.Cascade.SaveUpdate();
        }

        #endregion
    }
}
