using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using MbUnit.Framework;
using Laan.ContentMatters.Models;
using Laan.ContentMatters.Engine;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class ItemDefinitionTest
    {

        private void VerifyTypeProperty( object supplier, string name, Type type )
        {
            System.Reflection.PropertyInfo[] props = supplier.GetType().GetProperties();
            Assert.IsTrue( props.Any( p => p.Name == name ) );
            Assert.IsTrue( props.Any( p => p.GetType() == type ) );
        }

        [Test]
        public void Test_Can_Construct_Item_By_Definition()
        {
            // Setup
            var def = new ItemDefinition();
            def.Name = "Supplier";
            def.Namespace = "Laan.Test.Model";
            def.Description = "A list of corporate suppliers";
            def.Fields.Add( new FieldDefinition() { Name = "Name", FieldType = FieldType.Text } );
            def.Fields.Add( new FieldDefinition() { Name = "Address", FieldType = FieldType.Memo } );
            def.Fields.Add( new FieldDefinition() { Name = "LastUsed", FieldType = FieldType.DateTime } );

            // Exercise
            Type supplierType = TypeConstructor.Create( def );

            object supplier = Activator.CreateInstance( supplierType );

            // Verify outcome
            Assert.IsNotNull( supplier );

            VerifyTypeProperty( supplier, "Name", typeof( string ) );
            VerifyTypeProperty( supplier, "Address", typeof( string ) );
            VerifyTypeProperty( supplier, "LastUsed", typeof( DateTime ) );
        }
    }
}

