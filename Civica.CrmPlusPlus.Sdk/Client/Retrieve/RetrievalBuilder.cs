using System;

namespace Civica.CrmPlusPlus.Sdk.Client.Retrieve
{
    public static class Retrieval
    {
        public static Retrieval<T> ForEntity<T>(Guid id) where T : CrmPlusPlusEntity, new()
        {
            return new Retrieval<T>(id);
        }
    }
}
