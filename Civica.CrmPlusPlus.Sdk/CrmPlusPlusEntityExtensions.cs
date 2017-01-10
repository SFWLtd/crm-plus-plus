using System;
using System.ComponentModel;
using System.Linq;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;

namespace Civica.CrmPlusPlus.Sdk
{
    public static class CrmPlusPlusEntityExtensions
    {
        internal static Microsoft.Xrm.Sdk.Entity ToCrmEntity<T>(this T crmPlusPlusEntity) where T : CrmPlusPlusEntity, new()
        {
            var entity = new Microsoft.Xrm.Sdk.Entity(EntityNameAttribute.GetFromType<T>(), crmPlusPlusEntity.Id);

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(typeof(T)))
            {
                var attributes = property.Attributes.AsEnumerable();

                var propertyNameAttr = attributes.SingleOrDefault(attr => attr.GetType() == typeof(PropertyNameAttribute));
                var propertyInfoAttr = attributes.SingleOrDefault(attr => attr.GetType() == typeof(PropertyInfoAttribute));
                var typeInfoAttr = attributes
                    .SingleOrDefault(attr => (attr.GetType() == typeof(BooleanAttribute) && property.PropertyType == typeof(bool))
                        || (attr.GetType() == typeof(DateTimeAttribute) && property.PropertyType == typeof(DateTime))
                        || (attr.GetType() == typeof(DecimalAttribute) && property.PropertyType == typeof(decimal))
                        || (attr.GetType() == typeof(DoubleAttribute) && property.PropertyType == typeof(double))
                        || (attr.GetType() == typeof(IntegerAttribute) && property.PropertyType == typeof(int))
                        || (attr.GetType() == typeof(StringAttribute) && property.PropertyType == typeof(string))
                        || (attr.GetType() == typeof(LookupAttribute) && property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(EntityReference<>))
                        || (attr.GetType() == typeof(OptionSetAttribute) && property.PropertyType.IsEnum));

                if (propertyNameAttr != null && propertyInfoAttr != null && typeInfoAttr != null)
                {
                    var propertyName = ((PropertyNameAttribute)propertyNameAttr).PropertyName;
                    var value = property.GetValue(crmPlusPlusEntity);

                    if (value != null)
                    {
                        if (value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(EntityReference<>))
                        {
                            var entityReferenceType = property.PropertyType.GetGenericArguments().Single();
                            var entityName = EntityNameAttribute.GetFromType(entityReferenceType);

                            value = new Microsoft.Xrm.Sdk.EntityReference(entityName, ((dynamic)value).Id);
                        }
                        else if (value.GetType().IsEnum)
                        {
                            value = new Microsoft.Xrm.Sdk.OptionSetValue((int)value);
                        }

                        entity[propertyName] = value;
                    }
                }
            }

            return entity;
        }

        public static EntityReference<T> AsEntityReference<T>(this T crmPlusPlusEntity) where T : CrmPlusPlusEntity, new()
        {
            return new EntityReference<T>(crmPlusPlusEntity.Id);
        }
    }
}
