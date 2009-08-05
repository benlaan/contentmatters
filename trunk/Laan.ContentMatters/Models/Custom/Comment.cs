using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Laan.ContentMatters.Models
{
    public class Comment : Item
    {
        public virtual string Body { get; set; }
    }
}
