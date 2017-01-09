using System;
using System.Collections.Generic;
using System.Linq;
using Civica.CrmPlusPlus.Sdk.Client;
using Civica.CrmPlusPlus.Sdk.Querying;
using Civica.CrmPlusPlus.Sdk.Settings;
using Xunit;

namespace Civica.CrmPlusPlus.Sdk.IntegrationTests.Client.Relationships
{
    public class EntityRelationshipTests : IDisposable
    {
        private readonly List<Action> cleanupActions;
        private readonly CrmPlusPlusCustomizationClient customizationClient;
        private readonly CrmPlusPlusEntityClient entityClient;

        public EntityRelationshipTests()
        {
            cleanupActions = new List<Action>();

            var crmPlusPlus = CrmPlusPlus.ForTenant(Settings.ConnectionString);
            entityClient = (CrmPlusPlusEntityClient)crmPlusPlus.EntityClient;
            customizationClient = (CrmPlusPlusCustomizationClient)crmPlusPlus.GetCustomizationClientForSolution(PublisherSettings.Default, SolutionSettings.Default);
        }

        [Fact]
        public void CanCreateOneToManyRelationship_AndWhenRelatedRecordsCreated_CanRetrieveRelated1ToNRecords()
        {
            customizationClient.CreateEntity<RelatedEntityOne>();
            cleanupActions.Add(() => customizationClient.Delete<RelatedEntityOne>());

            customizationClient.CreateEntity<RelatedEntityMany>();
            cleanupActions.Insert(0, () => customizationClient.Delete<RelatedEntityMany>());

            customizationClient.CreateOneToManyRelationship<RelatedEntityOne, RelatedEntityMany>(e => e.RelatedEntityOne, EntityAttributes.Metadata.AttributeRequiredLevel.None);

            var relatedFrom = new RelatedEntityOne();
            entityClient.Create(relatedFrom);

            var relatedTo = new RelatedEntityMany();
            relatedTo.RelatedEntityOne = new EntityReference<RelatedEntityOne>(relatedFrom.Id);
            entityClient.Create(relatedTo);

            var query = Query.ForEntity<RelatedEntityOne>()
                .Join1ToN(j => j.RelatedManyEntities, r => r.RelatedEntityOne, JoinType.Inner, queryBuilder => { });

            var result = entityClient.RetrieveMultiple(query);

            Assert.Equal(1, result.Count());
            Assert.Equal(1, result.Single().RelatedManyEntities.Count());
        }

        [Fact]
        public void CanCreateOneToManyRelationship_AndWhenRelatedRecordsCreate_CanRetrieveRelatedNTo1Records()
        {
            customizationClient.CreateEntity<RelatedEntityOne>();
            cleanupActions.Add(() => customizationClient.Delete<RelatedEntityOne>());

            customizationClient.CreateEntity<RelatedEntityMany>();
            cleanupActions.Insert(0, () => customizationClient.Delete<RelatedEntityMany>());

            customizationClient.CreateOneToManyRelationship<RelatedEntityOne, RelatedEntityMany>(e => e.RelatedEntityOne, EntityAttributes.Metadata.AttributeRequiredLevel.None);

            var relatedFrom = new RelatedEntityOne();
            entityClient.Create(relatedFrom);

            var relatedTo = new RelatedEntityMany();
            relatedTo.RelatedEntityOne = new EntityReference<RelatedEntityOne>(relatedFrom.Id);
            entityClient.Create(relatedTo);

            var query = Query.ForEntity<RelatedEntityMany>()
                .JoinNTo1(j => j.RelatedEntityOne, JoinType.Inner, queryBuilder => { });

            var result = entityClient.RetrieveMultiple(query);

            Assert.Equal(1, result.Count());
            Assert.NotNull(result.Single().RelatedEntityOne.Entity);
        }

        public void Dispose()
        {
            foreach (var cleanupAction in cleanupActions)
            {
                cleanupAction();
            }
        }
    }
}
