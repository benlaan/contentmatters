using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using Laan.ContentMatters.Configuration;
using Laan.Persistence;
using NHibernate;
using NHibernate.Criterion;
using Laan.ContentMatters.Engine.Interfaces;
using Laan.ContentMatters.Models;

namespace Laan.ContentMatters.Engine.Data
{
    public class DataProviderRepository<T> : Repository<T> where T : Laan.ContentMatters.Models.Item
    {
        private const string AscendingSortOrder = "asc";

        private IDefinitionService _definitionService;

        public DataProviderRepository( ILogger logger, ISessionFactory factory, IDefinitionService definitionService )
            : base( logger, factory )
        {
            _definitionService = definitionService;
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
                        string field = direction == AscendingSortOrder ? orderParts[index++] : orderParts[index];
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

        public IList<T> SelectAll( SitePage page, DataSource data )
        {
            ICriteria criteria = GetCriteria( data );
            return criteria.List<T>();
        }

        public IList<T> SelectParent( SitePage page, DataSource data )
        {
            var defs = _definitionService.LoadDefinitions();
            ItemDefinition itemDefinition = defs.First( d => String.Compare( d.Name, data.Type, true ) == 0 );
 
            ICriteria criteria = GetCriteria( data );
            SitePage sitePage = page;
            while ( sitePage != null )
            {
                if (sitePage.Key != null)
                    criteria = criteria.Add( Expression.InsensitiveLike( "Title", sitePage.Key ) );

                sitePage = sitePage.Parent;
                if (sitePage == null)
                    break;

                itemDefinition = defs.FirstOrDefault( d => String.Compare( d.Name, itemDefinition.Parent, true ) == 0 );
                if ( itemDefinition == null )
                    break;

                criteria = criteria.CreateCriteria( itemDefinition.Name );
            }
            return criteria.List<T>();
        }

        public IList<T> SelectKey( SitePage page, DataSource data )
        {
            ICriteria criteria = GetCriteria( data );
            criteria = criteria.Add( Expression.Eq( "Description", page.Key ) );
            return criteria.List<T>();
        }

        public IList<T> SelectRandom( SitePage page, DataSource data )
        {
            data.Order = "NEWID()";
            ICriteria criteria = GetCriteria( data );
            criteria = criteria.Add( Expression.Eq( "Description", page.Key ) );
            return criteria.List<T>();
        }
    }
}
