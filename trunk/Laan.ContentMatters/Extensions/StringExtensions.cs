using System;
using System.Web;

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
    }
}
