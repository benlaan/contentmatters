using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Laan.ContentMatters.Models
{
    public class Blog : Item
    {
        /// <summary>
        /// Initializes a new instance of the Blog class.
        /// </summary>
        public Blog()
        {
            Posts = new ItemList<Post>();
        }

        public virtual IList<Post> Posts { get; set; }
    }
}
