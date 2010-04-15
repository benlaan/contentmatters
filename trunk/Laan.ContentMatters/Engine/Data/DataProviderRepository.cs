using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using Laan.ContentMatters.Configuration;
using Laan.Persistence;
using NHibernate;
using NHibernate.Criterion;

namespace Laan.ContentMatters.Engine.Data
{
    public class DataProviderRepository<T> : Repository<T> where T : Laan.ContentMatters.Models.Item
    {
        public DataProviderRepository( ILogger logger, ISessionFactory factory )
            : base( logger, factory )
        {

        }

        private ICriteria ApplyTop( DataSource data, ICriteria criteria )
        {
            return data.Top > 0 ? criteria.SetMaxResults( data.Top ) : criteria;
        }

        private ICriteria ApplySortOrder( DataSource data, ICriteria criteria )
        {
            if ( data.Order != null )
            {
                string[] orderParts = data.Order.Split( ' ' );

                for ( int index = 0; index < orderParts.Length; index++ )
                {
                    Order order;
                    if ( index + 1 >= orderParts.Length )
                        order = Order.Asc( orderParts[index] );
                    else
                    {
                        string direction = orderParts[index + 1];
                        string field = direction == "asc" ? orderParts[index++] : orderParts[index];
                        order = Order.Desc( field );
                    }
                    criteria = criteria.AddOrder( order );
                }
            }

            return criteria;
        }

        private ICriteria GetCriteria( DataSource data )
        {
            var criteria = Session.CreateCriteria<T>();
            criteria = ApplyTop( data, criteria );
            return ApplySortOrder( data, criteria );
        }

        public IList<T> SelectAll( Page page, DataSource data )
        {
            ICriteria criteria = GetCriteria( data );
            return criteria.List<T>();
        }

        public IList<T> SelectParent( Page page, DataSource data )
        {
            ICriteria criteria = GetCriteria( data );
            criteria = criteria.Add( Expression.Eq( "ParentID", page.Parent.Key ) );
            return criteria.List<T>();
        }

        public IList<T> SelectKey( Page page, DataSource data )
        {
            ICriteria criteria = GetCriteria( data );
            criteria = criteria.Add( Expression.Eq( "Description", page.Key ) );
            return criteria.List<T>();
        }

        public IList<T> SelectRandom( Page page, DataSource data )
        {
            data.Order = "NEWID()";
            ICriteria criteria = GetCriteria( data );
            criteria = criteria.Add( Expression.Eq( "Description", page.Key ) );
            return criteria.List<T>();
        }
    }
}
