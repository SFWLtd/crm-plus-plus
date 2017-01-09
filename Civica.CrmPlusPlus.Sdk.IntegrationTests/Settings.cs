using System.Configuration;

namespace Civica.CrmPlusPlus.Sdk.IntegrationTests
{
    public static class Settings
    {
        public static string ConnectionString { get { return ConfigurationManager.ConnectionStrings["CRMConnectionString"].ConnectionString; } }
    }
}
