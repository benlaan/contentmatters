using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

using Laan.ContentMatters.Models;

namespace Laan.ContentMatters.Engine
{
    public class TypeConstructor
    {
        private static MethodBuilder GetGetter( TypeBuilder tb, string fieldName, FieldBuilder accessField )
        {
            var getter = tb.DefineMethod( "get" + fieldName, /*MethodAttributes.Virtual |*/ MethodAttributes.Public );
            var gen = getter.GetILGenerator();

            gen.Emit( OpCodes.Ldarg_0 );
            gen.Emit( OpCodes.Ldfld, accessField );
            gen.Emit( OpCodes.Ret );

            return getter;
        }

        private static MethodBuilder GetSetter( TypeBuilder tb, string fieldName, FieldBuilder accessField )
        {
            var setter = tb.DefineMethod( "set" + fieldName, /*MethodAttributes.Virtual |*/ MethodAttributes.Public );

            var gen = setter.GetILGenerator();
            gen.Emit( OpCodes.Ldarg_0 );
            gen.Emit( OpCodes.Ldarg_1 );
            gen.Emit( OpCodes.Stfld, accessField );
            gen.Emit( OpCodes.Ret );

            return setter;
        }

        private static FieldBuilder GetAccessField( TypeBuilder tb, Type fieldType, string fieldName )
        {
            return tb.DefineField( fieldName, fieldType, FieldAttributes.Private );
        }

        public static Type Create( Laan.ContentMatters.Models.ItemDefinition def )
        {
            AppDomain domain = AppDomain.CurrentDomain;

            AssemblyBuilder ab = domain.DefineDynamicAssembly
            (
                new AssemblyName( def.Namespace ),
                AssemblyBuilderAccess.Run
            );

            // create a new module to hold code in the assembly
            ModuleBuilder mb = ab.DefineDynamicModule( def.Namespace );

            var attr =
                TypeAttributes.Public | TypeAttributes.Class |
                TypeAttributes.AutoClass | TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout;

            // create a type in the module
            TypeBuilder tb = mb.DefineType( def.Name, attr, typeof( Item ) );

            // build the properties
            foreach ( var field in def.Fields )
            {
                Type fieldType = field.ToSystemType();

                string fieldName = String.Format(
                    "_{0}{1}",
                    field.Name.Substring( 0, 1 ).ToLower(), field.Name.Substring( 1 )
                );

                var accessField = GetAccessField( tb, fieldType, fieldName );
                var getter = GetGetter( tb, fieldName, accessField );
                var setter = GetSetter( tb, fieldName, accessField );

                var property = tb.DefineProperty( fieldName, PropertyAttributes.None, fieldType, null );
                property.SetGetMethod( getter );
                property.SetSetMethod( setter );
            }

            // finish creating the type and make it available
            return tb.CreateType();
        }
    }
}
