using System;
using System.Linq;
using System.Linq.Expressions;
using Civica.CrmPlusPlus.Sdk.Validation;
using Microsoft.Xrm.Sdk.Metadata;

namespace Civica.CrmPlusPlus.Sdk.EntityAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyInfoAttribute : Attribute
    {
        public string DisplayName { get; }

        public AttributeRequiredLevel AttributeRequiredLevel { get; }

        public string Description { get; }

        public PropertyInfoAttribute(string displayName, Metadata.AttributeRequiredLevel attributeRequiredLevel, string description = null)
        {
            Guard.This(displayName).AgainstNullOrEmpty();

            DisplayName = displayName;
            AttributeRequiredLevel = attributeRequiredLevel.ToSimilarEnum<AttributeRequiredLevel>();
            Description = string.IsNullOrEmpty(description) ? displayName : description;
        }

        internal static PropertyInfoAttribute GetFromType<T, TProperty>(Expression<Func<T, TProperty>> propertyExpr) where T : CrmPlusPlusEntity, new()
        {
            Guard.This(propertyExpr.Body).AgainstNonMemberExpression();

            var propertyInfo = ((MemberExpression)propertyExpr.Body).Member;

            var propertyNameAttributes = propertyInfo.GetCustomAttributes(true)
                .Where(attr => attr.GetType() == typeof(PropertyInfoAttribute));

            if (propertyNameAttributes.Any())
            {
                return ((PropertyInfoAttribute)propertyNameAttributes.Single());
            }

            throw new InvalidOperationException(string.Format("Cannot retrieve property information from member '{0}' of type '{1}'. PropertyInfoAttribute not found for this type", propertyInfo.Name, typeof(T).Name));
        }
    }
}
