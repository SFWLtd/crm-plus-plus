using System;
namespace Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DoubleAttribute : Attribute
    {
        public int MaxValue { get; }

        public int MinValue { get; }

        public int Precision { get; }

        public DoubleAttribute(int maxValue, int minValue, int precision)
        {
            MaxValue = maxValue;
            MinValue = minValue;
            Precision = precision;
        }
    }
}
