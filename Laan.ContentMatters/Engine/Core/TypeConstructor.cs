using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Laan.ContentMatters.Models;
using Laan.ContentMatters.Models.Extensions;
using Laan.Utilities.IO;

namespace Laan.ContentMatters.Engine
{
    public class TypeConstructor
    {
        private Dictionary<FieldDefinition, MethodInfo> _setterMethods;

        private AssemblyBuilder _assemblyBuilder;
        private string _fileName;
        private TypeBuilder _currentType;
        private ModuleBuilder _moduleBuilder;
        private string _namespace;

        private Dictionary<string, TypeBuilder> _types;

        private MethodAttributes ConstructorAttr =
            MethodAttributes.HideBySig |
            MethodAttributes.Public |
            MethodAttributes.SpecialName |
            MethodAttributes.RTSpecialName;

        private MethodAttributes MethodAttributes =
            MethodAttributes.Public |
            MethodAttributes.Virtual |
            MethodAttributes.HideBySig |
            MethodAttributes.NewSlot |
            MethodAttributes.SpecialName;

        public TypeConstructor( string @namespace )
            : this( @namespace, null )
        {
        }

        public TypeConstructor( string @namespace, string path )
        {
            _types = new Dictionary<string, TypeBuilder>();
            _namespace = @namespace;
            _setterMethods = new Dictionary<FieldDefinition, MethodInfo>();
            _fileName = @namespace + ".dll";

            var access = path != null ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run;
            path = path ?? null;

            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly( new AssemblyName( @namespace ), access, path );

            if ( !String.IsNullOrEmpty( path ) )
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule( @namespace, _fileName );
            else
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule( @namespace );
        }

        private FieldBuilder CreateBackingField( string fieldName, Type fieldType )
        {
            return _currentType.DefineField( fieldName, fieldType, FieldAttributes.Private );
        }

        private MethodBuilder CreateGetter( FieldBuilder accessField, string propertyName )
        {
            var getter = _currentType.DefineMethod( "get_" + propertyName, MethodAttributes, accessField.FieldType, Type.EmptyTypes );

            var gen = getter.GetILGenerator();
            gen.Emit( OpCodes.Ldarg_0 );
            gen.Emit( OpCodes.Ldfld, accessField );
            gen.Emit( OpCodes.Ret );

            return getter;
        }

        private MethodBuilder CreateSetter( FieldBuilder field, string propertyName )
        {
            var setter = _currentType.DefineMethod( "set_" + propertyName, MethodAttributes, null, new[] { field.FieldType } );

            var gen = setter.GetILGenerator();
            gen.Emit( OpCodes.Ldarg_0 );
            gen.Emit( OpCodes.Ldarg_1 );
            gen.Emit( OpCodes.Stfld, field );
            gen.Emit( OpCodes.Ret );

            return setter;
        }

        private void CreateProperty( FieldDefinition definition )
        {
            var fieldType = GetSystemType( definition, _types );
            var fieldName = "_" + definition.Name.ToJavaCase();

            // backing field
            var accessField = CreateBackingField( fieldName, fieldType );

            // accessors
            var getter = CreateGetter( accessField, definition.Name );
            var setter = CreateSetter( accessField, definition.Name );
            _setterMethods[ definition ] = setter;

            // property, with linked accessors
            var property = _currentType.DefineProperty( definition.Name, PropertyAttributes.None, fieldType, null );
            property.SetGetMethod( getter );
            property.SetSetMethod( setter );
        }

        private TypeBuilder GetReferenceType( string type )
        {
            return GetType( type.Contains( '.' ) ? type : _moduleBuilder.ScopeName + "." + type );
        }

        private TypeBuilder GetType( string referenceType )
        {
            TypeBuilder result;
            if ( _types.TryGetValue( referenceType, out result ) )
                return result;

            throw new Exception( String.Format( "Type Not Found: {0}", referenceType ) );
        }

