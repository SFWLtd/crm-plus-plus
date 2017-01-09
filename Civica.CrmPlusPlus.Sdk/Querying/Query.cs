using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;

namespace Civica.CrmPlusPlus.Sdk.Querying
{
    public class Query<T> where T : CrmPlusPlusEntity, new()
    {
        private readonly Query query;

        private int linkedEntityDepth;

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

            linkedEntityDepth = 0;
        }

        internal Query(Query query, string from, string to, JoinType joinType, int linkedEntityDepth)
        {
            var entityName = EntityNameAttribute.GetFromType<T>();

            this.query = query;
            this.linkedEntityDepth = linkedEntityDepth;

            EntityRootElement = new XElement("link-entity");
            EntityRootElement.Add(new XAttribute("name", entityName));
            EntityRootElement.Add(new XAttribute("alias", entityName));
            EntityRootElement.Add(new XAttribute("from", from));
            EntityRootElement.Add(new XAttribute("to", to));
            EntityRootElement.Add(new XAttribute("link-type", joinType.ToString().ToLower()));

            if (linkedEntityDepth < 2)
            {
                var createdOn = new XElement("attribute");
                createdOn.Add(new XAttribute("name", "createdon"));
                EntityRootElement.Add(createdOn);

                var modifiedOn = new XElement("attribute");
                modifiedOn.Add(new XAttribute("name", "modifiedon"));
                EntityRootElement.Add(modifiedOn);
            }
        }

        public Query<T> Include<TProperty>(Expression<Func<T, TProperty>> propertyExpr)
        {
            if (!EntityRootElement.Elements().Any(e => e.Name == "all-attributes"))
            {

                var propertyName = PropertyNameAttribute.GetFromType(propertyExpr);
                if (propertyName == "modifiedon" || propertyName == "createdon" || propertyName == "id"
                    || linkedEntityDepth > 1)
                {
                    return this;
                }

                var element = new XElement("attribute");
                element.Add(new XAttribute("name", propertyName));

                EntityRootElement.Add(element);
            }

            return this;
        }

        public Query<T> IncludeAllProperties()
        {
            var elementsToRemove = EntityRootElement.Elements().Where(e => e.Name == "attribute");

            while (elementsToRemove.Any())
            {
                elementsToRemove.First().Remove();
            }

            EntityRootElement.Add(new XElement("all-attributes"));

            return this;
        }

        public Query<T> Filter(FilterType filterType, Action<QueryFilterBuilder<T>> filterAction)
        {
            var queryBuilder = new QueryFilterBuilder<T>(this, filterType);
            filterAction(queryBuilder);

            if (queryBuilder.RootElement.Elements().Any(e => e.Name == "condition" || e.Name == "filter"))
            {
                EntityRootElement.Add(queryBuilder.RootElement);
            }

            return this;
        }

        public Query<T> JoinNTo1<TRelatedEntity>(Expression<Func<T, EntityReference<TRelatedEntity>>> joinExpr,
            JoinType joinType,
            Action<Query<TRelatedEntity>> queryBuilder) where TRelatedEntity : CrmPlusPlusEntity, new()
        {
            var to = EntityNameAttribute.GetFromType<TRelatedEntity>() + "id";
            var from = PropertyNameAttribute.GetFromType(joinExpr);

            var entityQuery = new Query<TRelatedEntity>(query, from, to, joinType, linkedEntityDepth + 1);
            queryBuilder(entityQuery);

            EntityRootElement.Add(entityQuery.EntityRootElement);

            return this;
        }

        public Query<T> Join1ToN<TRelatedEntity>(Expression<Func<T, IEnumerable<TRelatedEntity>>> joinExpr, 
            Expression<Func<TRelatedEntity, EntityReference<T>>> toExpr,  
            JoinType joinType, 
            Action<Query<TRelatedEntity>> queryBuilder) where TRelatedEntity : CrmPlusPlusEntity, new()
        {
            var from = EntityNameAttribute.GetFromType<T>() + "id";
            var to = PropertyNameAttribute.GetFromType(toExpr);

            var entityQuery = new Query<TRelatedEntity>(query, from, to, joinType, linkedEntityDepth + 1);
            queryBuilder(entityQuery);

            EntityRootElement.Add(entityQuery.EntityRootElement);

            return this;
        }

        internal string ToFetchXml()
        {
            query.QueryXml.Add(EntityRootElement);

            return query.QueryXml.ToString()
                .Replace("\"", "'");
        }
    }
}
