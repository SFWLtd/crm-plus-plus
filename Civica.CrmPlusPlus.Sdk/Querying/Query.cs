using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;

namespace Civica.CrmPlusPlus.Sdk.Querying
{
    public class Query<T> where T : CrmPlusPlusEntity, new()
    {
        private readonly Query query;

        internal XElement EntityRootElement { get; set; }

        internal Query(Query query)
        {
            this.query = query;

            EntityRootElement = new XElement("entity");
            EntityRootElement.Add(new XAttribute("name", EntityNameAttribute.GetFromType<T>()));

            var createdOn = new XElement("attribute");
            createdOn.Add(new XAttribute("name", "createdon"));
            EntityRootElement.Add(createdOn);

            var modifiedOn = new XElement("attribute");
            modifiedOn.Add(new XAttribute("name", "modifiedon"));
            EntityRootElement.Add(modifiedOn);
        }

        internal Query<T> Include<TProperty>(Expression<Func<T, TProperty>> propertyExpr)
        {
            var propertyName = PropertyNameAttribute.GetFromType(propertyExpr);
            if (propertyName == "modifiedon" || propertyName == "createdon" || propertyName == "id")
            {
                return this;
            }

            var element = new XElement("attribute");
            element.Add(new XAttribute("name", propertyName));

            EntityRootElement.Add(element);

            return this;
        }

        internal QueryFilterBuilder<T> Filter(FilterType filterType)
        {
            return new QueryFilterBuilder<T>(this, filterType);
        }

        internal string ToFetchXml()
        {
            query.QueryXml.Add(EntityRootElement);

            return query.QueryXml.ToString()
                .Replace("\"", "'");
        }
    }
}
