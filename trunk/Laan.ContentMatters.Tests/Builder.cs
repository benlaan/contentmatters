using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace Laan.Testing
{
    public class Builder<T> where T : new()
    {
        public enum PropertyType
        {
            Field, Property
        }

        public class Property
        {
            public PropertyType Type { get; set; }
            public object Value { get; set; }
        }

        private Dictionary<string, Property> _items;
        private T _instance;

        public Builder( T instance )
        {
            _instance = instance;
            _items = new Dictionary<string, Property>();
        }

        public Builder()
        {
            _instance = new T();
            _items = new Dictionary<string, Property>();
        }

        private static void SetField( object obj, string fieldName, object value )
        {
            FieldInfo field = null;
            for ( var type = obj.GetType(); field == null && type.BaseType != null; type = type.BaseType )
                field = type.GetField( fieldName, BindingFlags.Instance | BindingFlags.NonPublic );

            if ( field == null )
                throw new ArgumentException( String.Format( "Couldn't find field with name '{0}'", fieldName ) );

            field.SetValue( obj, value );
        }

        private static void SetProperty( object obj, string propertyName, object value )
        {
            var property = obj.GetType()
                .GetProperties( BindingFlags.Instance | BindingFlags.Public )
                .Where( p => p.Name == propertyName )
                .LastOrDefault();

            // try non-public as a fallback
            if ( property == null )
                property =
                    obj.GetType()
                    .GetProperties( BindingFlags.Instance | BindingFlags.NonPublic )
                    .Where( p => p.Name == propertyName )
                    .LastOrDefault();

            if ( property == null )
                throw new ArgumentException( String.Format( "Couldn't find property with name '{0}'", propertyName ) );

            try
            {
                property.SetValue( obj, value, null );
            }
            catch ( TargetInvocationException ex )
            {

                // rethrow inner exception instead of TargetInvocationException
                if ( ex.InnerException != null )
                {
                    DiagnosticLog.WriteLine( "Warning: property {0} can not be set to {1}. Exception: {2}", propertyName, value, ex.InnerException.Message );

                    throw ex.InnerException;
                }

                // otherwise just rethrow
                throw; 
            }
        }

        public Builder<T> Set<V>( Expression<Func<T, V>> expression, V value )
        {
            var memberName = ( expression.Body as MemberExpression ).Member.Name;

            _items.Add( memberName, new Property() { Type = PropertyType.Property, Value = value } );
            return this;
        }

        public Builder<T> Set<V>( string propertyName, V value )
        {
            //var memberName = ( expression.Body as MemberExpression ).Member.Name;

            _items.Add( propertyName, new Property() { Type = PropertyType.Property, Value = value } );
            return this;
        }

        public Builder<T> SetField<V>( Expression<Func<T, V>> expression, V value )
        {
            var memberName = ( expression.Body as MemberExpression ).Member.Name;

            _items.Add( memberName, new Property() { Type = PropertyType.Field, Value = value } );
            return this;
        }

        public T Execute()
        {
            foreach ( var keyValue in _items )
            {
                Property property = keyValue.Value;
                switch ( property.Type )
                {
                    case Builder<T>.PropertyType.Field:
                        SetField( _instance, keyValue.Key, property.Value );
                        break;

                    case Builder<T>.PropertyType.Property:
                        SetProperty( _instance, keyValue.Key, property.Value );
                        break;
                }
            }
            return _instance;
        }
    }
}
