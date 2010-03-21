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
        private TypeBuilder _builder;
        private ModuleBuilder _moduleBuilder;
        private string _namespace;

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

        public TypeConstructor( string @namespace ) : this( @namespace, null )
        {
        }

        public TypeConstructor( string @namespace, string path )
        {
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
            return _builder.DefineField( fieldName, fieldType, FieldAttributes.Private );
        }

        private MethodBuilder CreateGetter( FieldBuilder accessField )
        {
            var getter = _builder.DefineMethod( "get" + accessField.Name, MethodAttributes, accessField.FieldType, Type.EmptyTypes );

            var gen = getter.GetILGenerator();
            gen.Emit( OpCodes.Ldarg_0 );
            gen.Emit( OpCodes.Ldfld, accessField );
            gen.Emit( OpCodes.Ret );

            return getter;
        }

        private MethodBuilder CreateSetter( FieldBuilder field )
        {
            var setter = _builder.DefineMethod( "set" + field.Name, MethodAttributes, null, new[] { field.FieldType } );

            var gen = setter.GetILGenerator();
            gen.Emit( OpCodes.Ldarg_0 );
            gen.Emit( OpCodes.Ldarg_1 );
            gen.Emit( OpCodes.Stfld, field );
            gen.Emit( OpCodes.Ret );

            return setter;
        }

        private void CreateProperty( FieldDefinition definition )
        {
            var fieldType = definition.ToSystemType( _moduleBuilder );
            var fieldName = "_" + definition.Name.ToJavaCase();

            // backing field
            var accessField = CreateBackingField( fieldName, fieldType );

            // accessors
            var getter = CreateGetter( accessField );
            var setter = CreateSetter( accessField );
            _setterMethods[ definition ] = setter;

            // property, with linked accessors
            var property = _builder.DefineProperty( definition.Name, PropertyAttributes.None, fieldType, null );
            property.SetGetMethod( getter );
            property.SetSetMethod( setter );
        }

        public Type AddType( Type baseType, ItemDefinition definition )
        {
            var attr =
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout;

            // create a type in the module
            _builder = _moduleBuilder.DefineType(
                String.Format( "{0}.{1}", definition.Namespace ?? _namespace, definition.Name ),
                attr,
                baseType
            );

            // build the properties
            foreach ( var fieldDefinition in definition.Fields )
                CreateProperty( fieldDefinition );

            CreateConstructor( definition );

            return _builder.CreateType();
        }

        private void CreateConstructor( ItemDefinition definition )
        {
            var ctor = _builder.DefineConstructor( ConstructorAttr, CallingConventions.Standard, null );

            var gen = ctor.GetILGenerator();

            // call base constructor
            gen.Emit( OpCodes.Ldarg_0 );
            gen.Emit( OpCodes.Call, _builder.BaseType.GetConstructor( Type.EmptyTypes ) );

            // instantiate an instance of each List field
            foreach ( var def in definition.Fields.Where( fld => fld.FieldType == FieldType.List ) )
            {
                Type referredType = _moduleBuilder.GetType( def.GetReferenceType( _moduleBuilder ), true, false );
                Type type = typeof( List<> ).MakeGenericType( referredType );

                gen.Emit( OpCodes.Ldarg_0 );
                gen.Emit( OpCodes.Newobj, type.GetConstructor( Type.EmptyTypes ) );
                gen.Emit( OpCodes.Callvirt, _setterMethods[ def ] );
            }

            gen.Emit( OpCodes.Ret );
        }

        public void SaveAssembly()
        {
            _assemblyBuilder.Save( _fileName );
        }

        public void BuildAssemblyFromDefintions( Type baseType, List<ItemDefinition> definitions )
        {
            foreach ( var def in definitions )
                AddType( baseType, def );

            SaveAssembly();
        }
    }
}
