using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using MbUnit.Framework;
using Laan.ContentMatters.Models;
using Laan.ContentMatters.Engine;
using System.Reflection;
using System.IO;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class ItemDefinitionTest
    {

        private void VerifyTypeProperty( object supplier, string name, Type type )
        {
            System.Reflection.PropertyInfo[] props = supplier.GetType().GetProperties();
            var prop = props.FirstOrDefault( p => p.Name == name );
            Assert.IsTrue( prop != null );
            Assert.IsTrue( prop.PropertyType == type );
        }

        private void SetValue( object supplier, string propertyName, string value )
        {
            var prop = supplier.GetType().GetProperty( propertyName );
            prop.SetValue( supplier, value, null );
        }

        private void VerifyPropertyValue( object supplier, string propertyName, object expected )
        {
            var actual = supplier.GetType().GetProperty( propertyName ).GetValue( supplier, null );
            Assert.AreEqual( expected, actual );
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
            var constructor = new TypeConstructor( def.Namespace );
            Type supplierType = constructor.AddType( typeof( Item ), def );

            object supplier = Activator.CreateInstance( supplierType );

            // Verify outcome
            Assert.IsNotNull( supplier );

            VerifyTypeProperty( supplier, "Name", typeof( string ) );
            VerifyTypeProperty( supplier, "Address", typeof( string ) );
            VerifyTypeProperty( supplier, "LastUsed", typeof( DateTime ) );
        }

        [Test]
        public void Test_Can_Construct_Item_By_Definition_And_Set_Value()
        {
            // Setup
            var def = new ItemDefinition();
            def.Name = "Supplier";
            def.Namespace = "Laan.Test.Model";
            def.Description = "A list of corporate suppliers";
            def.Fields.Add( new FieldDefinition() { Name = "Name", FieldType = FieldType.Text } );

            // Exercise
            var constructor = new TypeConstructor( def.Namespace );
            Type supplierType = constructor.AddType( typeof( Item ), def );

            object supplier = Activator.CreateInstance( supplierType );
            SetValue( supplier, "Name", "Ben Laan" );

            // Verify outcome
            Assert.IsNotNull( supplier );

            VerifyPropertyValue( supplier, "Name", "Ben Laan" );
        }

        [Test]
        public void Test_Can_Construct_Item_By_Definition_And_Set_Value_As_Reference_Type()
        {
            string path = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
            var constructor = new TypeConstructor( "Laan.Test.Model", path );

            // Setup
            var cityDef = new ItemDefinition
            {
                Name = "City",
                Namespace = "Laan.Test.Model",
                Description = "a city"
            };
            cityDef.Fields.Add( new FieldDefinition() { Name = "Name", FieldType = FieldType.Text } );
            cityDef.Fields.Add( new FieldDefinition() { Name = "Population", FieldType = FieldType.Number } );

            // Exercise
            //String.Format( "{0}\\{1}.dll", path, def.Namespace ) 
            var cityType = constructor.AddType( typeof( Item ), cityDef );

            // Setup
            var supplierDef = new ItemDefinition
            {
                Name = "Supplier",
                Namespace = "Laan.Test.Model",
                Description = "A list of corporate suppliers"
            };
            supplierDef.Fields.Add(
                new FieldDefinition()
                {
                    Name = "City",
                    FieldType = FieldType.Lookup,
                    ReferenceType = cityType
                }
            );

            // Exercise
            var supplierType = constructor.AddType( typeof( Item ), supplierDef );

            //constructor.SaveAssembly();

            object supplier = Activator.CreateInstance( supplierType );

            // Verify outcome
            Assert.IsNotNull( supplier );
            Assert.AreEqual( cityType, supplier.GetType().GetProperty( "City" ).PropertyType );
        }
    }
}
