using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Civica.CrmPlusPlus.Sdk.Client.Association;
using Civica.CrmPlusPlus.Sdk.Client.Retrieve;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.Querying;
using Civica.CrmPlusPlus.Sdk.Validation;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Civica.CrmPlusPlus.Sdk.Client
{
    public class CrmPlusPlusEntityClient : ICrmPlusPlusEntityClient
    {
        private readonly IOrganizationService service;

        internal CrmPlusPlusEntityClient(IOrganizationService service)
        {
            this.service = service;
        }

        public void Associate(string relationshipName, Associate association)
        {
            Guard.This(relationshipName).AgainstNullOrEmpty();

            var relationship = new Relationship(relationshipName);

            var relatedEntities = new EntityReferenceCollection();
            relatedEntities.AddRange(association.RelatedEntities.Select(s => new EntityReference(s.Value, s.Key)));

            service.Associate(association.EntityName, association.EntityId, relationship, relatedEntities);
        }

        public void Disassociate(string relationshipName, Associate association)
        {
            Guard.This(relationshipName).AgainstNullOrEmpty();

            var relationship = new Relationship(relationshipName);

            var relatedEntities = new EntityReferenceCollection();
            relatedEntities.AddRange(association.RelatedEntities.Select(s => new EntityReference(s.Value, s.Key)));

            service.Disassociate(association.EntityName, association.EntityId, relationship, relatedEntities);
        }

        public Guid Create<T>(T entity) where T : CrmPlusPlusEntity, new()
        {
            service.Create(entity.ToCrmEntity());

            return entity.Id;
        }

        public void Update<T>(T entity) where T: CrmPlusPlusEntity, new()
        {
            service.Update(entity.ToCrmEntity());
        }

        public T Retrieve<T>(Retrieval<T> retrieval) where T : CrmPlusPlusEntity, new()
        {
            var crmEntity = service.Retrieve(EntityNameAttribute.GetFromType<T>(),
                retrieval.Id,
                retrieval.AllColumns
                    ? new ColumnSet(true)
                    : new ColumnSet(retrieval.IncludedColumns.ToArray()));

            return crmEntity.ToCrmPlusPlusEntity<T>();
        }

        public void Delete<T>(T entity) where T : CrmPlusPlusEntity, new()
        {
            service.Delete(EntityNameAttribute.GetFromType<T>(), entity.Id);
        }

        public IEnumerable<T> RetrieveMultiple<T>(Query<T> query) where T : CrmPlusPlusEntity, new()
        {
            var results = service.RetrieveMultiple(new FetchExpression(query.ToFetchXml()));
            var entityName = EntityNameAttribute.GetFromType<T>();

            var groupedById = results.Entities
                .GroupBy(e => e.Id, e => e, (key, g) => new { EntityId = key, Entities = g.ToList() });

            // Identify related entity enumerables
            var relatedEntityEnumerables = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(typeof(T)))
            {
                if (property.PropertyType.IsGenericType
                    && property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    && property.PropertyType.GetGenericArguments().Count() == 1
                    && typeof(CrmPlusPlusEntity).IsAssignableFrom(property.PropertyType.GetGenericArguments().Single()))
                {
                    relatedEntityEnumerables.Add(property);
                }
            }

            var crmPlusPlusEntities = new List<T>();
            foreach (var grouping in groupedById)
            {
                T crmPlusPlusEntity = default(T);

                foreach (var relatedEntityEnumerable in relatedEntityEnumerables)
                {
                    var relatedEntityEntities = new List<CrmPlusPlusEntity>();

                    var relatedCrmPlusPlusEntityType = relatedEntityEnumerable.PropertyType.GetGenericArguments().Single();
                    var relatedEntityName = EntityNameAttribute.GetFromType(relatedCrmPlusPlusEntityType);

                    foreach (var entity in grouping.Entities)
                    {
                        if (crmPlusPlusEntity == default(T))
                        {
                            crmPlusPlusEntity = entity.ToCrmPlusPlusEntity<T>();
                        }

                        relatedEntityEntities.Add(entity.ToCrmPlusPlusEntity(relatedCrmPlusPlusEntityType, relatedEntityName + "."));
                    }

                    var cast = typeof(Enumerable).GetMethod("Cast")
                        .MakeGenericMethod(new Type[] { relatedCrmPlusPlusEntityType });

                    relatedEntityEnumerable.SetValue(crmPlusPlusEntity, cast.Invoke(null, new object[] { relatedEntityEntities }));
                }

                crmPlusPlusEntities.Add(crmPlusPlusEntity);
            }

            return crmPlusPlusEntities;
        }
    }
}
