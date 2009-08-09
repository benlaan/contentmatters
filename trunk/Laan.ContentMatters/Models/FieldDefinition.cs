using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laan.ContentMatters.Models
{
    public class FieldDefinition : IIdentifiable
    {
        public FieldDefinition()
        {
            FieldType = FieldType.Number;
        }

        #region IIdentifiable Members

        public virtual int ID { get; set; }

        #endregion

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

        public virtual string Name { get; set; }
        public virtual FieldType FieldType { get; set; }
        public virtual Type ReferenceType { get; set; }

        public virtual string ToViewHtml( object value )
        {
            return value.ToString();
        }

        public virtual string ToEditHtml( object value )
        {
            return HtmlHelper.ToEditHtml( this, value );
        }

        public Type ToSystemType()
        {
            var lookup = new Dictionary<FieldType, Type>()
            {
                { FieldType.Number, typeof(Int32) },
                { FieldType.CheckBox, typeof(bool) },
                { FieldType.Text, typeof(string) },
                { FieldType.Memo, typeof(string) },
                { FieldType.Date, typeof(DateTime) },
                { FieldType.Time, typeof(DateTime) },
                { FieldType.DateTime, typeof(DateTime) },
                { FieldType.Money, typeof(Decimal) },
                { FieldType.Decimal, typeof(Decimal) },
                { FieldType.Percentage, typeof(Decimal) },
                { FieldType.Image, typeof(string) },
                { FieldType.Password, typeof(string) },
                { FieldType.Hidden, typeof(string) }
            };

            Type type;
            if ( lookup.TryGetValue( FieldType, out type ) )
                return type;
            else
            {
                switch ( FieldType )
                {
                    case FieldType.Lookup:
                        {
                            return ReferenceType;
                            //Type argType = Type.GetType( ReferenceType, true );
                            //return argType;// typeof( IList<> ).MakeGenericType( argType );
                        }
                    default:
                        throw new NotSupportedException( String.Format( "FieldType {0} not supported", FieldType ) );
                }
            }
        }

    }
}
