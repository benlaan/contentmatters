using System;
using System.Collections.Generic;

using Castle.Core.Logging;

using Laan.ContentMatters.Configuration;
using Laan.ContentMatters.Models;
using Laan.ContentMatters.Tests;
using Laan.ContentMatters.Engine.Data;

using MbUnit.Framework;

using NHibernate;
using NHibernate.Criterion;

using Rhino.Mocks;
using Laan.ContentMatters.Engine.Interfaces;

namespace Laan.ContentMatters.Tests
{
    public class TestItem : Item
    {
        public TestItem()
        {
            
        }
    }

    [TestFixture]
    public class DataProviderTest
    {
        private DataProvider _dataProvider;
        private ISessionFactory _sessionFactory;
        private MockRepository _mock;

        [SetUp]
        public void Setup()
        {
            _mock = new MockRepository();
            _sessionFactory = _mock.DynamicMock<ISessionFactory>();
        }

        [Test]
        public void Test()
        {
            ISession session = _mock.DynamicMock<ISession>();
            ICriteria criteria = _mock.DynamicMock<ICriteria>();
            using ( _mock.Record() )
            {
                Expect.Call( _sessionFactory.OpenSession() ).Return( session ).Repeat.Any();
                Expect.Call( session.CreateCriteria<TestItem>() ).Return( criteria ).Repeat.Once();
                Expect.Call( criteria.List<TestItem>() ).Return( new List<TestItem> { new TestItem() } ).Repeat.Once();
                Expect.Call( criteria.AddOrder( Order.Asc( "name" ) ) ).IgnoreArguments().Return( criteria ).Repeat.Once();
            }

            Page page = new Page();
            page.DataSources.Add( 
                new DataSource() 
                { 
                    Name = "blogs", 
                    Type = "Laan.ContentMatters.Tests.TestItem, Laan.ContentMatters.Tests", 
                    Order = "Name",
                    Select = SelectionMode.All 
                }
            );
            SitePage sitePage = new SitePage();
            sitePage.Page = page;

            IDataDictionary data;
            using ( _mock.Playback() )
            {
                _dataProvider = new DataProvider( NullLogger.Instance, _sessionFactory, new DataDictionary( true ), null );
                data = _dataProvider.Build( sitePage );
            }
            Assert.IsNotNull( data );
            //Assert.AreEqual( 1, data.Count );
            //Assert.IsTrue( data.ContainsKey( "blogs" ) );
            Assert.IsTrue( data["blogs"] is IList<TestItem> );
            IList<TestItem> blogs = (IList<TestItem>)data["blogs"];
            Assert.AreEqual( 1, blogs.Count );
        }
    }
}