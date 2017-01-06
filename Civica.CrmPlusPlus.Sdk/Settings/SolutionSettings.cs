namespace Civica.CrmPlusPlus.Sdk.Settings
{
    public class SolutionSettings
    {
        public static SolutionSettings Default
        {
            get
            {
                return new SolutionSettings("CrmPlusPlus", "CRM++", "1.0.0.0");
            }
        }

        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public string Version { get; private set; }

        public SolutionSettings(string name, string displayName, string version)
        {
            Name = name;
            DisplayName = displayName;
            Version = version;
        }
    }
}
