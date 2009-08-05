using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.IO;
using System.Text;

namespace Laan.ContentMatters.Models.Files
{
    public class BaseItem
    {
        public string Location { get; set; }

        public string GetLinkList()
        {
            string result = "";
            string[] parts = Location.Split( new[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries );

            string subdirectory = "";

            foreach ( string part in parts.Take( parts.Length - 1 ) )
            {
                subdirectory += part + "/";

                string link = String.Format( "/File/List/{0}", subdirectory );

                result += String.Format( "{0}<a href=\"{1}\">{2}</a>",
                    result.Length > 0 ? " / " : "",
                    link,
                    part
                );
            }
            result += " / " + parts.Last();

            return result;
        }
    }
}
