using System;
using System.Collections.Generic;
using Civica.CrmPlusPlus.Sdk.Client;
using Civica.CrmPlusPlus.Sdk.Settings;
using Xunit;

namespace Civica.CrmPlusPlus.Sdk.IntegrationTests.Client
{
    public class CrmPlusPlusEntityCustomizationClientTests : IDisposable
    {
        private readonly List<Action> cleanupActions;
        private readonly CrmPlusPlusCustomizationClient client;

        public CrmPlusPlusEntityCustomizationClientTests()
        {
            cleanupActions = new List<Action>();

            client = Client();
        }

        [Fact]
        public void CanCreateValidEntityWithoutProperties()
        {
            client.CreateEntityWithoutProperties<IntegrationTestEntity>();
            cleanupActions.Add(() => client.Delete<IntegrationTestEntity>());
        }

        [Fact]
        public void CanCreateValidEntityWithProperties()
        {
            client.CreateEntityWithoutProperties<IntegrationTestEntity>();

            cleanupActions.Add(() => client.Delete<IntegrationTestEntity>());

            client.CreateAllProperties<IntegrationTestEntity>();
        }

        private CrmPlusPlusCustomizationClient Client()
        {
            var crmPlusPlus = CrmPlusPlus.ForTenant(Settings.ConnectionString);
            return (CrmPlusPlusCustomizationClient)crmPlusPlus.GetCustomizationClientForSolution(PublisherSettings.Default, SolutionSettings.Default);
        }

        public void Dispose()
        {
            foreach (var action in cleanupActions)
            {
                action();
            }
        }
    }
}
