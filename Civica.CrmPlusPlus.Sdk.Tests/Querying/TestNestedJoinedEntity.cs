using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.Metadata;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;

namespace Civica.CrmPlusPlus.Sdk.Tests.Querying
{
    [EntityName("testnestedjoinedentity")]
    [EntityInfo("Test Nested Joined Entity", OwnershipType.UserOwned)]
    public class TestNestedJoinedEntity : CrmPlusPlusEntity
    {
        [PropertyName("testnestedlookupname")]
        [PropertyInfo("Test lookup", AttributeRequiredLevel.Recommended)]
        [Lookup]
        public EntityReference<TestJoinedEntity> TestNestedLookupName { get; set; }

        [PropertyName("testnumber")]
        [PropertyInfo("Test Number", AttributeRequiredLevel.None)]
        [Integer(100, 0, IntegerFormat.None)]
        public int Number { get; set; }
    }
}
