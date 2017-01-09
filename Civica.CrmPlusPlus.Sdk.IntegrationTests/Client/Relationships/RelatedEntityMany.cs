using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;

namespace Civica.CrmPlusPlus.Sdk.IntegrationTests.Client.Relationships
{
    [EntityName("civica_relentmany")]
    [EntityInfo("Related Entity Many", EntityAttributes.Metadata.OwnershipType.UserOwned)]
    public class RelatedEntityMany : CrmPlusPlusEntity
    {
        [PropertyName("civica_relentoneid")]
        [PropertyInfo("Related entity one lookup", EntityAttributes.Metadata.AttributeRequiredLevel.None)]
        [Lookup]
        public EntityReference<RelatedEntityOne> RelatedEntityOne { get; set; }
    }
}
