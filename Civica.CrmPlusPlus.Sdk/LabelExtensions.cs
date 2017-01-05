using Microsoft.Xrm.Sdk;

namespace Civica.CrmPlusPlus.Sdk
{
    internal static class LabelExtensions
    {
        internal static Label ToLabel(this string s)
        {
            return new Label(s, 1033);
        }
    }
}
