using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laan.ContentMatters.Engine
{
    public abstract class Component
    {
    }

    public abstract class RepeaterComponent : Component
    {
        public string DataSource { get; set; }
    }

    public class ListRepeater : RepeaterComponent
    {
    }

    public class GridRepeater : RepeaterComponent
    {
    }

    public class DetailRepeater : RepeaterComponent
    {
    }
}
