using System;

namespace Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DecimalAttribute : Attribute
    {
        public int MaxValue { get; }

        public int MinValue { get; }

        public int Precision { get; }

        public DecimalAttribute(int maxValue, int minValue, int precision)
        {
            MaxValue = maxValue;
            MinValue = minValue;
            Precision = precision;
        }
    }
}
