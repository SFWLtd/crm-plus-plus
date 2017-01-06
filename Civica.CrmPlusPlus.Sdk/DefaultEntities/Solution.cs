using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;

namespace Civica.CrmPlusPlus.Sdk.DefaultEntities
{
    [EntityName("solution")]
    [EntityInfo("Solution", EntityAttributes.Metadata.OwnershipType.OrganizationOwned)]
    public class Solution : CrmPlusPlusEntity
    {
        [PropertyName("uniquename")]
        [PropertyInfo("Name", EntityAttributes.Metadata.AttributeRequiredLevel.ApplicationRequired)]
        [String(100, EntityAttributes.Metadata.StringFormatName.Text)]
        public string Name { get; set; } 

        [PropertyName("friendlyname")]
        [PropertyInfo("Display Name", EntityAttributes.Metadata.AttributeRequiredLevel.ApplicationRequired)]
        [String(100, EntityAttributes.Metadata.StringFormatName.Text)]
        public string DisplayName { get; set; }

        [PropertyName("version")]
        [PropertyInfo("Version", EntityAttributes.Metadata.AttributeRequiredLevel.ApplicationRequired)]
        [String(100, EntityAttributes.Metadata.StringFormatName.Text)]
        public string Version { get; set; }

        [PropertyName("publisherid")]
        [PropertyInfo("Publisher", EntityAttributes.Metadata.AttributeRequiredLevel.ApplicationRequired)]
        [EntityReference]
        public CrmPlusPlusEntityReference<Publisher> Publisher { get; set; }
    }
}
