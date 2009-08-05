using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laan.ContentMatters.Models
{
    public class ItemDefinition : Item
    {
        public ItemDefinition()
        {
            MasterPage = "Site";
        }

        public virtual string MasterPage { get; private set; }
    }
}
