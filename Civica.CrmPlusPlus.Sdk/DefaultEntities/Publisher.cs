using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;

namespace Civica.CrmPlusPlus.Sdk.DefaultEntities
{
    [EntityName("publisher")]
    [EntityInfo("Publisher", EntityAttributes.Metadata.OwnershipType.OrganizationOwned)]
    public class Publisher : CrmPlusPlusEntity
    {
        [PropertyName("friendlyname")]
        [PropertyInfo("Display Name", EntityAttributes.Metadata.AttributeRequiredLevel.ApplicationRequired)]
        [String(255, EntityAttributes.Metadata.StringFormatName.Text)]
        public string DisplayName { get; set; }

        [PropertyName("uniquename")]
        [PropertyInfo("Name", EntityAttributes.Metadata.AttributeRequiredLevel.ApplicationRequired)]
        [String(255, EntityAttributes.Metadata.StringFormatName.Text)]
        public string Name { get; set; }

        [PropertyName("customizationprefix")]
        [PropertyInfo("Prefix", EntityAttributes.Metadata.AttributeRequiredLevel.ApplicationRequired)]
        [String(8, EntityAttributes.Metadata.StringFormatName.Text)]
        public string Prefix { get; set; }

        [PropertyName("customizationoptionvalueprefix")]
        [PropertyInfo("Option Value Prefix", EntityAttributes.Metadata.AttributeRequiredLevel.ApplicationRequired)]
        [Integer(10000, 99999, EntityAttributes.Metadata.IntegerFormat.None)]
        public int OptionValuePrefix { get; set; } 
    }
}
