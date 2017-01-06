using System;
using System.ComponentModel;
using System.Linq;
using Civica.CrmPlusPlus.Sdk.EntityAttributes;
using Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes;

namespace Civica.CrmPlusPlus.Sdk
{
    internal static class EntityExtensions
    {
        internal static T ToCrmPlusPlusEntity<T>(this Microsoft.Xrm.Sdk.Entity entity) where T : CrmPlusPlusEntity, new()
        {
            var crmPlusPlusEntity = Activator.CreateInstance<T>();
            crmPlusPlusEntity.Id = entity.Id;

            crmPlusPlusEntity.CreatedOn = DateTime.Parse(entity["createdon"].ToString());
            crmPlusPlusEntity.ModifiedOn = DateTime.Parse(entity["modifiedon"].ToString()); 

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(typeof(T)))
            {
                if (property.Name == "ModifedOn" || property.Name == "CreatedOn")
                {
                    continue;
                }

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
                        || (attr.GetType() == typeof(EntityReference) && property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(CrmPlusPlusEntityReference<>)));

                if (propertyNameAttr != null && propertyInfoAttr != null && typeInfoAttr != null)
                {
                    var propertyName = ((PropertyNameAttribute)propertyNameAttr).PropertyName;

                    if (entity.Contains(propertyName))
                    {
                        var value = entity[propertyName];

                        if (value.GetType() == typeof(Microsoft.Xrm.Sdk.EntityReference))
                        {
                            var referenceEntityName = property.PropertyType.GetGenericArguments().Single();
                            var entityReferenceType = typeof(CrmPlusPlusEntityReference<>).MakeGenericType(referenceEntityName);

                            value = Activator.CreateInstance(entityReferenceType, new object[] { ((Microsoft.Xrm.Sdk.EntityReference)value).Id });
                        }

                        property.SetValue(crmPlusPlusEntity, value);
                    }
                }
            }

            return crmPlusPlusEntity;
        }
    }
}
