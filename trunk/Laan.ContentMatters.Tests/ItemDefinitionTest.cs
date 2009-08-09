using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Laan.ContentMatters.Engine;
using Laan.ContentMatters.Models;

using MbUnit.Framework;

namespace Laan.ContentMatters.Tests
{
    [TestFixture]
    public class ItemDefinitionTest
    {
        private const string Namespace = "Laan.Test.Model";

        private TypeConstructor _constructor;

        #region Utilities

        private void VerifyTypeProperty( object supplier, string name, Type type )
        {
            System.Reflection.PropertyInfo[] props = supplier.GetType().GetProperties();
            var prop = props.FirstOrDefault( p => p.Name == name );
            Assert.IsTrue( prop != null );
            Assert.IsTrue( prop.PropertyType == type );
        }

        private void SetValue( object supplier, string propertyName, object value )
        {
            var prop = supplier.GetType().GetProperty( propertyName );
            prop.SetValue( supplier, value, null );
        }

        private void VerifyPropertyValue( object supplier, string propertyName, object expected )
        {
            var actual = supplier.GetType().GetProperty( propertyName ).GetValue( supplier, null );
            Assert.AreEqual( expected, actual );
        }

        private ItemDefinition GetSupplierDefinition()
        {
            var def = new ItemDefinition();
            def.Name = "Supplier";
            def.Namespace = Namespace;
            def.Description = "A list of corporate suppliers";
            def.Fields.Add( new FieldDefinition() { Name = "Name", FieldType = FieldType.Text } );
            def.Fields.Add( new FieldDefinition() { Name = "Age", FieldType = FieldType.Number } );
            def.Fields.Add( new FieldDefinition() { Name = "BirthDate", FieldType = FieldType.DateTime } );
            return def;
        }

        private ItemDefinition GetCityDefinition()
        {
            var def = new ItemDefinition();
            def.Name = "City";
            def.Namespace = Namespace;
            def.Description = "a city";
            def.Fields.Add( new FieldDefinition() { Name = "Name", FieldType = FieldType.Text } );
            def.Fields.Add( new FieldDefinition() { Name = "Population", FieldType = FieldType.Number } );
            return def;
        }

        #endregion

        [SetUp]
        public void Setup()
        {
            string path = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
            _constructor = new TypeConstructor( Namespace, path );
        }

        [Test]
        public void Can_Construct_Item_By_Definition()
        {
            // Setup
            ItemDefinition def = GetSupplierDefinition();

            // Exercise
            Type supplierType = _constructor.AddType( typeof( Item ), def );
            object supplier = Activator.CreateInstance( supplierType );

            // Verify outcome
            Assert.IsNotNull( supplier );

            VerifyTypeProperty( supplier, "Name", typeof( string ) );
            VerifyTypeProperty( supplier, "Age", typeof( Int32 ) );
            VerifyTypeProperty( supplier, "BirthDate", typeof( DateTime ) );
        }

        [Test]
        public void Can_Construct_Item_By_Definition_And_Set_Value()
        {
            // Setup
            var def = GetSupplierDefinition();

            // Exercise
            Type supplierType = _constructor.AddType( typeof( Item ), def );
            object supplier = Activator.CreateInstance( supplierType );
            SetValue( supplier, "Name", "Ben Laan" );
            SetValue( supplier, "Age", 21 );
            SetValue( supplier, "BirthDate", DateTime.Parse( "25/12/2009" ) );
            

            // Verify outcome
            Assert.IsNotNull( supplier );
            VerifyPropertyValue( supplier, "Name", "Ben Laan" );
            VerifyPropertyValue( supplier, "Age", 21 );
            VerifyPropertyValue( supplier, "BirthDate", DateTime.Parse( "25/12/2009" ) );
        }

        [Test]
        public void Can_Construct_Item_By_Definition_And_Set_Value_As_Reference_Type()
        {
            // Setup
            ItemDefinition cityDef = GetCityDefinition();
            var supplierDef = GetSupplierDefinition();
            var cityType = _constructor.AddType( typeof( Item ), cityDef );
            supplierDef.Fields.Add( new FieldDefinition { Name = "City", FieldType = FieldType.Lookup, ReferenceType = "Laan.Test.Model.City" } );
   
            // Exercise
            var supplierType = _constructor.AddType( typeof( Item ), supplierDef );
            object supplier = Activator.CreateInstance( supplierType );

            //constructor.SaveAssembly();

            // Verify outcome
            Assert.IsNotNull( supplier );
            Assert.AreEqual( cityType, supplier.GetType().GetProperty( "City" ).PropertyType );
        }

        [Test]
        public void Can_Construct_Item_By_Definition_And_Set_Value_As_Reference_Type_Using_Implicit_Reference_Type()
        {
            // Setup
            ItemDefinition cityDef = GetCityDefinition();
            var supplierDef = GetSupplierDefinition();
            var cityType = _constructor.AddType( typeof( Item ), cityDef );
            supplierDef.Fields.Add( new FieldDefinition { Name = "City", FieldType = FieldType.Lookup } );

            // Exercise
            var supplierType = _constructor.AddType( typeof( Item ), supplierDef );
            object supplier = Activator.CreateInstance( supplierType );

            //constructor.SaveAssembly();

            // Verify outcome
            Assert.IsNotNull( supplier );
            Assert.AreEqual( cityType, supplier.GetType().GetProperty( "City" ).PropertyType );
        }
    }
}
