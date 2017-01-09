using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Civica.CrmPlusPlus.Sdk.Querying
{
    public class Query
    {
        internal XElement QueryXml { get; }

        internal Query(bool withDistinctResults)
        {
            QueryXml = new XElement("fetch");
            QueryXml.Add(new XAttribute("mapping", "logical"));
            QueryXml.Add(new XAttribute("distinct", withDistinctResults.ToString().ToLower()));
        }

        public static Query<T> ForEntity<T>(bool distinct = false) where T : CrmPlusPlusEntity, new()
        {
            var query = new Query(distinct);

            return new Query<T>(query);
        }
    }
}
