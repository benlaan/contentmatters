using System;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Laan.ContentMatters.Tests
{
    public class BaseTestFixture
    {
        protected MockRepository _mock;

        public BaseTestFixture()
        {

        }

        [SetUp]
        public virtual void Setup()
        {
            _mock = new MockRepository();
        }

        protected T Dynamic<T>() where T : class
        {
            return _mock.DynamicMock<T>();
        }
    }
}
