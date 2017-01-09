using System.Collections.Generic;
using System.ComponentModel;

namespace Civica.CrmPlusPlus.Sdk
{
    internal static class PropertyDescriptorCollectionExtensions
    {
        internal static IEnumerable<PropertyDescriptor> AsEnumerable(this PropertyDescriptorCollection propertyDescriptorCollection)
        {
            var properties = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor property in propertyDescriptorCollection)
            {
                properties.Add(property);
            }

            return properties;
        }
    }
}
