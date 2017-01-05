using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Metadata;

namespace Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IntegerAttribute : Attribute
    {
        public int MaxValue { get; }

        public int MinValue { get; }

        public IntegerFormat Format { get; }

        public IntegerAttribute(int maxValue, int minValue, Metadata.IntegerFormat format)
        {
            MaxValue = maxValue;
            MinValue = minValue;
            Format = format.ToSimilarEnum<IntegerFormat>();
        }
    }
}
