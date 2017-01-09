using System;

namespace Civica.CrmPlusPlus.Sdk.Validation
{
    internal class GuardThis<T>
    {
        internal T Obj { get; private set; }

        internal GuardThis(T obj)
        {
            Obj = obj;
        }

        internal GuardThis<T> CustomRule(Func<T, bool> guardFunc, string customErrorMessage = null)
        {
            if (!guardFunc(Obj))
            {
                throw new ArgumentException(customErrorMessage == null
                    ? string.Format("A validation error occured for type '{0}'", typeof(T).Name)
                    : customErrorMessage);
            }

            return this;
        }
    }
}
