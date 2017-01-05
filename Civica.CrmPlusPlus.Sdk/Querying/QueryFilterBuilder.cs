using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;

namespace Civica.CrmPlusPlus.Sdk.Querying
{
    public class QueryFilterBuilder<T> where T : CrmPlusPlusEntity, new()
    {
        internal Guid Id { get; }

        internal XElement RootElement { get; }

        internal QueryFilterBuilder<T> Parent { get; private set; }

        private readonly Query<T> query;
        private readonly FilterType FilterType;
        

        internal QueryFilterBuilder(Query<T> query, FilterType filterType, QueryFilterBuilder<T> parent = null)
        {
            this.Parent = parent;
            this.query = query;
            this.FilterType = filterType;

            Id = Guid.NewGuid();

            RootElement = new XElement("filter");
            RootElement.Add(new XAttribute("type", filterType.ToString().ToLower()));
        }

        public QueryFilterBuilder<T> Where<TProperty>(Expression<Func<T, TProperty>> propertyExpr, ConditionOperator conditionOperator, string value)
        {
            var condition = new XElement("condition");
            condition.Add(new XAttribute("attribute", PropertyNameAttribute.GetFromType(propertyExpr)));
            condition.Add(new XAttribute("operator", conditionOperator.Value.Trim().ToLower()));
            condition.Add(new XAttribute("value", value));

            RootElement.Add(condition);

            return this;
        }

        public QueryFilterBuilder<T> ChildFilter(FilterType filterType)
        {
            return new QueryFilterBuilder<T>(query, filterType, this);
        }

        public QueryFilterBuilder<T> EndFilter()
        {
            if (Parent == null)
            {
                return this;
            }

            Parent.RootElement.Add(RootElement);

            return Parent;
        }

        public Query<T> EndFiltering()
        {
            var currentFilter = this;

            while (currentFilter.Parent != null)
            {
                currentFilter.Parent.RootElement.Add(RootElement);
                currentFilter = currentFilter.Parent;
            }

            query.EntityRootElement.Add(currentFilter.RootElement);

            return query;
        }
    }
}
