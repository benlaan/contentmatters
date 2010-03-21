using System;
using System.Collections.Generic;

namespace Laan.ContentMatters.Models.Custom
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
