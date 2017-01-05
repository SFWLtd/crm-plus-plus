namespace Civica.CrmPlusPlus.Sdk.Querying
{
    public static class StringExtensions
    {
        public static string ClearXmlFormatting(this string str)
        {
            return str
                .Replace("\r\n", string.Empty)
                .Replace(" />", "/>")
                .Replace("  ", string.Empty)
                .Replace("> <", "><");
        }
    }
}
