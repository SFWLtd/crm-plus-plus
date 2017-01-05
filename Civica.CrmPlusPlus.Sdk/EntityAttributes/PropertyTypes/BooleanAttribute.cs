using System;

namespace Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BooleanAttribute : Attribute
    {
        public BooleanAttribute()
        {
        }
    }
}
