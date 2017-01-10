namespace Civica.CrmPlusPlus.Sdk.Querying
{
    internal static class StringExtensions
    {
        internal static string ClearXmlFormatting(this string str)
        {
            return str
                .Replace("\r\n", string.Empty)
                .Replace(" />", "/>")
                .Replace("  ", string.Empty)
                .Replace("> <", "><");
        }

        internal static string ToIdAlias(this string alias)
        {
            return string.IsNullOrEmpty(alias) ? "id" : alias + ".id";
        }
    }
}
