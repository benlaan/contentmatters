using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Laan.ContentMatters.Models;

namespace Laan.ContentMatters.Models
{
    public class Post : Item
    {
        /// <summary>
        /// Initializes a new instance of the Post class.
        /// </summary>
        public Post()
        {
            Comments = new ItemList<Comment>();
        }

        public virtual IList<Comment> Comments { get; set; }
        public virtual string Body { get; set; }
    }
}
