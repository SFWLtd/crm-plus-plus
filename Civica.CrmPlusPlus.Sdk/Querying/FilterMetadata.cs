using System;
using System.Xml.Linq;

namespace Civica.CrmPlusPlus.Sdk.Querying
{
    internal class FilterMetadata
    {

        internal Guid? ParentFilterId { get; private set; }

        internal XElement Element { get; private set; }

        internal FilterType FilterType { get; private set; }

        internal FilterMetadata(Guid? parentFilter, XElement element, FilterType filter)
        {
            ParentFilterId = parentFilter;
            Element = element;
            FilterType = filter;
        }
    }
}
