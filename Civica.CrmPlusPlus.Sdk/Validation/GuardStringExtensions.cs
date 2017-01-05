using System;

namespace Civica.CrmPlusPlus.Sdk.Validation
{
    internal static class GuardStringExtensions
    {
        internal static GuardThis<string> AgainstNullOrEmpty(this GuardThis<string> guard, string customErrorMessage = null)
        {
            if (string.IsNullOrEmpty(guard.Obj))
            {
                throw new ArgumentException(customErrorMessage != null
                    ? customErrorMessage
                    : "String was found to be either empty or null when it should have a value");
            }

            return guard;
        }

        internal static GuardThis<string> AgainstSpaces(this GuardThis<string> guard, string customErrorMessage = null)
        {
            if (guard.Obj.Trim().Contains(" "))
            {
                throw new ArgumentException(customErrorMessage != null
                    ? customErrorMessage
                    : "String was found to be contain white space, but white space is not allowed");
            }

            return guard;
        }
    }
}
