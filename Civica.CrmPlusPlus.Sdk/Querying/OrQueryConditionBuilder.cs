using System;
using System.Linq.Expressions;

namespace Civica.CrmPlusPlus.Sdk.Querying
{
    public class OrQueryConditionBuilder<T> where T : CrmPlusPlusEntity, new()
    {
        private readonly Query<T> query;
        private readonly QueryConditionBuilder<T> queryConditionBuilder;

        internal OrQueryConditionBuilder(Query<T> query, QueryConditionBuilder<T> queryConditionBuilder)
        {
            this.query = query;
            this.queryConditionBuilder = queryConditionBuilder;
        }

        public OrQueryConditionBuilder<T> Or<TProperty>(Expression<Func<T, TProperty>> propertyExpr, ConditionOperator operation, string value)
        {
            queryConditionBuilder.AddCondition(propertyExpr, operation, value);

            return this;
        }

        public Query<T> WithNoMoreConditions()
        {
            return queryConditionBuilder.WithNoMoreConditions();
        }
    }
}
