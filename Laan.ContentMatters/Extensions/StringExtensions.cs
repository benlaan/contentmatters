using System;
using System.Web;
using System.IO;
using System.Text;

namespace Laan.ContentMatters.Models.Extensions
{
    public static class StringExtensions
    {
        public static string HtmlEncoded( this string value )
        {
            return System.Web.HttpUtility.HtmlEncode( value );
        }

        public static string HtmlDecoded( this string value )
        {
            return System.Web.HttpUtility.HtmlDecode( value );
        }

        public static string ToJavaCase( this string value )
        {
            return value.Substring( 0, 1 ).ToLower() + value.Substring( 1 );
        }
    }
}