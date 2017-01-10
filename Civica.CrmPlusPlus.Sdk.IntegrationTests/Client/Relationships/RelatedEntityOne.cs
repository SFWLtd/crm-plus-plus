using System.Collections.Generic;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;

namespace Civica.CrmPlusPlus.Sdk.IntegrationTests.Client.Relationships
{
    [EntityName("civica_relentone")]
    [EntityInfo("Related Entity One", EntityAttributes.Metadata.OwnershipType.UserOwned)]
    public class RelatedEntityOne : CrmPlusPlusEntity
    {
        public IEnumerable<RelatedEntityMany> RelatedManyEntities { get; set; }

        [PropertyName("civica_string")]
        [PropertyInfo("My String", EntityAttributes.Metadata.AttributeRequiredLevel.None)]
        [String(100, EntityAttributes.Metadata.StringFormatName.Text)]
        public string MyString { get; set; }
    }
}
