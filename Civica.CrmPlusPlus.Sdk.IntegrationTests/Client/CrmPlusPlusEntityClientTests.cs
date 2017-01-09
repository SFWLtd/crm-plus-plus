using System;
using System.Collections.Generic;
using Civica.CrmPlusPlus.Sdk.Client;
using Civica.CrmPlusPlus.Sdk.Client.Retrieve;
using Civica.CrmPlusPlus.Sdk.Settings;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Xunit;

namespace Civica.CrmPlusPlus.Sdk.IntegrationTests.Client
{
    public class CrmPlusPlusEntityClientTests : IClassFixture<IntegrationTestEntityFixture>, IDisposable
    {
        private readonly IntegrationTestEntityFixture fixture;
        private readonly CrmPlusPlusEntityClient client;

        private readonly List<Action> cleanupActions;

        public CrmPlusPlusEntityClientTests(IntegrationTestEntityFixture fixture)
        {
            this.fixture = fixture;
            client = Client();

            cleanupActions = new List<Action>();
        }

        [Fact]
        public void WhenCreatingAnEntity_AndThenRetrieving_CreatedDateShouldBePopulated_AndModifiedDateShouldBeTheSameAsCreatedDate()
        {
            var entity = new EntityClientTestEntity
            {
                Cost = 0.15M,
                Email = "testemail@123.com",
                IsATest = true,
                Length = 123.12,
                SomeDate = DateTime.Now,
                WholeNumber = -63
            };

            client.Create(entity);
            cleanupActions.Add(() => client.Delete(entity));

            entity = client.Retrieve(Retrieval
                .ForEntity<EntityClientTestEntity>(entity.Id));

            Assert.NotEqual(DateTime.MinValue, entity.CreatedOn);
            Assert.Equal(entity.CreatedOn, entity.ModifiedOn);
        }

        [Fact]
        public void WhenRetrievingAnEntity_WithAllColumns_AllColumnsShouldBePopulated()
        {
            var cost = 0.15M;
            var email = "testemail@123.com";
            var isATest = true;
            var length = 123.12;
            var someDate = DateTime.Now;
            var wholeNumber = -63;

            var entity = new EntityClientTestEntity
            {
                Cost = cost,
                Email = email,
                IsATest = isATest,
                Length = length,
                SomeDate = someDate,
                WholeNumber = wholeNumber
            };

            client.Create(entity);
            cleanupActions.Add(() => client.Delete(entity));

            var retrieval = Retrieval
                .ForEntity<EntityClientTestEntity>(entity.Id)
                .IncludeAllColumns(true);

            entity = client.Retrieve(retrieval);

            Assert.Equal(cost, entity.Cost);
            Assert.Equal(email, entity.Email);
            Assert.Equal(isATest, entity.IsATest);
            Assert.Equal(length, entity.Length);

            // Can only check the date accuracy to the nearest second (CRM does not store millisecond information)
            Assert.Equal(someDate.Date, entity.SomeDate.Date);
            Assert.Equal(someDate.TimeOfDay.Hours, entity.SomeDate.TimeOfDay.Hours);
            Assert.Equal(someDate.TimeOfDay.Minutes, entity.SomeDate.TimeOfDay.Minutes);
            Assert.Equal(someDate.TimeOfDay.Seconds, entity.SomeDate.TimeOfDay.Seconds);

            Assert.Equal(wholeNumber, entity.WholeNumber);
        }

        [Fact]
        public void WhenDeletingAnEntity_RetrievingThatEntityShouldThrowAnException()
        {
            var entity = new EntityClientTestEntity
            {
                Cost = 0.15M,
                Email = "testemail@123.com",
                IsATest = true,
                Length = 123.12,
                SomeDate = DateTime.Now,
                WholeNumber = -63
            };

            client.Create(entity);
            cleanupActions.Add(() => client.Delete(entity));

            client.Delete(entity);
            cleanupActions.Clear();

            var retrieval = Retrieval
                .ForEntity<EntityClientTestEntity>(entity.Id)
                .IncludeAllColumns(true);

            Assert.ThrowsAny<Exception>(() => client.Retrieve(retrieval));
        }

        private CrmPlusPlusEntityClient Client()
        {
            var crmConnection = new CrmServiceClient(Settings.ConnectionString);

            var service = crmConnection.OrganizationWebProxyClient != null
                ? crmConnection.OrganizationWebProxyClient
                : (IOrganizationService)crmConnection.OrganizationServiceProxy;

            return new CrmPlusPlusEntityClient(service);
        }

        public void Dispose()
        {
            foreach (var action in cleanupActions)
            {
                action();
            }
        }
    }

    public class IntegrationTestEntityFixture : IDisposable
    {
        private readonly CrmPlusPlusCustomizationClient client;

        public IntegrationTestEntityFixture()
        {
            client = Client();
            client.CreateEntity<EntityClientTestEntity>();
            client.CreateAllProperties<EntityClientTestEntity>();
        }

        public void Dispose()
        {
            client.Delete<EntityClientTestEntity>();
        }

        private CrmPlusPlusCustomizationClient Client()
        {
            var crmPlusPlus = CrmPlusPlus.ForTenant(Settings.ConnectionString);
            return (CrmPlusPlusCustomizationClient)crmPlusPlus.GetCustomizationClientForSolution(PublisherSettings.Default, SolutionSettings.Default);
        }
    }
}
