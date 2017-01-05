using System;

namespace Civica.CrmPlusPlus.Sdk.Validation
{
    internal static class GuardIntExtensions
    {
        internal static GuardThis<int> AgainstNegative(this GuardThis<int> guard)
        {
            if (guard.Obj < 0)
            {
                throw new ArgumentException("Value should not be less than zero");
            }

            return guard;
        }

        internal static GuardThis<int> AgainstZero(this GuardThis<int> guard)
        {
            if (guard.Obj == 0)
            {
                throw new ArgumentException("Value should not be zero");
            }

            return guard;
        }
    }
}
