using System;
using System.Linq.Expressions;
using System.Xml.Linq;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;

namespace Civica.CrmPlusPlus.Sdk.Querying
{
    public class QueryConditionBuilder<T> where T : CrmPlusPlusEntity, new()
    {
        private readonly Query<T> query;
        private readonly QueryFilterBuilder<T> queryFilterBuilder;
        private readonly XElement queryConditionRoot;

        internal QueryConditionBuilder(Query<T> query, QueryFilterBuilder<T> queryFilterBuilder)
        {
            this.queryFilterBuilder = queryFilterBuilder;
            this.query = query;

            queryConditionRoot = new XElement("filter");

            // Use and filter by default
            UpdateFilterType(FilterType.And);
        }

        private void UpdateFilterType(FilterType filterType)
        {
            queryConditionRoot.Add(new XAttribute("type", filterType.ToString()));
        }

        internal QueryConditionBuilder<T> AddCondition<TProperty>(Expression<Func<T, TProperty>> propertyExpr, ConditionOperator operation, string value)
        {
            var condition = new XElement("condition");
            condition.Add(new XAttribute("attribute", PropertyNameAttribute.GetFromType(propertyExpr)));
            condition.Add(new XAttribute("operator", operation.ToString()));
            condition.Add(new XAttribute("value", value));

            return this;
        }

        internal Query<T> WithNoMoreConditions()
        {
            query.EntityRootElement.Add(new XElement(queryConditionRoot));

            return query;
        }

        public AndQueryConditionBuilder<T> And<TProperty>(Expression<Func<T, TProperty>> propertyExpr, ConditionOperator operation, string value)
        {
            UpdateFilterType(FilterType.And);

            return new AndQueryConditionBuilder<T>(query, this);
        }

        public OrQueryConditionBuilder<T> Or<TProperty>(Expression<Func<T, TProperty>> propertyExpr, ConditionOperator operation, string value)
        {
            UpdateFilterType(FilterType.Or);

            return new OrQueryConditionBuilder<T>(query, this);
        }
    }
}
