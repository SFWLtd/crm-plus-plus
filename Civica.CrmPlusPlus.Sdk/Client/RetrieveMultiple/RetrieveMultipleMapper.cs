using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Microsoft.Xrm.Sdk;

namespace Civica.CrmPlusPlus.Sdk.Client.RetrieveMultiple
{
    public class RetrieveMultipleMapper<T> where T: CrmPlusPlusEntity, new()
    {
        private readonly DataCollection<Entity> entities;

        public RetrieveMultipleMapper(DataCollection<Entity> entities)
        {
            this.entities = entities;
        }

        public IEnumerable<T> GetMainEntities()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> PopulateOneToManyEntity(IEnumerable<T> mainEntities, string oneToManyEntityAlias)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> PopulateManyToOneEntity(IEnumerable<T> mainEntities, string manyToOneAlias)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, RelationshipType> GetManyToOneRelationships()
        {
            var aliases = GetAliasesInCollection();

            var manyToOneRelationships = TypeDescriptor.GetProperties(typeof(T)).AsEnumerable()
                .Where(p => p.PropertyType.IsGenericType
                    && p.PropertyType.GetGenericTypeDefinition() == typeof(EntityReference<>)
                    && p.PropertyType.GetGenericArguments().Count() == 1
                    && p.PropertyType.GetGenericArguments().Single() != typeof(T)
                    && typeof(CrmPlusPlusEntity).IsAssignableFrom(p.PropertyType.GetGenericArguments().Single()));

            var relationships = new Dictionary<string, RelationshipType>();
            foreach (var manyToOneRelationship in manyToOneRelationships)
            {
                var lookupEntity = manyToOneRelationship.PropertyType.GetGenericArguments().Single();
                var alias = EntityNameAttribute.GetFromType(lookupEntity);

                if (aliases.Contains(alias) && !relationships.ContainsKey(alias))
                {
                    relationships.Add(alias, RelationshipType.ManyToOne);
                }
            }

            return relationships;
        }

        public Dictionary<string, RelationshipType> GetOneToManyRelationships()
        {
            var aliases = GetAliasesInCollection();

            var oneToManyRelationships = TypeDescriptor.GetProperties(typeof(T)).AsEnumerable()
                .Where(p => p.PropertyType.IsGenericType
                    && p.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    && p.PropertyType.GetGenericArguments().Count() == 1
                    && p.PropertyType.GetGenericArguments().Single() != typeof(T)
                    && typeof(CrmPlusPlusEntity).IsAssignableFrom(p.PropertyType.GetGenericArguments().Single()));

            var relationships = new Dictionary<string, RelationshipType>();
            foreach (var oneToManyRelationship in oneToManyRelationships)
            {
                var manyEntity = oneToManyRelationship.PropertyType.GetGenericArguments().Single();
                var alias = EntityNameAttribute.GetFromType(manyEntity);

                if (aliases.Contains(alias) && !relationships.ContainsKey(alias))
                {
                    relationships.Add(alias, RelationshipType.OneToMany);
                }
            }

            return relationships;
        }

        private IEnumerable<string> GetAliasesInCollection()
        {
            return entities
                .Where(e => e.Attributes.Keys.Contains("."))
                .Select(e => e.Attributes.Keys.Select(k => k.Split(new[] { "." }, StringSplitOptions.None)[0]).Distinct().ToList())
                .Aggregate((prevKeys, currKeys) =>
                {
                    prevKeys.AddRange(currKeys);
                    return prevKeys;
                })
                .Distinct();
        }
    }
}
