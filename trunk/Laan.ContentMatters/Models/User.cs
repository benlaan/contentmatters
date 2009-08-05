using System;
using System.Collections.Generic;

using Laan.ContentMatters.Models.Extensions;

namespace Laan.ContentMatters.Models
{
    public class User : IIdentifiable, IItem
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string EmailAddress { get; set; }

        public virtual string Link( string displayText )
        {
            return String.Format( "<a href=\"/User/{0}/{1}\">{1}</a>", ID, displayText.HtmlEncoded() );
        }
        
        public virtual string Link()
        {
            return Link( Name );
        }

        #region IItem Members

        public virtual string TypeName
        {
            get { return GetType().Name; }
        }

        public virtual string Description
        {
            get { return Name; }
        }

        public virtual string Title
        {
            get { return Name; }
        }

        #endregion
    }
}
