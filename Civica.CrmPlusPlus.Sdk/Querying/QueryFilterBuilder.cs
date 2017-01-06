using System;
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

        public QueryFilterBuilder<T> Condition<TProperty>(Expression<Func<T, TProperty>> propertyExpr, ConditionOperator conditionOperator, string value)
        {
            var condition = new XElement("condition");
            condition.Add(new XAttribute("attribute", PropertyNameAttribute.GetFromType(propertyExpr)));
            condition.Add(new XAttribute("operator", conditionOperator.Value.Trim().ToLower()));
            condition.Add(new XAttribute("value", value));

            RootElement.Add(condition);

            return this;
        }

        public void InnerFilter(FilterType filterType, Action<QueryFilterBuilder<T>> filterAction)
        {
            var queryBuilder = new QueryFilterBuilder<T>(query, filterType, this);
            filterAction(queryBuilder);

            RootElement.Add(queryBuilder.RootElement);
        }
    }
}
