namespace Civica.CrmPlusPlus.Sdk.Validation
{
    internal static class Guard
    {
        internal static GuardThis<T> This<T>(T obj)
        {
            return new GuardThis<T>(obj);
        }
    }
}
