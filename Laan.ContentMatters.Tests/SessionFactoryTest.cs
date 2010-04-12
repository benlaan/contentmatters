using System;

using MbUnit.Framework;

using Rhino.Mocks;
using NHibernate.Cfg;
using Castle.Windsor;
using NHibernate;

namespace Laan.ContentMatters.Tests
{
    [Ignore]
    [TestFixture]
    public class SessionFactoryTest
    {
        private IWindsorContainer _container;

        private void Measure( Action action, TimeSpan max )
        {
            Assert.Between( Measure( action ), new TimeSpan( 0 ), max );
        }

        private TimeSpan Measure( Action action )
        {
            TimeSpan result;
            DateTime start = DateTime.Now;
            try
            {
                action();
            }
            finally
            {
                result = DateTime.Now - start;
            }
            return result;
        }

        [SetUp]
        public void Setup()
        {
            IoC.ConfigPath = @".\castle.config";
        }

        [Test]
        public void Can_Create_Session_Factory()
        {
            // Setup
            ISessionFactory factory = IoC.Resolve<ISessionFactory>();

            // Verify outcome
            Assert.IsNotNull( factory );
        }

        [Test]
        public void Can_Get_Different_Session_Factory_After_Releasing_It()
        {
            // Setup
            Measure( () => _container = IoC.Load(), new TimeSpan( 0, 0, 10 ) );
            ISessionFactory factory1 = IoC.Container.Resolve<ISessionFactory>();

            // Exercise
            Measure( () => _container = IoC.Rebuild(), new TimeSpan( 0, 0, 10 ) );

            ISessionFactory factory2 = IoC.Container.Resolve<ISessionFactory>();

            // Verify outcome
            Assert.AreNotSame( factory1, factory2 );
        }
    }
}
