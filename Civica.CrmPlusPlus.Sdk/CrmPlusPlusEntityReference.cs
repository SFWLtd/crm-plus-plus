using System;

namespace Civica.CrmPlusPlus.Sdk
{
    public class CrmPlusPlusEntityReference<T> where T : CrmPlusPlusEntity, new()
    {
        public Guid Id { get; }

        public CrmPlusPlusEntityReference(Guid id)
        {
            Id = id;
        }
    }
}
