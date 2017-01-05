namespace Civica.CrmPlusPlus.Sdk.Settings
{
    public class PublisherSettings
    {
        public static PublisherSettings Default
        {
            get
            {
                return new PublisherSettings();
            }
        }

        public string DisplayName { get; set; }

        public string Name { get; set; }

        public string Prefix { get; set; }

        public int OptionValuePrefix { get; set; }

        private PublisherSettings()
        {
            DisplayName = "Civica";
            Name = "civica";
            Prefix = "civica";
            OptionValuePrefix = 48871;
        }

        public PublisherSettings(string displayName, string name, string prefix, int optionValuePrefix)
        {
            DisplayName = displayName;
            Name = name;
            Prefix = prefix;
            OptionValuePrefix = optionValuePrefix;
        }
    }
}
