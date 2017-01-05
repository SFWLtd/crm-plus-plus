namespace Civica.CrmPlusPlus.Sdk.Validation
{
    internal class GuardThis<T>
    {
        internal T Obj { get; private set; }

        internal GuardThis(T obj)
        {
            Obj = obj;
        }
    }
}
