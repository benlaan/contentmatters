using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.IO;

using Laan.ContentMatters.Models;
using Laan.Utilities.IO;

namespace Laan.ContentMatters.Engine
{
    public class TypeConstructor
    {
        private AssemblyBuilder _assemblyBuilder;
        private string _fileName;
        private TypeBuilder _builder;
        private ModuleBuilder _moduleBuilder;

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
            _fileName = @namespace + ".dll";
            var access = path != null ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run;
            path = !String.IsNullOrEmpty( path ) ? Path.GetDirectoryName( path ) : null;

            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly( new AssemblyName( @namespace ), access, path );

            if ( !String.IsNullOrEmpty( path ) )
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule( @namespace, _fileName );
            else
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule( @namespace );
        }

        private FieldBuilder CreateField( TypeBuilder tb, Type fieldType, string fieldName )
        {
            return tb.DefineField( fieldName, fieldType, FieldAttributes.Private );
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
            var fieldType = definition.ToSystemType();
            var fieldName = String.Format( "_{0}{1}", definition.Name.Substring( 0, 1 ).ToLower(), definition.Name.Substring( 1 ) );

            // backing field
            var accessField = CreateField( _builder, fieldType, fieldName );

            // accessors
            var getter = CreateGetter( accessField );
            var setter = CreateSetter( accessField );

            // property, with linked accessors
            var property = _builder.DefineProperty( definition.Name, PropertyAttributes.None, fieldType, null );
            property.SetGetMethod( getter );
            property.SetSetMethod( setter );
        }

        public Type AddType( Type baseType, ItemDefinition itemDefinition )
        {
            var attr =
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout;

            // create a type in the module
            _builder = _moduleBuilder.DefineType( String.Format( "{0}.{1}", itemDefinition.Namespace, itemDefinition.Name ), attr, baseType );

            // build the properties
            itemDefinition.Fields.ForEach( CreateProperty );

            return _builder.CreateType();
        }

        public void SaveAssembly()
        {
            _assemblyBuilder.Save( _fileName );
        }
    }
}
