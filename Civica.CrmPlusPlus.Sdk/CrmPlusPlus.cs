using System;
using System.Linq;
using Civica.CrmPlusPlus.Sdk.Client;
using Civica.CrmPlusPlus.Sdk.DefaultEntities;
using Civica.CrmPlusPlus.Sdk.Querying;
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

            if (service == null)
            {
                throw new ArgumentException("An error occurred whilst trying to connect to CRM with the specified connection string");
            }

            return new CrmPlusPlus(service);
        }


        public ICrmPlusPlusCustomizationClient GetCustomizationClientForSolution(PublisherSettings publisherSettings, SolutionSettings solutionSettings)
        {
            var publisherQuery = Query.ForEntity<Publisher>()
                .Include(e => e.DisplayName)
                .Include(e => e.Name)
                .Include(e => e.OptionValuePrefix)
                .Include(e => e.Prefix)
                .Filter(FilterType.And, filter =>
                {
                    filter.Condition(e => e.Name, ConditionOperator.Equal, publisherSettings.Name);
                });

            var publisherResults = EntityClient.RetrieveMultiple(publisherQuery);

            Publisher publisher = null;
            if (!publisherResults.Any())
            {
                publisher = new Publisher
                {
                    DisplayName = publisherSettings.DisplayName,
                    Name = publisherSettings.Name,
                    OptionValuePrefix = publisherSettings.OptionValuePrefix,
                    Prefix = publisherSettings.Prefix
                };

                EntityClient.Create(publisher);
            }
            else
            {
                publisher = publisherResults.Single(); // Cannot be more than one with the same name
            }

            var solutionQuery = Query.ForEntity<Solution>()
                .Include(e => e.Name)
                .Include(e => e.DisplayName)
                .Include(e => e.Publisher)
                .Include(e => e.Version)
                .Filter(FilterType.And, filter =>
                {
                    filter.Condition(e => e.Name, ConditionOperator.Equal, solutionSettings.Name);
                });

            var solutionResults = EntityClient.RetrieveMultiple(solutionQuery);

            Solution solution = null;
            if (!solutionResults.Any())
            {
                solution = new Solution
                {
                    Name = solutionSettings.Name,
                    DisplayName = solutionSettings.DisplayName,
                    Version = solutionSettings.Version,
                    Publisher = publisher.AsEntityReference()
                };

                EntityClient.Create(solution);
            }
            else
            {
                solution = solutionResults.Single(); // Cannot be more than one with the same name
            }

            return new CrmPlusPlusCustomizationClient(publisher, solution, service);
        }
    }
}
