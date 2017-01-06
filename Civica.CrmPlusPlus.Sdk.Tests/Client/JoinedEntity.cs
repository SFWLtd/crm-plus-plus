using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.Metadata;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;

namespace Civica.CrmPlusPlus.Sdk.Tests.Client
{
    [EntityName("civica_joinedentity")]
    [EntityInfo("Joined Entity", OwnershipType.OrganizationOwned)]
    public class JoinedEntity
    {
        [PropertyName("lookup")]
        [PropertyInfo("Lookup", AttributeRequiredLevel.ApplicationRequired)]
        [Lookup]
        public EntityReference<EntityWithProperties> EntityWithPropertiesLookup { get; set; }
    }
}
