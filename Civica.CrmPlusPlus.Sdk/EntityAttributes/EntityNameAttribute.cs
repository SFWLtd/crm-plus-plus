using System;
using System.Linq;
using Civica.CrmPlusPlus.Sdk.Validation;

namespace Civica.CrmPlusPlus.Sdk.EntityAttributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityNameAttribute : Attribute
    {
        public string EntityLogicalName { get; }

        public EntityNameAttribute(string entityLogicalName)
        {
            Guard.This(entityLogicalName).AgainstNullOrEmpty("CrmPlusPlusEntity should not have a null or empty logical name");

            EntityLogicalName = entityLogicalName;
        }

        internal static string GetFromType<T>() where T : CrmPlusPlusEntity, new()
        {
            var crmPlusPlusEntity = typeof(T).GetCustomAttributes(true)
                .Where(a => a.GetType() == typeof(EntityNameAttribute));

            if (crmPlusPlusEntity.Any())
            {
                return ((EntityNameAttribute)crmPlusPlusEntity.Single()).EntityLogicalName;
            }

            throw new InvalidOperationException(string.Format("Cannot retrieve entity name from type '{0}'. EntityNameAttribute not found for this type", typeof(T).Name));
        }
    }
}
