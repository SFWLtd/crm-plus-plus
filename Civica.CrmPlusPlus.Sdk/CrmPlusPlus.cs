using System;
using Civica.CrmPlusPlus.Sdk.Client;
using Civica.CrmPlusPlus.Sdk.Settings;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace Civica.CrmPlusPlus.Sdk
{
    public class CrmPlusPlus : ICrmPlusPlus
    {
        private readonly IOrganizationService service;

        public ICrmPlusPlusEntityClient EntityClient { get; }

        private CrmPlusPlus(IOrganizationService service)
        {
            this.service = service;

            EntityClient = new CrmPlusPlusEntityClient(service);
        }

        public static ICrmPlusPlus ForTenant(string connectionString)
        {
            CrmServiceClient crmConnection = null;
            IOrganizationService service = null;

            try
            {
                crmConnection = new CrmServiceClient(connectionString);
                service = crmConnection.OrganizationWebProxyClient != null
                ? crmConnection.OrganizationWebProxyClient
                : (IOrganizationService)crmConnection.OrganizationServiceProxy;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("An error occurred whilst trying to connect to CRM with the specified connection string. See inner exception for more details", ex);
            }

            if (service != null)
            {
                throw new ArgumentException("An error occurred whilst trying to connect to CRM with the specified connection string");
            }

            return new CrmPlusPlus(service);
        }


        public ICrmPlusPlusCustomizationClient GetCustomizationClientForSolution(PublisherSettings publisherSettings, SolutionSettings solutionSettings)
        {
            throw new NotImplementedException();
            // var result = EntityClient.RetrieveMultiple(queryForPublisher);
        }
    }
}
