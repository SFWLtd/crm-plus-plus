using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;

namespace Civica.CrmPlusPlus.Sdk.IntegrationTests.Client.Relationships
{
    [EntityName("civica_relentsecmany")]
    [EntityInfo("Related Entity Many", EntityAttributes.Metadata.OwnershipType.UserOwned)]
    public class RelatedSecondEntityMany : CrmPlusPlusEntity
    {
        [PropertyName("civica_relentseconeid")]
        [PropertyInfo("Related entity many lookup", EntityAttributes.Metadata.AttributeRequiredLevel.None)]
        [Lookup]
        public EntityReference<RelatedEntityMany> RelatedEntityMany { get; set; }

        [PropertyName("civica_string")]
        [PropertyInfo("My String", EntityAttributes.Metadata.AttributeRequiredLevel.None)]
        [String(100, EntityAttributes.Metadata.StringFormatName.Text)]
        public string MyString { get; set; }
    }
}
