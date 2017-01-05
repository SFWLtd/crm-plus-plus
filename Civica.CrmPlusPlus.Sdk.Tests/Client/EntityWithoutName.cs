using Civica.CrmPlusPlus.Sdk.EntityAttributes;

namespace Civica.CrmPlusPlus.Sdk.Tests.Client
{
    [EntityInfo("Entity without name", EntityAttributes.Metadata.OwnershipType.OrganizationOwned)]
    public class EntityWithoutName : CrmPlusPlusEntity
    {
    }
}
