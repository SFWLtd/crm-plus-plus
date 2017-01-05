using System;
using System.Collections.Generic;
using Civica.CrmPlusPlus.Sdk.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
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
            var crmConnection = new CrmServiceClient(Settings.ConnectionString);

            var service = crmConnection.OrganizationWebProxyClient != null
                ? crmConnection.OrganizationWebProxyClient
                : (IOrganizationService)crmConnection.OrganizationServiceProxy;

            return new CrmPlusPlusCustomizationClient(service);
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
