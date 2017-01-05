using System;
using Microsoft.Xrm.Sdk.Metadata;

namespace Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateTimeAttribute : Attribute
    {
        public DateTimeFormat Format { get; }

        public ImeMode ImeMode { get; }

        public DateTimeAttribute(Metadata.DateTimeFormat format, Metadata.ImeMode mode = Metadata.ImeMode.Disabled)
        {
            Format = format.ToSimilarEnum<DateTimeFormat>();
            ImeMode = mode.ToSimilarEnum<ImeMode>();
        }
    }
}
