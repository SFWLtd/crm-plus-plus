using System;
using System.Linq;
using System.Linq.Expressions;
using Civica.CrmPlusPlus.Sdk.Validation;

namespace Civica.CrmPlusPlus.Sdk.EntityAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyNameAttribute : Attribute
    {
        public string PropertyName { get; }

        public PropertyNameAttribute(string propertyName)
        {
            Guard.This(propertyName).AgainstNullOrEmpty("CrmPlusPlusEntity should not have a null or empty logical name");

            PropertyName = propertyName;
        }

        internal static string GetFromType<T, TProperty>(Expression<Func<T, TProperty>> propertyExpr) where T : CrmPlusPlusEntity, new()
        {
            Guard.This(propertyExpr.Body).AgainstNonMemberExpression();

            var propertyInfo = ((MemberExpression)propertyExpr.Body).Member;

            var propertyNameAttributes = propertyInfo.GetCustomAttributes(true)
                .Where(attr => attr.GetType() == typeof(PropertyNameAttribute));

            if (propertyNameAttributes.Any())
            {
                return ((PropertyNameAttribute)propertyNameAttributes.Single()).PropertyName;
            }

            throw new InvalidOperationException(string.Format("Cannot retrieve property name from member '{0}' of type '{1}'. PropertyAttribute not found for this type", propertyInfo.Name, typeof(T).Name));
        }
    }
}
