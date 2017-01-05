using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.Metadata;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;

namespace Civica.CrmPlusPlus.Sdk.Tests.Querying
{
    [EntityName("testentity")]
    [EntityInfo("Test Entity", OwnershipType.UserOwned)]
    public class TestEntity : CrmPlusPlusEntity
    {
        [PropertyName("test")]
        [PropertyInfo("Test", AttributeRequiredLevel.ApplicationRequired)]
        [String(100, StringFormatName.Text)]
        public string StringTestProperty { get; set; }
    }
}
