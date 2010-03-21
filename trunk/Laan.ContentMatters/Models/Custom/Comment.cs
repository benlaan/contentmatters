using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Laan.ContentMatters.Models.Custom
{
    public class Comment : Item
    {
        public virtual string Body { get; set; }
    }
}
