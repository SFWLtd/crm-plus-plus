using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;

namespace Civica.CrmPlusPlus.Sdk.Client.Retrieve
{
    public class Retrieval<T> where T : CrmPlusPlusEntity, new()
    {
        internal Guid Id { get; }

        internal List<string> IncludedColumns { get; private set; }

        internal bool AllColumns { get; private set; }

        internal Retrieval(Guid id)
        {
            Id = id;
            AllColumns = false;
            IncludedColumns = new List<string>();
        }

        public Retrieval<T> IncludeAllColumns(bool flag)
        {
            AllColumns = flag;

            return this;
        }

        public Retrieval<T> Include<TProperty>(Expression<Func<T, TProperty>> propertyExpr)
        {
            var propertyName = PropertyNameAttribute.GetFromType(propertyExpr);
            if (propertyName == "modifiedon" || propertyName == "createdon" || propertyName == "id")
            {
                return this;
            }

            if (!IncludedColumns.Contains(propertyName.ToLower()))
            {
                IncludedColumns.Add(propertyName);
            }

            return this;
        }
    }
}
