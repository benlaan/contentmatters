using System;

using MbUnit.Framework;
using Laan.ContentMatters.Configuration;
using System.Text;
using Laan.ContentMatters.Tests;
using Laan.ContentMatters.Loaders;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class ViewLoaderTest
    {
        private ViewLoader _viewLoader;

        [SetUp]
        public void Setup()
        {
            _viewLoader = new ViewLoader( null );
        }

        [Test]
        public void Can_Create_ViewLoader()
        {
            
        }

        [Test]
        public void Can_Output_View_Without_Zones()
        {
            View view = _viewLoader.Load( @"Home\welcome" );
            Assert.IsNotNull( view );

            var expected = new[] 
            {
                "<view id=\"welcome\">",
                "    <h1>Welcome to CuMulouS</h1>",
                "    <p>by B. Laan</p>",
                "    <br/>",
                "    <div style=\"display:inline;float:left;width:70%\">",
                "        <p>Content Management made easy..</p>",
                "    </div>",
                "    <div style=\"width:30%\">",
                "        <img src=\"welcome.jpg\"/>",
                "    </div>",
                "</view>",
                ""
            };

            expected.Compare( view.Body );
        }
    }
}
