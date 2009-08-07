using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Text;

namespace Laan.ContentMatters.Models
{
    public static class HtmlHelper
    {

        private static string BuildTag( string tag, RouteValueDictionary attributes )
        {
            var html = new StringBuilder( String.Format( "<{0}", tag ) );
            foreach ( var attribute in attributes )
            {
                html.AppendFormat( " {0}=\"{1}\"", attribute.Key, attribute.Value );
            }
            html.Append( "/>" );

            return html.ToString();
        }

        public static string TextBox( string name, object attributes )
        {
            var dictionary = new RouteValueDictionary( attributes );
            dictionary.Add( "type", "text" );

            return Input( name, dictionary );
        }

        public static string CheckBox( string name, object attributes )
        {
            var dictionary = new RouteValueDictionary( attributes );
            dictionary.Add( "type", "checkbox" );

            return Input( name, dictionary );
        }

        public static string Input( string name, RouteValueDictionary attributes )
        {
            return BuildTag( "input", attributes );
        }

        public static string TextArea( string name, object attributes )
        {
            return BuildTag( "textarea", new RouteValueDictionary( attributes ) );
        }

        public static string ToViewHtml( string name, object value )
        {
            return value.ToString();
        }

        public static string ToEditHtml( FieldDefinition def, object value )
        {
            switch ( def.FieldType )
            {
                case FieldType.Number:
                    return TextBox( def.Name, value );

                case FieldType.CheckBox:
                    return CheckBox( def.Name, value );

                case FieldType.Text:
                    return TextBox( def.Name, value );

                case FieldType.Memo:
                    return TextArea( def.Name, value );

                //case FieldType.Lookup:
                //    return Select( def.Name, value );

                case FieldType.Money:
                    return TextBox( def.Name, value );

                case FieldType.Decimal:
                    return TextBox( def.Name, value );

                case FieldType.Percentage:
                    return TextBox( def.Name, value );

                default:
                    throw new NotSupportedException( String.Format( "FieldType not supported: {0}", def.FieldType ) );
            }
        }
    }
}
