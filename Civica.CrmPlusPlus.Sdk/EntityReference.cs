using System;

namespace Civica.CrmPlusPlus.Sdk
{
    public class EntityReference<T> where T : CrmPlusPlusEntity, new()
    {
        public Guid Id { get; }

        public EntityReference(Guid id)
        {
            Id = id;
        }
    }
}
