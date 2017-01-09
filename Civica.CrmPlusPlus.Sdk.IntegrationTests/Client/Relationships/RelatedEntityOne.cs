using System.Collections.Generic;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;

namespace Civica.CrmPlusPlus.Sdk.IntegrationTests.Client.Relationships
{
    [EntityName("civica_relentone")]
    [EntityInfo("Related Entity One", EntityAttributes.Metadata.OwnershipType.UserOwned)]
    public class RelatedEntityOne : CrmPlusPlusEntity
    {
        public IEnumerable<RelatedEntityMany> RelatedManyEntities { get; set; }
    }
}
