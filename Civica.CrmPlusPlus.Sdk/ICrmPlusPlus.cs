using Civica.CrmPlusPlus.Sdk.Client;
using Civica.CrmPlusPlus.Sdk.Settings;

namespace Civica.CrmPlusPlus.Sdk
{
    public interface ICrmPlusPlus
    {
        ICrmPlusPlusEntityClient EntityClient { get; }

        ICrmPlusPlusCustomizationClient GetCustomizationClientForSolution(PublisherSettings publisherSettings, SolutionSettings solutionSettings);
    }
}
