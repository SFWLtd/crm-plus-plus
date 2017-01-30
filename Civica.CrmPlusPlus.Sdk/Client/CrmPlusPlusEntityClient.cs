using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Civica.CrmPlusPlus.Sdk.Client.Association;
using Civica.CrmPlusPlus.Sdk.Client.Retrieve;
using Civica.CrmPlusPlus.Sdk.Client.RetrieveMultiple;
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

            var parser = new RetrieveMultipleParser<T>(results.Entities);
            var entities = parser.GetMainEntities();
            parser.PopulateOneToManyEntities(entities);
            parser.PopulateManyToOneEntity(entities);

            return entities;
        }
    }
}
