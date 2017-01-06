using System;

namespace Civica.CrmPlusPlus.Sdk.Validation
{
    internal static class GuardCrmPlusPlusEntityExtensions
    {
        internal static GuardThis<Type> AgainstNonCrmPlusPlusEntity(this GuardThis<Type> guard)
        {
            if (!typeof(CrmPlusPlusEntity).IsAssignableFrom(guard.Obj))
            {
                throw new ArgumentException("Type was expected to be CrmPlusPlusEntity but it was not");
            }

            return guard;
        }
    }
}
