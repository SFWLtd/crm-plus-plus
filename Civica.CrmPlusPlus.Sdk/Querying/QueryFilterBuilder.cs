using System;
using System.Linq.Expressions;
using System.Xml.Linq;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;

namespace Civica.CrmPlusPlus.Sdk.Querying
{
    public class QueryFilterBuilder<T> where T : CrmPlusPlusEntity, new()
    {
        private readonly Query<T> query;

        internal XElement RootElement { get; }

        internal QueryFilterBuilder(Query<T> query, FilterType filterType, QueryFilterBuilder<T> parent = null)
        {
            this.query = query;

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
