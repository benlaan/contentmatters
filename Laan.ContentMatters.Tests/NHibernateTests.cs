using System;
using MbUnit.Framework;
using Rhino.Mocks;
using Laan.ContentMatters.Models.Custom;
using Laan.Persistence.Interfaces;
using NHibernate;
using Laan.Persistence;
using Castle.Core.Logging;
using NHibernate.Tool.hbm2ddl;
using System.Collections.Generic;
using Laan.ContentMatters.Engine.Data;
using Laan.ContentMatters.Configuration;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class NHibernateTests : BaseTestFixture
    {
        private ISessionFactory _factory;

        [SetUp]
        public override void Setup()
        {
            var container = IoC.Container;
            _factory = container.Resolve<ISessionFactory>();
        }

        [Test]
        public void Can_Get_All_Blogs()
        {
            using ( var session = _factory.OpenSession() )
            {
                var repository = new Repository<Blog>( NullLogger.Instance, _factory );
                Assert.IsNotNull( repository );

                var blog = repository.FindAll( new Dictionary<string, object> { { "TypeName", "Blog" } } );
                Assert.IsNotNull( blog );
            }
        }

        [Test]
        public void Can_Get_All_Blogs_Using_DataProviderRepository()
        {
            using ( var session = _factory.OpenSession() )
            {
                var repository = new DataProviderRepository<Blog>( NullLogger.Instance, _factory );
                Assert.IsNotNull( repository );

                DataSource data = new DataSource { Type = typeof( Blog ).FullName, Select = SelectionMode.All };
                var blog = repository.SelectAll( null, data );
                Assert.IsNotNull( blog );
            }

            // DataProvider
        }

        [Test]
        public void Can_Get_All_Blogs_Using_DataProvider()
        {
            using ( var session = _factory.OpenSession() )
            {
                DataDictionary data = new DataDictionary();
                var repository = new DataProvider( NullLogger.Instance, _factory, data );
                Assert.IsNotNull( repository );

                DataSource dataSource = new DataSource { Name = "blog", Type = "blog", Select = SelectionMode.All };
                Page page = new Page();
                page.DataSources.Add( dataSource );
                var blog = repository.Build( page );
                Assert.IsNotNull( blog );
                Assert.IsTrue( data.Keys.Contains( "blog" ) );
            }
        }

        [Test]
        public void Can_Get_All_Blogs_Using_DataProvider_With_Order_By()
        {
            using ( var session = _factory.OpenSession() )
            {
                DataDictionary data = new DataDictionary();
                var repository = new DataProvider( NullLogger.Instance, _factory, data );
                Assert.IsNotNull( repository );

                DataSource dataSource = new DataSource { Name = "blog", Type = "blog", Select = SelectionMode.All, Order = "Created" };
                Page page = new Page();
                page.DataSources.Add( dataSource );
                var blog = repository.Build( page );
                Assert.IsNotNull( blog );
                Assert.IsTrue( data.Keys.Contains( "blog" ) );
            }
        }
    }

}
