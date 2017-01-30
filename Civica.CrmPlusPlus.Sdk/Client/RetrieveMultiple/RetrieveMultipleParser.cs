using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.Querying;
using Microsoft.Xrm.Sdk;

namespace Civica.CrmPlusPlus.Sdk.Client.RetrieveMultiple
{
    public class RetrieveMultipleParser<T> where T: CrmPlusPlusEntity, new()
    {
        private readonly DataCollection<Entity> entities;

        public RetrieveMultipleParser(DataCollection<Entity> entities)
        {
            this.entities = entities;
        }

        public IEnumerable<T> GetMainEntities()
        {
            var groupedMainEntities = entities.GroupBy(e => e.Id, e => e, (k, r) => r);

            var mainEntities = new List<T>();
            foreach (var group in groupedMainEntities)
            {
                var mainEntity = group.First();
                mainEntities.Add(mainEntity.ToCrmPlusPlusEntity<T>());
            }

            return mainEntities;
        }

        public IEnumerable<T> PopulateOneToManyEntities(IEnumerable<T> mainEntities)
        {
            var oneToManyRelationships = GetOneToManyRelationships();

            foreach (var oneToManyRelationship in oneToManyRelationships)
            {
                var relatedEntityType = oneToManyRelationship.Value;

                var oneToManyPropertiesOfType = TypeDescriptor.GetProperties(typeof(T)).AsEnumerable()
                        .Where(p => p.PropertyType.IsGenericType
                            && p.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                            && p.PropertyType.GetGenericArguments().Count() == 1
                            && p.PropertyType.GetGenericArguments().Single() == relatedEntityType);

                var groupedOneToManyEntities = entities
                    .WhereEntitiesHaveKeyThatContains(oneToManyRelationship.Key.ToIdAlias())
                    .GroupBy(e => e[oneToManyRelationship.Key.ToIdAlias()], e => e, (k, r) => r);

                var manyEntities = new List<CrmPlusPlusEntity>();
                foreach (var groupedOneToManyEntity in groupedOneToManyEntities)
                {
                    var manyEntity = groupedOneToManyEntity.First();
                    manyEntities.Add(manyEntity.ToCrmPlusPlusEntity(relatedEntityType, oneToManyRelationship.Key));
                }

                foreach (var mainEntity in mainEntities)
                {
                    foreach (var oneToManyPropertyOfType in oneToManyPropertiesOfType)
                    {
                        var cast = typeof(Enumerable).GetMethod("Cast")
                            .MakeGenericMethod(new Type[] { relatedEntityType });

                        oneToManyPropertyOfType.SetValue(mainEntity, cast.Invoke(null, new object[] { manyEntities }));
                    }
                }
            }

            return mainEntities;
        }

        public IEnumerable<T> PopulateManyToOneEntity(IEnumerable<T> mainEntities)
        {
            var manyToOneRelationships = GetManyToOneRelationships();

            foreach (var manyToOneRelationship in manyToOneRelationships)
            {
                var relatedEntityType = manyToOneRelationship.Value;

                var manyToOnePropertiesOfType = TypeDescriptor.GetProperties(typeof(T)).AsEnumerable()
                        .Where(p => p.PropertyType.IsGenericType
                            && p.PropertyType.GetGenericTypeDefinition() == typeof(EntityReference<>)
                            && p.PropertyType.GetGenericArguments().Count() == 1
                            && p.PropertyType.GetGenericArguments().Single() == relatedEntityType);

                var groupedManyToOneEntities = entities.GroupBy(e => e.Id, e => e, (k, r) => new { Id = k, Entities = r });

                foreach (var groupedManyToOneEntity in groupedManyToOneEntities)
                {
                    var lookupEntity = groupedManyToOneEntity.Entities.First();
                    var crmPlusPlusLookupEntity = lookupEntity.ToCrmPlusPlusEntity(relatedEntityType, manyToOneRelationship.Key);

                    var mainEntity = mainEntities.Single(e => e.Id == groupedManyToOneEntity.Id);

                    foreach (var manyToOnePropertyOfType in manyToOnePropertiesOfType)
                    {
                        var lookup = Activator.CreateInstance(typeof(EntityReference<>).MakeGenericType(relatedEntityType), new object[] { crmPlusPlusLookupEntity.Id });
                        var lookupValueProperty = TypeDescriptor.GetProperties(lookup).AsEnumerable().SingleOrDefault(p => p.PropertyType == relatedEntityType);

                        lookupValueProperty.SetValue(lookup, crmPlusPlusLookupEntity);

                        manyToOnePropertyOfType.SetValue(mainEntity, lookup);
                    }
                }
            }

            return mainEntities;
        }

        public Dictionary<string, Type> GetManyToOneRelationships()
        {
            var aliases = GetAliasesInCollection();

            var manyToOneRelationships = TypeDescriptor.GetProperties(typeof(T)).AsEnumerable()
                .Where(p => p.PropertyType.IsGenericType
                    && p.PropertyType.GetGenericTypeDefinition() == typeof(EntityReference<>)
                    && p.PropertyType.GetGenericArguments().Count() == 1
                    && p.PropertyType.GetGenericArguments().Single() != typeof(T)
                    && typeof(CrmPlusPlusEntity).IsAssignableFrom(p.PropertyType.GetGenericArguments().Single()));

            var relationships = new Dictionary<string, Type>();
            foreach (var manyToOneRelationship in manyToOneRelationships)
            {
                var lookupEntity = manyToOneRelationship.PropertyType.GetGenericArguments().Single();
                var alias = EntityNameAttribute.GetFromType(lookupEntity);

                if (aliases.Contains(alias) && !relationships.ContainsKey(alias))
                {
                    relationships.Add(alias, lookupEntity);
                }
            }

            return relationships;
        }

        public Dictionary<string, Type> GetOneToManyRelationships()
        {
            var aliases = GetAliasesInCollection();

            var oneToManyRelationships = TypeDescriptor.GetProperties(typeof(T)).AsEnumerable()
                .Where(p => p.PropertyType.IsGenericType
                    && p.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    && p.PropertyType.GetGenericArguments().Count() == 1
                    && p.PropertyType.GetGenericArguments().Single() != typeof(T)
                    && typeof(CrmPlusPlusEntity).IsAssignableFrom(p.PropertyType.GetGenericArguments().Single()));

            var relationships = new Dictionary<string, Type>();
            foreach (var oneToManyRelationship in oneToManyRelationships)
            {
                var manyEntity = oneToManyRelationship.PropertyType.GetGenericArguments().Single();
                var alias = EntityNameAttribute.GetFromType(manyEntity);

                if (aliases.Contains(alias) && !relationships.ContainsKey(alias))
                {
                    relationships.Add(alias, manyEntity);
                }
            }

            return relationships;
        }

        private IEnumerable<string> GetAliasesInCollection()
        {
            var aliases = new List<string>();

            foreach (var entity in entities)
            {
                var keys = entity.Attributes.Keys;
                var aliasedKeys = keys.Where(k => k.Contains("."));

                foreach (var aliasedKey in aliasedKeys)
                {
                    var alias = aliasedKey.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0];

                    if (!aliases.Contains(alias))
                    {
                        aliases.Add(alias);
                    }
                }
            }

            return aliases;
        }
    }
}
