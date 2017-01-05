using System;
using System.Collections.Generic;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;

namespace Civica.CrmPlusPlus.Sdk.Client.Association
{
    public class Associate
    {
        internal Dictionary<Guid, string> RelatedEntities { get; private set; }

        internal Guid EntityId { get; }

        internal string EntityName { get; }

        private Associate(Guid entityId, string entityName)
        {
            EntityName = entityName;
            EntityId = entityId;

            RelatedEntities = new Dictionary<Guid, string>();
        }

        public static Associate ThisEntity<T>(T entity) where T : CrmPlusPlusEntity, new()
        {
            var entityName = EntityNameAttribute.GetFromType<T>();

            return new Associate(entity.Id, entityName);
        }

        public Associate With<TRelatedEntity>(TRelatedEntity entity) where TRelatedEntity : CrmPlusPlusEntity, new()
        {
            RelatedEntities.Add(entity.Id, EntityNameAttribute.GetFromType<TRelatedEntity>());

            return this;
        }
    }
}
