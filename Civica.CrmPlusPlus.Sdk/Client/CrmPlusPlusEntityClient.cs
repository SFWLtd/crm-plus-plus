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
            return GetEntities(typeof(T), results.Entities).Cast<T>();
        }

        private IEnumerable<CrmPlusPlusEntity> GetEntities(Type crmPlusPlusEntityType, IEnumerable<Entity> entities, Dictionary<Guid, string> idFilters = null, string alias = "", CrmPlusPlusEntity parentEntity = null, Type parentEntityType = null)
        {
            var crmPlusPlusEntities = new List<CrmPlusPlusEntity>();

            if (idFilters == null)
            {
                idFilters = new Dictionary<Guid, string>();
            }

            // Ensure Id's are stored as entity attributes before filtering
            foreach (var entity in entities)
            {
                entity["id"] = entity.Id;
            }

            var filteredEntities = new List<Entity>(entities).AsEnumerable();
            foreach (var filter in idFilters)
            {
                filteredEntities = filteredEntities
                    .Where(e =>
                    {
                        if (e[filter.Value].GetType() == typeof(AliasedValue))
                        {
                            return (Guid)((AliasedValue)e[filter.Value]).Value == filter.Key;
                        }
                        else
                        {
                            return (Guid)e[filter.Value] == filter.Key;
                        }
                    }); 
            }

            if (!string.IsNullOrEmpty(alias) && !entities.Any(e => e.Attributes.Keys.Any(k => k.StartsWith(alias.ToIdAlias()))))
            {
                foreach (var entity in filteredEntities)
                {
                    var crmPlusPlusEntity = entity.ToCrmPlusPlusEntity(crmPlusPlusEntityType, alias);
                    MapParentLookup(crmPlusPlusEntityType, crmPlusPlusEntity, entities, idFilters, alias, parentEntity, parentEntityType);
                    crmPlusPlusEntities.Add(crmPlusPlusEntity);
                }

                return crmPlusPlusEntities;
            }

            var groupedById = filteredEntities
                .GroupBy(e => string.IsNullOrEmpty(alias) 
                    ? e.Id 
                    : (Guid)((AliasedValue)e[alias.ToIdAlias()]).Value, e => e, (key, g) => new { EntityId = key, Entities = g.ToList() });

            if (groupedById.All(g => !g.Entities.Any()))
            {
                return crmPlusPlusEntities;
            }

            foreach (var grouping in groupedById)
            {
                if (!grouping.Entities.Any())
                {
                    continue;
                }

                var mappedEntity = grouping.Entities.First().ToCrmPlusPlusEntity(crmPlusPlusEntityType, alias); // Doesn't matter which one in the grouping is mapped - they're all the same for this entity

                MapNToOneLookups(crmPlusPlusEntityType, mappedEntity, entities, idFilters, alias, parentEntityType);
                MapOneToNCollections(crmPlusPlusEntityType, mappedEntity, entities, idFilters, alias, parentEntityType);
                MapParentLookup(crmPlusPlusEntityType, mappedEntity, entities, idFilters, alias, parentEntity, parentEntityType);

                crmPlusPlusEntities.Add(mappedEntity);
            }

            return crmPlusPlusEntities;
        }

        private void MapParentLookup(Type crmPlusPlusEntityType, CrmPlusPlusEntity crmPlusPlusEntity, IEnumerable<Entity> entities, Dictionary<Guid, string> idFilters = null, string alias = "", CrmPlusPlusEntity parentEntity = null, Type parentEntityType = null)
        {
            var parentLookup = TypeDescriptor.GetProperties(crmPlusPlusEntityType).AsEnumerable()
                .SingleOrDefault(p => p.PropertyType.IsGenericType
                    && p.PropertyType.GetGenericTypeDefinition() == typeof(EntityReference<>)
                    && p.PropertyType.GetGenericArguments().Count() == 1
                    && p.PropertyType.GetGenericArguments().Single() == parentEntityType);

            if (parentLookup != null)
            {
                var lookupType = parentLookup.PropertyType.GetGenericArguments().Single();
                var lookup = Activator.CreateInstance(typeof(EntityReference<>).MakeGenericType(lookupType), new object[] { parentEntity.Id });

                var lookupValueProperty = TypeDescriptor.GetProperties(lookup).AsEnumerable()
                    .SingleOrDefault(p => p.PropertyType == lookupType);

                if (lookupValueProperty != null)
                {
                    lookupValueProperty.SetValue(lookup, parentEntity);
                }

                parentLookup.SetValue(crmPlusPlusEntity, lookup);
            }
        }

        private void MapNToOneLookups(Type crmPlusPlusEntityType, CrmPlusPlusEntity crmPlusPlusEntity, IEnumerable<Entity> entities, Dictionary<Guid, string> idFilters = null, string alias = "", Type parentEntityType = null)
        {
            var nToOneLookups = TypeDescriptor.GetProperties(crmPlusPlusEntityType).AsEnumerable()
                .Where(p => p.PropertyType.IsGenericType
                    && p.PropertyType.GetGenericTypeDefinition() == typeof(EntityReference<>)
                    && p.PropertyType.GetGenericArguments().Count() == 1
                    && p.PropertyType.GetGenericArguments().Single() != parentEntityType
                    && typeof(CrmPlusPlusEntity).IsAssignableFrom(p.PropertyType.GetGenericArguments().Single()));

            foreach (var nToOneLookup in nToOneLookups)
            {
                var lookupType = nToOneLookup.PropertyType.GetGenericArguments().Single();
                var lookupEntityName = EntityNameAttribute.GetFromType(lookupType);

                var idFilter = GetIdFilter(alias, crmPlusPlusEntityType, entities, lookupType);

                AddToIdFilter(idFilters, crmPlusPlusEntity.Id, idFilter);

                var nTo1LookupEntity = GetEntities(lookupType, entities, idFilters, lookupEntityName, crmPlusPlusEntity, crmPlusPlusEntityType).SingleOrDefault();
                RemoveFromIdFilter(idFilters, crmPlusPlusEntity.Id);

                if (nTo1LookupEntity != null)
                {
                    var lookup = Activator.CreateInstance(typeof(EntityReference<>).MakeGenericType(lookupType), new object[] { nTo1LookupEntity.Id });
                    var lookupValueProperty = TypeDescriptor.GetProperties(lookup).AsEnumerable()
                        .SingleOrDefault(p => p.PropertyType == lookupType);

                    if (lookupValueProperty != null)
                    {
                        lookupValueProperty.SetValue(lookup, nTo1LookupEntity);
                    }

                    nToOneLookup.SetValue(crmPlusPlusEntity, lookup);
                }
            }
        }

        private void MapOneToNCollections(Type crmPlusPlusEntityType, CrmPlusPlusEntity crmPlusPlusEntity, IEnumerable<Entity> entities, Dictionary<Guid, string> idFilters = null, string alias = "", Type parentEntityType = null)
        {
            var onetoNCollections = TypeDescriptor.GetProperties(crmPlusPlusEntityType).AsEnumerable()
                .Where(p => p.PropertyType.IsGenericType
                    && p.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    && p.PropertyType.GetGenericArguments().Count() == 1
                    && p.PropertyType.GetGenericArguments().Single() != parentEntityType
                    && typeof(CrmPlusPlusEntity).IsAssignableFrom(p.PropertyType.GetGenericArguments().Single()));

            foreach (PropertyDescriptor property in onetoNCollections)
            {
                var childEntityType = property.PropertyType.GetGenericArguments().Single();
                var childEntityName = EntityNameAttribute.GetFromType(childEntityType);

                var idFilter = GetIdFilter(alias, crmPlusPlusEntityType, entities, childEntityType);

                AddToIdFilter(idFilters, crmPlusPlusEntity.Id, idFilter);
                var children = GetEntities(childEntityType, entities, idFilters, childEntityName, crmPlusPlusEntity, crmPlusPlusEntityType);
                RemoveFromIdFilter(idFilters, crmPlusPlusEntity.Id);

                var cast = typeof(Enumerable).GetMethod("Cast")
                    .MakeGenericMethod(new Type[] { childEntityType });

                property.SetValue(crmPlusPlusEntity, cast.Invoke(null, new object[] { children }));
            }
        }

        private string GetIdFilter(string alias, Type crmPlusPlusEntityType, IEnumerable<Entity> entities, Type childEntityType)
        {
            var entityAlias = EntityNameAttribute.GetFromType(crmPlusPlusEntityType);

            var idFilter = string.IsNullOrEmpty(alias) ? "id" : entityAlias.ToIdAlias();

            if (!entities.Any(e => e.Attributes.Contains(idFilter)))
            {
                var childLookupToParent = TypeDescriptor.GetProperties(childEntityType).AsEnumerable()
                    .SingleOrDefault(p => p.PropertyType.IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(EntityReference<>)
                        && p.PropertyType.GetGenericArguments().Count() == 1
                        && p.PropertyType.GetGenericArguments().Single() == crmPlusPlusEntityType);

                if (childLookupToParent != null)
                {
                    idFilter = PropertyNameAttribute.GetFromDescriptor(childLookupToParent);
                }
            }

            return idFilter;
        }

        private void AddToIdFilter(Dictionary<Guid, string> idFilters, Guid id, string alias)
        {
            RemoveFromIdFilter(idFilters, id);
            idFilters.Add(id, alias);
        }

        private void RemoveFromIdFilter(Dictionary<Guid, string> idFilters, Guid id)
        {
            if (idFilters.Keys.Contains(id))
            {
                idFilters.Remove(id);
            }
        }
    }
}
