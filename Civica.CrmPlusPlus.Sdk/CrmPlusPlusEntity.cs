using System;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;

namespace Civica.CrmPlusPlus
{
    public abstract class CrmPlusPlusEntity
    {
        [PropertyName("id")]
        public Guid Id { get; internal set; }

        [PropertyName("createdon")]
        public DateTime CreatedOn { get; internal set; }

        [PropertyName("modifiedon")]
        public DateTime ModifiedOn { get; internal set; }

        protected CrmPlusPlusEntity(Guid? id = null)
        {
            Id = id.HasValue ? id.Value : Guid.NewGuid();
        }
    }
}
