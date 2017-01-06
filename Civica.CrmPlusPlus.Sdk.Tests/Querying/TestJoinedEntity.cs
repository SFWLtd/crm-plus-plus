using System.Collections.Generic;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.Metadata;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;

namespace Civica.CrmPlusPlus.Sdk.Tests.Querying
{
    [EntityName("testjoinedentity")]
    [EntityInfo("Test Joined Entity", OwnershipType.UserOwned)]
    public class TestJoinedEntity : CrmPlusPlusEntity
    {
        [PropertyName("testlookupname")]
        [PropertyInfo("Test lookup", AttributeRequiredLevel.Recommended)]
        [Lookup]
        public EntityReference<TestEntity> TestEntityId { get; set; }

        [PropertyName("testnumber")]
        [PropertyInfo("Test Number", AttributeRequiredLevel.None)]
        [Integer(100, 0, IntegerFormat.None)]
        public int Number { get; set; }

        public IEnumerable<TestNestedJoinedEntity> NestedJoinedEntities { get; set; }
    }
}
