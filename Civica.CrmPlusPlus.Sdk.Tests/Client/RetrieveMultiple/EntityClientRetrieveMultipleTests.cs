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
        public void Has1ToNRelatedEntityRecords_ReturnsDataCorrectly()
        {
            var testEntity1Id = Guid.NewGuid();
            var testEntity1String = "dshajdsghjk";
            var testEntity2FirstId = Guid.NewGuid();
            var testEntity2SecondId = Guid.NewGuid();
            var testEntity2FirstInt = 3;
            var testEntity2SecondInt = 5;

            var flatData = new List<Entity>();

            var first = new Entity(EntityNameAttribute.GetFromType<RetrieveMultipleTestEntity1>(), testEntity1Id);
            first["id"] = testEntity1Id;
            first["createdon"] = DateTime.Now;
            first["modifiedon"] = DateTime.Now;
            first["teststring"] = testEntity1String;
            first["retrievemultipletestentity2.id"] = new AliasedValue("retrievemultipletestentity2", "id", testEntity2FirstId);
            first["retrievemultipletestentity2.testint"] = new AliasedValue("retrievemultipletestentity2", "testint", testEntity2FirstInt);

            var second = new Entity(EntityNameAttribute.GetFromType<RetrieveMultipleTestEntity1>(), testEntity1Id);
            second["id"] = testEntity1Id;
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

        [Fact]
        public void HasNTo1RelatedEntityRecords_ReturnsDataCorrectly()
        {
            var testEntity2Id = Guid.NewGuid();
            var testEntity2Int = 2;
            var testEntity1Id = Guid.NewGuid();
            var testEntity1String = "dshjakl";

            var flatData = new List<Entity>();

            var entity = new Entity(EntityNameAttribute.GetFromType<RetrieveMultipleTestEntity2>(), testEntity2Id);
            entity["id"] = testEntity2Id;
            entity["createdon"] = DateTime.Now;
            entity["modifiedon"] = DateTime.Now;
            entity["testint"] = testEntity2Int;
            entity["retrievemultipletestentity1.id"] = new AliasedValue("retrievemultipletestentity2", "id", testEntity1Id);
            entity["retrievemultipletestentity1.teststring"] = new AliasedValue("retrievemultipletestentity2", "testint", testEntity1String);

            flatData.Add(entity);

            var service = A.Fake<IOrganizationService>();

            A.CallTo(() => service.RetrieveMultiple(A<QueryBase>._))
                .Returns(new EntityCollection(flatData));

            var crmPlusPlusEntityClient = new CrmPlusPlusEntityClient(service);

            var result = crmPlusPlusEntityClient.RetrieveMultiple(Query.ForEntity<RetrieveMultipleTestEntity2>());

            Assert.Equal(1, result.Count());

            var singleResult = result.Single();

            Assert.Equal(testEntity2Int, singleResult.TestInt);
            Assert.NotNull(singleResult.RetrieveMultipleTestEntity1Lookup);
            Assert.Equal(testEntity1Id, singleResult.RetrieveMultipleTestEntity1Lookup.Id);
            Assert.NotNull(singleResult.RetrieveMultipleTestEntity1Lookup.Entity);

            Assert.Equal(testEntity1Id, singleResult.RetrieveMultipleTestEntity1Lookup.Entity.Id);
            Assert.Equal(testEntity1String, singleResult.RetrieveMultipleTestEntity1Lookup.Entity.TestString);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasNTo1NestedRelatedEntityRecords_ReturnsDataCorrectly(bool includeNestedEntityId)
        {
            var entity3Id = Guid.NewGuid();
            var entity2Id = Guid.NewGuid();
            var entity1Id = Guid.NewGuid();

            var entity3TestInt = 13;
            var entity2TestInt = 66;
            var entity1TestString = "gfhjk";

            var entity = new Entity("retrievemultipletestentity3", entity3Id);
            entity["id"] = entity.Id;
            entity["testint"] = entity3TestInt;
            entity["retrievemultipletestentity2.id"] = Guid.NewGuid();
            entity["retrievemultipletestentity2.testint"] = entity2TestInt;
            entity["retrievemultipletestentity1.id"] = entity1Id;
            entity["retrievemultipletestentity1.teststring"] = entity1TestString; 
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

        [PropertyName("testlookup1")]
        [PropertyInfo("Test Lookup 1", EntityAttributes.Metadata.AttributeRequiredLevel.None)]
        [Lookup]
        public EntityReference<RetrieveMultipleTestEntity1> RetrieveMultipleTestEntity1Lookup { get; set; }

        public IEnumerable<RetrieveMultipleTestEntity3> RelatedEntities2 { get; set; }
    }

    [EntityName("retrievemultipletestentity3")]
    [EntityInfo("Test Entity 3", EntityAttributes.Metadata.OwnershipType.UserOwned)]
    public class RetrieveMultipleTestEntity3 : CrmPlusPlusEntity
    {
        [PropertyName("testint")]
        [PropertyInfo("Test Int", EntityAttributes.Metadata.AttributeRequiredLevel.None)]
        [Integer(100, 0, EntityAttributes.Metadata.IntegerFormat.None)]
        public int TestInt { get; set; }

        [PropertyName("testlookup2")]
        [PropertyInfo("Test Lookup 2", EntityAttributes.Metadata.AttributeRequiredLevel.None)]
        [Lookup]
        public EntityReference<RetrieveMultipleTestEntity2> RetrieveMultipleTestEntity2Lookup { get; set; }
    }
}
