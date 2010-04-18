using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Laan.ContentMatters.Engine;
using Laan.ContentMatters.Models;
using MbUnit.Framework;
using Laan.ContentMatters.Engine.Services;
using Laan.Persistence.Interfaces;
using System.Collections.Generic;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class DefinitionServiceTest : BaseTestFixture
    {
        [Test]
        public void Ensure_When_Definition_Is_Defined_With_A_Parent_That_Doesnt_Exist_Then_It_Gets_Added()
        {
            var mapper = Dynamic<IMapper>();
            var service = new DefinitionService( mapper );

            var defs = new List<ItemDefinition>();
            var blog = new ItemDefinition { Name = "Blog" };
            blog.Fields.Add(
                new FieldDefinition
                {
                    FieldType = FieldType.List,
                    ReferenceType = "Post",
                    Name = "Posts"
                }
            );
            defs.Add( blog );

            var post = new ItemDefinition { Name = "Post", Parent = "Blog" };
            post.Fields.Add(
                new FieldDefinition
                {
                    Name = "Body",
                    FieldType = FieldType.Memo
                }
            );
            defs.Add( post );

            service.EnsureDefinitionsHasParentProperty( defs );

            Assert.AreEqual( 2, post.Fields.Count );
            Assert.IsTrue( post.Fields.Any( fd => fd.Name == "Blog" ) );
        }
    }
}
