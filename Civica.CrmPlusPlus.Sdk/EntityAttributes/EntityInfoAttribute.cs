using System;
using System.Linq;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.Metadata;
using Civica.CrmPlusPlus.Sdk.Validation;
using Microsoft.Xrm.Sdk.Metadata;

namespace Civica.CrmPlusPlus.Sdk.EntityAttributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityInfoAttribute : Attribute
    {
        public string DisplayName { get; }

        public OwnershipTypes OwnershipType { get; }

        public string PluralDisplayName { get; }

        public string Description { get; }

        public EntityInfoAttribute(string displayName, OwnershipType ownershipType, string pluralDisplayName = null, string description = null)
        {
            Guard.This(displayName).AgainstNullOrEmpty();

            DisplayName = displayName;
            OwnershipType = ownershipType.ToSimilarEnum<OwnershipTypes>();
            PluralDisplayName = string.IsNullOrEmpty(pluralDisplayName) ? displayName + "s" : pluralDisplayName;
            Description = string.IsNullOrEmpty(description) ? displayName : description;
        }

        internal static EntityInfoAttribute GetFromType<T>() where T : CrmPlusPlusEntity, new()
        {
            var crmPlusPlusEntity = typeof(T).GetCustomAttributes(true)
                .Where(a => a.GetType() == typeof(EntityInfoAttribute));

            if (crmPlusPlusEntity.Any())
            {
                return (EntityInfoAttribute)crmPlusPlusEntity.Single();
            }

            throw new InvalidOperationException(string.Format("Cannot retrieve entity info from type '{0}'. EntityInfoAttribute not found for this type", typeof(T).Name));
        }
    }
}
