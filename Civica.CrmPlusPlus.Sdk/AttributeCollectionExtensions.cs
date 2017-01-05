using System.Collections.Generic;
using System.ComponentModel;

namespace Civica.CrmPlusPlus.Sdk
{
    internal static class AttributeCollectionExtensions
    {
        internal static IEnumerable<object> AsEnumerable(this AttributeCollection attrCollection)
        {
            var attributes = new List<object>();
            foreach (var attr in attrCollection)
            {
                attributes.Add(attr);
            }

            return attributes;
        }
    }
}