        private Type GetSystemType( FieldDefinition definition, Dictionary<string, TypeBuilder> types )
        {
            var lookup = new Dictionary<FieldType, Type>()
            {
                { FieldType.Number, typeof(Int32) },
                { FieldType.CheckBox, typeof(bool) },
                { FieldType.Text, typeof(string) },
                { FieldType.Memo, typeof(string) },
                { FieldType.Date, typeof(DateTime) },
                { FieldType.Time, typeof(DateTime) },
                { FieldType.DateTime, typeof(DateTime) },
                { FieldType.Money, typeof(Decimal) },
                { FieldType.Decimal, typeof(Decimal) },
                { FieldType.Percentage, typeof(Decimal) },
                { FieldType.Image, typeof(string) },
                { FieldType.Password, typeof(string) },
                { FieldType.Hidden, typeof(string) }
            };

            if ( definition.FieldType == FieldType.Reference && definition.ReferenceType == null )
                definition.ReferenceType = types[definition.Name].FullName;

            if ( definition.ReferenceType != null )
            {
                var referenceType = GetReferenceType( definition.ReferenceType );

                switch ( definition.FieldType )
                {
                    case FieldType.List:

                        Type listOfType = typeof( IList<> ).MakeGenericType( referenceType );
                        lookup.Add( FieldType.List, listOfType );
                        break;

                    case FieldType.Reference:
                        lookup.Add( FieldType.Reference, referenceType );
                        break;
                }
            }

            Type type;
            if ( lookup.TryGetValue( definition.FieldType, out type ) && type != null )
                return type;
            else
                throw new NotSupportedException( String.Format( "FieldType {0} not supported", definition.FieldType ) );
        }

        public Type AddType( Type baseType, ItemDefinition definition )
        {
            Type type = BuildType( baseType, definition );
            BuildPropertiesAndConstructor( definition );
            return type;
        }

        private Type BuildType( Type baseType, ItemDefinition definition )
        {
            var attr =
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout;

            // create a type in the module
            string typeName = String.Format( "{0}.{1}", definition.Namespace ?? _namespace, definition.Name );
            _currentType = _moduleBuilder.DefineType(
                typeName,
                attr,
                baseType
            );

            _types[typeName] = _currentType;
            return _currentType;
        }

        private void BuildPropertiesAndConstructor( ItemDefinition definition )
        {
            // build the properties
            foreach ( var fieldDefinition in definition.Fields )
                CreateProperty( fieldDefinition );

            CreateConstructor( definition );
        }

        private void CreateConstructor( ItemDefinition definition )
        {
            var ctor = _currentType.DefineConstructor( ConstructorAttr, CallingConventions.Standard, null );

            var gen = ctor.GetILGenerator();

            // call base constructor
            gen.Emit( OpCodes.Ldarg_0 );
            gen.Emit( OpCodes.Call, _currentType.BaseType.GetConstructor( Type.EmptyTypes ) );

            // instantiate an instance of each List field
            foreach ( var def in definition.Fields.Where( fld => fld.FieldType == FieldType.List ) )
            {
                Type referredType = GetReferenceType( def.ReferenceType );
                Type type = typeof( List<> ).MakeGenericType( referredType );
                ConstructorInfo open = typeof( List<> ).GetConstructor( new Type[ 0 ] );
                ConstructorInfo ci   = TypeBuilder.GetConstructor( type, open );

                gen.Emit( OpCodes.Ldarg_0 );
                gen.Emit( OpCodes.Newobj, ci );
                gen.Emit( OpCodes.Callvirt, _setterMethods[def] );
            }

            gen.Emit( OpCodes.Ret );
        }

        public void SaveAssembly()
        {
            _assemblyBuilder.Save( _fileName );
        }

        public void BuildAssemblyFromDefintions( Type baseType, List<ItemDefinition> definitions )
        {
            // define the types
            foreach ( var def in definitions )
                BuildType( baseType, def );

            foreach ( var definition in definitions )
            {
                _currentType = GetReferenceType( definition.Name );
                BuildPropertiesAndConstructor( definition );
            }

            // Actually construct the types
            foreach ( var type in _types )
                type.Value.CreateType();

            SaveAssembly();
        }
    }

}
