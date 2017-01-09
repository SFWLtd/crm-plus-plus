using System;
using System.Collections.Generic;
using System.Linq;
using Civica.CrmPlusPlus.Sdk.Client;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;
using Civica.CrmPlusPlus.Sdk.Querying;
using FakeItEasy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Xunit;

namespace Civica.CrmPlusPlus.Sdk.Tests.Client.RetrieveMultiple
{
    public class EntityClientRetrieveMultipleTests
    {
        [Fact]
        public void HasRelatedEntityRecords_ReturnsDataCorrectly()
        {
            var testEntity1Id = Guid.NewGuid();
            var testEntity1String = "dshajdsghjk";
            var testEntity2FirstId = Guid.NewGuid();
            var testEntity2SecondId = Guid.NewGuid();
            var testEntity2FirstInt = 3;
            var testEntity2SecondInt = 5;

            var flatData = new List<Entity>();

            var first = new Entity(EntityNameAttribute.GetFromType<RetrieveMultipleTestEntity1>(), testEntity1Id);
            first["createdon"] = DateTime.Now;
            first["modifiedon"] = DateTime.Now;
            first["teststring"] = testEntity1String;
            first["retrievemultipletestentity2.id"] = new AliasedValue("retrievemultipletestentity2", "id", testEntity2FirstId);
            first["retrievemultipletestentity2.testint"] = new AliasedValue("retrievemultipletestentity2", "testint", testEntity2FirstInt);

            var second = new Entity(EntityNameAttribute.GetFromType<RetrieveMultipleTestEntity1>(), testEntity1Id);
            second["createdon"] = DateTime.Now;
            second["modifiedon"] = DateTime.Now;
            second["teststring"] = testEntity1String;
            second["retrievemultipletestentity2.id"] = new AliasedValue("retrievemultipletestentity2", "id", testEntity2SecondId);
            second["retrievemultipletestentity2.testint"] = new AliasedValue("retrievemultipletestentity2", "testint", testEntity2SecondInt);

            flatData.Add(first);
            flatData.Add(second);

            var service = A.Fake<IOrganizationService>();

            A.CallTo(() => service.RetrieveMultiple(A<QueryBase>._))
                .Returns(new EntityCollection(flatData));

            var crmPlusPlusEntityClient = new CrmPlusPlusEntityClient(service);

            var result = crmPlusPlusEntityClient.RetrieveMultiple(Query.ForEntity<RetrieveMultipleTestEntity1>());

            Assert.Equal(1, result.Count());
            Assert.Equal(2, result.Single().RelatedEntities.Count());

            var firstEntity = result.Single();

            Assert.Equal(testEntity1String, firstEntity.TestString);
            Assert.Contains(testEntity2FirstInt, firstEntity.RelatedEntities.Select(e => e.TestInt));
            Assert.Contains(testEntity2SecondInt, firstEntity.RelatedEntities.Select(e => e.TestInt));
        }
    }

    [EntityName("retrievemultipletestentity1")]
    [EntityInfo("Test Entity 1", EntityAttributes.Metadata.OwnershipType.UserOwned)]
    public class RetrieveMultipleTestEntity1 : CrmPlusPlusEntity
    {
        [PropertyName("teststring")]
        [PropertyInfo("Test String", EntityAttributes.Metadata.AttributeRequiredLevel.None)]
        [String(100, EntityAttributes.Metadata.StringFormatName.Text)]
        public string TestString { get; set; }

        public IEnumerable<RetrieveMultipleTestEntity2> RelatedEntities { get; set; }
    }

    [EntityName("retrievemultipletestentity2")]
    [EntityInfo("Test Entity 2", EntityAttributes.Metadata.OwnershipType.UserOwned)]
    public class RetrieveMultipleTestEntity2 : CrmPlusPlusEntity
    {
        [PropertyName("testint")]
        [PropertyInfo("Test Int", EntityAttributes.Metadata.AttributeRequiredLevel.None)]
        [Integer(100, 0, EntityAttributes.Metadata.IntegerFormat.None)]
        public int TestInt { get; set; }

        public EntityReference<RetrieveMultipleTestEntity1> RetrieveMultipleTestEntity1Lookup { get; set; }
    }
}
